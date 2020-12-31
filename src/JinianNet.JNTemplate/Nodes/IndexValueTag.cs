/********************************************************************************
 Copyright (c) jiniannet (http://www.jiniannet.com). All rights reserved.
 Licensed under the MIT license. See licence.txt file in the project root for full license information.
 ********************************************************************************/
using System;
using System.IO;
#if !NET20
using System.Threading.Tasks;
#endif

namespace JinianNet.JNTemplate.Nodes
{
    /// <summary>
    /// 索引标签
    /// </summary>
    [Serializable]
    public class IndexValueTag : ChildrenTag
    {
        /// <summary>
        /// 索引
        /// </summary>
        public ITag Index { get; set; }
        /// <summary>
        /// 解析标签
        /// </summary>
        /// <param name="context">上下文</param>
        public override object ParseResult(TemplateContext context)
        {
            object obj = this.Parent.ParseResult(context);
            object index = this.Index.ParseResult(context);
            return context.Actuator.CallIndexValue(obj, index);
        }

#if NETCOREAPP || NETSTANDARD
        /// <summary>
        /// 解析标签
        /// </summary>
        /// <param name="context">上下文</param>
        public override async Task<object> ParseResultAsync(TemplateContext context)
        {
            object obj = await this.Parent.ParseResultAsync(context);
            object index = await this.Index.ParseResultAsync(context);
            return context.Actuator.CallIndexValue(obj, index);
        }
#endif
    }
}
