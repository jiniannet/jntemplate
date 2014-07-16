/*****************************************************
 * 本类库的核心系 JNTemplate
 * 作者：翅膀的初衷 QQ:4585839
 * Mail: i@Jiniannet.com
 *****************************************************/

using System;
using JinianNet.JNTemplate.Context;

namespace JinianNet.JNTemplate.Parser.Node
{
    public class VariableTag : SimpleTag
    {

        private String name;
        public String Name
        {
            get { return name; }
            set { name = value; }
        }

        public override Object Parse(TemplateContext context)
        {
            return context.TempData[this.Name];
        }

        public override Object Parse(Object baseValue, TemplateContext context)
        {
            return ParserAccessor.Eval(baseValue, this.Name);
        }

    }
}