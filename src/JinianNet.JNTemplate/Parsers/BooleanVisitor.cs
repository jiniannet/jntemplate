/********************************************************************************
 Copyright (c) jiniannet (http://www.jiniannet.com). All rights reserved.
 Licensed under the MIT license. See licence.txt file in the project root for full license information.
 ********************************************************************************/
using JinianNet.JNTemplate.CodeCompilation;
using JinianNet.JNTemplate.Dynamic;
using JinianNet.JNTemplate.Nodes;
using System;
using System.Reflection;
using System.Reflection.Emit;

namespace JinianNet.JNTemplate.Parsers
{
    /// <summary>
    /// The <see cref="BooleanTag"/> registrar
    /// </summary>
    public class BooleanVisitor : TagVisitor<BooleanTag>, ITagVisitor
    {
        /// <inheritdoc />
        public ITag Parse(TemplateParser parser, TokenCollection tc)
        {
            if (tc.Count == 1
                && (tc.First.Text == "true" || tc.First.Text == "false"))
            {
                var tag = new BooleanTag();
                tag.Value = Utility.StringToBoolean(tc.First.Text);
                return tag;
            }

            return null;

        }
        /// <inheritdoc />
        public MethodInfo Compile(ITag tag, CompileContext context)
        {
            var t = tag as BooleanTag;
            var type = t.Value.GetType();
            var mb = context.CreateReutrnMethod<BooleanTag>(type);
            var il = mb.GetILGenerator();
            il.DeclareLocal(type);
            il.CallTypeTag(t);
            il.Emit(OpCodes.Stloc_0);
            il.Emit(OpCodes.Ldloc_0);
            il.Emit(OpCodes.Ret);
            return mb.GetBaseDefinition();
        }
        /// <inheritdoc />
        public Type GuessType(ITag tag, CompileContext context)
        {
            return typeof(bool);

        }

        /// <inheritdoc />
        public object Excute(ITag tag, TemplateContext context)
        {
            var t = tag as BooleanTag;
            return t.Value; 
        }
    }
}