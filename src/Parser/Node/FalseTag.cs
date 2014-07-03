using System;
using System.Collections.Generic;
using System.Text;

namespace JinianNet.JNTemplate.Parser.Node
{
    public class FalseTag : Tag
    {
        public override Object Parse(TemplateContext context)
        {
            return false;
        }

        public override Object Parse(Object baseValue, TemplateContext context)
        {
            return false;
        }

        public override void Parse(TemplateContext context, System.IO.TextWriter write)
        {
            write.Write("false");
        }

        public override bool ToBoolean(TemplateContext context)
        {
            return false;
        }

        public override String ToString()
        {
            return "false";
        }
    }
}
