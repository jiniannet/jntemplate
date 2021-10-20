/********************************************************************************
 Copyright (c) jiniannet (http://www.jiniannet.com). All rights reserved.
 Licensed under the MIT license. See licence.txt file in the project root for full license information.
 ********************************************************************************/
using System;
using System.IO;
using System.Reflection;
using System.Reflection.Emit;
using JinianNet.JNTemplate.CodeCompilation;
using JinianNet.JNTemplate.Dynamic;
using JinianNet.JNTemplate.Nodes;
using JinianNet.JNTemplate.Exceptions;

namespace JinianNet.JNTemplate.Parsers
{
    /// <summary>
    /// The <see cref="LayoutTag"/> registrar
    /// </summary>
    public class LayoutRegistrar : TagRegistrar<LayoutTag>, IRegistrar
    {
        /// <inheritdoc />
        public override Func<TemplateParser, TokenCollection, ITag> BuildParseMethod()
        {
            return (parser, tc) =>
            {
                if (tc != null
                                  && parser != null
                                  && tc.Count > 2
                                  && Utility.IsEqual(tc.First.Text, Const.KEY_LAYOUT)
                                  && (tc[1].TokenKind == TokenKind.LeftParentheses)
                                  && tc.Last.TokenKind == TokenKind.RightParentheses)
                {
                    var tag = new LayoutTag();
                    tag.Path = parser.Read(new TokenCollection(tc, 2, tc.Count - 2));
                    while (parser.MoveNext())
                    {
                        tag.Children.Add(parser.Current);
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
                var t = tag as LayoutTag;
                var type = c.GuessType(t);
                var mb = c.CreateReutrnMethod<LayoutTag>(type);
                var il = mb.GetILGenerator();
                var strTag = t.Path as StringTag;
                if (strTag == null)
                {
                    throw new CompileException(tag, $"[LayoutTag] : path must be a string.");
                }
                var res = c.Load(strTag.Value);
                if (res == null)
                {
                    throw new CompileException(tag, $"[LayoutTag] : \"{strTag.Value}\" cannot be found.");
                }

                var tags = c.Lexer(res.Content);

                for (int i = 0; i < tags.Length; i++)
                {
                    if (tags[i] is BodyTag)
                    {
                        BodyTag body = (BodyTag)tags[i];
                        for (int j = 0; j < t.Children.Count; j++)
                        {
                            body.AddChild(t.Children[j]);
                        }
                    }
                }

                c.BlockCompile(il, tags);

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
                var t = tag as LayoutTag;
                object path = context.Execute(t.Path);
                if (path == null)
                {
                    return null;
                }
                var res = context.FindFullPath(path.ToString());
                if (string.IsNullOrEmpty(res))
                {
                    return null;
                }
                var reader = new Resources.ResourceReader(res, context);

                var tags = context.Lexer(res, reader);

                for (int i = 0; i < tags.Length; i++)
                {
                    if (tags[i] is BodyTag)
                    {
                        BodyTag body = (BodyTag)tags[i];
                        for (int j = 0; j < t.Children.Count; j++)
                        {
                            body.AddChild(t.Children[j]);
                        }
                        tags[i] = body;
                    }
                }
                using (System.IO.StringWriter writer = new StringWriter())
                {
                    context.Render(writer, tags);
                    return writer.ToString();
                }

            };
        }

    }
}