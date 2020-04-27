/********************************************************************************
 Copyright (c) jiniannet (http://www.jiniannet.com). All rights reserved.
 Licensed under the MIT license. See licence.txt file in the project root for full license information.
 ********************************************************************************/
using System;
#if !NET20
using System.Threading.Tasks;
#endif

namespace JinianNet.JNTemplate.Nodes
{
    /// <summary>
    /// 函数（方法）标签
    /// </summary>
    [Serializable]
    public class FunctaionTag : BasisTag
    {
        private BasisTag func;
        /// <summary>
        /// 函数
        /// </summary>
        public BasisTag Func
        {
            get { return this.func; }
            set { this.func = value; }
        }

        /// <summary>
        /// 解析标签
        /// </summary>
        /// <param name="context">上下文</param>
        public override object ParseResult(TemplateContext context)
        {
            object[] args = new object[Children.Count];
            for (int i = 0; i < this.Children.Count; i++)
            {
                args[i] = Children[i].ParseResult(context);
            }

            object result = this.Func.ParseResult(context);

            if (result != null)
            {
                if (result is FuncHandler)
                {
                    return (result as FuncHandler)(args);
                }
                if (result is Delegate)
                {
                    return (result as Delegate).DynamicInvoke(args);
                }
            }

            return null;
        }

        /// <summary>
        /// 调用方法
        /// </summary>
        /// <param name="baseValue"></param>
        /// <param name="context"></param>
        /// <returns></returns>
        private object CallMethod(object baseValue, TemplateContext context)
        {
            if (baseValue != null)
            {
                object[] args = new object[Children.Count];
                for (int i = 0; i < Children.Count; i++)
                {
                    args[i] = Children[i].ParseResult(context);
                }

                object name;
                if (this.Func is VariableTag)
                {
                    name = ((VariableTag)this.Func).Name;
                }
                else
                {
                    name = this.Func.ParseResult(context);
                }

                if (name == null)
                {
                    return null;
                }
                object result = context.Actuator.CallMethod(baseValue, name.ToString(), args);

                if (result != null)
                {
                    return result;
                }

                result = context.Actuator.CallPropertyOrField(baseValue, name.ToString());

                if (result != null && result is Delegate)
                {
                    return (result as Delegate).DynamicInvoke(args);
                }
            }

            return null;
        }

        /// <summary>
        /// 执行标签
        /// </summary>
        /// <param name="context">上下文</param>
        /// <param name="baseValue">baseValue</param>
        public override object ParseResult(object baseValue, TemplateContext context)
        {
            object r = CallMethod(baseValue, context);
#if NETCOREAPP || NETSTANDARD
            if (r != null && r is Task)
            {
                Task task = r as Task;
                task.GetAwaiter().GetResult();
                return context.Actuator.CallPropertyOrField(task, "Result");
            }
#endif
            return r;
        }
#if NETCOREAPP || NETSTANDARD
        /// <summary>
        /// 异步执行标签
        /// </summary>
        /// <param name="baseValue"></param>
        /// <param name="context"></param>
        /// <returns></returns>
        public override async Task<object> ParseResultAsync(object baseValue, TemplateContext context)
        {
            object r = await Task.Run<object>(() => CallMethod(baseValue, context));
            if (r != null && r is Task)
            {
                Task task = r as Task;
                await task;
                return context.Actuator.CallPropertyOrField(task, "Result");
            }
            return r;
        }

        /// <summary>
        /// 解析标签
        /// </summary>
        /// <param name="context">上下文</param>
        public override async Task<object> ParseResultAsync(TemplateContext context)
        {
            object[] args = new object[Children.Count];
            for (int i = 0; i < this.Children.Count; i++)
            {
                args[i] = await Children[i].ParseResultAsync(context);
            }

            object result = await this.Func.ParseResultAsync(context);

            if (result != null)
            {
                if (result is FuncHandler)
                {
                    return (result as FuncHandler)(args);
                }
                if (result is Delegate)
                {
                    return (result as Delegate).DynamicInvoke(args);
                }
            }
            return null;
        }
#endif
    }
}