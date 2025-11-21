/********************************************************************************
 Copyright (c) jiniannet (http://www.jiniannet.com). All rights reserved.
 Licensed under the MIT license. See licence.txt file in the project root for full license information.
 ********************************************************************************/
using JinianNet.JNTemplate.CodeCompilation;
using JinianNet.JNTemplate.Dynamic;
using JinianNet.JNTemplate.Exceptions;
using JinianNet.JNTemplate.Nodes;
using System;
using System.Collections.Generic;
using System.Reflection;
using System.Reflection.Emit;

namespace JinianNet.JNTemplate.Parsers
{
    /// <summary>
    /// The <see cref="JsonTag"/> registrar
    /// </summary>
    public class JsonVisitor : TagVisitor<JsonTag>, ITagVisitor
    {
        /// <inheritdoc />
        public ITag Parse(TemplateParser parser, TokenCollection tc)
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
                    var key = parser.ReadSimple(keyValuePair[0]);
                    var value = parser.ReadSimple(keyValuePair[2]);
                    tag.Dict.Add(key, value);
                }
                return tag;
            }

            return null;
        }
        /// <inheritdoc />
        public MethodInfo Compile(ITag tag, CompileContext context)
        {
            var t = tag as JsonTag;
            var type = typeof(Dictionary<object, object>);
            var mb = context.CreateReutrnMethod<JsonTag>(type);
            var il = mb.GetILGenerator();
            il.DeclareLocal(type);
            il.DeclareLocal(type);
            il.Emit(OpCodes.Newobj, type.GetConstructor(Type.EmptyTypes)); 
            il.Emit(OpCodes.Stloc_0);
    
            var dict = t.Dict;
            foreach(var kv in dict)
            {
                if (kv.Value == null)
                    throw new CompileException($"The Value cannot be null:{tag.ToSource()}");
                var keyMethod = context.CompileTag(kv.Key); 
                var localVar1 = il.DeclareLocal(keyMethod.ReturnType);
                il.Emit(OpCodes.Ldarg_0);
                il.Emit(OpCodes.Ldarg_1);
                il.Emit(OpCodes.Call, keyMethod);
                il.Emit(OpCodes.Stloc, localVar1.LocalIndex);

                if (kv.Value != null)
                {
                    var valueMethod = context.CompileTag(kv.Value);
                    var localVar2 = il.DeclareLocal(valueMethod.ReturnType);
                    il.Emit(OpCodes.Ldarg_0);
                    il.Emit(OpCodes.Ldarg_1);
                    il.Emit(OpCodes.Call, valueMethod);
                    il.Emit(OpCodes.Stloc, localVar2.LocalIndex);

                    il.Emit(OpCodes.Ldloc_0);
                    il.Emit(OpCodes.Ldloc, localVar1.LocalIndex);
                    if (keyMethod.ReturnType.IsValueType)
                        il.Emit(OpCodes.Box,keyMethod.ReturnType);
                    il.Emit(OpCodes.Ldloc_S, localVar2.LocalIndex);
                    if (valueMethod.ReturnType.IsValueType)
                        il.Emit(OpCodes.Box, valueMethod.ReturnType);
                }
                else
                {
                    il.Emit(OpCodes.Ldloc_0);
                    il.LoadVariable(keyMethod.ReturnType, localVar1.LocalIndex);
                    il.Emit(OpCodes.Ldnull);
                }
                il.Emit(OpCodes.Callvirt, type.GetMethod("set_Item")); 
            }
            il.Emit(OpCodes.Ldloc_0);
            il.Emit(OpCodes.Stloc_1);
            il.Emit(OpCodes.Ldloc_1);
            il.Emit(OpCodes.Ret);
            return mb.GetBaseDefinition();
        }
        /// <inheritdoc />
        public Type GuessType(ITag tag, CompileContext context)
        {
            return typeof(Dictionary<object, object>);
        }
        /// <inheritdoc />
        public object Excute(ITag tag, TemplateContext context)
        {
            var t = tag as JsonTag;
            var result = new Dictionary<object, object>();
            foreach (var kv in t.Dict)
            {
                var key = kv.Key == null ? null : context.Execute(kv.Key);
                var value = kv.Value == null ? null : context.Execute(kv.Value);
                result.Add(key, value);
            }
            return result;
        }
    }
}