/********************************************************************************
 Copyright (c) jiniannet (http://www.jiniannet.com). All rights reserved.
 Licensed under the MIT license. See licence.txt file in the project root for full license information.
 ********************************************************************************/
using System;

namespace JinianNet.JNTemplate.Node
{
    /// <summary>
    /// 字符串标签
    /// </summary>
    public class StringTag : TypeTag<String>
    {
        /// <summary>
        /// 转换成BOOLEAN
        /// </summary>
        /// <param name="context">上下文</param>
        public override Boolean ToBoolean(TemplateContext context)
        {
            return !String.IsNullOrEmpty(Value);
        }
    }
}