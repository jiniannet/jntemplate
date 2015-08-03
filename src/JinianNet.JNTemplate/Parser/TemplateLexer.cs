/********************************************************************************
 Copyright (c) jiniannet (http://www.jiniannet.com). All rights reserved.
 Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.
 ********************************************************************************/
#define ALLOWCOMMENT
using System;
using System.Collections.Generic;
using JinianNet.JNTemplate.Parser.Node;

namespace JinianNet.JNTemplate.Parser
{
    /// <summary>
    /// 词素分析器
    /// </summary>
    public class TemplateLexer
    {
        /// <summary>
        /// 标记模式
        /// </summary>
        private FlagMode flagMode;
        /// <summary>
        /// 当前文档
        /// </summary>
        private String document;
        /// <summary>
        /// 当前列
        /// </summary>
        private Int32 column;
        /// <summary>
        /// 当前行
        /// </summary>
        private Int32 line;
        /// <summary>
        /// 当前TokenKind
        /// </summary>
        private TokenKind kind;
        /// <summary>
        /// 起始列
        /// </summary>
        private Int32 startColumn;
        /// <summary>
        /// 起始行
        /// </summary>
        private Int32 startLine;
        /// <summary>
        /// 扫描器
        /// </summary>
        private CharScanner scanner;
        /// <summary>
        /// token集合
        /// </summary>
        private List<Token> collection;

        /// <summary>
        /// 
        /// </summary>
        private Stack<String> pos;

        /// <summary>
        /// 简写标签标记
        /// </summary>
        private Char tagFlag;

        /// <summary>
        /// 完整标签前缀
        /// </summary>
        private String tagPrefix;

        /// <summary>
        /// 完整标签后缀
        /// </summary>
        private String tagSuffix;

        /// <summary>
        /// TemplateLexer
        /// </summary>
        /// <param name="text">待分析内容</param>
        /// <param name="flag">简写标记</param>
        /// <param name="prefix">标签前缀</param>
        /// <param name="suffix">标签内容</param>
        public TemplateLexer(String text, Char flag, String prefix, String suffix)
        {
            this.document = text;
            this.tagFlag = flag;
            this.tagPrefix = prefix;
            this.tagSuffix = suffix;

            Reset();
        }
        /// <summary>
        /// 重置分析器
        /// </summary>
        public void Reset()
        {
            this.flagMode = FlagMode.None;
            this.line = 1;
            this.column = 1;
            this.kind = TokenKind.Text;
            this.startColumn = 1;
            this.startLine = 1;
            this.scanner = new CharScanner(this.document);
            this.collection = new List<Token>();
            this.pos = new Stack<String>();
        }


        private Token GetToken(TokenKind tokenKind)
        {

            Token token;
            if (tokenKind == TokenKind.StringEnd)
            {
                token = new Token(this.kind, this.scanner.GetEscapeString());
            }
            else
            {
                token = new Token(this.kind, this.scanner.GetString());
            }
            token.BeginLine = this.startLine;
            token.BeginColumn = this.startColumn;
            token.EndColumn = this.column;
            token.BeginLine = this.line;
            this.kind = tokenKind;
            this.startColumn = this.column;
            this.startLine = this.line;
            return token;
        }

        private Boolean Next()
        {
            return Next(1);
        }
        private Boolean Next(Int32 i)
        {
            if (this.scanner.Next(i))
            {
                if (this.scanner.Read() == '\n')
                {
                    this.line++;
                    this.column = 1;
                }
                else
                {
                    this.column++;
                }
                return true;
            }

            return false;
        }

        private Boolean IsTagStart()
        {
            if (this.scanner.IsEnd() || this.flagMode != FlagMode.None)
            {
                return false;
            }
            Boolean find = true;
            for (Int32 i = 0; i < this.tagPrefix.Length; i++)
            {
                if (this.tagPrefix[i] != this.scanner.Read(i))
                {
                    find = false;
                    break;
                }
            }
            if (find)
            {
                this.flagMode = FlagMode.Full;
                return true;
            }
            if (this.scanner.Read() == this.tagFlag)
            {
#if ALLOWCOMMENT
                if (this.scanner.Read(1) == '*')
                {
                    this.flagMode = FlagMode.Comment;
                    return true;
                }
                else
#endif
                    if (Char.IsLetter(this.scanner.Read(1)))
                    {
                        this.flagMode = FlagMode.Logogram;
                        return true;
                    }
            }
            return false;
        }

