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
    /// An template object.
    /// </summary>
    public class Template : TemplateBase, ITemplate
    {

        private IReader reader;
        private IResult templateResult;

        /// <summary>
        /// Initializes a new instance of the <see cref="Template"/> class
        /// </summary>
        /// <param name="context">The <see cref="TemplateContext"/>.</param>
        /// <param name="reader">The template contents.</param>
        public Template(TemplateContext context,
            IReader reader)
        {
            this.Context = context;
            this.reader = reader;
        }

        /// <inheritdoc />
        public bool EnableCompile => this.Context.Mode == EngineMode.Compiled;

        /// <inheritdoc />
        public virtual void Render(TextWriter writer, TemplateContext context)
        {
            if (templateResult == null)
            {
                if (this.Context.Mode == EngineMode.Interpreted)
                {
                    templateResult = context.InterpretTemplate(this.TemplateKey, this.reader);
                }
                else
                {
                    templateResult = context.CompileTemplate(this.TemplateKey, this.reader);
                }
                if (templateResult == null)
                {
                    throw new TemplateException($"template error.");
                }
            }
            try
            {
                templateResult.Render(writer, context);
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
                throw new TemplateException($"template error.");
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