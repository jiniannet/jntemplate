/********************************************************************************
 Copyright (c) jiniannet (http://www.jiniannet.com). All rights reserved.
 Licensed under the MIT license. See licence.txt file in the project root for full license information.
 ********************************************************************************/
using JinianNet.JNTemplate.CodeCompilation;
using JinianNet.JNTemplate.Nodes;
using System;
using System.Linq;
using System.Reflection;
using System.Reflection.Emit;

namespace JinianNet.JNTemplate.Parsers
{
    /// <summary>
    /// The <see cref="TextTag"/> registrar
    /// </summary>
    public class TextVisitor : ITagVisitor
    {
        /// <inheritdoc />
        public ITag Parse(TemplateParser parser, TokenCollection tc)
        {
            return null;
        }

        /// <inheritdoc />
        public string Name => nameof(TextTag);

        /// <inheritdoc />
        public MethodInfo Compile(ITag tag, CompileContext c)
        {
            var t = tag as TextTag;
            if (!string.IsNullOrEmpty(t.Text))
            {
                var type = typeof(string);
                var mb = c.CreateReutrnMethod<TextTag>(type);
                var il = mb.GetILGenerator();
                il.Emit(OpCodes.Ldstr, t.Text);
                il.Emit(OpCodes.Ret);
                return mb.GetBaseDefinition();
            }
            return null;
        }
        /// <inheritdoc />
        public Type GuessType(ITag tag, CompileContext c)
        {
            return typeof(string);
        }
        /// <inheritdoc />
        public object Excute(ITag tag, TemplateContext context)
        {
            var t = tag as TextTag;
            return t.ToString(context.OutMode);
        } 
    }
}