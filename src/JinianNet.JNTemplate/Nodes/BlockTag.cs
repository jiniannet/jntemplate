/********************************************************************************
 Copyright (c) jiniannet (http://www.jiniannet.com). All rights reserved.
 Licensed under the MIT license. See licence.txt file in the project root for full license information.
 ********************************************************************************/
using System;

namespace JinianNet.JNTemplate.Nodes
{
    /// <summary>
    /// BlockTag
    /// </summary>
    [Serializable]
    public class BlockTag : ComplexTag
    {
        private TemplateRender render;

        /// <summary>
        /// Gets or sets the context of the template.
        /// </summary>
        public string TemplateKey
        {
            get { return this.render.TemplateKey; }
            set { this.render.TemplateKey = value; }
        }

        /// <summary>
        /// Gets or sets the content of the template.
        /// </summary>
        public string TemplateContent
        {
            get { return this.render.TemplateContent; }
            set { this.render.TemplateContent = value; }
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="BlockTag"/> class
        /// </summary>
        public BlockTag()
        {
            this.render = new TemplateRender();
        }

        /// <summary>
        /// Rreads the contents of the text into a tag array.
        /// </summary>
        /// <returns></returns>
        protected virtual ITag[] ReadTags()
        {
            return this.render.ReadAll(this.TemplateContent);
        }
    }
}