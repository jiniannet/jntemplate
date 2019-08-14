/********************************************************************************
 Copyright (c) jiniannet (http://www.jiniannet.com). All rights reserved.
 Licensed under the MIT license. See licence.txt file in the project root for full license information.
 ********************************************************************************/
using System.Collections.ObjectModel;
#if !NET20
using System.Threading.Tasks;
#endif

namespace JinianNet.JNTemplate.Nodes
{
    /// <summary>
    /// 标签接口
    /// </summary>
    public interface ITag
    {
        /// <summary>
        /// 子标签
        /// </summary>
        Collection<ITag> Children { get; }

        /// <summary>
        /// 添加一个子标签
        /// </summary>
        /// <param name="node"></param>
        void AddChild(ITag node);


        /// <summary>
        /// 解析结果
        /// </summary>
        /// <param name="context">TemplateContext</param>
        /// <returns></returns>
        object ParseResult(TemplateContext context);

        /// <summary>
        /// 解析
        /// </summary>
        /// <param name="context">TemplateContext</param>
        /// <param name="write">TextWriter</param>
        void Parse(TemplateContext context, System.IO.TextWriter write);

        /// <summary>
        /// 开始Token
        /// </summary>
        Token FirstToken { get; set; }
        /// <summary>
        /// 结束Token
        /// </summary>
        Token LastToken { get; set; }


#if NETCOREAPP || NETSTANDARD
        /// <summary>
        /// 异步解析
        /// </summary>
        /// <param name="context">TemplateContext</param>
        /// <param name="write">TextWriter</param>
        /// <returns></returns>
        Task ParseAsync(TemplateContext context, System.IO.TextWriter write);

        /// <summary>
        /// 异步解析结果
        /// </summary>
        /// <param name="context">TemplateContext</param>
        /// <returns></returns>
        Task<object> ParseResultAsync(TemplateContext context);
#endif

    }
}
