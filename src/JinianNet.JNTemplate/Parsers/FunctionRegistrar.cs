/********************************************************************************
 Copyright (c) jiniannet (http://www.jiniannet.com). All rights reserved.
 Licensed under the MIT license. See licence.txt file in the project root for full license information.
 ********************************************************************************/
using System;
using System.Reflection;
using System.Reflection.Emit;
using System.Text;
using JinianNet.JNTemplate.CodeCompilation;
using JinianNet.JNTemplate.Dynamic;
using JinianNet.JNTemplate.Exception;
using JinianNet.JNTemplate.Nodes;

namespace JinianNet.JNTemplate.Parsers
{
    /// <summary>
    /// The <see cref="FunctaionTag"/> registrar
    /// </summary>
    public class FunctionRegistrar : TagRegistrar<FunctaionTag>, IRegistrar
    {
        /// <inheritdoc />
        public override Func<ITag, CompileContext, MethodInfo> BuildCompileMethod()
        {
            return (tag, c) =>
            {
                var getVariableScope = DynamicHelpers.GetPropertyGetMethod(typeof(TemplateContext), "TempData");
                var getVariableValue = DynamicHelpers.GetMethod(typeof(VariableScope), "get_Item", new[] { typeof(string) });
                var t = tag as FunctaionTag;
                Type baseType;
                MethodInfo method;
                Type[] paramType = new Type[t.Children.Count];
                for (int i = 0; i < t.Children.Count; i++)
                {
                    paramType[i] = c.GuessType(t.Children[i]);
                    if (paramType[i].FullName == "System.Void")
                    {
                        throw new CompileException("[FunctaionTag]:parameter error");
                    }
                }

                MethodInfo childMethd = null;
                FieldInfo field = null;
                if (t.Parent != null)
                {
                    baseType = c.GuessType(t.Parent);
                    method = DynamicHelpers.GetMethod(baseType, t.Name, paramType, false);
                    if (method == null)
                    {
                        var property = DynamicHelpers.GetPropertyInfo(baseType, t.Name);
                        Type funcType = null;
                        if (property == null)
                        {
                            field = DynamicHelpers.GetFieldInfo(baseType, t.Name);
                            if (field != null)
                            {
                                funcType = field.FieldType;
                            }
                        }
                        else
                        {
                            funcType = property.PropertyType;
#if NET40 || NET20
                            childMethd = property.GetGetMethod();
#else
                            childMethd = property.GetMethod;
#endif
                        }
                        if (funcType != null)
                        {
                            if (funcType.BaseType.Name != "MulticastDelegate")
                            {
                                throw new ArgumentException($"[FunctaionTag]:\"{t.Name}\" must be delegate");
                            }
                            method = funcType.GetMethod("Invoke");
                        }
                        if (method == null)
                        {
                            throw new CompileException($"[FunctaionTag]:method \"{t.Name}\" cannot be found!");
                        }
                    }
                }
                else
                {
                    baseType = c.Data.GetType(t.Name);
                    if (baseType.BaseType.Name != "MulticastDelegate")
                    {
                        throw new ArgumentException("[FunctaionTag]:functaion must be delegate"); // Delegate’s actions must be addressed !
                    }
                    method = baseType.GetMethod("Invoke");
                }
                var isVoid = method.ReturnType.FullName == "System.Void";
                var mb = c.CreateReutrnMethod<FunctaionTag>(method.ReturnType);
                var il = mb.GetILGenerator();
                il.DeclareLocal(baseType);
                for (int i = 0; i < paramType.Length; i++)
                {
                    il.DeclareLocal(paramType[i]);
                }
                if (t.Parent != null)
                {
                    if (!method.IsStatic
                    && (childMethd == null || !childMethd.IsStatic)
                    && (field == null || !field.IsStatic))
                    {
                        il.Emit(OpCodes.Nop);
                        var parentMethod = c.CompileTag(t.Parent);
                        il.Emit(OpCodes.Ldarg_0);
                        il.Emit(OpCodes.Ldarg_1);
                        il.Emit(OpCodes.Call, parentMethod);
                        il.Emit(OpCodes.Stloc_0);
                    }

                }
                else
                {
                    var localVar = il.DeclareLocal(typeof(object));
                    il.Emit(OpCodes.Nop);
                    il.Emit(OpCodes.Ldarg_1);
                    il.Emit(OpCodes.Callvirt, getVariableScope);
                    il.Emit(OpCodes.Ldstr, t.Name);
                    il.Emit(OpCodes.Callvirt, getVariableValue);
                    il.Emit(OpCodes.Stloc, localVar.LocalIndex);
                    il.Emit(OpCodes.Ldloc, localVar.LocalIndex);
                    il.Emit(OpCodes.Isinst, baseType);
                    il.Emit(OpCodes.Stloc_0);
                }

                for (int i = 0; i < t.Children.Count; i++)
                {

                    il.CallTag(c, t.Children[i], (nil, hasReturn, needCall) =>
                    {
                        if (!hasReturn)
                        {
                            throw new ArgumentNullException($"[FunctaionTag]:cannot compile functaion \"{t.Name}\" when the parameter is void.");
                        }
                        if (needCall)
                        {
                            nil.Emit(OpCodes.Ldarg_0);
                            nil.Emit(OpCodes.Ldarg_1);
                        }
                    }, (nil, returnType) =>
                    {
                        if (returnType == null)
                        {
                            return;
                        }
                        il.Emit(OpCodes.Stloc, i + 1);
                    });
                }
                if (!method.IsStatic
                    && (childMethd == null || !childMethd.IsStatic)
                    && (field == null || !field.IsStatic))
                {
                    il.LoadVariable(baseType, 0);
                }
                if (childMethd != null)
                {
                    il.Emit(OpCodes.Call, childMethd);
                }
                if (field != null)
                {
                    if (field.IsStatic)
                    {
                        il.Emit(OpCodes.Ldsfld, field);
                    }
                    else
                    {
                        il.Emit(OpCodes.Ldfld, field);
                    }
                }
                for (int i = 0; i < paramType.Length; i++)
                {
                    il.Emit(OpCodes.Ldloc, i + 1);
                }
                il.Call(baseType, method);
                //il.Emit(OpCodes.Callvirt, method);
                if (!isVoid)
                {
                    var localVar = il.DeclareLocal(method.ReturnType);
                    il.Emit(OpCodes.Stloc, localVar.LocalIndex);
                    il.Emit(OpCodes.Ldloc, localVar.LocalIndex);
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
                var t = tag as FunctaionTag;
                if (t.Parent == null)
                {
                    var bodyType = c.Data.GetType(t.Name);
                    if (bodyType.BaseType.FullName != "System.MulticastDelegate")
                    {
                        throw new Exception.CompileException($"[FunctaionTag]: \"{bodyType.BaseType}\" is not supported.");
                    }
                    var invokeMethod = bodyType.GetMethod("Invoke");
                    return invokeMethod.ReturnType;

                }
                var parentType = c.GuessType(t.Parent);
                Type[] types = new Type[t.Children.Count];
                for (int i = 0; i < types.Length; i++)
                {
                    types[i] = c.GuessType(t.Children[i]);
                }
                var method = DynamicHelpers.GetMethod(parentType, t.Name, types);
                if (method != null)
                {
                    return method.ReturnType;
                }
                throw new Exception.CompileException($"[FunctaionTag]: \"{t.Name}\" is not defined");
            };
        }
        /// <inheritdoc />
        public override Func<TemplateParser, TokenCollection, ITag> BuildParseMethod()
        {
            return (parser, tc) =>
            {
                if (tc != null
                    && parser != null
                    && tc.First.TokenKind == TokenKind.TextData
                    && tc.Count > 2
                    && tc[1].TokenKind == TokenKind.LeftParentheses
                    && tc.Last.TokenKind == TokenKind.RightParentheses
                    && tc.Split(0, tc.Count, TokenKind.Operator).Length == 1)
                {
                    var tag = new FunctaionTag();
                    tag.Name = tc.First.Text;

                    TokenCollection[] tcs = tc.Split(2, tc.Count - 1, TokenKind.Comma);
                    for (int i = 0; i < tcs.Length; i++)
                    {
                        if (tcs[i].Count == 1 && tcs[i][0].TokenKind == TokenKind.Comma)
                        {
                            continue;
                        }
                        tag.AddChild(parser.Read(tcs[i]));
                    }

                    return tag;

                }

                return null;
            };
        }
    }
}