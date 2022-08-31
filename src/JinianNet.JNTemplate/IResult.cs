/********************************************************************************
 Copyright (c) jiniannet (http://www.jiniannet.com). All rights reserved.
 Licensed under the MIT license. See licence.txt file in the project root for full license information.
 ********************************************************************************/
using System.IO;
using System.Threading;
#if !NF35 && !NF20
using System.Threading.Tasks;
#endif


namespace JinianNet.JNTemplate
{
    /// <summary>
    /// 
    /// </summary>
    public interface IResult
    {
        /// <summary>
        /// Performs the render for a template.
        /// </summary>
        /// <param name="writer">The <see cref="TextWriter"/>.</param>
        /// <param name="context">The <see cref="TemplateContext"/>.</param>
        void Render(TextWriter writer, TemplateContext context);

#if !NF40 && !NF45 && !NF35 && !NF20
        /// <summary>
        /// Performs the render for a template.
        /// </summary>
        /// <param name="writer">The <see cref="TextWriter"/>.</param>
        /// <param name="context">The <see cref="TemplateContext"/>.</param>
        /// <param name="cancellationToken">See the <see cref="CancellationToken"/>.</param>
        Task RenderAsync(TextWriter writer, TemplateContext context, CancellationToken cancellationToken = default);
#endif
    }
}
