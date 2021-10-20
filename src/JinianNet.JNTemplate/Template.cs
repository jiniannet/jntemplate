/********************************************************************************
 Copyright (c) jiniannet (http://www.jiniannet.com). All rights reserved.
 Licensed under the MIT license. See licence.txt file in the project root for full license information.
 ********************************************************************************/
using JinianNet.JNTemplate.Resources;
using System;
using System.IO;
using System.Threading.Tasks;

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
            : this(null, null)
        {

        }

        /// <summary>
        /// Initializes a new instance of the <see cref="Template"/> class
        /// </summary>
        /// <param name="ctx">The <see cref="TemplateContext"/>.</param>
        /// <param name="reader">The <see cref="TextReader"/>.</param>
        public Template(TemplateContext ctx, IReader reader)
        {
            Context = ctx;
            this.Reader = reader;
        }

        /// <inheritdoc />
        public bool EnableCompile => false; 
    }
}