/********************************************************************************
 Copyright (c) jiniannet (http://www.jiniannet.com). All rights reserved.
 Licensed under the MIT license. See licence.txt file in the project root for full license information.
 ********************************************************************************/
using JinianNet.JNTemplate.Dynamic;
using System;
using System.Collections.Generic;

namespace JinianNet.JNTemplate.Nodes
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
            object[] value = new object[Children.Count];

            for (int i = 0; i < Children.Count; i++)
            {
                if (Children[i] is TextTag)
                {
                    value[i] = OperatorConvert.Parse(Children[i].Parse(context).ToString());
                }
                else
                {
                    value[i] = Children[i].Parse(context);
                }
            }

            Stack<object> stack = ExpressionEvaluator.ProcessExpression(value);

            return ExpressionEvaluator.Calculate(stack);
        }

    }
}