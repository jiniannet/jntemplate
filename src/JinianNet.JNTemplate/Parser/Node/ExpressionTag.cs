/********************************************************************************
 Copyright (c) jiniannet (http://www.jiniannet.com). All rights reserved.
 Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.
 ********************************************************************************/
using System;
using System.Collections.Generic;

namespace JinianNet.JNTemplate.Parser.Node
{
    /// <summary>
    /// 表达式
    /// </summary>
    public class ExpressionTag : BaseTag
    {
        /// <summary>
        /// 解析标签
        /// </summary>
        /// <param name="context">上下文</param>
        public override object Parse(TemplateContext context)
        {
            Object[] value = new Object[this.Children.Count];

            for (Int32 i = 0; i < this.Children.Count; i++)
            {
                if (this.Children[i] is TextTag)
                {
                    value[i] = Common.OperatorHelpers.Parse(this.Children[i].Parse(context).ToString());
                }
                else
                {
                    value[i] = this.Children[i].Parse(context);
                }
            }

            Stack<Object> stack = Common.Calculator.ProcessExpression(value);

            return Common.Calculator.Calculate(stack);
        }

    }
}