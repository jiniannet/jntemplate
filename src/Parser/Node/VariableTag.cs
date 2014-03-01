/*****************************************************
 * 本类库的核心系 JNTemplate
 * 作者：翅膀的初衷 QQ:4585839
 * Mail: i@Jiniannet.com
 *****************************************************/

using System;

namespace JinianNet.JNTemplate.Parser.Node
{
    public class VariableTag : SimpleTag
    {
        public VariableTag(String name, Int32 line, Int32 col)
            : base(ElementType.Var, line, col)
        {
            this.Name = name;
        }

        private String _name;
        public String Name 
        { 
            get { return _name; }
            private set { _name = value; }
        }

        public override Object Parse(TemplateContext context)
        {
            return ParserAccessor.Eval(context.TempData, this.Name);
        }
    }
}