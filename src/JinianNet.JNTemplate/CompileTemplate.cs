/********************************************************************************
 Copyright (c) jiniannet (http://www.jiniannet.com). All rights reserved.
 Licensed under the MIT license. See licence.txt file in the project root for full license information.
 ********************************************************************************/
using JinianNet.JNTemplate.CodeCompilation;
using JinianNet.JNTemplate.Exceptions;
using System;
using System.IO;

namespace JinianNet.JNTemplate
{
    /// <summary>
    ///  The compile template.
    /// </summary>
    public class CompileTemplate : TemplateBase, ICompileTemplate, ITemplate
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="CompileTemplate"/> class
        /// </summary> 
        public CompileTemplate()
            : this(null, null)
        {
        }


        /// <summary>
        /// Initializes a new instance of the <see cref="CompileTemplate"/> class
        /// </summary>
        /// <param name="context">The <see cref="TemplateContext"/>.</param>
        /// <param name="text">The template contents.</param>
        public CompileTemplate(TemplateContext context, string text)
        {
            Context = context;
            TemplateContent = text;
        }

        /// <inheritdoc />
        public virtual void Render(TextWriter writer, TemplateContext context)
        {
            var text = this.TemplateContent;
            var t = context.Options.CompilerResults.GetOrAdd(this.TemplateKey, (key) =>
            {
                return TemplateCompiler.Compile(key, text, context.Options, (ctx) =>
                 {
                     context.CopyTo(ctx);
                 });
            });
            if (t == null)
            {
                throw new TemplateException($"compile error.");
            }
            try
            {
                t.Render(writer, context);
            }
            catch(System.Exception e)
            {
                context.AddError(e);
            }
        }



        /// <inheritdoc />
        public virtual void Render(System.IO.TextWriter writer)
        {
            Render(writer, this.Context);
        }
    }
}
