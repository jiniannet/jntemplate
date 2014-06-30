using System;
using System.Collections.Generic;
using System.Text;

namespace JinianNet.JNTemplate.Parser.Node
{
    public class TextTag : SimpleTag
    {
        public override object Parse(JinianNet.JNTemplate.Context.TemplateContext context)
        {
            return this.ToString();
        }

        public override object Parse(object baseValue, JinianNet.JNTemplate.Context.TemplateContext context)
        {
            return this.ToString();
        }

        public override void Parse(JinianNet.JNTemplate.Context.TemplateContext context, System.IO.TextWriter write)
        {
            write.Write(this.ToString());
        }
    }
}
