/********************************************************************************
 Copyright (c) jiniannet (http://www.jiniannet.com). All rights reserved.
 Licensed under the MIT license. See licence.txt file in the project root for full license information.
 ********************************************************************************/
using JinianNet.JNTemplate.CodeCompilation;
using JinianNet.JNTemplate.Dynamic;
using JinianNet.JNTemplate.Nodes;
using System;
using System.Linq;
using System.Reflection;
using System.Reflection.Emit;

namespace JinianNet.JNTemplate.Parsers
{
    /// <summary>
    /// The <see cref="BodyTag"/> registrar
    /// </summary>
    public class BodyRegistrar : TagRegistrar<BodyTag>, IRegistrar
    {
        /// <inheritdoc />
        public override Func<TemplateParser, TokenCollection, ITag> BuildParseMethod()
        {
            return (parser, tc) =>
            {
                if (tc != null
                    && tc.Count == 1
                    && Utility.IsEqual(tc.First.Text, Const.KEY_BODY))
                {
                    return new BodyTag();
                }

                return null;
            };
        }
        /// <inheritdoc />
        public override Func<ITag, CompileContext, MethodInfo> BuildCompileMethod()
        {
            return (tag, c) =>
            {
                var t = tag as BodyTag;
                var type = typeof(string);
                var mb = c.CreateReutrnMethod<BodyTag>(type);
                var il = mb.GetILGenerator();
                c.BlockCompile(il, t.Children.ToArray());
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
                var t = tag as BodyTag;
                if (t.Children.Count == 0)
                {
                    return null;
                }
                if (t.Children.Count == 1)
                {
                    return context.Execute(t.Children[0]);
                }
                var sb = new System.Text.StringBuilder();
                for (int i = 0; i < t.Children.Count; i++)
                {
                    sb.Append(context.Execute(t.Children[i]));
                }
                return sb.ToString();
            };
        }
    }
}