/********************************************************************************
 Copyright (c) jiniannet (http://www.jiniannet.com). All rights reserved.
 Licensed under the MIT license. See licence.txt file in the project root for full license information.
 ********************************************************************************/
using System;

namespace JinianNet.JNTemplate
{
    /// <summary>
    /// An template object.
    /// </summary>
    public class Template : TemplateRender, ITemplate
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="Template"/> class
        /// </summary>
        public Template()
            : this(null, string.Empty)
        {

        }

        /// <summary>
        /// Initializes a new instance of the <see cref="Template"/> class
        /// </summary>
        /// <param name="ctx">The <see cref="TemplateContext"/>.</param>
        /// <param name="text">The contents of template.</param>
        public Template(TemplateContext ctx, string text)
        {
            Context = ctx;
            TemplateContent = text;
        }

        /// <inheritdoc />
        public bool EnableCompile => false;
    }
}