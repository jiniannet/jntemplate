using System;
using System.Collections.Generic;
using System.Text;


namespace JinianNet.JNTemplate.Parser.Node
{
    public abstract class SimpleTag : Tag
    {
        public SimpleTag(ElementType type, Int32 line, Int32 col)
            : base(type, line, col)
        {
            
        }

        public override void Parse(TemplateContext context, System.IO.TextWriter writer)
        {
            writer.Write(this.Parse(context));
        }
    }
}
