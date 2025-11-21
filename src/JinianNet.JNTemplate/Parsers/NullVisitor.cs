/********************************************************************************
 Copyright (c) jiniannet (http://www.jiniannet.com). All rights reserved.
 Licensed under the MIT license. See licence.txt file in the project root for full license information.
 ********************************************************************************/
using JinianNet.JNTemplate.CodeCompilation;
using JinianNet.JNTemplate.Nodes;
using System;
using System.Reflection;
using System.Reflection.Emit;

namespace JinianNet.JNTemplate.Parsers
{
    /// <summary>
    /// The <see cref="NullTag"/> registrar
    /// </summary>
    public class NullVisitor : TagVisitor<NullTag>, ITagVisitor
    {
        /// <inheritdoc />
        public ITag Parse(TemplateParser parser, TokenCollection tc)
        {
            if (tc.Count == 1
                && tc.First.TokenKind == TokenKind.TextData
                && tc.First.Text == "null")
            {
                return new NullTag();
            }
            return null;
        }
        /// <inheritdoc />
        public MethodInfo Compile(ITag tag, CompileContext context)
        {
            var type = typeof(object);
            var mb = context.CreateReutrnMethod<NullTag>(type);
            var il = mb.GetILGenerator();
            il.Emit(OpCodes.Ldnull);
            il.Emit(OpCodes.Ret);
            return mb.GetBaseDefinition();

        }
        /// <inheritdoc />
        public Type GuessType(ITag tag, CompileContext context)
        {
            return typeof(object);
        }
        /// <inheritdoc />
        public object Excute(ITag tag, TemplateContext context)
        {
            return null;
        }
    }
}