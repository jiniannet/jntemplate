/********************************************************************************
 Copyright (c) jiniannet (http://www.jiniannet.com). All rights reserved.
 Licensed under the MIT license. See licence.txt file in the project root for full license information.
 ********************************************************************************/
using System;

namespace JinianNet.JNTemplate.Nodes
{
    /// <summary>
    /// 变量标签
    /// </summary>
    public class VariableTag : SimpleTag
    {

        private string _name;
        /// <summary>
        /// 变量名
        /// </summary>
        public string Name
        {
            get { return this._name; }
            set { this._name = value; }
        }
        /// <summary>
        /// 解析标签
        /// </summary>
        /// <param name="context">上下文</param>
        public override object ParseResult(TemplateContext context)
        {
            return context.TempData[this._name];
        }
        /// <summary>
        /// 解析标签
        /// </summary>
        /// <param name="context">上下文</param>
        /// <param name="baseValue">baseValue</param>
        public override object Parse(object baseValue, TemplateContext context)
        {
            if (baseValue == null)
            {
                return null;
            }
            return Engine.Runtime.CallPropertyOrField(baseValue, this._name);
        }

    }
}