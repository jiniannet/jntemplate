using System;
using System.Collections.Generic;
using System.Text;
using JinianNet.JNTemplate.Context;

namespace JinianNet.JNTemplate.Parser.Node
{
    public abstract class SimpleTag : Tag
    {

        public override void Parse(TemplateContext context, System.IO.TextWriter write)
        {
            write.Write(Parse(context));
        }
    }
}
