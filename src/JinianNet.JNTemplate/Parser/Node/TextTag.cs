using System;
using System.Collections.Generic;
using System.Text;

namespace JinianNet.JNTemplate.Parser.Node
{
    public class TextTag : Tag
    {

        public override object Parse(TemplateContext context)
        {
            return this.ToString();
        }

        public override object Parse(object baseValue, TemplateContext context)
        {
            return this.ToString();
        }

        public override void Parse(TemplateContext context, System.IO.TextWriter write)
        {
            write.Write(this.ToString());
        }

        public override string ToString()
        {
            return this.FirstToken.ToString();
        }

    }
}
