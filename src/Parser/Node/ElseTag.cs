using System;
using System.Collections.Generic;
using System.Text;

namespace JinianNet.JNTemplate.Parser.Node
{
    public class ElseTag : ElseifTag
    {
        public override Boolean ToBoolean(TemplateContext context)
        {
            return true;
        }
    }
}
