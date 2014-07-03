using System;
using System.Collections.Generic;
using System.Text;

namespace JinianNet.JNTemplate.Parser.Node
{
    public class WordTag : SimpleTag
    {
        private String text;
        public String Name
        {
            get { return this.text; }
            set { this.text = value; }
        }

        public override Object Parse(TemplateContext context)
        {
            return this.Name;
        }

        public override Object Parse(Object baseValue, TemplateContext context)
        {
            return this.Name;
        }

        public override String ToString()
        {
            return this.Name;
        }

        public override void Parse(TemplateContext context, System.IO.TextWriter write)
        {

        }
    }
}
