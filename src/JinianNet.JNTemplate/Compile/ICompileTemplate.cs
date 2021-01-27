/********************************************************************************
 Copyright (c) jiniannet (http://www.jiniannet.com). All rights reserved.
 Licensed under the MIT license. See licence.txt file in the project root for full license information.
 ********************************************************************************/
using System.IO;

namespace JinianNet.JNTemplate.Compile
{
    /// <summary>
    /// Compile Template
    /// </summary>
    public interface ICompileTemplate //: ITemplate
    {
        /// <summary>
        /// Render
        /// </summary>
        /// <param name="writer">TextWriter</param>
        /// <param name="context">Template Context</param>
        void Render(TextWriter writer, TemplateContext context);
    }

}
