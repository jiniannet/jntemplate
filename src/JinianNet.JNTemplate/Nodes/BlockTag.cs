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
    /// 标签块
    /// </summary>
    [Serializable]
    public class BlockTag : ComplexTag
    {
        private TemplateRender render;

        /// <summary>
        /// 模板KEY(用于缓存，默认为文路径)
        /// </summary>
        public string TemplateKey
        {
            get { return this.render.TemplateKey; }
            set { this.render.TemplateKey = value; }
        }

        /// <summary>
        /// 模板内容
        /// </summary>
        public string TemplateContent
        {
            get { return this.render.TemplateContent; }
            set { this.render.TemplateContent = value; }
        }

        /// <summary>
        /// 标签块
        /// </summary>
        public BlockTag()
        {
            this.render = new TemplateRender();
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
            this.render.Context = context;
            var ts = ReadTags();
            this.render.Render(writer, ts);
        }

        /// <summary>
        /// 读取取签
        /// </summary>
        /// <returns></returns>
        protected virtual ITag[] ReadTags()
        {
            return this.render.ReadTags();
        }

#if NETCOREAPP || NETSTANDARD
        /// <summary>
        /// 读取取签
        /// </summary>
        /// <returns></returns>
        protected virtual async Task<ITag[]> ReadTagsAsync()
        {
            return await this.render.ReadTagsAsync();
        }
        /// <summary>
        /// 异步解析结果
        /// </summary>
        /// <param name="context">TemplateContext</param>
        /// <param name="writer">TextWriter</param>
        /// <returns></returns>
        public override async Task ParseAsync(TemplateContext context, TextWriter writer)
        {
            this.render.Context = context;
            var ts = await ReadTagsAsync();
            await this.render.RenderAsync(writer, ts);
        }
#endif
    }
}