/********************************************************************************
 Copyright (c) jiniannet (http://www.jiniannet.com). All rights reserved.
 Licensed under the MIT license. See licence.txt file in the project root for full license information.
 ********************************************************************************/
using System;

namespace JinianNet.JNTemplate.Nodes
{
    /// <summary>
    /// 空标签
    /// </summary>
    public class NullTag : TagBase
    {        /// <summary>
        /// 解析标签
        /// </summary>
        /// <param name="context">上下文</param>
        public override object Parse(TemplateContext context)
        {
            return null;
        }
        /// <summary>
        /// 获取标签的BOOLEAN
        /// </summary>
        /// <param name="context">上下文</param>
        public override bool ToBoolean(TemplateContext context)
        {
            return false;
        }
        /// <summary>
        /// 获取对象的字符串引用
        /// </summary>
        public override string ToString()
        {
            return string.Empty;
        }
    }
}