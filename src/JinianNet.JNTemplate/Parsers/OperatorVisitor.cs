/********************************************************************************
 Copyright (c) jiniannet (http://www.jiniannet.com). All rights reserved.
 Licensed under the MIT license. See licence.txt file in the project root for full license information.
 ********************************************************************************/
using JinianNet.JNTemplate.CodeCompilation;
using JinianNet.JNTemplate.Nodes;
using System.Reflection;
using System.Reflection.Emit;
using System;

namespace JinianNet.JNTemplate.Parsers
{
    /// <summary>
    /// The <see cref="OperatorVisitor"/> registrar
    /// </summary>
    public class OperatorVisitor : TagVisitor<OperatorTag>, ITagVisitor
    {
        /// <inheritdoc />
        public ITag Parse(TemplateParser parser, TokenCollection tc)
        {
            return null;
        }
        /// <inheritdoc />
        public MethodInfo Compile(ITag tag, CompileContext c)
        {
            return null;
        }
        /// <inheritdoc />
        public Type GuessType(ITag tag, CompileContext c)
        {
            return null;
        }
        /// <inheritdoc />
        public object Excute(ITag tag, TemplateContext context)
        {
            var t = tag as OperatorTag;
            return t.Value;
        }
    }
}