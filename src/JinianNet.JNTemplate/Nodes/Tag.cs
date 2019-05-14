/********************************************************************************
 Copyright (c) jiniannet (http://www.jiniannet.com). All rights reserved.
 Licensed under the MIT license. See licence.txt file in the project root for full license information.
 ********************************************************************************/
using JinianNet.JNTemplate.Dynamic;
using System;
using System.Collections.ObjectModel;
#if NETCOREAPP || NETSTANDARD
using System.Threading.Tasks;
#endif


namespace JinianNet.JNTemplate.Nodes
{
    /// <summary>
    /// 标签基类
    /// </summary>
    public abstract class Tag
    {
        private Token _first, _last;
        //private Tag parent;
        private Collection<Tag> _children = new Collection<Tag>();

        /// <summary>
        /// 子标签
        /// </summary>
        public Collection<Tag> Children
        {
            get { return this._children; }
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
        public async Task ParseAsync(TemplateContext context, System.IO.TextWriter write)
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
        public async Task<object> ParseResultAsync(TemplateContext context)
        {
            return await Task<object>.Run(() =>
            {
                return ParseResult(context);
            });
        }

        
        /// <summary>
        /// 转换为 bool 
        /// </summary>
        /// <param name="context">TemplateContext</param>
        /// <returns></returns>
        public async virtual Task<bool> ToBooleanAsync(TemplateContext context)
        {
            return await Task<object>.Run(() =>
            {
                return ToBoolean(context);
            });
        }
#endif
        /// <summary>
        /// 转换为 bool 
        /// </summary>
        /// <param name="context">TemplateContext</param>
        /// <returns></returns>
        public virtual bool ToBoolean(TemplateContext context)
        {
            object value = ParseResult(context);
            return ExpressionEvaluator.CalculateBoolean(value);
        }


        /// <summary>
        /// 开始Token
        /// </summary>
        public Token FirstToken
        {
            get { return this._first; }
            set { this._first = value; }
        }
        /// <summary>
        /// 结束Token
        /// </summary>
        public Token LastToken
        {
            set { this._last = value; }
            get { return this._last; }
        }

        /// <summary>
        /// 添加一个子标签
        /// </summary>
        /// <param name="node"></param>
        public void AddChild(Tag node)
        {
            if (node != null)
            {
                Children.Add(node);
            }
        }
    }
}