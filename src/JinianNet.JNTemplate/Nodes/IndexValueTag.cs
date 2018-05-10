/********************************************************************************
 Copyright (c) jiniannet (http://www.jiniannet.com). All rights reserved.
 Licensed under the MIT license. See licence.txt file in the project root for full license information.
 ********************************************************************************/
using System;
using System.IO;

namespace JinianNet.JNTemplate.Nodes
{
    /// <summary>
    /// 索引标签
    /// </summary>
    public class IndexValueTag : SimpleTag
    {
        /// <summary>
        /// 容器
        /// </summary>
        public SimpleTag Container { get; set; }

        /// <summary>
        /// 索引
        /// </summary>
        public Tag Index { get; set; }
        /// <summary>
        /// 解析标签
        /// </summary>
        /// <param name="context">上下文</param>
        public override object Parse(TemplateContext context)
        {
            object obj = this.Container.Parse(context);
            object index = this.Index.Parse(context);
            return Engine.Runtime.CallIndexValue(obj, index);
        }

        public override object Parse(object baseValue, TemplateContext context)
        {
            object obj = this.Container.Parse(baseValue,context);
            object index = this.Index.Parse(context);
            return Engine.Runtime.CallIndexValue(obj, index);
        }
    }
}
