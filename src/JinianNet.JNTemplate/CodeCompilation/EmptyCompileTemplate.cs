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
namespace JinianNet.JNTemplate.CodeCompilation
{
    /// <summary>
    /// Returns a blank template.
    /// </summary>
    public class EmptyCompileTemplate : TemplateBase, ICompileTemplate, ITemplate
    {
        private string message;
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
            this.message = message;
        }

        /// <inheritdoc />
        public bool IsCompileMode => true;

        /// <inheritdoc />
        public void Dispose()
        {

        }

        /// <inheritdoc />
        public void Render(TextWriter writer, TemplateContext context)
        {
            if (
#if NF35 || NF20
                !string.IsNullOrEmpty(this.message)
#else
                !string.IsNullOrWhiteSpace(this.message)
#endif
                && context.ThrowExceptions)
            {
                writer.Write(message);
            }
        }

        /// <inheritdoc />
        public void Render(TextWriter writer)
        {
            Render(writer, this.Context);
        }

#if !NF40 && !NF45 && !NF35 && !NF20
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
            if (!string.IsNullOrWhiteSpace(this.message) && context.ThrowExceptions)
            {
                return writer.WriteAsync(message);
            }
            return Task.CompletedTask;
        }
#endif
    }
}