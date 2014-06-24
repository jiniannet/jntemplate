using System;
using System.Collections.Generic;
using System.Text;

namespace JinianNet.JNTemplate.Parser.Node
{
    public class ElseifTag : SimpleTag
    {
        private Tag test;
        public virtual Tag Test
        {
            get { return test; }
            set { test = value; }
        }

        public override Object Parse(JinianNet.JNTemplate.Context.TemplateContext context)
        {
            if (this.Test.ToBoolean(context))
            {
                if (this.Children.Count == 1)
                {
                    return this.Children[0].Parse(context);
                }
                else
                {
                    using (System.IO.StringWriter write = new System.IO.StringWriter())
                    {
                        for (Int32 i = 0; i < this.Children.Count; i++)
                        {
                            this.Children[0].Parse(context, write);
                        }
                        return write.ToString();
                    }
                }
            }

            return null;
        }

        public override Object Parse(Object baseValue, JinianNet.JNTemplate.Context.TemplateContext context)
        {
            if (this.Test.ToBoolean(context))
            {
                if (this.Children.Count == 1)
                {
                    return this.Children[0].Parse(context, context);
                }
                else
                {
                    using (System.IO.StringWriter write = new System.IO.StringWriter())
                    {
                        for (Int32 i = 0; i < this.Children.Count; i++)
                        {
                            write.Write(this.Children[0].Parse(baseValue, context));
                        }
                        return write.ToString();
                    }
                }
            }

            return null;
        }

        public override void Parse(JinianNet.JNTemplate.Context.TemplateContext context, System.IO.TextWriter write)
        {
            if (this.Test.ToBoolean(context))
            {
                for (Int32 i = 0; i < this.Children.Count; i++)
                {
                    this.Children[0].Parse(context, write);
                }
            }
        }


        public override Boolean ToBoolean(JinianNet.JNTemplate.Context.TemplateContext context)
        {
            return this.Test.ToBoolean(context);
        }

    }
}
