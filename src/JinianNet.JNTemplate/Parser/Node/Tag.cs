/*****************************************************
   Copyright (c) 2013-2014 翅膀的初衷  (http://www.jiniannet.com)

   Licensed under the Apache License, Version 2.0 (the "License");
   you may not use this file except in compliance with the License.
   You may obtain a copy of the License at

       http://www.apache.org/licenses/LICENSE-2.0

   Unless required by applicable law or agreed to in writing, software
   distributed under the License is distributed on an "AS IS" BASIS,
   WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
   See the License for the specific language governing permissions and
   limitations under the License.

   Redistributions of source code must retain the above copyright notice
 *****************************************************/
using System;
using System.Collections.Generic;
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
        private List<Tag> children;
        /// <summary>
        /// 标签
        /// </summary>
        public Tag()
        {
            this.children = new List<Tag>();
        }
        /// <summary>
        /// 子标签
        /// </summary>
        public List<Tag> Children { 
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
        /// <param name="baseValue">基本值</param>
        /// <param name="context">TemplateContext</param>
        /// <returns></returns>
        public abstract Object Parse(Object baseValue, TemplateContext context);

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
            }
            return value != null;
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