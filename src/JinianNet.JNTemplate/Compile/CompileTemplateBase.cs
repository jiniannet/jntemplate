/********************************************************************************
 Copyright (c) jiniannet (http://www.jiniannet.com). All rights reserved.
 Licensed under the MIT license. See licence.txt file in the project root for full license information.
 ********************************************************************************/
using System;
using System.IO;

namespace JinianNet.JNTemplate.Compile
{
    /// <summary>
    /// CompileTemplate
    /// </summary>
    [Serializable]
    public abstract class CompileTemplateBase : TemplateRender, ICompileTemplate, ITemplate
    {

        /// <summary>
        /// 呈现模板
        /// </summary>
        /// <param name="writer">TextWriter</param>
        /// <param name="context">Template Context</param>
        public abstract void Render(TextWriter writer, TemplateContext context);


        /// <summary>
        /// 呈现模板
        /// </summary>
        /// <param name="writer">TextWriter</param>
        public override void Render(TextWriter writer)
        {
            Render(writer, this.Context);
        }
    }
}
