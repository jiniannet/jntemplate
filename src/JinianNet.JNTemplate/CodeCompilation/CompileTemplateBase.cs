/********************************************************************************
 Copyright (c) jiniannet (http://www.jiniannet.com). All rights reserved.
 Licensed under the MIT license. See licence.txt file in the project root for full license information.
 ********************************************************************************/
using System;
using System.IO;
using System.Threading.Tasks;

namespace JinianNet.JNTemplate.CodeCompilation
{
    /// <summary>
    /// The base class of the <see cref="ICompileTemplate"/>
    /// </summary>
    [Serializable]
    public abstract class CompileTemplateBase : TemplateRender, ICompileTemplate, ITemplate
    {
        /// <inheritdoc />
        public abstract void Render(TextWriter writer, TemplateContext context);

        /// <inheritdoc />
        public override void Render(TextWriter writer)
        {
            Render(writer, this.Context);
        }
        /// <inheritdoc />
        public virtual bool EnableCompile => true;

#if !NF40 && !NF45
        /// <inheritdoc />
        public virtual Task RenderAsync(TextWriter writer, TemplateContext context)
        {
            var textWriter = writer;
            var templateContext = context;
            return Task.Run(() => Render(textWriter, templateContext));
        }

        /// <inheritdoc />
        public virtual Task RenderAsync(TextWriter writer)
        {
            return RenderAsync(writer, this.Context);
        }
#endif
    }
}
