/********************************************************************************
 Copyright (c) jiniannet (http://www.jiniannet.com). All rights reserved.
 Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.
 ********************************************************************************/
using System;


namespace JinianNet.JNTemplate.Parser.Node
{
    /// <summary>
    /// 赋值标签
    /// </summary>
    public class SetTag : BaseTag
    {
        private String _name;
        /// <summary>
        /// 变量名
        /// </summary>
        public String Name
        {
            get { return _name; }
            set { _name = value; }
        }

        private Tag _value;
        /// <summary>
        /// 值
        /// </summary>
        public Tag Value
        {
            get { return _value; }
            set { _value = value; }
        }

        /// <summary>
        /// 解析标签
        /// </summary>
        /// <param name="context">上下文</param>
        public override Object Parse(TemplateContext context)
        {
            Object value = this.Value.Parse(context);
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