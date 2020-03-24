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
    /// 基本标签(ParseResult)
    /// </summary>
    [Serializable]
    public abstract class BasisTag  : Tag,ITag
    {
        /// <summary>
        /// 解析结果
        /// </summary>
        /// <param name="baseValue">基本值</param>
        /// <param name="context">TemplateContext</param>
        /// <returns></returns>
        public abstract object ParseResult(object baseValue, TemplateContext context);

        /// <summary>
        /// 解析标签
        /// </summary>
        /// <param name="context">上下文</param>
        /// <param name="write">write</param>
        public override void Parse(TemplateContext context, System.IO.TextWriter write)
        {
            write.Write(ParseResult(context));
        }

#if NETCOREAPP || NETSTANDARD

        /// <summary>
        /// 异步执行标签
        /// </summary>
        /// <param name="context"></param>
        /// <param name="write"></param>
        /// <returns></returns>
        public override async Task ParseAsync(TemplateContext context, TextWriter write)
        {
            var data = await ParseResultAsync(context);
            await write.WriteAsync(data?.ToString());
        }

        /// <summary>
        /// 解析结果
        /// </summary>
        /// <param name="baseValue">基本值</param>
        /// <param name="context">TemplateContext</param>
        /// <returns></returns>
        public virtual async Task<object> ParseResultAsync(object baseValue, TemplateContext context)
        {
            return await Task.Run<object>(() => ParseResult(baseValue, context));
        }
#endif
    }
}
