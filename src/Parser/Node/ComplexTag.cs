using System;
using System.Collections.Generic;
using System.Text;

namespace JinianNet.JNTemplate.Parser.Node
{
    public abstract class ComplexTag: Tag
    {
        public ComplexTag(ElementType type, Int32 line, Int32 col)
            : base(type, line, col)
        {
            
        }

        public override Object Parse(VariableScope vars)
        {
            using (System.IO.StringWriter write = new System.IO.StringWriter())
            {
                this.Parse(vars, write);
                return write.ToString();
            }
        }
    }
}
