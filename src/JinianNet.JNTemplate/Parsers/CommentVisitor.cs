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
    /// The <see cref="CommentTag"/> registrar
    /// </summary>
    public class CommentVisitor : TagVisitor<CommentTag>, ITagVisitor
    {
        /// <inheritdoc />
        public ITag Parse(TemplateParser parser, TokenCollection tc)
        {
            if (tc.Count == 1
                && tc[0].TokenKind == TokenKind.Comment
                )
            {
                var tag = new CommentTag();
                tag.FirstToken = tc[0];
                return tag;
            }
            return null;
        }

        /// <inheritdoc />
        public MethodInfo Compile(ITag tag, CompileContext context)
        {
            var type = typeof(void);
            var mb = context.CreateReutrnMethod<CommentTag>(type);
            var il = mb.GetILGenerator();
            il.Emit(OpCodes.Ret);
            return mb.GetBaseDefinition();
        }
        /// <inheritdoc />
        public Type GuessType(ITag tag, CompileContext context)
        {
            return typeof(void);
        }

        /// <inheritdoc />
        public object Excute(ITag tag, TemplateContext context)
        {
            return null;
        }
    }
}