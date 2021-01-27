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
    public class CompileTemplate : CompileTemplateBase
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
        public override void Render(TextWriter writer, TemplateContext context)
        {
            if (string.IsNullOrEmpty(this.TemplateKey))
            {
                if (!string.IsNullOrEmpty(this.Path))
                {
                    var full = context.FindFullPath(this.Path);
                    if (string.IsNullOrEmpty(full))
                    {
                        throw new Exception.TemplateException($"Path:\"{this.Path}\", the file could not be found.");
                    }
                    this.TemplateKey = full;
                }
                else if (!string.IsNullOrEmpty(this.TemplateContent))
                {
                    this.TemplateKey = this.TemplateContent.GetHashCode().ToString();
                }
                else
                {
                    throw new Exception.TemplateException("TemplateKey cannot be null.");
                }
            }
            var t = Runtime.Templates[this.TemplateKey];
            if (t == null)
            {
                var text = base.ReadTemplateContent();
                t = Runtime.Templates[this.TemplateKey] = Compiler.Compile(this.TemplateKey, text, (ctx) =>
                {
                    ctx.Data = context.TempData;
                    ctx.CurrentPath = context.CurrentPath;
                    ctx.Charset = context.Charset;
                    ctx.ResourceDirectories.AddRange(context.ResourceDirectories);
                    ctx.StripWhiteSpace = context.StripWhiteSpace;
                    ctx.ThrowExceptions = context.ThrowExceptions;
                });
                this.TemplateContent = null;
            } 
            if (t == null)
            {
                throw new Exception.CompileException("compile error.");
            }
            t.Render(writer, context);
        }
    }
}
