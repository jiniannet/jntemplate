/********************************************************************************
 Copyright (c) jiniannet (http://www.jiniannet.com). All rights reserved.
 Licensed under the MIT license. See licence.txt file in the project root for full license information.
 ********************************************************************************/
using System;

namespace JinianNet.JNTemplate.Node
{
    /// <summary>
    /// 空标签
    /// </summary>
    public class NullTag : TagBase
    {        /// <summary>
        /// 解析标签
        /// </summary>
        /// <param name="context">上下文</param>
        public override Object Parse(TemplateContext context)
        {
            return null;
        }
        /// <summary>
        /// 获取标签的BOOLEAN
        /// </summary>
        /// <param name="context">上下文</param>
        public override Boolean ToBoolean(TemplateContext context)
        {
            return false;
        }
        /// <summary>
        /// 获取对象的字符串引用
        /// </summary>
        public override String ToString()
        {
            return String.Empty;
        }
    }
}