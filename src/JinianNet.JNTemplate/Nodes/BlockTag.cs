/********************************************************************************
 Copyright (c) jiniannet (http://www.jiniannet.com). All rights reserved.
 Licensed under the MIT license. See licence.txt file in the project root for full license information.
 ********************************************************************************/
using System;
using System.IO;

namespace JinianNet.JNTemplate.Nodes
{
    /// <summary>
    /// 标签块
    /// </summary>
    public class BlockTag : ComplexTag
    {
        private TemplateRender _render;

        /// <summary>
        /// 模板KEY(用于缓存，默认为文路径)
        /// </summary>
        public string TemplateKey
        {
            get { return this._render.TemplateKey; }
            set { this._render.TemplateKey = value; }
        }

        /// <summary>
        /// 模板内容
        /// </summary>
        public string TemplateContent
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
        public override object ParseResult(TemplateContext context)
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
            this._render.Render(writer, ReadTags());
        }

        /// <summary>
        /// read all tags
        /// </summary>
        /// <returns></returns>
        protected virtual ITag[] ReadTags()
        {
            return this._render.ReadTags();
        } 
    }
}