/********************************************************************************
 Copyright (c) jiniannet (http://www.jiniannet.com). All rights reserved.
 Licensed under the MIT license. See licence.txt file in the project root for full license information.
 ********************************************************************************/
using System;

namespace JinianNet.JNTemplate.Nodes
{
    /// <summary>
    /// 文本标签
    /// </summary>
    public class CommentTag : Tag
    {

        /// <summary>
        /// 注释标签
        /// </summary>
        /// <param name="context">上下文</param>
        public override object Parse(TemplateContext context)
        {
            return null;
        }
        /// <summary>
        /// 解析标签
        /// </summary>
        /// <param name="context">上下文</param>
        /// <param name="write">write</param>
        public override void Parse(TemplateContext context, System.IO.TextWriter write)
        {

        }

        /// <summary>
        /// 获取对象的字符串引用
        /// </summary>
        public override string ToString()
        {
            return this.FirstToken.ToString();
        }

    }
}