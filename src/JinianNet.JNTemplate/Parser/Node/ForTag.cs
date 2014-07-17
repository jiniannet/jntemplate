/*****************************************************
 * 本类库的核心系 JNTemplate
 * 作者：翅膀的初衷 QQ:4585839
 * Mail: i@Jiniannet.com
 * 网址：http://www.JiNianNet.com
 *****************************************************/
using System;
using System.Collections.Generic;
using System.Text;

namespace JinianNet.JNTemplate.Parser.Node
{
    public class ForTag : SimpleTag
    {
        public Tag initial;
        public Tag test;
        public Tag dothing;

        public Tag Initial
        {
            get { return initial; }
            set { initial = value; }
        }
        public Tag Test
        {
            get { return test; }
            set { test = value; }
        }
        public Tag Do
        {
            get { return dothing; }
            set { dothing = value; }
        }

        private void Excute(TemplateContext context, System.IO.TextWriter writer)
        {
            this.Initial.Parse(context);
            Boolean run = this.Test == null ? true : this.Test.ToBoolean(context) ;
            while (run)
            {
                for (Int32 i = 0; i < this.Children.Count; i++)
                {
                    this.Children[i].Parse(context, writer);
                }
                if (this.Do != null)
                {
                    this.Do.Parse(context);
                }
                run = this.Test == null ? true : this.Test.ToBoolean(context);
            }
        }

        public override Object Parse(TemplateContext context)
        {
            using (System.IO.StringWriter write = new System.IO.StringWriter())
            {
                Excute(context, write);
                return write.ToString();
            }
        }

        public override Object Parse(Object baseValue, TemplateContext context)
        {
            return Parse(context);
        }

        public override void Parse(TemplateContext context, System.IO.TextWriter write)
        {
            Excute(context, write);
        }
    }
}
