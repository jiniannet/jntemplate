/********************************************************************************
 Copyright (c) jiniannet (http://www.jiniannet.com). All rights reserved.
 Licensed under the MIT license. See licence.txt file in the project root for full license information.
 ********************************************************************************/
using JinianNet.JNTemplate.CodeCompilation;
using JinianNet.JNTemplate.Dynamic;
using JinianNet.JNTemplate.Nodes;
using System;
using System.Reflection;
using System.Reflection.Emit;

namespace JinianNet.JNTemplate.Parsers
{
    /// <summary>
    /// The <see cref="IncludeTag"/> registrar
    /// </summary>
    public class IncludeRegistrar : TagRegistrar<IncludeTag>, IRegistrar
    {
        /// <inheritdoc />
        public override Func<TemplateParser, TokenCollection, ITag> BuildParseMethod()
        {
            return (parser, tc) =>
            {
                if (Utility.IsEqual(tc.First.Text, Field.KEY_INCLUDE))
                {
                    if (tc != null
                        && parser != null
                        && tc.Count > 2
                        && (tc[1].TokenKind == TokenKind.LeftParentheses)
                        && tc.Last.TokenKind == TokenKind.RightParentheses)
                    {
                        IncludeTag tag = new IncludeTag();
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
                var t = tag as IncludeTag;
                var type = c.GuessType(t);
                var mb = c.CreateReutrnMethod<IncludeTag>(type);
                var il = mb.GetILGenerator();
                var strTag = t.Path as StringTag;
                if (strTag != null)
                {
                    var res = c.Load(strTag.Value);
                    if (res != null)
                    {
                        il.Emit(OpCodes.Ldstr, res.Content);
                    }
                    else
                    {
                        il.Emit(OpCodes.Ldstr, $"\"{strTag.Value}\" not found!");
                    }
                }
                else
                {
                    var strType = typeof(string);
                    var resType = typeof(Resources.ResourceInfo);
                    var ctxType = typeof(TemplateContext);
                    var labelEnd = il.DefineLabel();
                    var labelSuccess = il.DefineLabel();
                    il.DeclareLocal(strType);
                    il.DeclareLocal(typeof(bool));
                    il.DeclareLocal(resType);
                    il.DeclareLocal(typeof(bool));
                    il.DeclareLocal(type);

                    var m = c.CompileTag(t.Path);

                    il.Emit(OpCodes.Ldarg_0);
                    il.Emit(OpCodes.Ldarg_1);
                    il.Emit(OpCodes.Call, m);
                    if (m.ReturnType.FullName != "System.String")
                    {
                        var localVar = il.DeclareLocal(m.ReturnType);
                        il.Emit(OpCodes.Stloc, localVar.LocalIndex);
                        il.LoadVariable(m.ReturnType, localVar.LocalIndex);
                        il.Call(m.ReturnType, DynamicHelpers.GetMethod(typeof(object), "ToString", Type.EmptyTypes));
                    }
                    il.Emit(OpCodes.Stloc_0);
                    il.Emit(OpCodes.Ldloc_0);
                    il.Emit(OpCodes.Ldnull);
                    il.Emit(OpCodes.Cgt_Un);
                    il.Emit(OpCodes.Stloc_1);
                    il.Emit(OpCodes.Ldloc_1);
                    il.Emit(OpCodes.Brfalse, labelEnd);

                    il.Emit(OpCodes.Ldarg_1);
                    il.Emit(OpCodes.Ldloc_0);
                    il.Emit(OpCodes.Call, DynamicHelpers.GetMethod(typeof(TemplateContextExtensions), "Load", new Type[] { typeof(Context), strType }));
                    il.Emit(OpCodes.Stloc_2);
                    il.Emit(OpCodes.Ldloc_2);
                    il.Emit(OpCodes.Ldnull);
                    il.Emit(OpCodes.Cgt_Un);
                    il.Emit(OpCodes.Stloc, 3);
                    il.Emit(OpCodes.Ldloc, 3);
                    il.Emit(OpCodes.Brfalse, labelEnd);

                    il.Emit(OpCodes.Ldloc_2);
                    il.Emit(OpCodes.Callvirt, DynamicHelpers.GetPropertyGetMethod(resType, "Content"));
                    il.Emit(OpCodes.Stloc, 4);
                    il.Emit(OpCodes.Br, labelSuccess);

                    il.MarkLabel(labelEnd);
                    il.Emit(OpCodes.Ldstr, $"[IncludeTag] : \"{t.Path.ToSource()}\" cannot be found.");
                    il.Emit(OpCodes.Stloc, 4);


                    il.MarkLabel(labelSuccess);
                    il.Emit(OpCodes.Ldloc, 4);
                }
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
                var t = tag as IncludeTag;
                object path = TagExecutor.Execute(t.Path, context);
                if (path == null)
                {
                    return null;
                }
                var res = context.Load(path.ToString());
                if (res != null)
                {
                    return res.Content;
                }
                return null;
            };
        }

    }
}