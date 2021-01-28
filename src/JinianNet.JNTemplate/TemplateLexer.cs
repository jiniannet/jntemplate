/********************************************************************************
 Copyright (c) jiniannet (http://www.jiniannet.com). All rights reserved.
 Licensed under the MIT license. See licence.txt file in the project root for full license information.
 ********************************************************************************/
#define ALLOWCOMMENT
using System;
using System.Collections.Generic;
using JinianNet.JNTemplate.Nodes;
using System.Collections;
using JinianNet.JNTemplate.Configuration;

namespace JinianNet.JNTemplate
{
    /// <summary>
    /// 词素分析器
    /// </summary>
    public class TemplateLexer : Executer<Token[]>, IEnumerable<Token>, IEnumerator<Token>
    {
        /// <summary>
        /// 标记模式
        /// </summary>
        private FlagMode flagMode;
        /// <summary>
        /// 当前文档
        /// </summary>
        private string document;
        /// <summary>
        /// 当前列
        /// </summary>
        private int column;
        /// <summary>
        /// 当前行
        /// </summary>
        private int line;
        /// <summary>
        /// 当前TokenKind
        /// </summary>
        private TokenKind kind;
        /// <summary>
        /// 起始列
        /// </summary>
        private int startColumn;
        /// <summary>
        /// 起始行
        /// </summary>
        private int startLine;
        /// <summary>
        /// 扫描器
        /// </summary>
        private CharScanner scanner;
        /// <summary>
        /// token集合
        /// </summary>
        private List<Token> collection;
        /// <summary>
        /// 当前TOKEN
        /// </summary>
        private Token token;

        /// <summary>
        /// 符号集合
        /// </summary>
        private Stack<string> pos;
        private string prefix;
        private char flag;
        private string suffix;

        /// <summary>
        /// TemplateLexer
        /// </summary>
        /// <param name="text">待分析内容</param>
        public TemplateLexer(string text)
        {
            this.document = text;
            this.prefix = Runtime.GetEnvironmentVariable(nameof(IConfig.TagPrefix));
            this.flag = Runtime.GetEnvironmentVariable(nameof(IConfig.TagFlag))[0];
            this.suffix = Runtime.GetEnvironmentVariable(nameof(IConfig.TagSuffix));
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
            this.pos = new Stack<string>();
        }

        /// <summary>
        /// 当前TOKEN
        /// </summary>
        public Token Current
        {
            get { return token; }
            set { token = value; }
        }

        object IEnumerator.Current
        {
            get
            {
                return this.Current;
            }
        }

        private Token GetToken(TokenKind tokenKind)
        {

            string text;
            if (tokenKind == TokenKind.StringEnd)
            {
                text = this.scanner.GetEscapeString();
            }
            else
            {
                text = this.scanner.GetEscapeString();
            }

            Token token = new Token(this.kind, text ?? "");
            this.kind = tokenKind;
            if (token.TokenKind == TokenKind.Text && text == null)
            {
                return null;
            }
            token.BeginLine = this.startLine;
            token.BeginColumn = this.startColumn;
            token.EndColumn = this.column;
            token.BeginLine = this.line;
            this.startColumn = this.column;
            this.startLine = this.line;
            return token;
        }

        private bool Move()
        {
            return Move(1);
        }
        private bool Move(int i)
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

