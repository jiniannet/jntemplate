/********************************************************************************
 Copyright (c) jiniannet (http://www.jiniannet.com). All rights reserved.
 Licensed under the MIT license. See licence.txt file in the project root for full license information.
 ********************************************************************************/
using System;
using System.Collections.Generic;
using System.IO;
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
    /// The <see cref="ForeachTag"/> registrar
    /// </summary>
    public class ForeachRegistrar : TagRegistrar<ForeachTag>, IRegistrar
    {
        /// <inheritdoc />
        public override Func<TemplateParser, TokenCollection, ITag> BuildParseMethod()
        {
            return (parser, tc) =>
            {
                if (tc != null
                                && parser != null
                                && tc.Count > 5
                                && (Utility.IsEqual(Field.KEY_FOREACH, tc.First.Text) || Utility.IsEqual(Field.KEY_FOR, tc.First.Text))
                                && tc[1].TokenKind == TokenKind.LeftParentheses
                                && tc[2].TokenKind == TokenKind.TextData
                                && Utility.IsEqual(tc[3].Text, Field.KEY_IN)
                                && tc.Last.TokenKind == TokenKind.RightParentheses)
                {

                    var tag = new ForeachTag();
                    tag.Name = tc[2].Text;
                    tag.Source = parser.Read(tc[4, -1]);

                    while (parser.MoveNext())
                    {
                        tag.Children.Add(parser.Current);
                        if (parser.Current is EndTag)
                        {
                            return tag;
                        }
                    }

                    throw new ParseException($"foreach is not properly closed by a end tag: {tc.ToString()}", tc.First.BeginLine, tc.First.BeginColumn);

                }
                return null;
            };
        }

        /// <inheritdoc />
        public override Func<ITag, CompileContext, MethodInfo> BuildCompileMethod()
        {
            return (tag, c) =>
            {
                var t = tag as ForeachTag;
                var sourceType = c.GuessType(t.Source);
                if (sourceType.IsArray)
                {
                    return ArrayForeachCompile(c, t, sourceType);
                }
                return EnumerableForeachCompile(c, t, sourceType);
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

        /// <summary>
        /// 
        /// </summary>
        /// <param name="tag"></param>
        /// <param name="c"></param>
        /// <param name="sourceType"></param>
        /// <returns></returns>
        public static MethodInfo EnumerableForeachCompile(CompileContext c, ForeachTag tag, Type sourceType)
        {
            var getVariableScope = typeof(TemplateContext).GetPropertyGetMethod("TempData");
            var getVariableValue = typeof(VariableScope).GetMethod("get_Item", new[] { typeof(string) });

            var stringBuilderType = typeof(StringBuilder);
            var t = tag;
            var type = c.GuessType(t);
            var variableScopeType = typeof(VariableScope);
            var templateContextType = typeof(TemplateContext);
            var childType = TypeGuesser.InferChildType(sourceType);
            var enumerableType = sourceType.GetIEnumerableGenericType();// typeof(System.Collections.IEnumerable);
            Type enumeratorType;
            bool mustTo = false;
            if (enumerableType == null)
            {
                enumerableType = typeof(System.Collections.IEnumerable);
                enumeratorType = typeof(System.Collections.IEnumerator);
                mustTo = true;
            }
            else
            {
                enumeratorType = typeof(IEnumerator<>).MakeGenericType(childType);
            }
            if (childType.Length != 1)
            {
                throw new CompileException(tag,"[ForeachTag]:source error.");
            }
            var old = c.Data;
            var scope = new VariableScope(old);
            c.Data = scope;
            c.Set(t.Name, childType[0]);
            c.Set("foreachIndex", typeof(int));
            var mb = c.CreateReutrnMethod<ForeachTag>(type);
            var il = mb.GetILGenerator();
            Label labelEnd = il.DefineLabel();
            Label labelNext = il.DefineLabel();
            Label labelStart = il.DefineLabel();
            il.DeclareLocal(stringBuilderType);
            il.DeclareLocal(enumerableType);
            il.DeclareLocal(templateContextType);
            il.DeclareLocal(typeof(bool));
            il.DeclareLocal(enumeratorType);
            il.DeclareLocal(typeof(Int32));
            il.DeclareLocal(childType[0]);
            il.DeclareLocal(typeof(bool));
            il.DeclareLocal(typeof(string));
            var types = new Type[t.Children.Count];
            for (var i = 0; i < t.Children.Count; i++)
            {
                types[i] = c.GuessType(t.Children[i]);
                if (t.Children[i] is SetTag setTag)
                {
                    c.Set(setTag.Name, c.GuessType(setTag.Value));
                }
                if (types[i].FullName == "System.Void" || t.Children[i] is TextTag)
                {
                    continue;
                }
            }
            il.Emit(OpCodes.Nop);
            il.Emit(OpCodes.Newobj, stringBuilderType.GetConstructor(Type.EmptyTypes));
            il.Emit(OpCodes.Stloc_0);
            il.Emit(OpCodes.Ldarg_0);
            il.Emit(OpCodes.Ldarg_1);
            var method = c.CompileTag(t.Source);
            il.Emit(OpCodes.Call, method);
            il.Emit(OpCodes.Stloc_1);
            il.Emit(OpCodes.Ldloc_1);
            il.Emit(OpCodes.Ldnull);
            il.Emit(OpCodes.Cgt_Un);
            il.Emit(OpCodes.Stloc_3);
            il.Emit(OpCodes.Ldloc_3);
            il.Emit(OpCodes.Brfalse, labelEnd);
            il.Emit(OpCodes.Nop);
            il.Emit(OpCodes.Ldloc_1);
            il.Emit(OpCodes.Callvirt, enumerableType.GetMethodInfo("GetEnumerator", Type.EmptyTypes));
            il.Emit(OpCodes.Stloc_S, 4);
            il.Emit(OpCodes.Ldarg_1);
            il.Emit(OpCodes.Call, templateContextType.GetMethodInfo("CreateContext", new Type[] { templateContextType }));
            il.Emit(OpCodes.Stloc_2);
            il.Emit(OpCodes.Ldc_I4, 0);
            il.Emit(OpCodes.Stloc_S, 5);
            il.Emit(OpCodes.Br, labelNext);
            il.MarkLabel(labelStart);
            il.Emit(OpCodes.Nop);
            il.Emit(OpCodes.Ldloc_S, 5);
            il.Emit(OpCodes.Ldc_I4, 1);
            il.Emit(OpCodes.Add);
            il.Emit(OpCodes.Stloc_S, 5);
            il.Emit(OpCodes.Ldloc_S, 4);
            il.Emit(OpCodes.Callvirt, enumeratorType.GetPropertyGetMethod("Current"));
            if (mustTo)
            {
                if (childType[0].IsValueType)
                {
                    il.Emit(OpCodes.Unbox_Any, childType[0]);
                }
                else
                {
                    il.Emit(OpCodes.Isinst, childType[0]);
                }
            }
            il.Emit(OpCodes.Stloc_S, 6);
            il.Emit(OpCodes.Ldloc, 2);
            il.Emit(OpCodes.Callvirt, getVariableScope);
            il.Emit(OpCodes.Ldstr, t.Name);
            il.Emit(OpCodes.Ldloc_S, 6);
            il.Emit(OpCodes.Callvirt, variableScopeType.GetGenericMethod(childType, "Set", new Type[] { typeof(string), childType[0] }));

            il.Emit(OpCodes.Nop);
            il.Emit(OpCodes.Ldloc, 2);
            il.Emit(OpCodes.Callvirt, getVariableScope);
            il.Emit(OpCodes.Ldstr, "foreachIndex");
            il.Emit(OpCodes.Ldloc_S, 5);
            il.Emit(OpCodes.Callvirt, variableScopeType.GetGenericMethod(new Type[] { typeof(int) }, "Set", new Type[] { typeof(string), typeof(int) }));
            il.Emit(OpCodes.Nop);

            il.StringAppend(c, t.Children, 0, 2);

            il.Emit(OpCodes.Nop);
            il.MarkLabel(labelNext);
            il.Emit(OpCodes.Ldloc_S, 4);
            if (!mustTo)
            {
                il.Emit(OpCodes.Callvirt, typeof(System.Collections.IEnumerator).GetMethodInfo("MoveNext", Type.EmptyTypes));
            }
            else
            {
                il.Emit(OpCodes.Callvirt, enumeratorType.GetMethodInfo("MoveNext", Type.EmptyTypes));
            }
            il.Emit(OpCodes.Stloc_S, 7);
            il.Emit(OpCodes.Ldloc_S, 7);
            il.Emit(OpCodes.Brtrue, labelStart);
            il.MarkLabel(labelEnd);
            il.Emit(OpCodes.Ldloc_0);
            il.Call(stringBuilderType, typeof(object).GetMethodInfo("ToString", Type.EmptyTypes));
            il.Emit(OpCodes.Stloc_S, 8);
            il.Emit(OpCodes.Ldloc_S, 8);
            il.Emit(OpCodes.Ret);
            c.Data = old;
            return mb.GetBaseDefinition();
        }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="tag"></param>
        /// <param name="c"></param>
        /// <param name="sourceType"></param>
        /// <returns></returns>
        public static MethodInfo ArrayForeachCompile(CompileContext c, ForeachTag tag, Type sourceType)
        {
            var getVariableScope = typeof(TemplateContext).GetPropertyGetMethod("TempData");
            var getVariableValue = typeof(VariableScope).GetMethod("get_Item", new[] { typeof(string) });

            var stringBuilderType = typeof(StringBuilder);
            var t = tag;
            var type = c.GuessType(t);
            var childType = TypeGuesser.InferChildType(sourceType);
            var templateContextType = typeof(TemplateContext);
            var variableScopeType = typeof(VariableScope);
            if (childType.Length != 1)
            {
                throw new CompileException(tag,"[ForeachTag]:source error.");
            }

            var old = c.Data;
            var scope = new VariableScope(old);
            c.Data = scope;
            c.Set(t.Name, childType[0]);
            c.Set("foreachIndex", typeof(int));

            var mb = c.CreateReutrnMethod<ForeachTag>(type);
            var il = mb.GetILGenerator();
            var labelNext = il.DefineLabel();
            var labelStart = il.DefineLabel();
            var labelEnd = il.DefineLabel();
            il.DeclareLocal(stringBuilderType);
            il.DeclareLocal(sourceType);
            il.DeclareLocal(typeof(bool));
            il.DeclareLocal(templateContextType);
            il.DeclareLocal(typeof(int));
            il.DeclareLocal(sourceType);
            il.DeclareLocal(typeof(int));
            il.DeclareLocal(childType[0]);
            il.DeclareLocal(typeof(string));

            var types = new Type[t.Children.Count];
            for (var i = 0; i < t.Children.Count; i++)
            {
                types[i] = c.GuessType(t.Children[i]);
                if (t.Children[i] is SetTag setTag)
                {
                    c.Set(setTag.Name, c.GuessType(setTag.Value));
                }
                if (types[i].FullName == "System.Void" || t.Children[i] is TextTag)
                {
                    continue;
                }
                il.DeclareLocal(types[i]);
            }


            il.Emit(OpCodes.Newobj, stringBuilderType.GetConstructor(Type.EmptyTypes));
            il.Emit(OpCodes.Stloc, 0);

            il.Emit(OpCodes.Ldarg_0);
            il.Emit(OpCodes.Ldarg_1);
            var method = c.CompileTag(t.Source);
            il.Emit(OpCodes.Call, method);

            il.Emit(OpCodes.Stloc_1);
            il.Emit(OpCodes.Ldloc_1);
            il.Emit(OpCodes.Ldnull);
            il.Emit(OpCodes.Cgt_Un);
            il.Emit(OpCodes.Stloc_2);
            il.Emit(OpCodes.Ldloc_2);
            il.Emit(OpCodes.Brfalse, labelEnd);


            il.Emit(OpCodes.Ldarg_1);
            il.Emit(OpCodes.Call, templateContextType.GetMethodInfo("CreateContext", new Type[] { templateContextType }));
            il.Emit(OpCodes.Stloc_3);
            il.Emit(OpCodes.Ldc_I4_0);
            il.Emit(OpCodes.Stloc, 4);
            il.Emit(OpCodes.Ldloc_1);
            il.Emit(OpCodes.Stloc, 5);
            il.Emit(OpCodes.Ldc_I4_0);
            il.Emit(OpCodes.Stloc, 6);
            il.Emit(OpCodes.Br, labelStart);

            // loop start
            il.MarkLabel(labelNext);
            il.Emit(OpCodes.Ldloc, 5);
            il.Emit(OpCodes.Ldloc, 6);
            il.Ldelem(childType[0]);
            il.Emit(OpCodes.Stloc, 7);
            il.Emit(OpCodes.Ldloc, 4);
            il.Emit(OpCodes.Ldc_I4_1);
            il.Emit(OpCodes.Add);
            il.Emit(OpCodes.Stloc, 4);
            il.Emit(OpCodes.Ldloc, 3);
            il.Emit(OpCodes.Callvirt, getVariableScope);
            il.Emit(OpCodes.Ldstr, t.Name);
            il.Emit(OpCodes.Ldloc, 7);
            il.Emit(OpCodes.Callvirt, variableScopeType.GetGenericMethod(childType, "Set", new Type[] { typeof(string), childType[0] }));
            il.Emit(OpCodes.Ldloc, 3);
            il.Emit(OpCodes.Callvirt, getVariableScope);
            il.Emit(OpCodes.Ldstr, "foreachIndex");
            il.Emit(OpCodes.Ldloc, 4);
            il.Emit(OpCodes.Callvirt, variableScopeType.GetGenericMethod(new Type[] { typeof(int) }, "Set", new Type[] { typeof(string), typeof(int) }));

            il.StringAppend(c, t.Children, 0, 3);

            il.Emit(OpCodes.Ldloc, 6);
            il.Emit(OpCodes.Ldc_I4_1);
            il.Emit(OpCodes.Add);
            il.Emit(OpCodes.Stloc, 6);
            il.MarkLabel(labelStart);
            il.Emit(OpCodes.Ldloc, 6);
            il.Emit(OpCodes.Ldloc, 5);
            il.Emit(OpCodes.Ldlen);
            il.Emit(OpCodes.Conv_I4);
            il.Emit(OpCodes.Blt, labelNext);
            // end loop

            il.MarkLabel(labelEnd);
            il.Emit(OpCodes.Ldloc_0);
            il.Call(stringBuilderType, typeof(object).GetMethodInfo("ToString", Type.EmptyTypes));
            il.Emit(OpCodes.Stloc, 8);
            il.Emit(OpCodes.Ldloc, 8);
            il.Emit(OpCodes.Ret);

            return mb.GetBaseDefinition();

        }
        /// <inheritdoc />
        public override Func<ITag, TemplateContext, object> BuildExcuteMethod()
        {
            return (tag, context) =>
            {
                var t = tag as ForeachTag;
                if (t.Source != null)
                {
                    using (var writer = new StringWriter())
                    {
                        object value = TagExecutor.Execute(t.Source, context);
                        var enumerable = value.ToIEnumerable();
                        TemplateContext ctx;
                        if (enumerable != null)
                        {
                            var ienum = enumerable.GetEnumerator();
                            ctx = TemplateContext.CreateContext(context);
                            int i = 0;
                            while (ienum.MoveNext())
                            {
                                i++;
                                ctx.TempData.Set(t.Name, ienum.Current, ienum.Current == null ? typeof(object) : ienum.Current.GetType());
                                //为了兼容以前的用户 foreachIndex 保留
                                ctx.TempData.Set("foreachIndex", i);
                                for (int n = 0; n < t.Children.Count; n++)
                                {
                                    object result = TagExecutor.Execute(t.Children[n], ctx);
                                    if (i == 0 && t.Children.Count == 1)
                                    {
                                        return result;
                                    }
                                    if (result != null)
                                    {
                                        writer.Write(result.ToString());
                                    }
                                }
                            }
                        }
                        return writer.ToString();
                    }
                }
                return null;
            };
        }
    }
}