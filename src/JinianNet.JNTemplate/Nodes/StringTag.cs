/********************************************************************************
 Copyright (c) jiniannet (http://www.jiniannet.com). All rights reserved.
 Licensed under the MIT license. See licence.txt file in the project root for full license information.
 ********************************************************************************/
using System;

namespace JinianNet.JNTemplate.Nodes
{
    /// <summary>
    /// 字符串标签
    /// </summary>
    public class StringTag : TypeTag<string>
    {
        /// <summary>
        /// 转换成BOOLEAN
        /// </summary>
        /// <param name="context">上下文</param>
        public override bool ToBoolean(TemplateContext context)
        {
            return !string.IsNullOrEmpty(Value);
        }
    }
}