        private Boolean IsTagEnd()
        {
            if (this.flagMode != FlagMode.None && this.pos.Count == 0)
            {
                if (this.scanner.IsEnd())
                {
                    return true;
                }

                Char t = this.scanner.Read();

                if (this.scanner.Read() != '.')
                {
                    if (this.flagMode == FlagMode.Full)
                    {
                        for (Int32 i = 0; i < this.tagSuffix.Length; i++)
                        {
                            if (this.tagSuffix[i] != this.scanner.Read(i))
                            {
                                return false;
                            }
                        }

                        return true;
                    }
#if ALLOWCOMMENT
                    else if (this.flagMode == FlagMode.Comment)
                    {
                        return this.scanner.Read() == '*' && this.scanner.Read(1) == this.tagFlag;
                    }
#endif
                    else
                    {
                        Char value = this.scanner.Read();
                        if (((value == '(' || Common.ParserHelpers.IsWord(value)) && Common.ParserHelpers.IsWord(this.scanner.Read(-1)))
                        || (Common.ParserHelpers.IsWord(value) && (this.scanner.Read(-1) == '.')))
                        {
                            return false;
                        }
                        return true;
                    }
                }
                //else if (value != '.' && value != '(')
                //{
                //    if (Char.IsControl(value) || (Char.IsPunctuation(value) && value != '_') || Char.IsSeparator(value) || Char.IsSymbol(value) || Char.IsWhiteSpace(value) || (Int32)value > 167)
                //    {
                //        return true;
                //    }
                //}

            }
            return false;
        }
        /// <summary>
        /// 分析所有Token
        /// </summary>
        /// <returns></returns>
        public Token[] Parse()
        {
            if (this.kind != TokenKind.EOF)
            {
                do
                {
                    if (this.flagMode != FlagMode.None)
                    {
#if ALLOWCOMMENT
                        if (this.flagMode == FlagMode.Comment)
                        {
                            Next(1);
                            GetToken(TokenKind.TextData);
                            ReadCommentToken();
                        }
#else
                        if (false)
                        {

                        }
#endif
                        else
                        {
                            if (this.flagMode == FlagMode.Full)
                            {
                                Next(this.tagPrefix.Length - 1);
                            }

                            AddToken(GetTokenKind(this.scanner.Read()));
                            switch (this.kind)
                            {
                                case TokenKind.StringStart:
                                    this.pos.Push("\"");
                                    break;
                                case TokenKind.LeftParentheses:
                                    this.pos.Push("(");
                                    break;
                            }
                            ReadToken();
                        }

                    }
                    else if (IsTagStart())
                    {
                        AddToken(TokenKind.TagStart);
                    }
                }
                while (Next());

                AddToken(TokenKind.EOF);


                if (this.flagMode != FlagMode.None)
                {
                    this.flagMode = FlagMode.None;
                    AddToken(new Token(TokenKind.TagEnd, String.Empty));
                }

            }

            return this.collection.ToArray();

        }

        private void AddToken(TokenKind kind)
        {
            Token token = GetToken(kind);
            AddToken(token);
        }

        private void AddToken(Token token)
        {
            if (this.collection.Count > 0 && this.collection[this.collection.Count - 1].Next == null)
            {
                this.collection[this.collection.Count - 1].Next = token;
            }
            this.collection.Add(token);
        }


        private Boolean ReadEndToken()
        {
            if (IsTagEnd())
            {
                Boolean add = true;
                if (this.flagMode == FlagMode.Full)
                {
                    AddToken(TokenKind.TagEnd);
                    Next(this.tagSuffix.Length);
                }
#if ALLOWCOMMENT
                else if (this.flagMode == FlagMode.Comment)
                {
                    GetToken(TokenKind.TagEnd);
                    Next(2);
                    add = false;
                }
#endif
                else
                {
                    AddToken(TokenKind.TagEnd);
                }
                this.flagMode = FlagMode.None;
                Token token;

                if (IsTagStart())
                {
                    token = GetToken(TokenKind.TagStart);
                }
                else
                {
                    token = GetToken(TokenKind.Text);
                }

                if (add)
                {
                    AddToken(token);
                }

                return true;
            }
            return false;
        }

