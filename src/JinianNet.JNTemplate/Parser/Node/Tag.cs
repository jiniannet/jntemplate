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
        private Token first, last;
        //private Tag parent;
        private Collection<Tag> children = new Collection<Tag>();

        /// <summary>
        /// 子标签
        /// </summary>
        public Collection<Tag> Children
        { 
            get { return this.children; }
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
            if (value == null)
                return false;
            switch (value.GetType().FullName)
            {
                case "System.Boolean":
                    return (Boolean)value;
                case "System.String":
                    return !String.IsNullOrEmpty(value.ToString());
                case "System.UInt16":
                case "System.UInt32":
                case "System.UInt64":
                case "System.Int16":
                case "System.Int32":
                case "System.Int64":
                    return value.ToString()!="0";
                case "System.Decimal":
                    return (Decimal)value != 0;
                case "System.Double":
                    return (Double)value != 0;
                case "System.Single":
                    return (Single)value != 0;
                default:
                    return value != null;
            }
        }

        /// <summary>
        /// 开始Token
        /// </summary>
        public Token FirstToken
        {
            get { return first; }
            set { first = value; }
        }
        /// <summary>
        /// 结束Token
        /// </summary>
        public Token LastToken
        {
            set {  last = value; }
            get { return last;}
        }

        /// <summary>
        /// 添加一个子标签
        /// </summary>
        /// <param name="node"></param>
        public void AddChild(Tag node)
        {
            //node.Parent = this;
            Children.Add(node);
        }


    }
}