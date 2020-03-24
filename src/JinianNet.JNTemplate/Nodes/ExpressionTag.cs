/********************************************************************************
 Copyright (c) jiniannet (http://www.jiniannet.com). All rights reserved.
 Licensed under the MIT license. See licence.txt file in the project root for full license information.
 ********************************************************************************/
using JinianNet.JNTemplate.Dynamic;
using JinianNet.JNTemplate.Exception;
using System;
using System.Collections.Generic;
#if !NET20
using System.Threading.Tasks;
#endif

namespace JinianNet.JNTemplate.Nodes
{
    /// <summary>
    /// 表达式
    /// </summary>
    [Serializable]
    public class ExpressionTag : BasisTag
    {
        /// <summary>
        /// 解析标签
        /// </summary>
        /// <param name="context">上下文</param>
        public override object ParseResult(TemplateContext context)
        {
            List<object> parameters = new List<object>();

            for (int i = 0; i < Children.Count; i++)
            {
                object result = Children[i].ParseResult(context);
                bool isOperator = Children[i] is TextTag;
                if (Eval(parameters, isOperator, result))
                {
                    return parameters[parameters.Count - 1];
                }
            }

            var stack = ExpressionEvaluator.ProcessExpression(parameters.ToArray());
            return ExpressionEvaluator.Calculate(stack);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="baseValue"></param>
        /// <param name="context"></param>
        /// <returns></returns>
        public override object ParseResult(object baseValue, TemplateContext context)
        {
            throw new TemplateException("unsupported");
        }

        /// <summary>
        /// 计算
        /// </summary>
        /// <param name="list">list</param>
        /// <param name="isOperator">是否操作符</param>
        /// <param name="value">标签值</param>
        /// <returns>是否是最终结果</returns>
        private bool Eval(List<object> list, bool isOperator, object value)
        {
            if (isOperator)
            {
                Operator op = OperatorConvert.Parse(value.ToString());
                if (op == Operator.Or || op == Operator.And)
                {
                    object result;
                    bool isTrue;
                    if (list.Count > 1)
                    {
                        var stack = ExpressionEvaluator.ProcessExpression(list.ToArray());
                        result = ExpressionEvaluator.Calculate(stack);
                    }
                    else
                    {
                        result = list[0];
                    }
                    if (result is bool)
                    {
                        isTrue = (bool)result;
                    }
                    else
                    {
                        isTrue = ExpressionEvaluator.CalculateBoolean(result);
                    }
                    if (op == Operator.Or && isTrue)
                    {
                        list.Add(true);
                        return true;
                    }
                    if (op == Operator.And && !isTrue)
                    {
                        list.Add(false);
                        return true;
                    }
                    list.Clear();
                    list.Add(isTrue);
                }
                list.Add(op);
            }
            else
            {
                list.Add(value);
            }
            return false;
        }

#if NETCOREAPP || NETSTANDARD
        /// <summary>
        /// 解析标签
        /// </summary>
        /// <param name="context">上下文</param>
        public override async Task<object> ParseResultAsync(TemplateContext context)
        {
            List<object> parameters = new List<object>();

            for (int i = 0; i < Children.Count; i++)
            {
                object result = await Children[i].ParseResultAsync(context);
                bool isOperator = Children[i] is TextTag;
                if (Eval(parameters, isOperator, result))
                {
                    return parameters[parameters.Count - 1];
                }
            }

            var stack = ExpressionEvaluator.ProcessExpression(parameters.ToArray());
            return ExpressionEvaluator.Calculate(stack);
        }
#endif

    }
}