/********************************************************************************
 Copyright (c) jiniannet (http://www.jiniannet.com). All rights reserved.
 Licensed under the MIT license. See licence.txt file in the project root for full license information.
 ********************************************************************************/
using System;
using System.Collections.Generic;
using System.Reflection;
using System.Reflection.Emit;
using System.Text;
using JinianNet.JNTemplate.CodeCompilation;
using JinianNet.JNTemplate.Dynamic;
using JinianNet.JNTemplate.Exceptions;
using JinianNet.JNTemplate.Nodes;

namespace JinianNet.JNTemplate.Parsers
{
    /// <summary>
    /// The <see cref="ReferenceTag"/> registrar
    /// </summary>
    public class ReferenceVisitor : TagVisitor<ReferenceTag>, ITagVisitor
    {
        /// <inheritdoc />
        public ITag Parse(TemplateParser parser, TokenCollection tc)
        {
            if (tc.Count > 2)
            {
                var tcs = tc.Split(TokenKind.Dot, TokenKind.LeftBracket, TokenKind.RightBracket);
                if (tcs.Length == 1
                /*&& (
                    tc.First.TokenKind != TokenKind.LeftBrace
                    || tc.Last.TokenKind != TokenKind.RightBracket
                    || tc.Count <= 2
                    || (tcs = tc.Split(1, tc.Count - 1, TokenKind.Dot, TokenKind.LeftBracket, TokenKind.RightBracket)).Length == 1
                )*/)
                {
                    return null;
                }
                var tag = new ReferenceTag();
                for (int i = 0; i < tcs.Length; i++)
                {
                    if (tcs[i].Count == 1 && tcs[i][0].TokenKind == TokenKind.Dot)
                    {
                        if (tag.Children.Count == 0 || i == tcs[i].Count - 1 || (tcs[i + 1].Count == 1 && (tcs[i + 1][0].TokenKind == TokenKind.Dot || tcs[i + 1][0].TokenKind == TokenKind.Operator)))
                        {
                            throw new ParseException($"syntax error near '.': {tc.ToString()}", tcs[i][0].BeginLine, tcs[i][0].BeginColumn);
                        }
                    }
                    else if (tcs[i].Count > 0)
                    {
                        tag.AddChild(parser.ReadSimple(tcs[i]));
                    }
                }
                if (tag.Child == null)
                {
                    return null;
                }
                return tag;
            }

            return null;
        }

        /// <inheritdoc />
        public MethodInfo Compile(ITag tag, CompileContext context)
        {
            return context.CompileTag(((ReferenceTag)tag).Child);
        }
        /// <inheritdoc />
        public Type GuessType(ITag tag, CompileContext context)
        {
            return context.GuessType(((ReferenceTag)tag).Child);
        }
        /// <inheritdoc />
        public object Excute(ITag tag, TemplateContext context)
        {
            var t = tag as ReferenceTag;
            if (t.Child != null)
            {
                return context.Execute(t.Child);
            }
            return null;
        }
    }
}
