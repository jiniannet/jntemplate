/********************************************************************************
 Copyright (c) jiniannet (http://www.jiniannet.com). All rights reserved.
 Licensed under the MIT license. See licence.txt file in the project root for full license information.
 ********************************************************************************/
using System;
using System.IO;
using System.Threading;
using System.Threading.Tasks;

namespace JinianNet.JNTemplate.CodeCompilation
{
    /// <summary>
    /// Returns a blank template.
    /// </summary>
    public class EmptyCompileTemplate : TemplateBase, ICompileTemplate, ITemplate
    {

        /// <summary>
        /// Initializes a new instance of the <see cref="VariableElement"/> class
        /// </summary>
        public EmptyCompileTemplate()
        {

        }

        /// <summary>
        /// Initializes a new instance of the <see cref="VariableElement"/> class
        /// </summary>
        /// <param name="message">The output message.</param>
        public EmptyCompileTemplate(string message)
            : base()
        {
            this.TemplateContent = message;
        }

        /// <inheritdoc />
        public bool EnableCompile => true;

        /// <inheritdoc />
        public void Render(TextWriter writer, TemplateContext context)
        {
            if (!string.IsNullOrWhiteSpace(this.TemplateContent) && context.ThrowExceptions)
            {
                writer.Write(TemplateContent);
            }
        }

        /// <inheritdoc />
        public void Render(TextWriter writer)
        {
            Render(writer, this.Context);
        }

#if !NF40 && !NF45
        /// <inheritdoc />
        public Task RenderAsync(TextWriter writer, CancellationToken cancellationToken = default)
        {
            cancellationToken.ThrowIfCancellationRequested();
            return RenderAsync(writer, this.Context);
        }

        /// <inheritdoc />
        public Task RenderAsync(TextWriter writer, TemplateContext context, CancellationToken cancellationToken = default)
        {
            cancellationToken.ThrowIfCancellationRequested();
            if (!string.IsNullOrWhiteSpace(this.TemplateContent) && context.ThrowExceptions)
            {
                return writer.WriteAsync(TemplateContent);
            }
            return Task.CompletedTask;
        }
#endif
    }
}