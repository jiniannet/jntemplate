using System;
using System.Collections.Generic;
using System.Text;

namespace JinianNet.JNTemplate.Parser.Node
{
    public class BooleanTag : BaseTag<Boolean>
    {
        public override Boolean ToBoolean(TemplateContext context)
        {
            return this.Value;
        }
    }
}
