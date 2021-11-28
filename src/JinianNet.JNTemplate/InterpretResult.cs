/********************************************************************************
 Copyright (c) jiniannet (http://www.jiniannet.com). All rights reserved.
 Licensed under the MIT license. See licence.txt file in the project root for full license information.
 ********************************************************************************/
using System.IO;
using System.Threading;
using System.Threading.Tasks;
using JinianNet.JNTemplate.Nodes;

namespace JinianNet.JNTemplate
{
    /// <summary>
    /// 
    /// </summary>
    public class InterpretResult : IResult
    {
        /// <summary>
        /// 
        /// </summary>
        /// <param name="tags"></param>
        public InterpretResult(ITag[] tags)
        {
            this.Tags = tags;
        }

        /// <summary>
        /// 
        /// </summary>
        public ITag[] Tags { get; set; }

        /// <inheritdoc />
        public void Render(TextWriter writer, TemplateContext context)
        {
            context.Render(writer, Tags);
        }

#if !NF40 && !NF45
        /// <inheritdoc />
        public Task RenderAsync(TextWriter writer, TemplateContext context, CancellationToken cancellationToken = default)
        {
            cancellationToken.ThrowIfCancellationRequested();
            return context.RenderAsync(writer, Tags);
        }
#endif

    }
}
