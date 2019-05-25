/********************************************************************************
 Copyright (c) jiniannet (http://www.jiniannet.com). All rights reserved.
 Licensed under the MIT license. See licence.txt file in the project root for full license information.
 ********************************************************************************/
using System;
using System.Threading.Tasks;

namespace JinianNet.JNTemplate.Nodes
{
    /// <summary>
    /// 简单标签
    /// 可以组合的标签
    /// </summary>
    public abstract class SimpleTag : TagBase
    {
        /// <summary>
        /// 解析结果
        /// </summary>
        /// <param name="baseValue">基本值</param>
        /// <param name="context">TemplateContext</param>
        /// <returns></returns>
        public abstract object ParseResult(object baseValue, TemplateContext context);

#if NETCOREAPP || NETSTANDARD
        /// <summary>
        /// 解析结果
        /// </summary>
        /// <param name="baseValue">基本值</param>
        /// <param name="context">TemplateContext</param>
        /// <returns></returns>
        public virtual async Task<object> ParseResultAsync(object baseValue, TemplateContext context)
        {
            return await Task.Run<object>(()=> ParseResult(baseValue, context));
        }
#endif
    }
}