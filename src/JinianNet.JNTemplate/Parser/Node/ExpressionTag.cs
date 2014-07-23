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
    public class ExpressionTag : SimpleTag
    {
        public override object Parse(TemplateContext context)
        {
            Object[] value = new Object[this.Children.Count];

            for (Int32 i = 0; i < this.Children.Count; i++)
            {
                value[i] = this.Children[i].Parse(context);
            }

            Stack<Object> stack = Calculator.ProcessExpression(value);

            return Calculator.Calculate(stack);
        }

        public override object Parse(object baseValue, TemplateContext context)
        {
            Object[] value = new Object[this.Children.Count];

            for (Int32 i = 0; i < this.Children.Count; i++)
            {
                value[i] = this.Children[i].Parse(baseValue,context);
            }

            Stack<Object> stack = Calculator.ProcessExpression(value);

            return Calculator.Calculate(stack);
        }
    }
}