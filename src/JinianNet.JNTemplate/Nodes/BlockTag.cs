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
        /// 读取取签
        /// </summary>
        /// <returns></returns>
        protected virtual ITag[] ReadTags()
        {
            return this.render.ReadAll(this.TemplateContent);
        }
    }
}