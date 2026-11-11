/********************************************************************************
 Copyright (c) jiniannet (http://www.jiniannet.com). All rights reserved.
 Licensed under the MIT license. See licence.txt file in the project root for full license information.
 ********************************************************************************/

using System.IO;
using System.Threading;
#if !NF35 && !NF20
using System.Threading.Tasks;
#endif

namespace JinianNet.JNTemplate.CodeCompilation;

internal class EmptyCompilerResult : ICompilerResult
{
    public void Render(TextWriter writer, TemplateContext context)
    {
        throw new System.NotImplementedException();
    }
#if !NF40 && !NF45 && !NF35 && !NF20
    public Task RenderAsync(TextWriter writer, TemplateContext context, CancellationToken cancellationToken = default)
    {
        return Task.CompletedTask;
    }
#endif
}
