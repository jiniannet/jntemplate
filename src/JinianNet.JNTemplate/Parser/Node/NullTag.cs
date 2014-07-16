using System;
using System.Collections.Generic;
using System.Text;

namespace JinianNet.JNTemplate.Parser.Node
{
    public class NullTag : SimpleTag
    {
        public override Object Parse(TemplateContext context)
        {
            return null;
        }

        public override Object Parse(object baseValue, TemplateContext context)
        {
            return null;
        }

        public override bool ToBoolean(TemplateContext context)
        {
            return false;
        }

        public override string ToString()
        {
            return string.Empty;
        }
    }
}
