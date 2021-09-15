/********************************************************************************
 Copyright (c) jiniannet (http://www.jiniannet.com). All rights reserved.
 Licensed under the MIT license. See licence.txt file in the project root for full license information.
 ********************************************************************************/
using System.IO;
using System.Threading.Tasks;

namespace JinianNet.JNTemplate.CodeCompilation
{
    /// <summary>
    /// Represents the results of compilation that are returned from a compiler. 
    /// </summary>
    public interface ICompilerResult
    {
        /// <summary>
        /// Performs the render for a template.
        /// </summary>
        /// <param name="writer">The <see cref="TextWriter"/>.</param>
        /// <param name="context">The <see cref="TemplateContext"/>.</param>
        void Render(TextWriter writer, TemplateContext context);

#if !NF40 && !NF45
        /// <summary>
        /// Performs the render for a template.
        /// </summary>
        /// <param name="writer">The <see cref="TextWriter"/>.</param>
        /// <param name="context">The <see cref="TemplateContext"/>.</param>
        Task RenderAsync(TextWriter writer, TemplateContext context);
#endif
    }
}
