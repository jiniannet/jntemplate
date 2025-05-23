/********************************************************************************
 Copyright (c) jiniannet (http://www.jiniannet.com). All rights reserved.
 Licensed under the MIT license. See licence.txt file in the project root for full license information.
 ********************************************************************************/
using System;
using JinianNet.JNTemplate.Nodes;
using JinianNet.JNTemplate.Dynamic;
using JinianNet.JNTemplate.Exceptions;
#if !NF35 && !NF20
using System.Threading.Tasks;
#endif
using System.Threading;
using System.IO;
using JinianNet.JNTemplate.Resources;

namespace JinianNet.JNTemplate
{
    /// <summary>
    /// A template for Render
    /// </summary>
    public class TemplateRender : TemplateBase
    {

        /// <summary>
        /// Gets or sets the <see cref="TextReader"/> of the template.
        /// </summary>
        public IResourceReader Reader { get; set; }

        /// <summary>
        /// Performs the render for a template.
        /// </summary>
        /// <param name="writer">See the <see cref="System.IO.TextWriter"/>.</param>
        public virtual void Render(System.IO.TextWriter writer)
        {
            var interpretResult = Context.InterpretTemplate(this.TemplateKey, Reader);
            interpretResult.Render(writer, Context);
        }


#if !NF40 && !NF45 && !NF35 && !NF20
        /// <summary>
        /// Performs the render for a template.
        /// </summary>
        /// <param name="writer">See the <see cref="System.IO.TextWriter"/>.</param>
        /// <param name="cancellationToken">See the <see cref="CancellationToken"/>.</param>
        public Task RenderAsync(System.IO.TextWriter writer, CancellationToken cancellationToken = default)
        {
            if (cancellationToken.IsCancellationRequested)
            {
                return Task.FromCanceled(cancellationToken);
            }
            var interpretResult = Context.InterpretTemplate(this.TemplateKey, Reader);
            return interpretResult.RenderAsync(writer, Context, cancellationToken);
        }
#endif
    }
}