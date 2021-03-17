/********************************************************************************
 Copyright (c) jiniannet (http://www.jiniannet.com). All rights reserved.
 Licensed under the MIT license. See licence.txt file in the project root for full license information.
 ********************************************************************************/
using System;
using System.Collections.Generic;
using System.IO;

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
            : this(Engine.CreateContext(), string.Empty)
        {

        }

        /// <summary>
        /// Initializes a new instance of the <see cref="Template"/> class
        /// </summary>
        /// <param name="ctx">The <see cref="TemplateContext"/>.</param>
        /// <param name="text">The contents of template.</param>
        public Template(TemplateContext ctx, string text)
        {
            if (ctx == null)
            {
                throw new ArgumentNullException("\"ctx\" cannot be null.");
            }
            Context = ctx;
            TemplateContent = text;
        }
    }
}