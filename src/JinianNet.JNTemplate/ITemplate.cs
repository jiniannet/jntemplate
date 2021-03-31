/********************************************************************************
 Copyright (c) jiniannet (http://www.jiniannet.com). All rights reserved.
 Licensed under the MIT license. See licence.txt file in the project root for full license information.
 ********************************************************************************/
using System;
using System.IO;
#if !NET20
using System.Threading.Tasks;
#endif

namespace JinianNet.JNTemplate
{
    /// <summary>
    /// A interface for template
    /// </summary>
    public interface ITemplate
    {
        /// <summary>
        /// Gets or sets the context of the template.
        /// </summary>
        TemplateContext Context { get; set; }
        /// <summary>
        /// Gets or sets the Unique key of the template.
        /// </summary>
        string TemplateKey { get; set; }
        /// <summary>
        /// Gets or sets the content of the template.
        /// </summary>
        string TemplateContent { get; set; }
        /// <summary>
        /// Performs the render for a template.
        /// </summary>
        /// <param name="writer">See the <see cref="TextWriter"/>.</param>
        void Render(TextWriter writer);


        /// <summary>
        /// Set a new value for variables.
        /// </summary>
        /// <param name="key">The key of the element to get</param> 
        /// <param name="value">The element with the specified key.</param>
        void Set<T>(string key, T value);

        /// <summary>
        /// Set a static type for variables.
        /// </summary>
        /// <param name="key">The key of the element to get</param> 
        /// <param name="type">The type with the specified key.</param>
        void SetStaticType(string key, Type type);
    }
}