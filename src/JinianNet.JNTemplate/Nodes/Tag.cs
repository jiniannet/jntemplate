/********************************************************************************
 Copyright (c) jiniannet (http://www.jiniannet.com). All rights reserved.
 Licensed under the MIT license. See licence.txt file in the project root for full license information.
 ********************************************************************************/
using JinianNet.JNTemplate.Dynamic;
using System;
using System.Collections;
using System.Collections.Generic;
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
        public virtual void AddChild(ITag node)
        {
            if (node != null)
            {
                children.Add(node);
            }
        } 

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