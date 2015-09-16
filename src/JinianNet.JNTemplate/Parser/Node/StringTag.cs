/********************************************************************************
 Copyright (c) jiniannet (http://www.jiniannet.com). All rights reserved.
 Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.
 ********************************************************************************/
using System;
using System.Collections.Generic;
using System.Text;

namespace JinianNet.JNTemplate.Parser.Node
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