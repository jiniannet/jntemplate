/*****************************************************
 * 本类库的核心系 JNTemplate
 * 作者：翅膀的初衷 QQ:4585839
 * Mail: i@Jiniannet.com
 * 网址：http://www.JiNianNet.com
 *****************************************************/
using System;

namespace JinianNet.JNTemplate.Parser.Node
{
    public class SetTag : Tag
    {
        public SetTag(Int32 line, Int32 col)
            : base(ElementType.Set, line, col)
        {

        }
        private String _name;
        public String Name
        {
            get { return _name; }
            set { _name = value; }
        }

        private Tag _value;
        public Tag Value
        {
            get { return _value; }
            set { _value = value; }
        }


        public override Object Parse(TemplateContext context)
        {
            context.TempData[this.Name] = this.Value.Parse(context);
            return null;
        }

        public override void Parse(TemplateContext context, System.IO.TextWriter writer)
        {
            Parse(context);
        }
    }
}