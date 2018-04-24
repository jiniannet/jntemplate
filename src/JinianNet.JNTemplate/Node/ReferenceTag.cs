/********************************************************************************
 Copyright (c) jiniannet (http://www.jiniannet.com). All rights reserved.
 Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.
 ********************************************************************************/
using System;

namespace JinianNet.JNTemplate.Node
{
    /// <summary>
    /// 组合标签
    /// 用于执于复杂的方法或变量
    /// 类似于
    /// $User.CreateDate.ToString("yyyy-MM-dd")
    /// $Db.Query().Result.Count
    /// </summary>
    public class ReferenceTag : SimpleTag
    {
        /// <summary>
        /// 解析标签
        /// </summary>
        /// <param name="context">上下文</param>
        public override Object Parse(TemplateContext context)
        {
            if (Children.Count > 0)
            {
                Object result = Children[0].Parse(context);
                for (Int32 i = 1; i < Children.Count && result!=null; i++)
                {
                    result = ((SimpleTag)Children[i]).Parse(result, context);
                }
                return result;
            }
            return null;
        }
        /// <summary>
        /// 解析标签
        /// </summary>
        /// <param name="context">上下文</param>
        /// <param name="baseValue">基本值</param>
        public override Object Parse(Object baseValue, TemplateContext context)
        {
            Object result = baseValue;
            for (Int32 i = 0; i < Children.Count && result!=null; i++)
            {
                result = ((SimpleTag)Children[i]).Parse(result, context);
            }
            return result;
        }
    }
}