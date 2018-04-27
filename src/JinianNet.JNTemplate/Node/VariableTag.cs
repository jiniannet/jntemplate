/********************************************************************************
 Copyright (c) jiniannet (http://www.jiniannet.com). All rights reserved.
 Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.
 ********************************************************************************/
using System;

namespace JinianNet.JNTemplate.Node
{
    /// <summary>
    /// 变量标签
    /// </summary>
    public class VariableTag : SimpleTag
    {

        private String _name;
        /// <summary>
        /// 变量名
        /// </summary>
        public String Name
        {
            get { return this._name; }
            set { this._name = value; }
        }
        /// <summary>
        /// 解析标签
        /// </summary>
        /// <param name="context">上下文</param>
        public override Object Parse(TemplateContext context)
        {
            return context.TempData[this._name];
        }
        /// <summary>
        /// 解析标签
        /// </summary>
        /// <param name="context">上下文</param>
        /// <param name="baseValue">baseValue</param>
        public override Object Parse(Object baseValue, TemplateContext context)
        {
            if (baseValue == null)
            {
                return null;
            }
            return Engine.Runtime.CallPropertyOrField(baseValue, this._name);
        }

    }
}