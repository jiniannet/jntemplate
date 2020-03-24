/********************************************************************************
 Copyright (c) jiniannet (http://www.jiniannet.com). All rights reserved.
 Licensed under the MIT license. See licence.txt file in the project root for full license information.
 ********************************************************************************/
using JinianNet.JNTemplate.Dynamic;
using System;
using System.Collections.ObjectModel;
#if !NET20
using System.Threading.Tasks;
#endif


namespace JinianNet.JNTemplate.Nodes
{
    /// <summary>
    /// 标签基类
    /// </summary>
    public abstract class Tag : ITag
    {
        private Token first, last;
        private Collection<ITag> children = new Collection<ITag>();
        /// <summary>
        /// 子标签
        /// </summary>
        public Collection<ITag> Children
        {
            get { return children; }
        }

        /// <summary>
        /// 添加一个子标签
        /// </summary>
        /// <param name="node"></param>
        public void AddChild(ITag node)
        {
            if (node != null)
            {
                Children.Add(node);
            }
        }


        /// <summary>
        /// 解析结果
        /// </summary>
        /// <param name="context">TemplateContext</param>
        /// <returns></returns>
        public abstract object ParseResult(TemplateContext context);

        /// <summary>
        /// 解析结果
        /// </summary>
        /// <param name="context">TemplateContext</param>
        /// <param name="write">TextWriter</param>
        public abstract void Parse(TemplateContext context, System.IO.TextWriter write);

#if NETCOREAPP || NETSTANDARD
        /// <summary>
        /// 异步解析结果
        /// </summary>
        /// <param name="context">TemplateContext</param>
        /// <param name="write">TextWriter</param>
        /// <returns></returns>
        public async virtual Task ParseAsync(TemplateContext context, System.IO.TextWriter write)
        {
            await Task.Run(() =>
            {
                Parse(context, write);
            });
        }

        /// <summary>
        /// 异步解析结果
        /// </summary>
        /// <param name="context">TemplateContext</param>
        /// <returns></returns>
        public async virtual Task<object> ParseResultAsync(TemplateContext context)
        {
            return await Task<object>.Run(() =>
            {
                return ParseResult(context);
            });
        }
#endif

        /// <summary>
        /// 开始Token
        /// </summary>
        public Token FirstToken
        {
            get { return this.first; }
            set { this.first = value; }
        }
        /// <summary>
        /// 结束Token
        /// </summary>
        public Token LastToken
        {
            set { this.last = value; }
            get { return this.last; }
        }
    }
}