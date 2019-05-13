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
        public override object ParseResult(TemplateContext context)
        {
            List<object> value = new List<object>();
            Stack<object> stack;

            for (int i = 0; i < Children.Count; i++)
            {
                if (Children[i] is TextTag)
                {
                    Operator op = OperatorConvert.Parse(Children[i].ParseResult(context).ToString());
                    if(op== Operator.Or || op == Operator.And)
                    {
                        object result;
                        bool isTrue;
                        if (value.Count > 1)
                        {
                            stack = ExpressionEvaluator.ProcessExpression(value.ToArray());
                             result = ExpressionEvaluator.Calculate(stack);
                        }
                        else
                        {
                            result = value[0];
                        }
                        if (result is Boolean)
                        {
                            isTrue = (Boolean)result;
                        }
                        else
                        {
                            isTrue = ExpressionEvaluator.CalculateBoolean(result);
                        }
                        if(op == Operator.Or && isTrue)
                        {
                            return true;
                        }
                        if(op == Operator.And && !isTrue)
                        {
                            return false;
                        }
                        value.Clear();
                        value.Add(isTrue);
                    }
                    value.Add(op);
                }
                else
                {
                    value.Add(Children[i].ParseResult(context));
                }
            }
 
            stack = ExpressionEvaluator.ProcessExpression(value.ToArray());
            return ExpressionEvaluator.Calculate(stack);
            //Stack<Tag> stack = ExpressionEvaluator.ProcessExpression(Children,context);
            //return ExpressionEvaluator.Calculate(stack, context);
        }

    }
}