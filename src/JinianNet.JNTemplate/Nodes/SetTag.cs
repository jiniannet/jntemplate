/********************************************************************************
 Copyright (c) jiniannet (http://www.jiniannet.com). All rights reserved.
 Licensed under the MIT license. See licence.txt file in the project root for full license information.
 ********************************************************************************/
using System;

namespace JinianNet.JNTemplate.Nodes
{
    /// <summary>
    /// 赋值标签
    /// </summary>
    public class SetTag : TagBase
    {
        private string _name;
        private Tag _value;

        /// <summary>
        /// 变量名
        /// </summary>
        public string Name
        {
            get { return this._name; }
            set { this._name = value; }
        }

        /// <summary>
        /// 值
        /// </summary>
        public Tag Value
        {
            get { return this._value; }
            set { this._value = value; }
        }

        /// <summary>
        /// 解析标签
        /// </summary>
        /// <param name="context">上下文</param>
        public override object Parse(TemplateContext context)
        {
            object value = this.Value.Parse(context);
            if (!context.TempData.SetValue(this.Name,value))
            {
                context.TempData.Push(this.Name, value);
            }
            return null;
        }
        /// <summary>
        /// 解析标签
        /// </summary>
        /// <param name="context">上下文</param>
        /// <param name="write">write</param>
        public override void Parse(TemplateContext context, System.IO.TextWriter write)
        {
            Parse(context);
        }
    }
}