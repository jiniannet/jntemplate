using System;
using System.Collections.Generic;
using System.Text;

namespace JinianNet.JNTemplate.Parser.Node
{
    public abstract class BaseTag<T> : Tag
    {
        private T baseValue;
        public T Value
        {
            get { return this.baseValue; }
            set { this.baseValue = value; }
        }

        public override object Parse(TemplateContext context)
        {
            return this.Value;
        }

        public override object Parse(object baseValue, TemplateContext context)
        {
            return this.Value;
        }

        public override void Parse(TemplateContext context, System.IO.TextWriter write)
        {
            write.Write(this.Value.ToString());
        }

        public override string ToString()
        {
            if (this.Value != null)
            {
                return this.Value.ToString();
            }
            return string.Empty;
        }
    }
}
