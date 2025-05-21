/********************************************************************************
 Copyright (c) jiniannet (http://www.jiniannet.com). All rights reserved.
 Licensed under the MIT license. See licence.txt file in the project root for full license information.
 ********************************************************************************/
using JinianNet.JNTemplate.CodeCompilation;
using JinianNet.JNTemplate.Exceptions;
using JinianNet.JNTemplate.Hosting;
using JinianNet.JNTemplate.Resources;
using System;
using System.IO;
using System.Threading;
#if !NF35 && !NF20
using System.Threading.Tasks;
#endif

namespace JinianNet.JNTemplate
{
    /// <summary>
    /// An template object.
    /// </summary>
    public class Template : TemplateBase, ITemplate
    {
        private IResourceReader reader;

        /// <summary>
        /// Initializes a new instance of the <see cref="Template"/> class
        /// </summary>
        /// <param name="context">The <see cref="TemplateContext"/>.</param>
        /// <param name="reader">The template contents.</param>
        public Template(TemplateContext context,
            IResourceReader reader)
        {
            this.Context = context;
            this.reader = reader;
        }

        /// <inheritdoc />
        public bool IsCompileMode => Context.IsCompileMode;

        /// <inheritdoc />
        public virtual void Render(TextWriter writer, TemplateContext context)
        {
            try
            {
                GetResult(context)?.Render(writer, context);
            }
            catch (System.Exception e)
            {
                context.AddError(e);
            }
        }

        /// <inheritdoc />
        public void Render(TextWriter writer)
        {
            Render(writer, this.Context);
        }

        private ITemplateResult GetResult(TemplateContext context)
        {
            if (!this.Context.IsCompileMode)
                return context.InterpretTemplate(this.TemplateKey, this.reader);
            return context.CompileTemplate(this.TemplateKey, this.reader);
        }

#if !NF40 && !NF45 && !NF35 && !NF20
        /// <inheritdoc />
        public Task RenderAsync(TextWriter writer, TemplateContext context, CancellationToken cancellationToken = default)
        {
            cancellationToken.ThrowIfCancellationRequested();
            try
            {
                return GetResult(context)?.RenderAsync(writer, context, cancellationToken);
            }
            catch (System.Exception e)
            {
                context.AddError(e);
                return Task.FromException(e);
            }
        }

        /// <inheritdoc />
        public Task RenderAsync(TextWriter writer, CancellationToken cancellationToken = default)
        {
            return RenderAsync(writer, this.Context);
        }
#endif
    }
}