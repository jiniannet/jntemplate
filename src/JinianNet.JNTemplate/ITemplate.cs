/********************************************************************************
 Copyright (c) jiniannet (http://www.jiniannet.com). All rights reserved.
 Licensed under the MIT license. See licence.txt file in the project root for full license information.
 ********************************************************************************/
using System;
using System.IO;
#if !NET20
using System.Threading.Tasks;
#endif

namespace JinianNet.JNTemplate
{
    /// <summary>
    /// Template 接口
    /// </summary>
    public interface ITemplate
    {
        /// <summary>
        /// 模板上下文
        /// </summary>
        TemplateContext Context { get; set; }
        /// <summary>
        /// 模板内容
        /// </summary>
        string TemplateContent { get; set; }
        /// <summary>
        /// 结果呈现
        /// </summary>
        /// <param name="writer"></param>
        void Render(TextWriter writer);

#if NETCOREAPP || NETSTANDARD
        /// <summary>
        /// 结果异步呈现
        /// </summary>
        /// <param name="writer"></param>
        Task RenderAsync(TextWriter writer);
#endif

    }
}