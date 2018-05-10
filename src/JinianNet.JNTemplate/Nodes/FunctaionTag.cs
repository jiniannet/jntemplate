/********************************************************************************
 Copyright (c) jiniannet (http://www.jiniannet.com). All rights reserved.
 Licensed under the MIT license. See licence.txt file in the project root for full license information.
 ********************************************************************************/
using System;

namespace JinianNet.JNTemplate.Nodes
{
    /// <summary>
    /// 函数（方法）标签
    /// </summary>
    public class FunctaionTag : SimpleTag
    {
        private TagBase _func;
        /// <summary>
        /// 函数
        /// </summary>
        public TagBase Func
        {
            get { return this._func; }
            set { this._func = value; }
        }

        /// <summary>
        /// 解析标签
        /// </summary>
        /// <param name="context">上下文</param>
        public override object Parse(TemplateContext context)
        {
            object[] args = new object[Children.Count];
            for (int i = 0; i < this.Children.Count; i++)
            {
                args[i] = Children[i].Parse(context);
            }

            object result = this.Func.Parse(context);

            if (result != null)
            {
                if (result is FuncHandler)
                {
                    return (result as FuncHandler).Invoke(args);
                }
            }

            return null;
        }
        /// <summary>
        /// 解析标签
        /// </summary>
        /// <param name="context">上下文</param>
        /// <param name="baseValue">baseValue</param>
        public override object Parse(object baseValue, TemplateContext context)
        {
            if (baseValue != null)
            {
                object[] args = new object[Children.Count];
                for (int i = 0; i < Children.Count; i++)
                {
                    args[i] = Children[i].Parse(context);
                }

                object name;
                if (this.Func is VariableTag)
                {
                    name = ((VariableTag)this.Func).Name;
                }
                else
                {
                    name = this.Func.Parse(context);
                }

                if (name == null)
                {
                    return null;
                }
                object result = Engine.Runtime.CallMethod(baseValue, name.ToString(), args);

                if (result != null)
                {
                    return result;
                }

                result = Engine.Runtime.CallPropertyOrField(baseValue, name.ToString());

                if (result != null && result is FuncHandler)
                {
                    return (result as FuncHandler).Invoke(args);
                }
            }

            return null;
        }

    }
}