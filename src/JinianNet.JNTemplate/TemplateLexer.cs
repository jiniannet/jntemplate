/********************************************************************************
 Copyright (c) jiniannet (http://www.jiniannet.com). All rights reserved.
 Licensed under the MIT license. See licence.txt file in the project root for full license information.
 ********************************************************************************/
#define ALLOWCOMMENT
using System;
using System.Collections.Generic;
using JinianNet.JNTemplate.Nodes;

namespace JinianNet.JNTemplate
{
    /// <summary>
    /// 词素分析器
    /// </summary>
    public class TemplateLexer
    {
        /// <summary>
        /// 标记模式
        /// </summary>
        private FlagMode _flagMode;
        /// <summary>
        /// 当前文档
        /// </summary>
        private String _document;
        /// <summary>
        /// 当前列
        /// </summary>
        private Int32 _column;
        /// <summary>
        /// 当前行
        /// </summary>
        private Int32 _line;
        /// <summary>
        /// 当前TokenKind
        /// </summary>
        private TokenKind _kind;
        /// <summary>
        /// 起始列
        /// </summary>
        private Int32 _startColumn;
        /// <summary>
        /// 起始行
        /// </summary>
        private Int32 _startLine;
        /// <summary>
        /// 扫描器
        /// </summary>
        private CharScanner _scanner;
        /// <summary>
        /// token集合
        /// </summary>
        private List<Token> _collection;

        /// <summary>
        /// 
        /// </summary>
        private Stack<String> _pos;
        private String _prefix;
        private Char _flag;
        private String _suffix;

        /// <summary>
        /// TemplateLexer
        /// </summary>
        /// <param name="text">待分析内容</param>
        public TemplateLexer(String text)
        {
            this._document = text;
            this._prefix = Engine.GetEnvironmentVariable("TagPrefix");
            this._flag = Engine.GetEnvironmentVariable("TagFlag")[0];
            this._suffix = Engine.GetEnvironmentVariable("TagSuffix");

            Reset();
        }
        /// <summary>
        /// 重置分析器
        /// </summary>
        public void Reset()
        {
            this._flagMode = FlagMode.None;
            this._line = 1;
            this._column = 1;
            this._kind = TokenKind.Text;
            this._startColumn = 1;
            this._startLine = 1;
            this._scanner = new CharScanner(this._document);
            this._collection = new List<Token>();
            this._pos = new Stack<String>();
        }


        private Token GetToken(TokenKind tokenKind)
        {

            Token token;
            if (tokenKind == TokenKind.StringEnd)
            {
                token = new Token(this._kind, this._scanner.GetEscapeString());
            }
            else
            {
                token = new Token(this._kind, this._scanner.GetString());
            }
            token.BeginLine = this._startLine;
            token.BeginColumn = this._startColumn;
            token.EndColumn = this._column;
            token.BeginLine = this._line;
            this._kind = tokenKind;
            this._startColumn = this._column;
            this._startLine = this._line;
            return token;
        }

        private Boolean Next()
        {
            return Next(1);
        }
        private Boolean Next(Int32 i)
        {
            if (this._scanner.Next(i))
            {
                if (this._scanner.Read() == '\n')
                {
                    this._line++;
                    this._column = 1;
                }
                else
                {
                    this._column++;
                }
                return true;
            }

            return false;
        }

        private Boolean IsTagStart()
        {
            if (this._scanner.IsEnd() || this._flagMode != FlagMode.None)
            {
                return false;
            }
            Boolean find = true;
            for (Int32 i = 0; i < this._prefix.Length; i++)
            {
                if (this._prefix[i] != this._scanner.Read(i))
                {
                    find = false;
                    break;
                }
            }
            if (find)
            {
                this._flagMode = FlagMode.Full;
                return true;
            }
            if (this._scanner.Read() == this._flag)
            {
#if ALLOWCOMMENT
                if (this._scanner.Read(1) == '*')
                {
                    this._flagMode = FlagMode.Comment;
                    return true;
                }
                else
#endif
                    if (Char.IsLetter(this._scanner.Read(1)))
                {
                    this._flagMode = FlagMode.Logogram;
                    return true;
                }
            }
            return false;
        }

