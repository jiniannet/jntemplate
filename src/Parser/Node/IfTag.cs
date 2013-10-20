/*****************************************************
 * 本类库的核心系 JNTemplate
 * 作者：翅膀的初衷 QQ:4585839
 * Mail: i@Jiniannet.com
 * 网址：http://www.JiNianNet.com
 *****************************************************/
using System;
using System.Collections.Generic;

namespace JinianNet.JNTemplate.Parser.Node
{
    public class IfTag : ComplexTag
    {
        public IfTag(Int32 line, Int32 col)
            : base(ElementType.If, line, col)
        {
            this.Test = new List<Tag>();
            this.Value = new List<TagCollection>();
        }


        private List<Tag> _test;
        public List<Tag> Test
        {
            get { return _test; }
            set { _test = value; }
        }

        private List<TagCollection> _value;
        public List<TagCollection> Value
        {
            get { return _value; }
            set { _value = value; }
        }

        public override void Parse(TemplateContext context, System.IO.TextWriter writer)
        {
            Object value;
            for (Int32 i = 0; i < this.Test.Count; i++)
            {
                value =  this.Test[i].Parse(context);

                if (value != null && "TRUE".Equals(value.ToString(), StringComparison.OrdinalIgnoreCase))
                {
                    ParseCollection(this.Value[i], context, writer);
                    return;
                }
            }
            if (this.Value.Count > this.Test.Count)
            {
                ParseCollection(this.Value[this.Value.Count - 1], context, writer);
            }
        }

        private void ParseCollection(TagCollection tags, TemplateContext context, System.IO.TextWriter write)
        {
            for (Int32 i = 0; i < tags.Count; i++)
            {
                write.Write( tags[i].Parse(context));
            }
        }
    }
}