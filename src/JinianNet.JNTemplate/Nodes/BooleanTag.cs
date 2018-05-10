/********************************************************************************
 Copyright (c) jiniannet (http://www.jiniannet.com). All rights reserved.
 Licensed under the MIT license. See licence.txt file in the project root for full license information.
 ********************************************************************************/
using System;

namespace JinianNet.JNTemplate.Nodes
{
    /// <summary>
    /// 布尔标签
    /// </summary>
    public class BooleanTag : TypeTag<bool>
    {
        /// <summary>
        /// 获取布布值
        /// </summary>
        /// <param name="context">上下文</param>
        public override bool ToBoolean(TemplateContext context)
        {
            return Value;
        }
    }
}