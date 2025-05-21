/********************************************************************************
 Copyright (c) jiniannet (http://www.jiniannet.com). All rights reserved.
 Licensed under the MIT license. See licence.txt file in the project root for full license information.
 ********************************************************************************/
using System.IO;
using System.Threading; 
using JinianNet.JNTemplate.Nodes;
#if !NF35 && !NF20
using System.Threading.Tasks;
#endif
namespace JinianNet.JNTemplate
{
    /// <summary>
    /// 
    /// </summary>
    public class InterpretResult : ITemplateResult
    {
        /// <summary>
        /// 
        /// </summary>
        /// <param name="tags"></param>
        public InterpretResult(TagCollection tags)
        {
            this.Tags = tags;
        }

        /// <summary>
        /// 
        /// </summary>
        public TagCollection Tags { get; set; }

        /// <inheritdoc />
        public void Render(TextWriter writer, TemplateContext context)
        {
            context.Render(writer, Tags);
        }

#if !NF40 && !NF45 && !NF35 && !NF20
        /// <inheritdoc />
        public Task RenderAsync(TextWriter writer, TemplateContext context, CancellationToken cancellationToken = default)
        {
            cancellationToken.ThrowIfCancellationRequested();
            return context.RenderAsync(writer, Tags);
        }
#endif

    }
}
