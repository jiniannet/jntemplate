/********************************************************************************
 Copyright (c) jiniannet (http://www.jiniannet.com). All rights reserved.
 Licensed under the MIT license. See licence.txt file in the project root for full license information.
 ********************************************************************************/
using JinianNet.JNTemplate.CodeCompilation;
using JinianNet.JNTemplate.Nodes;
using System;

namespace JinianNet.JNTemplate.Parsers
{
    /// <summary>
    /// The tag parser.
    /// </summary>
    public interface ITagParser
    {
        /// <summary>
        /// Parsing then tag.
        /// </summary>
        /// <param name="parser">The <see cref="TemplateParser"/>.</param>
        /// <param name="tc">The token collection.</param>
        /// <returns>An tag.</returns>
        ITag Parse(TemplateParser parser, TokenCollection tc);
    }
}