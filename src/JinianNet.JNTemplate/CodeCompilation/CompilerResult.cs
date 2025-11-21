/********************************************************************************
 Copyright (c) jiniannet (http://www.jiniannet.com). All rights reserved.
 Licensed under the MIT license. See licence.txt file in the project root for full license information.
 ********************************************************************************/
using System;
using System.Collections.Generic;
using System.IO;
using System.Threading;


#if !NF35 && !NF20
using System.Threading.Tasks;
#endif
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
        public virtual void Render(TextWriter writer, TemplateContext context)
        {
            if (writer == null)
            {
                writer = new StringWriter();
            }
            if (context == null)
            {
                throw new ArgumentNullException(nameof(context));
            }
            RenderResult(writer, context);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="writer"></param>
        /// <param name="context"></param>
        protected abstract void RenderResult(TextWriter writer, TemplateContext context);

        /// <summary>
        /// 
        /// </summary>
        /// <param name="ctx"></param>
        /// <param name="exception"></param>
        /// <param name="line"></param>
        /// <param name="col"></param>
        /// <param name="source"></param>
        public static void ThrowException(TemplateContext ctx, Exception exception, int line, int col, string source)
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

#if !NF40 && !NF45 && !NF35 && !NF20
        /// <inheritdoc />
        public virtual async Task RenderAsync(TextWriter writer, TemplateContext context, CancellationToken cancellationToken = default)
        {
            cancellationToken.ThrowIfCancellationRequested();
            if (writer == null)
            {
                writer = new StringWriter();
            }
            if (context == null)
            {
                throw new ArgumentNullException(nameof(context));
            }
            var textWriter = writer;
            var templateContext = context;

            var tasks = GetRenderTask(templateContext);
            foreach(var t in tasks)
            {
                if (t is Task<string> task)
                {
                    var text = await task;
                    await textWriter.WriteAsync(text);
                    continue;
                }
                if (t is string str)
                {
                    await textWriter.WriteAsync(str);
                    continue;
                }
                await textWriter.WriteAsync(t?.ToString());
            } 
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="context"></param>
        /// <returns></returns>
        protected abstract List<object> GetRenderTask(TemplateContext context);
#endif
    }
}
