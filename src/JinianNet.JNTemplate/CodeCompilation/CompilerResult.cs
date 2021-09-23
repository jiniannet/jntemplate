/********************************************************************************
 Copyright (c) jiniannet (http://www.jiniannet.com). All rights reserved.
 Licensed under the MIT license. See licence.txt file in the project root for full license information.
 ********************************************************************************/
using System;
using System.IO;
using System.Threading;
using System.Threading.Tasks;
using JinianNet.JNTemplate.Exceptions;

namespace JinianNet.JNTemplate.CodeCompilation
{
    /// <summary>
    /// Represents the results of compilation that are returned from a compiler. 
    /// </summary>
    /// <summary>
    /// 
    /// </summary>
    public abstract class CompilerResult : ICompilerResult
    {

        /// <inheritdoc />
        public abstract void Render(TextWriter writer, TemplateContext context);

        /// <summary>
        /// 
        /// </summary>
        /// <param name="ctx"></param>
        /// <param name="exception"></param>
        /// <param name="line"></param>
        /// <param name="col"></param>
        /// <param name="source"></param>
        public void ThrowException(TemplateContext ctx, Exception exception, int line, int col, string source)
        {
            TemplateException templateException;
            if (exception is TemplateException tempException)
            {
                templateException = tempException;
            }
            else
            {
                string message = string.Concat(exception.Message, " on ", source, " [line:", line, ",col:", col, "]");
                templateException = new RuntimeException(message, exception);
                templateException.Line = line;
                templateException.Column = col;
            }
            ctx.AddError(templateException);
        }

#if !NF40 && !NF45
        /// <inheritdoc />
        public virtual Task RenderAsync(TextWriter writer, TemplateContext context, CancellationToken cancellationToken = default)
        {
            cancellationToken.ThrowIfCancellationRequested();
            var textWriter = writer;
            var templateContext = context;
            return Task.Run(() => Render(textWriter, templateContext));
        }
#endif
    }
}
