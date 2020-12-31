/********************************************************************************
 Copyright (c) jiniannet (http://www.jiniannet.com). All rights reserved.
 Licensed under the MIT license. See licence.txt file in the project root for full license information.
 ********************************************************************************/
using System;

namespace JinianNet.JNTemplate.Nodes
{
    /// <summary>
    /// 变量标签(ParseResult)
    /// </summary>
    [Serializable]
    public class VariableTag : ChildrenTag
    {
        private string name;
        /// <summary>
        /// 变量名
        /// </summary>
        public string Name
        {
            get { return this.name; }
            set { this.name = value; }
        }
        /// <summary>
        /// 解析标签
        /// </summary>
        /// <param name="context">上下文</param>
        public override object ParseResult(TemplateContext context)
        {
            object baseValue = null;
            if (this.Parent != null)
            {
                baseValue = this.Parent.ParseResult(context);
            }
            if (baseValue == null)
            {
                return context.TempData[this.name];
            }
            return context.Actuator.CallPropertyOrField(baseValue, this.name);
        }
    }
}
