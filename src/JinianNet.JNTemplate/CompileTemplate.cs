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
using System.Threading.Tasks;

namespace JinianNet.JNTemplate
{
    /// <summary>
    ///  The compile template.
    /// </summary>
    public class CompileTemplate : TemplateBase, ICompileTemplate, ITemplate
    {

        private IReader reader;

        /// <summary>
        /// Initializes a new instance of the <see cref="CompileTemplate"/> class
        /// </summary>
        /// <param name="context">The <see cref="TemplateContext"/>.</param>
        /// <param name="reader">The template contents.</param>
        public CompileTemplate(TemplateContext context, IReader reader)
        {
            this.Context = context;
            this.reader = reader;
        }

        /// <inheritdoc />
        public bool EnableCompile => true;

        /// <inheritdoc />
        public virtual void Render(TextWriter writer, TemplateContext context)
        {
            var t = context.CompileTemplate(this.TemplateKey, this.reader);

            if (t == null)
            {
                throw new TemplateException($"compile error.");
            }
            try
            {
                t.Render(writer, context);
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


#if !NF40 && !NF45
        /// <inheritdoc />
        public Task RenderAsync(TextWriter writer, TemplateContext context, CancellationToken cancellationToken = default)
        {
            cancellationToken.ThrowIfCancellationRequested();

            var t = context.CompileTemplate(this.TemplateKey, this.reader);

            if (t == null)
            {
                throw new TemplateException($"compile error.");
            }
            try
            {
                return t.RenderAsync(writer, context, cancellationToken);
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