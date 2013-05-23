/*****************************************************
 * 本类库的核心系 JNTemplate
 * 作者：翅膀的初衷 QQ:4585839
 * Mail: i@Jiniannet.com
 * 网址：http://www.JiNianNet.com
 *****************************************************/
using System;

namespace JinianNet.JNTemplate.Parser.Node
{

    public class Token
    {
        private Int32 _column;
        public Int32 Column
        {
            get { return _column; }
            set { _column = value; }
        }

        private Int32 _line;
        public Int32 Line
        {
            get { return _line; }
            set { _line = value; }
        }
        private TokenKind _tokenkind;
        public TokenKind TokenKind
        {
            get { return _tokenkind; }
            set { _tokenkind = value; }
        }

        private String _text;
        public String Text
        {
            get { return _text; }
        }

        public Token(TokenKind kind, String text)
        {
            this._tokenkind = kind;
            this._text = text;
        }

        public override String ToString()
        {
            return this.Text;
        }
    }

}