/********************************************************************************
 Copyright (c) jiniannet (http://www.jiniannet.com). All rights reserved.
 Licensed under the MIT license. See licence.txt file in the project root for full license information.
 ********************************************************************************/
using JinianNet.JNTemplate.Compile;
using System;
using System.IO;

namespace JinianNet.JNTemplate
{
    /// <summary>
    ///  The compile template.
    /// </summary>
    public class CompileTemplate : TemplateBase,ICompileTemplate, ITemplate
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="CompileTemplate"/> class
        /// </summary> 
        public CompileTemplate()
            : this(Engine.CreateContext(), null)
        {
        }


        /// <summary>
        /// Initializes a new instance of the <see cref="CompileTemplate"/> class
        /// </summary>
        /// <param name="context">The <see cref="TemplateContext"/>.</param>
        /// <param name="text">The template contents.</param>
        public CompileTemplate(TemplateContext context, string text)
        {
            if (context == null)
            {
                throw new ArgumentNullException("\"context\" cannot be null.");
            }
            Context = context;
            TemplateContent = text;
        }

        /// <inheritdoc />
        public virtual void Render(TextWriter writer, TemplateContext context)
        {
            var t = Runtime.Templates[this.TemplateKey];
            if (t == null)
            {
                var text = this.TemplateContent;
                t = Runtime.Templates[this.TemplateKey] = Compiler.Compile(this.TemplateKey, text, (ctx) =>
                {
                    context.CopyTo(ctx);
                });
                if (t == null)
                {
                    throw new Exception.TemplateException($"compile error.");
                }
                this.TemplateContent = null;
            }
            t.Render(writer, context); 
        }



        /// <inheritdoc />
        public virtual void Render(System.IO.TextWriter writer)
        {
            Render(writer, this.Context);
        }
    }
}
