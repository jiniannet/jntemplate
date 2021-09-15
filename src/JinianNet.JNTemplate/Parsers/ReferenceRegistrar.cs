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
    public class ReferenceRegistrar : TagRegistrar<ReferenceTag>, IRegistrar
    {
        /// <inheritdoc />
        public override Func<TemplateParser, TokenCollection, ITag> BuildParseMethod()
        {
            return (parser, tc) =>
            {
                if (tc != null
                    && parser != null
                    && tc.Count > 2)
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
                            tag.AddChild(parser.Read(tcs[i]));
                        }
                    }
                    if (tag.Child == null)
                    {
                        return null;
                    }
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
                return c.CompileTag(((ReferenceTag)tag).Child);
            };
        }
        /// <inheritdoc />
        public override Func<ITag, CompileContext, Type> BuildGuessMethod()
        {
            return (tag, c) =>
            {
                return c.GuessType(((ReferenceTag)tag).Child);
            };
        }
        /// <inheritdoc />
        public override Func<ITag, TemplateContext, object> BuildExcuteMethod()
        {
            return (tag, context) =>
            {
                var t = tag as ReferenceTag;
                if (t.Child != null)
                {
                    return context.Execute(t.Child);
                }
                return null;
            };
        }

    }
}
