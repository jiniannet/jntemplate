/********************************************************************************
 Copyright (c) jiniannet (http://www.jiniannet.com). All rights reserved.
 Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.
 ********************************************************************************/
using System;
using System.Collections.Generic;

namespace JinianNet.JNTemplate.Node
{
    /// <summary>
    /// 表达式
    /// </summary>
    public class ExpressionTag : TagBase
    {
        /// <summary>
        /// 解析标签
        /// </summary>
        /// <param name="context">上下文</param>
        public override object Parse(TemplateContext context)
        {
            Object[] value = new Object[Children.Count];

            for (Int32 i = 0; i < Children.Count; i++)
            {
                if (Children[i] is TextTag)
                {
                    value[i] = Common.OperatorConvert.Parse(Children[i].Parse(context).ToString());
                }
                else
                {
                    value[i] = Children[i].Parse(context);
                }
            }

            Stack<Object> stack = Common.ExpressionEvaluator.ProcessExpression(value);

            return Common.ExpressionEvaluator.Calculate(stack);
        }

    }
}