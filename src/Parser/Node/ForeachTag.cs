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
    public class ForeachTag : ComplexTag
    {
        public ForeachTag(Int32 line, Int32 col)
            : base(ElementType.Foreach, line, col)
        {
            this.Value = new List<Tag>();
        }

        private String _name;
        public String Name
        {
            get { return _name; }
            set { _name = value; }
        }

        private List<Tag> _value;
        public List<Tag> Value
        {
            get { return _value; }
            set { _value = value; }
        }

        private Tag _source;
        public Tag Source
        {
            get { return _source; }
            set { _source = value; }
        }

        public override void Parse(TemplateContext context, System.IO.TextWriter writer)
        {
            IEnumerable enumerable = ParserAccessor.ToIEnumerable(this.Source.Parse(context));
            TemplateContext ctx;
            if (enumerable != null)
            {
                IEnumerator ienum = enumerable.GetEnumerator();
                ctx = TemplateContext.CreateContext(context);
                Int32 i = 0;
                while (ienum.MoveNext())
                {
                    ctx.TempData[this.Name] = ienum.Current;
                    i++;
                    ctx.TempData["ForeachIndex"] = i;
                    for (Int32 n = 0; n < this.Value.Count; n++)
                    {
                        writer.Write(this.Value[n].Parse(ctx));
                    }
                }
            }
        }
    }
}