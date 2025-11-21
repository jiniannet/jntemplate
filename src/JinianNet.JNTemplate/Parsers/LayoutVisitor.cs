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
using System.Collections.Generic;

namespace JinianNet.JNTemplate.Parsers
{
    /// <summary>
    /// The <see cref="LayoutTag"/> registrar
    /// </summary>
    public class LayoutVisitor : TagVisitor<LayoutTag>, ITagVisitor
    {
        /// <inheritdoc />
        public ITag Parse(TemplateParser parser, TokenCollection tc)
        {
            if (tc.Count > 2
                && Utility.IsEqual(tc.First.Text, Const.KEY_LAYOUT)
                && (tc[1].TokenKind == TokenKind.LeftParentheses)
                && tc.Last.TokenKind == TokenKind.RightParentheses)
            {
                var tag = new LayoutTag();
                tag.Path = parser.ReadSimple(new TokenCollection(tc, 2, tc.Count - 2));
                if (tag.Path == null)
                    return null;
                while (parser.MoveNext())
                {
                    tag.Children.Add(parser.Current);
                }
                return tag;
            }
            return null;
        }
        /// <inheritdoc />
        public MethodInfo Compile(ITag tag, CompileContext context)
        {
            var t = tag as LayoutTag;
            var type = context.GuessType(t);
            var mb = context.CreateReutrnMethod<LayoutTag>(type);
            var il = mb.GetILGenerator();
            var strTag = t.Path as StringTag;
            if (strTag == null)
            {
                throw new CompileException(tag, $"[LayoutTag] : path must be a string.");
            }
            var res = context.Load(strTag.Value);
            if (res == null)
            {
                throw new CompileException(tag, $"[LayoutTag] : \"{strTag.Value}\" cannot be found.");
            }

            var tags = context.Lexer(res.Content);

            for (int i = 0; i < tags.Count; i++)
            {
                if (tags[i] is BodyTag body)
                {
                    for (int j = 0; j < t.Children.Count; j++)
                    {
                        body.AddChild(t.Children[j]);
                    }
                }
            }

            context.BlockCompile(il, tags);

            il.Emit(OpCodes.Ret);
            return mb.GetBaseDefinition();
        }
        /// <inheritdoc />
        public Type GuessType(ITag tag, CompileContext context)
        {
            return typeof(string);
        }
        /// <inheritdoc />
        public object Excute(ITag tag, TemplateContext context)
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
            var reader = new Resources.ResourceReader(res);

            var result = (InterpretResult)context.InterpretTemplate(res, reader);
            var tags = new TagCollection();
            for (int i = 0; i < result.Tags.Count; i++)
            {
                if (result.Tags[i] is BodyTag _)
                {
                    for (int j = 0; j < t.Children.Count; j++)
                    {
                        tags.Add(t.Children[j]);
                    }
                    continue;
                }
                tags.Add(result.Tags[i]);
            }
            using (System.IO.StringWriter writer = new StringWriter())
            {
                context.Render(writer, tags);
                return writer.ToString();
            }
        }
    }
}