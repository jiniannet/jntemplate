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
    /// The <see cref="StringTag"/> registrar
    /// </summary>
    public class StringRegistrar : TagRegistrar<StringTag>, IRegistrar
    {
        /// <inheritdoc />
        public override Func<TemplateParser, TokenCollection, ITag> BuildParseMethod()
        {
            return (parser, tc) =>
            {
                if (tc != null
                                && (tc.Count == 3 || tc.Count == 2)
                                && tc.First.TokenKind == TokenKind.StringStart
                                && tc.Last.TokenKind == TokenKind.StringEnd
                                )
                {
                    var tag = new StringTag();
                    tag.Value = tc.Count == 3 ? tc[1].Text : string.Empty;
                    return tag;
                }
                return null;
            };
        }

        /// <inheritdoc />
        public override Func<ITag, CompileContext, MethodInfo> BuildCompileMethod()
        {
            return (tag, c) =>
            {
                var t = tag as StringTag;
                var type = typeof(string);
                var mb = c.CreateReutrnMethod<StringTag>(type);
                var il = mb.GetILGenerator();
                il.CallTypeTag(t);
                il.Emit(OpCodes.Ret);
                return mb.GetBaseDefinition();
            };
        }
        /// <inheritdoc />
        public override Func<ITag, CompileContext, Type> BuildGuessMethod()
        {
            return (tag, c) =>
            {
                return typeof(string);
            };
        }
        /// <inheritdoc />
        public override Func<ITag, TemplateContext, object> BuildExcuteMethod()
        {
            return (tag, context) =>
            {
                var t = tag as StringTag;
                return t.Value;
            };
        }
    }
}