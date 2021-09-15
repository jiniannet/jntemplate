/********************************************************************************
 Copyright (c) jiniannet (http://www.jiniannet.com). All rights reserved.
 Licensed under the MIT license. See licence.txt file in the project root for full license information.
 ********************************************************************************/
using JinianNet.JNTemplate.CodeCompilation;
using JinianNet.JNTemplate.Exceptions;
using JinianNet.JNTemplate.Hosting;
using System;
using System.IO;

namespace JinianNet.JNTemplate
{
    /// <summary>
    ///  The compile template.
    /// </summary>
    public class CompileTemplate : CompileTemplateBase, ICompileTemplate, ITemplate
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
        public override void Render(TextWriter writer, TemplateContext context)
        {
            var text = this.TemplateContent;
            var environment = context.Environment;
            var t = environment.Results.GetOrAdd(this.TemplateKey, (key) =>
            {
                return environment.Compile(key, text, (ctx) =>
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
            catch (System.Exception e)
            {
                context.AddError(e);
            }
        }

    }
}
