/********************************************************************************
 Copyright (c) jiniannet (http://www.jiniannet.com). All rights reserved.
 Licensed under the MIT license. See licence.txt file in the project root for full license information.
 ********************************************************************************/
#define ALLOWCOMMENT
using System;
using System.Collections.Generic;
using JinianNet.JNTemplate.Nodes;
using System.Collections;

namespace JinianNet.JNTemplate
{
    /// <summary>
    /// 词素分析器
    /// </summary>
    public class TemplateLexer : IEnumerable<Token>, IEnumerator<Token>
    {
        /// <summary>
        /// 标记模式
        /// </summary>
        private FlagMode _flagMode;
        /// <summary>
        /// 当前文档
        /// </summary>
        private string _document;
        /// <summary>
        /// 当前列
        /// </summary>
        private int _column;
        /// <summary>
        /// 当前行
        /// </summary>
        private int _line;
        /// <summary>
        /// 当前TokenKind
        /// </summary>
        private TokenKind _kind;
        /// <summary>
        /// 起始列
        /// </summary>
        private int _startColumn;
        /// <summary>
        /// 起始行
        /// </summary>
        private int _startLine;
        /// <summary>
        /// 扫描器
        /// </summary>
        private CharScanner _scanner;
        /// <summary>
        /// token集合
        /// </summary>
        private List<Token> _collection;
        /// <summary>
        /// 当前TOKEN
        /// </summary>
        private Token _token;

        /// <summary>
        /// 符号集合
        /// </summary>
        private Stack<string> _pos;
        private string _prefix;
        private char _flag;
        private string _suffix;

        /// <summary>
        /// TemplateLexer
        /// </summary>
        /// <param name="text">待分析内容</param>
        public TemplateLexer(string text)
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
            this._pos = new Stack<string>();
        }

        /// <summary>
        /// 当前TOKEN
        /// </summary>
        public Token Current
        {
            get { return _token; }
            set { _token = value; }
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
                text = this._scanner.GetEscapeString();
            }
            else
            {
                text = this._scanner.GetEscapeString();
            }

