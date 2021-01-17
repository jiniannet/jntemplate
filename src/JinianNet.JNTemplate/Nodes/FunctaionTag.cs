/********************************************************************************
 Copyright (c) jiniannet (http://www.jiniannet.com). All rights reserved.
 Licensed under the MIT license. See licence.txt file in the project root for full license information.
 ********************************************************************************/
using System;
#if !NET20
using System.Threading.Tasks;
#endif

namespace JinianNet.JNTemplate.Nodes
{
    /// <summary>
    /// 函数（方法）标签
    /// </summary>
    [Serializable]
    public class FunctaionTag : ChildrenTag
    {
        private string name;
        /// <summary>
        /// 函数
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
            object[] args = new object[Children.Count];
            for (int i = 0; i < this.Children.Count; i++)
            {
                args[i] = Children[i].ParseResult(context);
            }

            object parentValue;
            if (this.Parent == null)
            {
                parentValue = context.TempData[this.name];
            }
            else
            {
                parentValue = this.Parent.ParseResult(context);
            }

            if (parentValue == null)
            {
                return null;
            }
            if (this.Parent == null || (this.Parent != null && string.IsNullOrEmpty(this.name)))
            {
                if (parentValue is FuncHandler)
                {
                    return (parentValue as FuncHandler)(args);
                }
                if (parentValue is Delegate)
                {
                    return (parentValue as Delegate).DynamicInvoke(args);
                }
                return null;
            }

            var result = context.Actuator.CallMethod(parentValue, this.name, args);

            if (result != null)
            {
                return result;
            }

            result = context.Actuator.CallPropertyOrField(parentValue, this.name);

            if (result != null && result is Delegate)
            {
                return (result as Delegate).DynamicInvoke(args);
            }

            return null;
        }
    }
}