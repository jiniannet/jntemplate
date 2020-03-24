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
    /// ELSE if 标签
    /// </summary>
    [Serializable]
    public class ElseifTag : ComplexTag
    {

        private ITag test;
        /// <summary>
        /// 条件
        /// </summary>
        public virtual ITag Test
        {
            get { return this.test; }
            set { this.test = value; }
        }

        /// <summary>
        /// 解析标签
        /// </summary>
        /// <param name="context">上下文</param>
        /// <param name="write">write</param>
        public override void Parse(TemplateContext context, TextWriter write)
        {
            for (int i = 0; i < Children.Count; i++)
            {
                Children[i].Parse(context, write);
            }
        }

        /// <summary>
        /// 获取布布值
        /// </summary>
        /// <param name="context">上下文</param>
        public virtual bool ToBoolean(TemplateContext context)
        {
            return Utility.ToBoolean(this.test.ParseResult(context));
        }

#if NETCOREAPP || NETSTANDARD
        /// <summary>
        /// 异步解析结果
        /// </summary>
        /// <param name="context">TemplateContext</param>
        /// <param name="writer">TextWriter</param>
        /// <returns></returns>
        public override async Task ParseAsync(TemplateContext context, TextWriter writer)
        {
            for (int i = 0; i < Children.Count; i++)
            {
                await Children[i].ParseAsync(context, writer);
            }
        }
#endif
    }
}