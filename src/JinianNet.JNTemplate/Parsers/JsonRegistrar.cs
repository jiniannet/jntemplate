/********************************************************************************
 Copyright (c) jiniannet (http://www.jiniannet.com). All rights reserved.
 Licensed under the MIT license. See licence.txt file in the project root for full license information.
 ********************************************************************************/
using JinianNet.JNTemplate.CodeCompilation;
using JinianNet.JNTemplate.Nodes;
using System;
using System.Reflection;

namespace JinianNet.JNTemplate.Parsers
{
    /// <summary>
    /// The <see cref="JsonTag"/> registrar
    /// </summary>
    public class JsonRegistrar : TagRegistrar<JsonTag>, IRegistrar
    {
        /// <inheritdoc />
        public override Func<TemplateParser, TokenCollection, ITag> BuildParseMethod()
        {
            return (parser, tc) =>
            {
                if (tc.Count > 2
                    && (tc[0].TokenKind == TokenKind.LeftBrace)
                    && tc.Last.TokenKind == TokenKind.RightBrace)
                {
                    var tag = new JsonTag();
                    var tcs = tc.Split(1, tc.Count - 1, TokenKind.Comma);
                    for (int i = 0; i < tcs.Length; i++)
                    {
                        if (tcs[i].Count == 1 && tcs[i][0].TokenKind == TokenKind.Comma)
                        {
                            continue;
                        }
                        var keyValuePair = tcs[i].Split(0, tcs[i].Count, TokenKind.Colon);
                        if (keyValuePair.Length != 3)
                        {
                            //不符合规范
                            return null;
                        }
                        var key = parser.Read(keyValuePair[0]);
                        var value = parser.Read(keyValuePair[2]);
                        tag.Dict.Add(key, value);
                    }
                    return tag;
                }

                return null;
            };
        }
        /// <inheritdoc />
        public override Func<ITag, CompileContext, MethodInfo> BuildCompileMethod()
        {
            return null;
        }
        /// <inheritdoc />
        public override Func<ITag, CompileContext, Type> BuildGuessMethod()
        {
            return null;
        }
    }
}