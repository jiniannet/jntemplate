/********************************************************************************
 Copyright (c) jiniannet (http://www.jiniannet.com). All rights reserved.
 Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.
 ********************************************************************************/
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Text;

namespace JinianNet.JNTemplate.Parser.Node
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
        public abstract Object Parse(TemplateContext context);

        /// <summary>
        /// 解析结果
        /// </summary>
        /// <param name="context">TemplateContext</param>
        /// <param name="write">TextWriter</param>
        public abstract void Parse(TemplateContext context, System.IO.TextWriter write);

        /// <summary>
        /// 转换为 Boolean 
        /// </summary>
        /// <param name="context">TemplateContext</param>
        /// <returns></returns>
        public virtual Boolean ToBoolean(TemplateContext context)
        {
            Object value = Parse(context);
            return Common.Calculator.CalculateBoolean(value);
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