        private void ReadCommentToken()
        {
            while (Next())
            {
                if (ReadEndToken())
                {
                    break;
                }
            }
        }

        private int GetPrevCharCount(Char c)
        {
            int i = 1;
            while (this.scanner.Read(-i) == c)
            {
                i++;
            }
            return --i;
        }

        private void ReadToken()
        {
            while (Next())
            {
                if (this.scanner.Read() == '"')
                {
                    if (this.pos.Count > 0 && this.pos.Peek() == "\"")
                    {
                        if (this.scanner.Read(-1) != '\\'
                            || GetPrevCharCount('\\') % 2 == 0)
                        {
                            if (this.kind == TokenKind.StringStart)
                            {
                                AddToken(TokenKind.String);
                            }
                            AddToken(TokenKind.StringEnd);
                            this.pos.Pop();
                        }
                        continue;
                    }

                    if (this.kind == TokenKind.TagStart
                        || this.kind == TokenKind.LeftBracket
                        || this.kind == TokenKind.LeftParentheses
                        || this.kind == TokenKind.Operator
                        || this.kind == TokenKind.Punctuation
                        || this.kind == TokenKind.Comma
                        || this.kind == TokenKind.Space)
                    {
                        AddToken(TokenKind.StringStart);
                        this.pos.Push("\"");
                        continue;
                    }
                }

                if (this.kind == TokenKind.StringStart)
                {
                    AddToken(TokenKind.String);
                    continue;
                }

                if (this.kind == TokenKind.String)
                {
                    continue;
                }

                if (this.scanner.Read() == '(')
                {
                    this.pos.Push("(");
                }
                else if (this.scanner.Read() == ')' && this.pos.Count > 0 && this.pos.Peek() == "(")// && this.pos.Count > 2
                {
                    this.pos.Pop();
                    if (this.pos.Count == 1)
                    {

                    }
                }
                else if (ReadEndToken())
                {
                    break;
                }

                TokenKind tk;
                if (this.scanner.Read() == '+' || this.scanner.Read() == '-') //正负数符号识别
                {
                    if (Char.IsNumber(this.scanner.Read(1)) &&
                        (this.kind == TokenKind.Operator || this.kind == TokenKind.LeftParentheses))
                    {
                        tk = TokenKind.Number;
                    }
                    else
                    {
                        tk = TokenKind.Operator;
                    }
                }
                else
                {
                    tk = GetTokenKind(this.scanner.Read());
                }
                //if (this.kind == tk || (tk == TokenKind.Number && this.kind == TokenKind.TextData))
                if ((this.kind != tk || this.kind == TokenKind.LeftParentheses || this.kind == TokenKind.RightParentheses)
                    && (tk != TokenKind.Number || this.kind != TokenKind.TextData)
                    //&& (this.kind == TokenKind.Number && tk != TokenKind.Dot)
                    )
                //|| (this.kind != TokenKind.Number && tk == TokenKind.Dot)
                {
                    if (tk == TokenKind.Dot && this.kind == TokenKind.Number)
                    {

                    }
                    else
                    {
                        AddToken(tk);
                    }
                }

            }
        }


        //private TokenKind GetTokenKind()
        //{
        //    return GetTokenKind(this.scanner.Read());
        //}

        private TokenKind GetTokenKind(Char c)
        {
            if (this.flagMode == FlagMode.None)
            {
                return TokenKind.Text;
            }
            switch (c)
            {
                case ' ':
                    return TokenKind.Space;
                case '(':
                    return TokenKind.LeftParentheses;
                case ')':
                    return TokenKind.RightParentheses;
                case '[':
                    return TokenKind.LeftBracket;
                case ']':
                    return TokenKind.RightBracket;

                case '0':
                case '1':
                case '2':
                case '3':
                case '4':
                case '5':
                case '6':
                case '7':
                case '8':
                case '9':
                    return TokenKind.Number;
                case '*':
                case '-':
                case '+':
                case '/':
                case '>':
                case '<':
                case '=':
                case '!':
                case '&':
                case '|':
                case '~':
                case '^':
                case '?':
                case '%':
                    return TokenKind.Operator;
                case ',':
                    return TokenKind.Comma;
                case '.':
                    return TokenKind.Dot;
                case '"':
                    return TokenKind.StringStart;
                case ';':
                    return TokenKind.Punctuation;
                default:
                    return TokenKind.TextData;
            }
        }

    }
}