            Token token = new Token(this._kind, text ?? "");
            this._kind = tokenKind;
            if (token.TokenKind == TokenKind.Text && text == null)
            {
                return null;
            }
            token.BeginLine = this._startLine;
            token.BeginColumn = this._startColumn;
            token.EndColumn = this._column;
            token.BeginLine = this._line;
            this._startColumn = this._column;
            this._startLine = this._line;
            return token;
        }

        private bool Move()
        {
            return Move(1);
        }
        private bool Move(int i)
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

        private bool IsTagStart()
        {
            if (this._scanner.IsEnd() || this._flagMode != FlagMode.None)
            {
                return false;
            }
            bool find = true;
            for (int i = 0; i < this._prefix.Length; i++)
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
                    if (char.IsLetter(this._scanner.Read(1)))
                {
                    this._flagMode = FlagMode.Logogram;
                    return true;
                }
            }
            return false;
        }

        private bool IsTagEnd()
        {
            if (this._flagMode != FlagMode.None && this._pos.Count == 0)
            {
                if (this._scanner.IsEnd())
                {
                    return true;
                }
                if (this._scanner.Read() != '.' && this._kind != TokenKind.TagStart)
                {
                    if (this._flagMode == FlagMode.Full)
                    {
                        for (int i = 0; i < this._suffix.Length; i++)
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
                        char value = this._scanner.Read();
                        char prev = this._scanner.Read(-1);
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
            while (this._scanner.Read(-i) == c)
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
                if (this._flagMode == FlagMode.None)
                {
                    if (IsTagStart())
                    {
                        token = GetToken(TokenKind.TagStart);
                        if (this._flagMode == FlagMode.Full && this._prefix.Length > 1)
                        {
                            Move(this._prefix.Length - 1);
                        }
#if ALLOWCOMMENT
                        else if (this._flagMode == FlagMode.Comment)
                        {
                            Move(1);
                        }
#endif
                        continue;
                    }
                    if (this._kind != TokenKind.TagEnd)
                    {
                        continue;
                    }
                }

                if (this._scanner.Read() == '"')
                {
                    if (this._pos.Count > 0 && this._pos.Peek() == "\"")
                    {
                        if (this._scanner.Read(-1) != '\\'
                            || GetPrevCharCount('\\') % 2 == 0)
                        {
                            if (this._kind == TokenKind.StringStart)
                            {
                                token = GetToken(TokenKind.String);
                            }
                            token = GetToken(TokenKind.StringEnd);
                            this._pos.Pop();
                        }
                        continue;
                    }
                    else
                    {
                        token = GetToken(TokenKind.StringStart);
                        this._pos.Push("\"");
                        continue;
                    }
                }

                if (this._kind == TokenKind.StringStart)
                {
                    token = GetToken(TokenKind.String);
                    continue;
                }

                if (this._kind == TokenKind.String)
                {
                    continue;
                }

                if (this._scanner.Read() == '(' || this._scanner.Read() == '[' || this._scanner.Read() == '{')
                {
                    this._pos.Push(this._scanner.Read().ToString());
                }
                else if (
                    (this._scanner.Read() == ')' && this._pos.Count > 0 && this._pos.Peek() == "(")
                    || (this._scanner.Read() == ']' && this._pos.Count > 0 && this._pos.Peek() == "[")
                    || (this._scanner.Read() == '}' && this._pos.Count > 0 && this._pos.Peek() == "{"))// && this.pos.Count > 2
                {
                    this._pos.Pop();
                }
                else if (IsTagEnd())
                {
                    token = GetToken(TokenKind.TagEnd);
                    if (this._flagMode == FlagMode.Full)
                    {
                        Move(this._suffix.Length - 1);
                    }
#if ALLOWCOMMENT
                    else if (this._flagMode == FlagMode.Comment)
                    {
                        Move(1);
                    }
#endif
                    else if (this._flagMode == FlagMode.Logogram)
                    {
                        Move(-1);
                    }
                    this._flagMode = FlagMode.None;
                    continue;
                }

                TokenKind tk;
                if (this._scanner.Read() == '+' || this._scanner.Read() == '-') //正负数符号识别
                {
                    if (char.IsNumber(this._scanner.Read(1)) &&
                        (this._kind != TokenKind.Number
                        && this._kind != TokenKind.RightBracket
                        && this._kind != TokenKind.RightParentheses
                        && this._kind != TokenKind.RightBrace
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
                if (
                    ((this._kind != tk || this._kind == TokenKind.LeftParentheses || this._kind == TokenKind.RightParentheses)
                    && (tk != TokenKind.Number || this._kind != TokenKind.TextData)
                    //&& (this.kind == TokenKind.Number && tk != TokenKind.Dot)
                    )
                    || (this._kind == tk && (tk == TokenKind.LeftBracket || tk == TokenKind.LeftParentheses || tk == TokenKind.LeftBrace || tk == TokenKind.RightBracket || tk == TokenKind.RightParentheses || tk == TokenKind.RightBrace))
                    )
                //|| (this.kind != TokenKind.Number && tk == TokenKind.Dot)
                {
                    if (tk == TokenKind.Dot && this._kind == TokenKind.Number)
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
                || (this._flagMode != FlagMode.None && (token = new Token(TokenKind.TagEnd, string.Empty)) != null)
                || (this._kind != TokenKind.EOF && (token = GetToken(TokenKind.EOF)) != null))
            {
                this.Current = token;
                return true;
            }

            return false;

        }
        private TokenKind GetTokenKind(char c)
        {
            if (this._flagMode == FlagMode.None)
            {
                return TokenKind.Text;
            }
            if (this._flagMode == FlagMode.Comment)
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
        /// 获取数组
        /// </summary>
        /// <returns></returns>
        public Token[] ToArray()
        {
            if (_collection.Count == 0)
            {
                while (MoveNext())
                {
                    if (this.Current != null)
                    {
                        if (this._collection.Count > 0 && this._collection[this._collection.Count - 1].Next == null)
                        {
                            this._collection[this._collection.Count - 1].Next = this.Current;
                        }
                        this._collection.Add(this.Current);
                    }
                }
            }
            return _collection.ToArray();
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
        /// 释主放资源
        /// </summary>
        public void Dispose()
        {

        }
    }
}