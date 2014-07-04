/*****************************************************
 * 本类库的核心系 JNTemplate
 * 作者：翅膀的初衷 QQ:4585839
 * Mail: i@Jiniannet.com
 * 网址：http://www.JiNianNet.com
 *****************************************************/
using System;
using System.Collections.Generic;
using JinianNet.JNTemplate.Context;

namespace JinianNet.JNTemplate.Parser.Node
{
    public class ExpressionTag : SimpleTag
    {
        public override object Parse(TemplateContext context)
        {
            Object[] value = new Object[this.Children.Count];

            for (Int32 i = 0; i < this.Children.Count; i++)
            {
                value[i] = this.Children[i].Parse(context);
            }

            Calculator actuator = new Calculator();

            return actuator.Calculate(new Calculator().ProcessExpression(value));
        }

        public override object Parse(object baseValue, TemplateContext context)
        {
            Object[] value = new Object[this.Children.Count];

            for (Int32 i = 0; i < this.Children.Count; i++)
            {
                value[i] = this.Children[i].Parse(baseValue,context);
            }

            Calculator actuator = new Calculator();

            return actuator.Calculate(new Calculator().ProcessExpression(value));
        }
    }
}