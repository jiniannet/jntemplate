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
    public class EndVisitor : TagVisitor<EndTag>, ITagVisitor
    {
        /// <inheritdoc />
        public ITag Parse(TemplateParser parser, TokenCollection tc)
        {

            if (tc.Count == 1
                && Utility.IsEqual(tc.First.Text, Const.KEY_END))
            {
                return new EndTag();
            }

            return null;
        }

        /// <inheritdoc />
        public MethodInfo Compile(ITag tag, CompileContext context)
        {
            var type = typeof(void);
            var mb = context.CreateReutrnMethod<EndTag>(type);
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