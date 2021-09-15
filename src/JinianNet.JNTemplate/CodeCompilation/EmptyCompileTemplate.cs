/********************************************************************************
 Copyright (c) jiniannet (http://www.jiniannet.com). All rights reserved.
 Licensed under the MIT license. See licence.txt file in the project root for full license information.
 ********************************************************************************/
using System;
using System.IO;

namespace JinianNet.JNTemplate.CodeCompilation
{
    /// <summary>
    /// Returns a blank template.
    /// </summary>
    public class EmptyCompileTemplate : CompileTemplate
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="VariableElement"/> class
        /// </summary>
        public EmptyCompileTemplate()
            : base()
        {

        }

        /// <summary>
        /// Initializes a new instance of the <see cref="VariableElement"/> class
        /// </summary>
        /// <param name="message">The output message.</param>
        public EmptyCompileTemplate(string message)
            : base()
        {
            this.TemplateContent = message;
        }


        /// <inheritdoc />
        public override void Render(TextWriter writer, TemplateContext context)
        {
            if (!string.IsNullOrWhiteSpace(this.TemplateContent) && context.ThrowExceptions)
            {
                writer.Write(TemplateContent);
            }
        }

    }
}