/********************************************************************************
 Copyright (c) jiniannet (http://www.jiniannet.com). All rights reserved.
 Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.
 ********************************************************************************/
using System;
using System.IO;

namespace JinianNet.JNTemplate.Parser.Node
{
    /// <summary>
    /// 标签块
    /// </summary>
    public class BlockTag : TagBase
    {
        private TemplateRender _render;

        /// <summary>
        /// 模板内容
        /// </summary>
        public String TemplateContent
        {
            get { return this._render.TemplateContent; }
            set { this._render.TemplateContent = value; }
        }

        /// <summary>
        /// 标签块
        /// </summary>
        public BlockTag()
        {
            this._render = new TemplateRender();
        }


        /// <summary>
        /// 解析标签
        /// </summary>
        /// <param name="context">上下文</param>
        public override Object Parse(TemplateContext context)
        {
            using (System.IO.StringWriter writer = new StringWriter())
            {
                Render(context, writer);

                return writer.ToString();
            }
        }
        /// <summary>
        /// 解析标签
        /// </summary>
        /// <param name="context">上下文</param>
        /// <param name="write">write</param>
        public override void Parse(TemplateContext context, TextWriter write)
        {
            Render(context, write);
        }
        /// <summary>
        /// 呈现标签
        /// </summary>
        /// <param name="context">上下文</param>
        /// <param name="writer">writer</param>
        protected void Render(TemplateContext context, TextWriter writer)
        {
            this._render.Context = context;
            this._render.Render(writer);
        }
    }
}