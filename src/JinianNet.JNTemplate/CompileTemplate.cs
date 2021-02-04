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
    ///  Compile Template
    /// </summary>
    public class CompileTemplate : TemplateBase,ICompileTemplate, ITemplate
    {
        /// <summary>
        /// CompileTemplate
        /// </summary> 
        public CompileTemplate()
            : this(Engine.CreateContext(), null)
        {
        }


        /// <summary>
        /// CompileTemplate
        /// </summary>
        /// <param name="ctx">TemplateContext 对象</param>
        /// <param name="text">模板内容</param>
        public CompileTemplate(TemplateContext ctx, string text)
        {
            if (ctx == null)
            {
                throw new ArgumentNullException("\"ctx\" cannot be null.");
            }
            Context = ctx;
            TemplateContent = text;
        }

        /// <summary>
        /// 呈现模板
        /// </summary>
        /// <param name="writer">writer</param>
        /// <param name="context">context</param>
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

         

        /// <summary>
        /// 呈现内容
        /// </summary>
        /// <param name="writer">TextWriter</param>
        public virtual void Render(System.IO.TextWriter writer)
        {
            Render(writer, this.Context);
        }
    }
}
