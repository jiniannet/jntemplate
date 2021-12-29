﻿/********************************************************************************
 Copyright (c) jiniannet (http://www.jiniannet.com). All rights reserved.
 Licensed under the MIT license. See licence.txt file in the project root for full license information.
 ********************************************************************************/
using JinianNet.JNTemplate.CodeCompilation;
using JinianNet.JNTemplate.Nodes;
using System;
using System.Collections.Generic;
using System.Reflection;
using System.Reflection.Emit;

namespace JinianNet.JNTemplate.Parsers
{
    /// <summary>
    /// The <see cref="ArrayTag"/> registrar
    /// </summary>
    public class ArrayRegistrar : TagRegistrar<ArrayTag>, IRegistrar
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
                    var list = new List<object>();
                    var tcs = tc.Split(1, tc.Count - 1, TokenKind.Comma);
                    for (int i = 0; i < tcs.Length; i++)
                    {
                        if (tcs[i].Count == 1 && tcs[i][0].TokenKind == TokenKind.Comma)
                        {
                            continue;
                        }
                        var itemTag = parser.Read(tcs[i]);
                        if (itemTag is not null && itemTag is ITypeTag t)
                        {
                            list.Add(t.Value);
                            continue;
                        }
                        return null;
                    }
                    var tag = new ArrayTag();
                    tag.Value = list.ToArray();
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
                var t = tag as ArrayTag;
                var type = t.Value.GetType();
                var mb = c.CreateReutrnMethod<ArrayTag>(type);
                var il = mb.GetILGenerator();
                il.DeclareLocal(type);
                il.DeclareLocal(type);
                il.Emit(OpCodes.Ldc_I4, t.Value.Length);
                il.Emit(OpCodes.Newarr, typeof(object));
                il.Emit(OpCodes.Stloc_0);

                for (var i = 0; i < t.Value.Length; i++)
                {
                    if (t.Value[i] == null)
                    {
                        continue;
                    }
                    il.Emit(OpCodes.Ldloc_0);
                    il.Emit(OpCodes.Ldc_I4, i);
                    var itemType = t.Value[i].GetType();
                    switch (itemType.Name)
                    {
                        case "Int32":
                            il.Emit(OpCodes.Ldc_I4, (int)t.Value[i]);
                            il.Emit(OpCodes.Box, itemType);
                            break;
                        case "Int64":
                            il.Emit(OpCodes.Ldc_I8, (long)t.Value[i]);
                            il.Emit(OpCodes.Box, itemType);
                            break;
                        case "Single":
                            il.Emit(OpCodes.Ldc_R4, (float)t.Value[i]);
                            il.Emit(OpCodes.Box, itemType);
                            break;
                        case "Double":
                            il.Emit(OpCodes.Ldc_R8, (double)t.Value[i]);
                            il.Emit(OpCodes.Box, itemType);
                            break;
                        case "Int16":
                            il.Emit(OpCodes.Ldc_I4, (short)t.Value[i]);
                            il.Emit(OpCodes.Box, itemType);
                            break;
                        case "Boolean":
                            il.Emit(OpCodes.Ldc_I4, (bool)t.Value[i] ? 1 : 0);
                            il.Emit(OpCodes.Box, itemType);
                            break;
                        case "String":
                            il.Emit(OpCodes.Ldstr, t.Value[i].ToString());
                            break;
                        default:
                            throw new NotSupportedException($"[ArrayTag] : [{t.Value[i]}] is not supported");
                    }
                    il.Emit(OpCodes.Stelem_Ref);
                }

                il.Emit(OpCodes.Ldloc_0);
                il.Emit(OpCodes.Stloc_1);
                il.Emit(OpCodes.Ldloc_1);
                il.Emit(OpCodes.Ret);
                return mb.GetBaseDefinition();
            };
        }
        /// <inheritdoc />
        public override Func<ITag, CompileContext, Type> BuildGuessMethod()
        {
            return (tag, c) =>
            {
                return typeof(object[]);
            };
        }
        /// <inheritdoc />
        public override Func<ITag, TemplateContext, object> BuildExcuteMethod()
        {
            return (tag, context) =>
            {
                var t = tag as ArrayTag;
                return t.Value;
            };
        }
    }
}