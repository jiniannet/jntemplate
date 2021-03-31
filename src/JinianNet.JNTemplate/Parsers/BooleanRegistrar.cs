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
    /// The <see cref="BooleanTag"/> registrar
    /// </summary>
    public class BooleanRegistrar : TagRegistrar<BooleanTag>, IRegistrar
    {
        /// <inheritdoc />
        public override Func<TemplateParser, TokenCollection, ITag> BuildParseMethod()
        {
            return (parser, tc) =>
            {
                if (tc != null
                    && tc.Count == 1
                    && (tc.First.Text == "true" || tc.First.Text == "false"))
                {
                    var tag = new BooleanTag();
                    tag.Value = Utility.StringToBoolean(tc.First.Text);
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
                var t = tag as BooleanTag;
                var type = t.Value.GetType();
                var mb = c.CreateReutrnMethod<BooleanTag>(type);
                var il = mb.GetILGenerator();
                il.DeclareLocal(type);
                il.CallTypeTag(t);
                il.Emit(OpCodes.Stloc_0);
                il.Emit(OpCodes.Ldloc_0);
                il.Emit(OpCodes.Ret);
                return mb.GetBaseDefinition();
            };
        }
        /// <inheritdoc />
        public override Func<ITag, CompileContext, Type> BuildGuessMethod()
        {
            return (tag, c) =>
            {
                return typeof(bool);
            };
        }
    }
}