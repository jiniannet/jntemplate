/*****************************************************
 * 本类库的核心系 JNTemplate
 * 作者：翅膀的初衷 QQ:4585839
 * Mail: i@Jiniannet.com
 * 网址：http://www.JiNianNet.com
 *****************************************************/
using System;

namespace JinianNet.JNTemplate.Parser.Node
{
    public class Element
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

        private ElementType _elementtype;
        public ElementType ElementType
        {
            get { return _elementtype; }
            set { _elementtype = value; }
        }

        public Element(ElementType type, Int32 line, Int32 col)
        {
            this.Line = line;
            this.Column = col;
            this.ElementType = type;
        }
    }
}