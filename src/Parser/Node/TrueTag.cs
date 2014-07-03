using System;
using System.Collections.Generic;
using System.Text;

namespace JinianNet.JNTemplate.Parser.Node
{
    public class TrueTag : Tag
    {
        public override Object Parse(TemplateContext context)
        {
            return true;
        }

        public override Object Parse(Object baseValue, TemplateContext context)
        {
            return true;
        }

        public override void Parse(TemplateContext context, System.IO.TextWriter write)
        {
            write.Write("true");
        }

        public override bool ToBoolean(TemplateContext context)
        {
            return true;
        }

        public override String ToString()
        {
            return "true";
        }
    }
}
