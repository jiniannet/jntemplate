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

    public class LoadRegistrar : TagRegistrar<LoadTag>, IRegistrar
    {
        /// <inheritdoc />
        public override Func<TemplateParser, TokenCollection, ITag> BuildParseMethod()
        {
            return (parser, tc) =>
            {
                if (Utility.IsEqual(tc.First.Text, Const.KEY_LOAD))
                {
                    if (tc != null
                        && parser != null
                        && tc.Count > 2
                        && (tc[1].TokenKind == TokenKind.LeftParentheses)
                        && tc.Last.TokenKind == TokenKind.RightParentheses)
                    {
                        var tag = new LoadTag();
                        tag.Path = parser.Read(new TokenCollection(tc, 2, tc.Count - 2));
                        return tag;
                    }
                }

                return null;
            };
        }
        /// <inheritdoc />
        public override Func<ITag, CompileContext, MethodInfo> BuildCompileMethod()
        {
            return (tag, c) =>
            {
                var t = tag as LoadTag;
                var type = c.GuessType(t);
                var mb = c.CreateReutrnMethod<LoadTag>(type);
                var il = mb.GetILGenerator();
                var strTag = t.Path as StringTag;
                if (strTag != null)
                {
                    var res = c.Load(strTag.Value);
                    if (res == null)
                    {
                        throw new CompileException(tag, $"[LoadTag] : \"{strTag.Value}\" cannot be found.");
                    }
                    var lexer = c.CreateTemplateLexer(res.Content);
                    var ts = lexer.Execute();

                    var parser = c.CreateTemplateParser(ts);
                    var tags = parser.Execute();

                    c.BlockCompile(il, tags);
                }
                else
                {
                    var labelEnd = il.DefineLabel();
                    var labelSuccess = il.DefineLabel();
                    var labelTry = il.DefineLabel();
                    var labelFinally = il.DefineLabel();

                    var m = c.CompileTag(t.Path);
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
                    il.Emit(OpCodes.Call, typeof(TemplateContextExtensions).GetMethodInfo("CompileFileAndExec", new Type[] { typeof(TemplateContext), typeof(string)}));
                    il.Emit(OpCodes.Stloc, 2);

                    il.Emit(OpCodes.Br, labelSuccess);

                    il.MarkLabel(labelEnd);
                    il.Emit(OpCodes.Ldstr, $"[LoadTag] : \"{t.Path.ToSource()}\" cannot be found.");
                    //il.Emit(OpCodes.Ldnull);
                    il.Emit(OpCodes.Stloc, 2);

                    il.MarkLabel(labelSuccess);
                    il.Emit(OpCodes.Ldloc, 2);
                }
                il.Emit(OpCodes.Ret);
                return mb.GetBaseDefinition();
            };
        }
        /// <inheritdoc />
        public override Func<ITag, CompileContext, Type> BuildGuessMethod()
        {
            return (tag, c) => typeof(string);
        }
        /// <inheritdoc />
        public override Func<ITag, TemplateContext, object> BuildExcuteMethod()
        {
            return (tag, context) =>
            {
                var t = tag as LoadTag;
                object path = context.Execute(t.Path);
                if (path == null)
                {
                    return null;
                }
                var res = context.Load(path.ToString());
                if (res != null)
                {
                    var render = new TemplateRender();
                    render.Context = context;
                    using (System.IO.StringWriter writer = new StringWriter())
                    {
                        render.Render(writer, render.ReadAll(res.Content));
                        return writer.ToString();
                    }
                }
                return null;
            };
        }
    }
}