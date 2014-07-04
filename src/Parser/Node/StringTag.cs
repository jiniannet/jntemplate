using System;
using System.Collections.Generic;
using System.Text;

namespace JinianNet.JNTemplate.Parser.Node
{
    public class StringTag : SimpleTag
    {
        private String text;
        public String Text
        {
            get { return this.text; }
            set { this.text = value; }
        }

        public override object Parse(TemplateContext context)
        {
            return this.Text;
        }

        public override object Parse(object baseValue, TemplateContext context)
        {
            return this.Text;
        }
    }
}
