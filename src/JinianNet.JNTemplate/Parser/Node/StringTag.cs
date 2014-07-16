using System;
using System.Collections.Generic;
using System.Text;

namespace JinianNet.JNTemplate.Parser.Node
{
    public class StringTag : BaseTag<String>
    {
        public override Boolean ToBoolean(TemplateContext context)
        {
            return !String.IsNullOrEmpty(this.Value);
        }
    }
}
