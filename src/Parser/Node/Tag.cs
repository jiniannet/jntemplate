/*****************************************************
 * 本类库的核心系 JNTemplate
 * 作者：翅膀的初衷 QQ:4585839
 * Mail: i@Jiniannet.com
 * 网址：http://www.JiNianNet.com
 *****************************************************/
using System;

namespace JinianNet.JNTemplate.Parser.Node
{
    public abstract class Tag : Element
    {
        public Tag(ElementType type, Int32 line, Int32 col)
            : base(type, line, col)
        {
            
        }

        public abstract Object Parse(VariableScope vars);

        public abstract void Parse(VariableScope vars,System.IO.TextWriter write);

    }
}