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
    public class IncludeVisitor : TagVisitor<IncludeTag>, ITagVisitor
    {
        /// <inheritdoc />
        public ITag Parse(TemplateParser parser, TokenCollection tc)
        {
            if (Utility.IsEqual(tc.First.Text, Const.KEY_INCLUDE)
                && tc.Count > 2
                    && (tc[1].TokenKind == TokenKind.LeftParentheses)
                    && tc.Last.TokenKind == TokenKind.RightParentheses)
            {
                IncludeTag tag = new IncludeTag();
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
            var t = tag as IncludeTag;
            var type = context.GuessType(t);
            var mb = context.CreateReutrnMethod<IncludeTag>(type);
            var il = mb.GetILGenerator();
            var strTag = t.Path as StringTag;
            if (strTag != null)
            {
                var res = context.Load(strTag.Value);
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
                var labelEnd = il.DefineLabel();
                var labelSuccess = il.DefineLabel();
                il.DeclareLocal(strType);
                il.DeclareLocal(typeof(bool));
                il.DeclareLocal(resType);
                il.DeclareLocal(typeof(bool));
                il.DeclareLocal(type);

                var m = context.CompileTag(t.Path);

                il.Emit(OpCodes.Ldarg_0);
                il.Emit(OpCodes.Ldarg_1);
                il.Emit(OpCodes.Call, m);
                if (m.ReturnType.FullName != "System.String")
                {
                    var localVar = il.DeclareLocal(m.ReturnType);
                    il.Emit(OpCodes.Stloc, localVar.LocalIndex);
                    il.LoadVariable(m.ReturnType, localVar.LocalIndex);
                    il.Call(m.ReturnType, typeof(object).GetMethodInfo("ToString", Type.EmptyTypes));
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
                il.Emit(OpCodes.Call, typeof(TemplateContextExtensions).GetMethodInfo("Load", new Type[] { typeof(Context), strType }));
                il.Emit(OpCodes.Stloc_2);
                il.Emit(OpCodes.Ldloc_2);
                il.Emit(OpCodes.Ldnull);
                il.Emit(OpCodes.Cgt_Un);
                il.Emit(OpCodes.Stloc, 3);
                il.Emit(OpCodes.Ldloc, 3);
                il.Emit(OpCodes.Brfalse, labelEnd);

                il.Emit(OpCodes.Ldloc_2);
                il.Emit(OpCodes.Callvirt, resType.GetPropertyGetMethod("Content"));
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
        }
        /// <inheritdoc />
        public Type GuessType(ITag tag, CompileContext context)
        {
            return typeof(string);
        }
        /// <inheritdoc />
        public object Excute(ITag tag, TemplateContext context)
        {
            var t = tag as IncludeTag;
            object path = context.Execute(t.Path);
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
        }

    }
}