        private bool IsTagStart()
        {
            if (this.scanner.IsEnd() || this.flagMode != FlagMode.None)
            {
                return false;
            }
            bool find = true;
            for (int i = 0; i < this.prefix.Length; i++)
            {
                if (this.prefix[i] != this.scanner.Read(i))
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
            if (this.scanner.Read() == this.flag)
            {
#if ALLOWCOMMENT
                if (this.scanner.Read(1) == '*')
                {
                    this.flagMode = FlagMode.Comment;
                    return true;
                }
                else
#endif
                    if (char.IsLetter(this.scanner.Read(1)))
                {
                    this.flagMode = FlagMode.Logogram;
                    return true;
                }
            }
            return false;
        }

        private bool IsTagEnd()
        {
            if (this.flagMode != FlagMode.None && this.pos.Count == 0)
            {
                if (this.scanner.IsEnd())
                {
                    return true;
                }
                if (this.scanner.Read() != '.' && this.kind != TokenKind.TagStart)
                {
                    if (this.flagMode == FlagMode.Full)
                    {
                        for (int i = 0; i < this.suffix.Length; i++)
                        {
                            if (this.suffix[i] != this.scanner.Read(i))
                            {
                                return false;
                            }
                        }

                        return true;
                    }
#if ALLOWCOMMENT
                    else if (this.flagMode == FlagMode.Comment)
                    {
                        return this.scanner.Read() == '*' && this.scanner.Read(1) == this.flag;
                    }
#endif
                    else
                    {
                        char value = this.scanner.Read();
                        char prev = this.scanner.Read(-1);
                        if (((value == '(' || Utility.AllowWord(value) || value == '[') && Utility.AllowWord(prev))
                        || (Utility.AllowWord(value) && (prev == '.')))
                        {
                            return false;
                        }
                        return true;
                    }
                }

            }
            return false;
        }


        private int GetPrevCharCount(char c)
        {
            int i = 1;
            while (this.scanner.Read(-i) == c)
            {
                i++;
            }
            return --i;
        }
        /// <summary>
        /// 获取下一个TOKEN
        /// </summary>
        /// <returns></returns>
        public bool MoveNext()
        {
            Token token = null;
            do
            {
                //处于非标签模式
                if (this.flagMode == FlagMode.None)
                {
                    if (IsTagStart())
                    {
                        token = GetToken(TokenKind.TagStart);
                        if (this.flagMode == FlagMode.Full && this.prefix.Length > 1)
                        {
                            Move(this.prefix.Length - 1);
                        }
#if ALLOWCOMMENT
                        else if (this.flagMode == FlagMode.Comment)
                        {
                            Move(1);
                        }
#endif
                        continue;
                    }
                    if (this.kind != TokenKind.TagEnd)
                    {
                        continue;
                    }
                }

                if (this.scanner.Read() == '"')
                {
                    if (this.pos.Count > 0 && this.pos.Peek() == "\"")
                    {
                        if (this.scanner.Read(-1) != '\\'
                            || GetPrevCharCount('\\') % 2 == 0)
                        {
                            //if (this.kind == TokenKind.StringStart)
                            //{
                            //    token = GetToken(TokenKind.String);
                            //}
                            token = GetToken(TokenKind.StringEnd);
                            this.pos.Pop();
                        }
                        continue;
                    }
                    else
                    {
                        if (this.kind == TokenKind.TagStart
                            || this.kind == TokenKind.LeftBracket
                           || this.kind == TokenKind.LeftParentheses
                           || this.kind == TokenKind.Operator
                           || this.kind == TokenKind.Punctuation
                           || this.kind == TokenKind.Comma
                           || this.kind == TokenKind.Colon
                           || this.kind == TokenKind.LeftBrace
                           || this.kind == TokenKind.Space)
                        {
                            token = GetToken(TokenKind.StringStart);
                            this.pos.Push("\"");
                            continue;
                        }

                    }
                }

                if (this.kind == TokenKind.StringStart)
                {
                    token = GetToken(TokenKind.String);
                    continue;
                }

                if (this.kind == TokenKind.String)
                {
                    continue;
                }

                if (this.scanner.Read() == '(' || this.scanner.Read() == '[' || this.scanner.Read() == '{')
                {
                    this.pos.Push(this.scanner.Read().ToString());
                }
                else if (
                    (this.scanner.Read() == ')' && this.pos.Count > 0 && this.pos.Peek() == "(")
                    || (this.scanner.Read() == ']' && this.pos.Count > 0 && this.pos.Peek() == "[")
                    || (this.scanner.Read() == '}' && this.pos.Count > 0 && this.pos.Peek() == "{"))// && this.pos.Count > 2
                {
                    this.pos.Pop();
                }
                else if (IsTagEnd())
                {
                    token = GetToken(TokenKind.TagEnd);
                    if (this.flagMode == FlagMode.Full)
                    {
                        Move(this.suffix.Length - 1);
                    }
#if ALLOWCOMMENT
                    else if (this.flagMode == FlagMode.Comment)
                    {
                        Move(1);
                    }
#endif
                    else if (this.flagMode == FlagMode.Logogram)
                    {
                        Move(-1);
                    }
                    this.flagMode = FlagMode.None;
                    continue;
                }

                TokenKind tk;
                if (this.scanner.Read() == '+' || this.scanner.Read() == '-') //正负数符号识别
                {
                    if (char.IsNumber(this.scanner.Read(1)) &&
                        (this.kind != TokenKind.Number
                        && this.kind != TokenKind.RightBracket
                        && this.kind != TokenKind.RightParentheses
                        && this.kind != TokenKind.RightBrace
                        && this.kind != TokenKind.String
                        && this.kind != TokenKind.Tag
                        && this.kind != TokenKind.TextData))
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
                if (
                    ((this.kind != tk || this.kind == TokenKind.LeftParentheses || this.kind == TokenKind.RightParentheses)
                    && (tk != TokenKind.Number || this.kind != TokenKind.TextData)
                    //&& (this.kind == TokenKind.Number && tk != TokenKind.Dot)
                    )
                    || (this.kind == tk && (tk == TokenKind.LeftBracket || tk == TokenKind.LeftParentheses || tk == TokenKind.LeftBrace || tk == TokenKind.RightBracket || tk == TokenKind.RightParentheses || tk == TokenKind.RightBrace))
                    )
                //|| (this.kind != TokenKind.Number && tk == TokenKind.Dot)
                {
                    if (tk == TokenKind.Dot && this.kind == TokenKind.Number)
                    {

                    }
                    else
                    {
                        token = GetToken(tk);
                    }
                }

            }
            while (Move() && token == null);

            if (token != null
                || (this.flagMode != FlagMode.None && (token = new Token(TokenKind.TagEnd, string.Empty)) != null)
                || (this.kind != TokenKind.EOF && (token = GetToken(TokenKind.EOF)) != null))
            {
                this.Current = token;
                return true;
            }

            return false;

        }
        private TokenKind GetTokenKind(char c)
        {
            if (this.flagMode == FlagMode.None)
            {
                return TokenKind.Text;
            }
            if (this.flagMode == FlagMode.Comment)
            {
                return TokenKind.Comment;
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
                case '{':
                    return TokenKind.LeftBrace;
                case '}':
                    return TokenKind.RightBrace;


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
                case ':':
                    return TokenKind.Colon;
                default:
                    return TokenKind.TextData;
            }
        } 

        /// <summary>
        /// 获取IEnumerator
        /// </summary>
        /// <returns></returns>
        public IEnumerator<Token> GetEnumerator()
        {
            return this;
        }
        /// <summary>
        /// 获取IEnumerator
        /// </summary>
        /// <returns></returns>
        IEnumerator IEnumerable.GetEnumerator()
        {
            return this.GetEnumerator();
        }
        /// <summary>
        /// 释放资源
        /// </summary>
        public void Dispose()
        {

        }

        /// <summary>
        /// 执行解析
        /// </summary>
        /// <returns></returns>
        public override Token[] Execute()
        {
            if (collection.Count == 0)
            {
                while (MoveNext())
                {
                    if (this.Current != null)
                    {
                        if (this.collection.Count > 0 && this.collection[this.collection.Count - 1].Next == null)
                        {
                            this.collection[this.collection.Count - 1].Next = this.Current;
                        }
                        this.collection.Add(this.Current);
                    }
                }
            }
            return collection.ToArray();
        }
    }
}