/*****************************************************
 * 本类库的核心系 JNTemplate
 * 作者：翅膀的初衷 QQ:4585839
 * Mail: i@Jiniannet.com
 * 网址：http://www.JiNianNet.com
 *****************************************************/
using System;
using System.Collections.Generic;
using JinianNet.JNTemplate.Parser.Node;

namespace JinianNet.JNTemplate.Parser
{
    public class TemplateLexer
    {
        /// <summary>
        /// 状态
        /// </summary>
        private LexerMode mode;
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

        private List<Token> collection;

        private Stack<String> pos;

        public TemplateLexer(String text)
        {
            this.document = text;
            Reset();
        }

        public void Reset()
        {
            this.mode = LexerMode.None;
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
            Token _token = new Token(this.kind, this.scanner.GetString());
            _token.Line = this.startLine;
            _token.Column = this.startColumn;
            this.kind = tokenKind;
            this.startColumn = this.column;
            this.startLine = this.line;
            return _token;
        }

        private bool Next()
        {
            return Next(1);
        }
        private bool Next(Int32 i)
        {
            if (this.scanner.Next(i))
            {
                this.column += i;
                return true;
            }

            return false;
        }

        private bool IsTagStart()
        {
            if (this.scanner.Read() == '$')
            {
                if (!this.scanner.IsEnd())
                {
                    Char value = this.scanner.Read(1);
                    if (value == '{')
                    {
                        this.pos.Push("${");
                        return true;
                    }
                    if (Char.IsLetter(value))
                    {
                        this.pos.Push("$");
                        return true;
                    }
                }
            }
            return false;
        }

        private bool IsTagEnd()
        {
            if (this.pos.Count == 1)
            {
                if (!this.scanner.IsEnd())
                {
                    Char value = this.scanner.Read();
                    if (this.pos.Peek().Length == 2)
                    {
                        if (value == '}')
                        {
                            //this.pos.Pop();
                            return true;
                        }
                    }
                    else if (value != '.' && value != '(')
                    {
                        if (Char.IsControl(value) || Char.IsPunctuation(value) || Char.IsSeparator(value) || Char.IsSymbol(value) || Char.IsWhiteSpace(value))
                        {
                            //this.pos.Pop();
                            return true;
                        }
                    }
                }
            }
            return false;
        }

        public Token[] Parse()
        {
            if (this.kind != TokenKind.EOF)
            {
                do
                {
                    //if (this.scanner.Read() == '\n')
                    //{
                    //    this.line++;
                    //    this.column = 1;
                    //}
                    //else if (IsTagStart())
                    //{
                    //    this.collection.Add(GetToken(TokenKind.TagStart));
                    //    this.mode = LexerMode.EnterLabel;
                    //    Next(this.pos.Peek().Length);
                    //    this.collection.Add(GetToken(GetTokenKind(this.scanner.Read())));
                    //    ReadToken();
                    //}
                    if (this.mode == LexerMode.EnterLabel)
                    {
                        Next(this.pos.Peek().Length - 1);
                        this.collection.Add(GetToken(GetTokenKind(this.scanner.Read())));
                        ReadToken();
                    }
                    else if (IsTagStart())
                    {
                        this.collection.Add(GetToken(TokenKind.TagStart));
                        this.mode = LexerMode.EnterLabel;

                    }
                    else if (this.scanner.Read() == '\n')
                    {
                        this.line++;
                        this.column = 1;
                    }
                }
                while (Next());

                this.collection.Add(GetToken(TokenKind.EOF));


                if (this.mode == LexerMode.EnterLabel)
                {
                    this.mode = LexerMode.LeaveLabel;
                    this.collection.Add(new Token(TokenKind.TagEnd, String.Empty));
                }

            }

            return this.collection.ToArray();

        }

        private void ReadToken()
        {
            while (Next())
            {
                if (this.scanner.Read() == '"' && this.pos.Count > 1)
                {
                    if (this.pos.Peek() == "\"")
                    {
                        if (this.kind == TokenKind.StringStart)
                        {
                            this.collection.Add(GetToken(TokenKind.String));
                        }
                        this.collection.Add(GetToken(TokenKind.StringEnd));
                        this.pos.Pop();
                        //Next(1);
                        //GetToken(GetTokenKind());

                    }
                    else
                    {
                        this.collection.Add(GetToken(TokenKind.StringStart));
                        this.pos.Push("\"");
                        //Next(1);
                        //this.collection.Add(GetToken(TokenKind.String));

                    }
                }
                else
                {
                    if (this.kind == TokenKind.StringStart)
                    {
                        this.collection.Add(GetToken(TokenKind.String));
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
                    else if (this.scanner.Read() == ')' && this.pos.Peek() == "(")// && this.pos.Count > 2
                    {
                        this.pos.Pop();
                        if (this.pos.Count == 1)
                        {

                        }
                    }
                    else if (IsTagEnd())
                    {
                        //Next(1);
                        //this.pos.Pop();
                        this.collection.Add(GetToken(TokenKind.TagEnd));
                        this.mode = LexerMode.LeaveLabel;
                        if (this.pos.Pop().Length == 2)
                        {
                            Next(1);
                        }
                        if (IsTagStart())
                        {
                            this.collection.Add(GetToken(TokenKind.TagStart));
                            this.mode = LexerMode.EnterLabel;
                        }
                        else
                        {
                            this.collection.Add(GetToken(GetTokenKind(this.scanner.Read())));
                        }
                        break;
                    }

                    TokenKind tk = GetTokenKind(this.scanner.Read());
                    //if (this.kind == tk || (tk == TokenKind.Number && this.kind == TokenKind.TextData))
                    if ((this.kind != tk || this.kind == TokenKind.LeftParentheses || this.kind == TokenKind.RightParentheses) && (tk != TokenKind.Number || this.kind != TokenKind.TextData))
                    {
                        this.collection.Add(GetToken(tk));
                    }
                }
            }
        }


        private TokenKind GetTokenKind()
        {
            return GetTokenKind(this.scanner.Read());
        }

        private TokenKind GetTokenKind(Char c)
        {
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
                default:
                    //if (Char.IsLetter(c) || Char.IsControl(c) || Char.IsSeparator(c))
                    //    return this.mode == LexerMode.EnterLabel ? TokenKind.TextData : TokenKind.Text;
                    //else if (Char.IsNumber(c))
                    //    return TokenKind.Number;
                    //else if (Char.IsPunctuation(c))
                    //    return TokenKind.Punctuation;
                    //else if (Char.IsSymbol(c))
                    //    return TokenKind.Operator;
                    //else if (Char.IsWhiteSpace(c))
                    //    return TokenKind.Space;
                    return this.mode == LexerMode.EnterLabel ? TokenKind.TextData : TokenKind.Text;
            }
        }

    }
}
