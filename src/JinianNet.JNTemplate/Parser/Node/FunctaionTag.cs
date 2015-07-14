/********************************************************************************
 Copyright (c) jiniannet (http://www.jiniannet.com). All rights reserved.
 Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.
 ********************************************************************************/
using System;
using System.Collections.Generic;
using System.Reflection;


namespace JinianNet.JNTemplate.Parser.Node
{
    /// <summary>
    /// 函数（方法）标签
    /// </summary>
    public class FunctaionTag : SimpleTag
    {
        private String name;
        /// <summary>
        /// 方法名
        /// </summary>
        public String Name
        {
            get { return name; }
            set { name = value; }
        }

        /// <summary>
        /// 解析标签
        /// </summary>
        /// <param name="context">上下文</param>
        public override Object Parse(TemplateContext context)
        {
            Object[] args = new Object[this.Children.Count];
            for (Int32 i = 0; i < this.Children.Count; i++)
            {
                args[i] = this.Children[i].Parse(context);
            }

            Object result = context.TempData[this.Name];

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
        public override Object Parse(Object baseValue, TemplateContext context)
        {
            if (baseValue != null)
            {
                Object[] args = new Object[this.Children.Count];
                for (Int32 i = 0; i < this.Children.Count; i++)
                {
                    args[i] = this.Children[i].Parse(context);
                }

                Object result = Common.ReflectionHelpers.InvokeMethod(baseValue, this.Name, args);

                if (result != null)
                {
                    return result;
                }

                result = Common.ReflectionHelpers.GetPropertyOrField(baseValue, this.Name);

                if (result != null && result is FuncHandler)
                {
                    return (result as FuncHandler).Invoke(args);
                }
            }

            return null;
        }

    }
}