/********************************************************************************
 Copyright (c) jiniannet (http://www.jiniannet.com). All rights reserved.
 Licensed under the MIT license. See licence.txt file in the project root for full license information.
 ********************************************************************************/
using System;
using System.IO;

namespace JinianNet.JNTemplate.Compile
{
    /// <summary>
    /// 空模板
    /// </summary>
    public class EmptyCompileTemplate : CompileTemplate
    {
        /// <summary>
        /// ctor
        /// </summary>
        public EmptyCompileTemplate()
        {

        }

        /// <summary>
        /// ctor
        /// </summary>
        /// <param name="message">要输出的信息</param>
        public EmptyCompileTemplate(string message) 
        {
            this.TemplateContent = message;
        }

        /// <summary>
        /// Render
        /// </summary>
        /// <param name="writer">writer</param>
        /// <param name="context">context</param>
        public override void Render(TextWriter writer, TemplateContext context)
        {
            if (!string.IsNullOrWhiteSpace(this.TemplateContent) && context.ThrowExceptions)
            {
                writer.Write(TemplateContent);
            }
        }

    }
}