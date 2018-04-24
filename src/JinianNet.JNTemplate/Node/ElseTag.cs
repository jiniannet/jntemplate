/********************************************************************************
 Copyright (c) jiniannet (http://www.jiniannet.com). All rights reserved.
 Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.
 ********************************************************************************/
using System;

namespace JinianNet.JNTemplate.Node
{
    /// <summary>
    /// else标签
    /// </summary>
    public class ElseTag : ElseifTag
    {
        /// <summary>
        /// 获取布布值
        /// </summary>
        /// <param name="context">上下文</param>
        public override Boolean ToBoolean(TemplateContext context)
        {
            return true;
        }
    }
}