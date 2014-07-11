/*****************************************************
 * 本类库的核心系 JNTemplate
 * 作者：翅膀的初衷 QQ:4585839
 * Mail: i@Jiniannet.com
 * 网址：http://www.JiNianNet.com
 *****************************************************/
using System;

namespace JinianNet.JNTemplate.Parser.Node
{

    public class Token : IComparable<Token>
    {
        private String _text;
        private Int32 _beginline;
        private Int32 _begincolumn;
        private Int32 _endline;
        private Int32 _endcolumn;
        private TokenKind _tokenkind;
        private Token _next;

        public Int32 BeginLine
        {
            get { return _beginline; }
            set { _beginline = value; }
        }

        public Int32 BeginColumn
        {
            get { return _begincolumn; }
            set { _begincolumn = value; }
        }

        public Int32 EndLine
        {
            get { return _endline; }
            set { _endline = value; }
        }

        public Int32 EndColumn
        {
            get { return _endcolumn; }
            set { _endcolumn = value; }
        }


        public String Text
        {
            get { return _text; }
        }

        public TokenKind TokenKind
        {
            get { return _tokenkind; }
        }

        public Token(TokenKind kind, String text)
        {
            this._tokenkind = kind;
            this._text = text;
        }

        public Token Next
        {
            get { return _next; }
            set { _next = value; }
        }

        public override String ToString()
        {
            return this.Text;
        }

        #region IComparable<Token> 成员

        public int CompareTo(Token other)
        {
            if (this.BeginLine > other.BeginLine)
                return 1;
            if (this.BeginLine < other.BeginLine)
                return -1;
            if (this.BeginColumn > other.BeginColumn)
                return 1;
            if (this.BeginColumn < other.BeginColumn)
                return -1;
            return 0;
        }

        #endregion
    }

}