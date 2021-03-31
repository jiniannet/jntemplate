/********************************************************************************
 Copyright (c) jiniannet (http://www.jiniannet.com). All rights reserved.
 Licensed under the MIT license. See licence.txt file in the project root for full license information.
 ********************************************************************************/
using JinianNet.JNTemplate.Nodes;
using System;
using System.Collections.Generic;

namespace JinianNet.JNTemplate.Dynamic
{
    /// <summary>
    /// The Execute the method builder
    /// </summary>
    public class ExecuteBuilder
    {

        Dictionary<string, Func<ITag, TemplateContext, object>> dict;

        /// <summary>
        /// Initializes a new instance of the <see cref="ExecuteBuilder"/> class
        /// </summary>
        public ExecuteBuilder()
        {
            dict = new Dictionary<string, Func<ITag, TemplateContext, object>>(StringComparer.OrdinalIgnoreCase);
        }

        /// <summary>
        /// Build a rendering method for the specified tag.
        /// </summary>
        /// <param name="tag">The <see cref="ITag"/>.</param>
        /// <returns></returns>
        public Func<ITag, TemplateContext, object> Build(ITag tag)
        {
            return Build(tag.GetType().Name);
        }

        /// <summary>
        /// Build a rendering method for the specified tag.
        /// </summary>
        /// <typeparam name="T">The type of the <see cref="ITag"/>.</typeparam>
        /// <returns></returns>
        public Func<ITag, TemplateContext, object> Build<T>() where T : ITag
        {
            return Build(typeof(T).Name);
        }

        /// <summary>
        /// Build a rendering method for the specified tag.
        /// </summary>
        /// <param name="name">The name of the tag.</param>
        /// <returns></returns>
        public Func<ITag, TemplateContext, object> Build(string name)
        {
            if (dict.TryGetValue(name, out var func))
            {
                return func;
            }
            throw new Exception.CompileException($"The tag \"{name}\" is not supported .");
        }

        /// <summary>
        /// Register a execute method for the specified tag.
        /// </summary>
        /// <typeparam name="T">The type of the tag.</typeparam>
        /// <param name="func">The method.</param>
        public void Register<T>(Func<ITag, TemplateContext, object> func) where T : ITag
        {
            Register(typeof(T).Name, func);
        }

        /// <summary>
        /// Register a execute method for the specified tag.
        /// </summary>
        /// <param name="name">The tag name.</param>
        /// <param name="func">The method.</param>
        public void Register(string name, Func<ITag, TemplateContext, object> func)
        {
            dict[name] = func;
        }

        /// <summary>
        /// eval expression
        /// </summary>
        /// <param name="list">list</param>
        /// <param name="isOperator"></param>
        /// <param name="value"></param>
        /// <returns></returns>
        private bool Eval(List<object> list, bool isOperator, object value)
        {
            if (isOperator)
            {
                Operator op = (Operator)value;
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
    }
}