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

        public override Object Parse(TemplateContext context)
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
                        this.Children[i].Parse(context, write);
                    }
                    return write.ToString();
                }
            }

        }

        public override Object Parse(Object baseValue, TemplateContext context)
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
                        write.Write(this.Children[i].Parse(baseValue, context));
                    }
                    return write.ToString();
                }
            }
        }

        public override void Parse(TemplateContext context, System.IO.TextWriter write)
        {

            for (Int32 i = 0; i < this.Children.Count; i++)
            {
                this.Children[0].Parse(context, write);
            }

        }


        public override Boolean ToBoolean(TemplateContext context)
        {
            return this.Test.ToBoolean(context);
        }

    }
}
