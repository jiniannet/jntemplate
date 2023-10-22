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
    /// The <see cref="ElseifTag"/> registrar
    /// </summary>
    public class ElseifVisitor : TagVisitor<ElseifTag>, ITagVisitor
    {
        /// <inheritdoc />
        public ITag Parse(TemplateParser parser, TokenCollection tc)
        {
            if (tc.Count > 3
                && (Utility.IsEqual(tc.First.Text, Const.KEY_ELSEIF) || Utility.IsEqual(tc.First.Text, Const.KEY_ELIF))
                && tc[1].TokenKind == TokenKind.LeftParentheses
                && tc.Last.TokenKind == TokenKind.RightParentheses)
            {
                var tag = new ElseifTag();

                var coll = new TokenCollection();
                tag.Condition = parser.Read(tc[2, -1]);
                if (!tag.Condition.IsSimple)
                    return null;
                return tag;
            }

            return null;
        }

        /// <inheritdoc />
        public MethodInfo Compile(ITag tag, CompileContext c)
        {

            return c.IfCompile((ElseifTag)tag);

        }
        /// <inheritdoc />
        public Type GuessType(ITag tag, CompileContext c)
        {


            return c.GuessIfType((ElseifTag)tag);

        }
        /// <inheritdoc />
        public object Excute(ITag tag, TemplateContext context)
        { 
            var t = tag as ElseifTag;
            var condition = context.Execute(t.Condition);
            if (Utility.ToBoolean(condition))
            {
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
            return null;

        }
    }
}