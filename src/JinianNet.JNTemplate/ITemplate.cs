/********************************************************************************
 Copyright (c) jiniannet (http://www.jiniannet.com). All rights reserved.
 Licensed under the MIT license. See licence.txt file in the project root for full license information.
 ********************************************************************************/
using System;
using System.IO;
using System.Threading;
#if !NF35 && !NF20
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
        /// Performs the render for a template.
        /// </summary>
        /// <param name="writer">See the <see cref="TextWriter"/>.</param>
        void Render(TextWriter writer);

        /// <summary>
        /// Enable or disenable the compile mode.
        /// </summary>
        bool EnableCompile { get; }

#if !NF40 && !NF45 && !NF35 && !NF20
        /// <summary>
        /// Performs the render for a template.
        /// </summary>
        /// <param name="writer">See the <see cref="TextWriter"/>.</param>
        /// <param name="cancellationToken">See the <see cref="CancellationToken"/>.</param>
        Task RenderAsync(TextWriter writer, CancellationToken cancellationToken = default);
#endif
    }
}