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
    public class TextRegistrar : TagRegistrar<TextTag>, IRegistrar
    {
        /// <inheritdoc />
        public override Func<TemplateParser, TokenCollection, ITag> BuildParseMethod()
        {
            return null;
        }
        /// <inheritdoc />
        public override Func<ITag, CompileContext, MethodInfo> BuildCompileMethod()
        {
            return (tag, c) =>
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
                var t = tag as TextTag;
                return t.ToString(context.OutMode);
            };
        }
    }
}