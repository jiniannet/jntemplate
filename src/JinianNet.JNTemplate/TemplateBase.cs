/********************************************************************************
 Copyright (c) jiniannet (http://www.jiniannet.com). All rights reserved.
 Licensed under the MIT license. See licence.txt file in the project root for full license information.
 ********************************************************************************/
using System;

namespace JinianNet.JNTemplate
{
    /// <summary>
    /// The base class of template.
    /// </summary>
    public abstract class TemplateBase
    {
        /// <summary>
        /// Gets or sets the key(unique) of the template.
        /// </summary>
        public string TemplateKey { get; set; }

        /// <summary>
        /// Gets or sets the context of the template.
        /// </summary>
        public TemplateContext Context { get; set; }

        /// <summary>
        /// Gets or sets the content of the template.
        /// </summary>
        public string TemplateContent { get; set; }
    }
}
