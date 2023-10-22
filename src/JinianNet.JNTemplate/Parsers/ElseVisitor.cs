/********************************************************************************
 Copyright (c) jiniannet (http://www.jiniannet.com). All rights reserved.
 Licensed under the MIT license. See licence.txt file in the project root for full license information.
 ********************************************************************************/
using JinianNet.JNTemplate.CodeCompilation;
using JinianNet.JNTemplate.Nodes;
using System.Reflection;
using System;
using JinianNet.JNTemplate.Dynamic;

namespace JinianNet.JNTemplate.Parsers
{
    /// <summary>
    /// The <see cref="ElseTag"/> registrar
    /// </summary>
    public class ElseVisitor : TagVisitor<ElseTag>, ITagVisitor
    {
        /// <inheritdoc />
        public ITag Parse(TemplateParser parser, TokenCollection tc)
        {
            if (tc.Count == 1
                && Utility.IsEqual(tc.First.Text, Const.KEY_ELSE))
            {
                return new ElseTag();
            }

            return null;
        }

        /// <inheritdoc />
        public MethodInfo Compile(ITag tag, CompileContext c)
        {
            return c.IfCompile((ElseTag)tag);
        }
        /// <inheritdoc />
        public Type GuessType(ITag tag, CompileContext c)
        {
            return c.GuessIfType((ElseTag)tag);
        }
        /// <inheritdoc />
        public object Excute(ITag tag, TemplateContext context)
        {
            var t = tag as ElseTag;
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
        }
    }
}