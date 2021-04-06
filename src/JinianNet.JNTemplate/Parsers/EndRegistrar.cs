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
    /// The <see cref="EndTag"/> registrar
    /// </summary>
    public class EndRegistrar : TagRegistrar<EndTag>, IRegistrar
    {
        /// <inheritdoc />
        public override Func<TemplateParser, TokenCollection, ITag> BuildParseMethod()
        {
            return (parser, tc) =>
            {
                if (tc != null
                    && tc.Count == 1
                    && Utility.IsEqual(tc.First.Text, Field.KEY_END))
                {
                    return new EndTag();
                }

                return null;
            };
        }

        /// <inheritdoc />
        public override Func<ITag, CompileContext, MethodInfo> BuildCompileMethod()
        {
            return (tag, c) =>
            {
                var type = typeof(void);
                var mb = c.CreateReutrnMethod<EndTag>(type);
                var il = mb.GetILGenerator();
                il.Emit(OpCodes.Ret);
                return mb.GetBaseDefinition();
            };
        }
        /// <inheritdoc />
        public override Func<ITag, CompileContext, Type> BuildGuessMethod()
        {
            return (tag, c) =>
            {
                return typeof(void);
            };
        }
        /// <inheritdoc />
        public override Func<ITag, TemplateContext, object> BuildExcuteMethod()
        {
            return (tag, context) =>
            {
                return null;
            };
        }
    }
}