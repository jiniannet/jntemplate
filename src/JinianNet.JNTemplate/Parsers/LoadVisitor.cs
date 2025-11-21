/********************************************************************************
 Copyright (c) jiniannet (http://www.jiniannet.com). All rights reserved.
 Licensed under the MIT license. See licence.txt file in the project root for full license information.
 ********************************************************************************/
using JinianNet.JNTemplate.CodeCompilation;
using JinianNet.JNTemplate.Dynamic;
using JinianNet.JNTemplate.Nodes;
using JinianNet.JNTemplate.Exceptions;
using System;
using System.IO;
using System.Reflection;
using System.Reflection.Emit;

namespace JinianNet.JNTemplate.Parsers
{
    /// <summary>
    /// The <see cref="LoadTag"/> registrar
    /// </summary>

    public class LoadVisitor : TagVisitor<LoadTag>, ITagVisitor
    {
        /// <inheritdoc />
        public ITag Parse(TemplateParser parser, TokenCollection tc)
        {
            if (Utility.IsEqual(tc.First.Text, Const.KEY_LOAD)
                && tc.Count > 2
                && (tc[1].TokenKind == TokenKind.LeftParentheses)
                && tc.Last.TokenKind == TokenKind.RightParentheses)
            {
                var tag = new LoadTag();
                tag.Path = parser.ReadSimple(new TokenCollection(tc, 2, tc.Count - 2));
                if (tag.Path == null)
                    return null;
                return tag;
            }

            return null;
        }
        /// <inheritdoc />
        public MethodInfo Compile(ITag tag, CompileContext context)
        {
            var t = tag as LoadTag;
            var type = context.GuessType(t);
            var mb = context.CreateReutrnMethod<LoadTag>(type);
            var il = mb.GetILGenerator();
            var strTag = t.Path as StringTag;
            if (strTag != null)
            {
                var res = context.Load(strTag.Value);
                if (res == null)
                {
                    throw new CompileException(tag, $"[LoadTag] : \"{strTag.Value}\" cannot be found.");
                }

                var tags = context.Lexer(res.Content);

                context.BlockCompile(il, tags);
            }
            else
            {
                var labelEnd = il.DefineLabel();
                var labelSuccess = il.DefineLabel();
                il.DefineLabel();
                il.DefineLabel();

                var m = context.CompileTag(t.Path);
                il.DeclareLocal(typeof(string));
                il.DeclareLocal(typeof(bool));
                il.DeclareLocal(typeof(string));

                il.Emit(OpCodes.Ldarg_0);
                il.Emit(OpCodes.Ldarg_1);
                il.Emit(OpCodes.Call, m);
                if (m.ReturnType.FullName != "System.String")
                {
                    il.DeclareLocal(m.ReturnType);
                    il.Emit(OpCodes.Stloc, 3);
                    if (m.ReturnType.IsValueType)
                    {
                        il.Emit(OpCodes.Ldloca, 3);
                    }
                    else
                    {
                        il.Emit(OpCodes.Ldloc, 3);
                        il.DeclareLocal(typeof(bool));
                        il.Emit(OpCodes.Ldnull);
                        il.Emit(OpCodes.Cgt_Un);
                        il.Emit(OpCodes.Stloc, 4);
                        il.Emit(OpCodes.Ldloc, 4);
                        il.Emit(OpCodes.Brfalse, labelEnd);

                        il.Emit(OpCodes.Ldloc, 3);
                    }
                    il.Call(m.ReturnType, typeof(object).GetMethodInfo("ToString", Type.EmptyTypes));
                    il.Emit(OpCodes.Stloc, 0);
                    il.Emit(OpCodes.Ldloc, 0);
                }
                else
                {
                    il.Emit(OpCodes.Stloc, 0);
                    il.Emit(OpCodes.Ldloc, 0);
                }
                il.Emit(OpCodes.Ldnull);
                il.Emit(OpCodes.Cgt_Un);
                il.Emit(OpCodes.Stloc, 1);
                il.Emit(OpCodes.Ldloc, 1);
                il.Emit(OpCodes.Brfalse, labelEnd);

                il.Emit(OpCodes.Ldarg_1);
                il.Emit(OpCodes.Ldloc_0);
                il.Emit(OpCodes.Call, typeof(TemplateContextExtensions).GetMethodInfo("CompileAndRenderFile", new Type[] { typeof(TemplateContext), typeof(string) }));
                il.Emit(OpCodes.Stloc, 2);

                il.Emit(OpCodes.Br, labelSuccess);

                il.MarkLabel(labelEnd);
                il.Emit(OpCodes.Ldstr, $"[LoadTag] : \"{t.Path.ToSource()}\" cannot be found.");
                il.Emit(OpCodes.Stloc, 2);

                il.MarkLabel(labelSuccess);
                il.Emit(OpCodes.Ldloc, 2);
            }
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
            var t = tag as LoadTag;
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

            var result = context.InterpretTemplate(res, reader);

            using (System.IO.StringWriter writer = new StringWriter())
            {
                result.Render(writer, context);
                return writer.ToString();
            }
        }
    }
}