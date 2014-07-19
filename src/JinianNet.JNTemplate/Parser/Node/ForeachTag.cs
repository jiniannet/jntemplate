/*****************************************************
 * 本类库的核心系 JNTemplate
 * 作者：翅膀的初衷 QQ:4585839
 * Mail: i@Jiniannet.com
 * 网址：http://www.JiNianNet.com
 *****************************************************/
using System;
using System.Collections;
using System.Collections.Generic;


namespace JinianNet.JNTemplate.Parser.Node
{
    public class ForeachTag : SimpleTag
    {

        private String name;
        public String Name
        {
            get { return name; }
            set { name = value; }
        }

        private Tag source;
        public Tag Source
        {
            get { return source; }
            set { source = value; }
        }

        private void Excute(Object value, TemplateContext context, System.IO.TextWriter writer)
        {
            IEnumerable enumerable = ParserAccessor.ToIEnumerable(value);
            TemplateContext ctx;
            if (enumerable != null)
            {
                IEnumerator ienum = enumerable.GetEnumerator();
                ctx = TemplateContext.CreateContext(context);
                Int32 i = 0;
                while (ienum.MoveNext())
                {
                    ctx.TempData[this.Name] = ienum.Current;
                    //为了兼容以前的用户 foreachIndex 保留
                    ctx.TempData["foreachIndex"] = i;
                    for (Int32 n = 0; n < this.Children.Count; n++)
                    {
                        //if (this.Children[n] is WordTag)
                        //{
                        //    if (this.Children[n].ToString() == "break")
                        //    {
                        //        break;
                        //    }
                        //    if (this.Children[n].ToString() == "continue")
                        //    {
                        //        continue;
                        //    }
                        //}
                        this.Children[n].Parse(ctx, writer);
                    }
                    i++;
                }
            }
        }

        public override void Parse(TemplateContext context, System.IO.TextWriter writer)
        {
            if (this.Source != null)
            {
                Excute(this.Source.Parse(context), context, writer);
            }
        }

        public override Object Parse(TemplateContext context)
        {
            using (System.IO.StringWriter write = new System.IO.StringWriter())
            {
                Excute(this.Source.Parse(context), context, write);
                return write.ToString();
            }
        }

        public override Object Parse(Object baseValue, TemplateContext context)
        {
            using (System.IO.StringWriter write = new System.IO.StringWriter())
            {
                Excute(this.Source.Parse(baseValue, context), context, write);
                return write.ToString();
            }
        }
    }
}