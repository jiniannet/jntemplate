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
    public class ExpressionTag : ComplexTag
    {
        public ExpressionTag(Int32 line, Int32 col)
            : base(ElementType.Expression, line, col)
        {
            this.Value = new List<Tag>();
        }

        private List<Tag> _value;
        public List<Tag> Value
        {
            get { return _value; }
            private set { _value = value; }
        }

        public override void Parse(TemplateContext context, System.IO.TextWriter writer)
        {
            Object[] value = new Object[this.Value.Count];
            for (Int32 i = 0; i < this.Value.Count; i++)
            {
                value[i] = this.Value[i].Parse(context);
            }
            Calculator actuator = new Calculator();
            writer.Write(actuator.Calculate(new Calculator().ProcessExpression(value)));
        }
    }
}