        private Boolean IsTagEnd()
        {
            if (this._flagMode != FlagMode.None && this._pos.Count == 0)
            {
                if (this._scanner.IsEnd())
                {
                    return true;
                }

                if (this._scanner.Read() != '.')
                {
                    if (this._flagMode == FlagMode.Full)
                    {
                        for (Int32 i = 0; i < this._suffix.Length; i++)
                        {
                            if (this._suffix[i] != this._scanner.Read(i))
                            {
                                return false;
                            }
                        }

                        return true;
                    }
#if ALLOWCOMMENT
                    else if (this._flagMode == FlagMode.Comment)
                    {
                        return this._scanner.Read() == '*' && this._scanner.Read(1) == this._flag;
                    }
#endif
                    else
                    {
                        Char value = this._scanner.Read();
                        if (((value == '(' || Common.Utility.AllowWord(value)) && Common.Utility.AllowWord(this._scanner.Read(-1)))
                        || (Common.Utility.AllowWord(value) && (this._scanner.Read(-1) == '.')))
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
            if (this._kind != TokenKind.EOF)
            {

                Token token;
                do
                {
                    if (this._flagMode != FlagMode.None)
                    {
#if ALLOWCOMMENT
                        if (this._flagMode == FlagMode.Comment)
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
                            if (this._flagMode == FlagMode.Full)
                            {
                                Next(this._prefix.Length - 1);
                            }
                            AddToken(GetTokenKind(this._scanner.Read()));
                            switch (this._kind)
                            {
                                case TokenKind.StringStart:
                                    this._pos.Push("\"");
                                    break;
                                case TokenKind.LeftParentheses:
                                    this._pos.Push("(");
                                    break;
                            }
                            ReadToken();
                        }

                    }
                    else if (IsTagStart())
                    {
                        token = GetToken(TokenKind.TagStart);
                        if (token.Text != "" && (this._collection.Count > 0 || !String.IsNullOrEmpty(token.Text.Trim())))
                        {
                            AddToken(token);
                        }
                    }
                }
                while (Next());

                token = GetToken(TokenKind.EOF);
                if (token.Text != "")
                {
                    AddToken(token);
                }


                if (this._flagMode != FlagMode.None)
                {
                    this._flagMode = FlagMode.None;
                    AddToken(new Token(TokenKind.TagEnd, String.Empty));
                }

            }

            return this._collection.ToArray();

        }

        private void AddToken(TokenKind kind)
        {
            Token token = GetToken(kind);
            AddToken(token);
        }

        private void AddToken(Token token)
        {
            if (this._collection.Count > 0 && this._collection[this._collection.Count - 1].Next == null)
            {
                this._collection[this._collection.Count - 1].Next = token;
            }
            this._collection.Add(token);
        }


        private Boolean ReadEndToken()
        {
            if (IsTagEnd())
            {
                Boolean add = true;
                if (this._flagMode == FlagMode.Full)
                {
                    AddToken(TokenKind.TagEnd);
                    Next(this._suffix.Length);
                }
#if ALLOWCOMMENT
                else if (this._flagMode == FlagMode.Comment)
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
                this._flagMode = FlagMode.None;
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
            while (this._scanner.Read(-i) == c)
            {
                i++;
            }
            return --i;
        }

        private void ReadToken()
        {
            while (Next())
            {
                if (this._scanner.Read() == '"')
                {
                    if (this._pos.Count > 0 && this._pos.Peek() == "\"")
                    {
                        if (this._scanner.Read(-1) != '\\'
                            || GetPrevCharCount('\\') % 2 == 0)
                        {
                            if (this._kind == TokenKind.StringStart)
                            {
                                AddToken(TokenKind.String);
                            }
                            AddToken(TokenKind.StringEnd);
                            this._pos.Pop();
                        }
                        continue;
                    }

                    if (this._kind == TokenKind.TagStart
                        || this._kind == TokenKind.LeftBracket
                        || this._kind == TokenKind.LeftParentheses
                        || this._kind == TokenKind.Operator
                        || this._kind == TokenKind.Punctuation
                        || this._kind == TokenKind.Comma
                        || this._kind == TokenKind.Space)
                    {
                        AddToken(TokenKind.StringStart);
                        this._pos.Push("\"");
                        continue;
                    }
                }

                if (this._kind == TokenKind.StringStart)
                {
                    AddToken(TokenKind.String);
                    continue;
                }

                if (this._kind == TokenKind.String)
                {
                    continue;
                }

                if (this._scanner.Read() == '(')
                {
                    this._pos.Push("(");
                }
                else if (this._scanner.Read() == ')' && this._pos.Count > 0 && this._pos.Peek() == "(")// && this.pos.Count > 2
                {
                    this._pos.Pop();
                    if (this._pos.Count == 1)
                    {

                    }
                }
                else if (ReadEndToken())
                {
                    break;
                }

                TokenKind tk;
                if (this._scanner.Read() == '+' || this._scanner.Read() == '-') //正负数符号识别
                {
                    if (Char.IsNumber(this._scanner.Read(1)) &&
                        (this._kind != TokenKind.Number
                        && this._kind != TokenKind.RightBracket
                        && this._kind != TokenKind.RightParentheses
                        && this._kind != TokenKind.String
                        && this._kind != TokenKind.Tag
                        && this._kind != TokenKind.TextData))
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
                    tk = GetTokenKind(this._scanner.Read());
                }
                //if (this.kind == tk || (tk == TokenKind.Number && this.kind == TokenKind.TextData))
                if ((this._kind != tk || this._kind == TokenKind.LeftParentheses || this._kind == TokenKind.RightParentheses)
                    && (tk != TokenKind.Number || this._kind != TokenKind.TextData)
                    //&& (this.kind == TokenKind.Number && tk != TokenKind.Dot)
                    )
                //|| (this.kind != TokenKind.Number && tk == TokenKind.Dot)
                {
                    if (tk == TokenKind.Dot && this._kind == TokenKind.Number)
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
            if (this._flagMode == FlagMode.None)
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