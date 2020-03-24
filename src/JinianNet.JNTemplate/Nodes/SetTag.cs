/********************************************************************************
 Copyright (c) jiniannet (http://www.jiniannet.com). All rights reserved.
 Licensed under the MIT license. See licence.txt file in the project root for full license information.
 ********************************************************************************/
using System;
using System.IO;
#if !NET20
using System.Threading.Tasks;
#endif

namespace JinianNet.JNTemplate.Nodes
{
    /// <summary>
    /// 赋值标签
    /// </summary>
    [Serializable]
    public class SetTag : ComplexTag
    {
        private string name;
        private ITag value;

        /// <summary>
        /// 变量名
        /// </summary>
        public string Name
        {
            get { return this.name; }
            set { this.name = value; }
        }

        /// <summary>
        /// 值
        /// </summary>
        public ITag Value
        {
            get { return this.value; }
            set { this.value = value; }
        }

        /// <summary>
        /// 解析标签
        /// </summary>
        /// <param name="context">上下文</param>
        public override object ParseResult(TemplateContext context)
        {
            object value = this.Value.ParseResult(context);
            if (!context.TempData.SetValue(this.Name, value))
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
        public override void Parse(TemplateContext context, TextWriter write)
        {
            ParseResult(context);
        }

#if NETCOREAPP || NETSTANDARD
        /// <summary>
        /// 解析标签
        /// </summary>
        /// <param name="context">上下文</param>
        public override async Task<object> ParseResultAsync(TemplateContext context)
        {
            object value = await this.Value.ParseResultAsync(context);
            if (!context.TempData.SetValue(this.Name, value))
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
        public override async Task ParseAsync(TemplateContext context, TextWriter write)
        {
            await ParseResultAsync(context);
        }
#endif
    }
}