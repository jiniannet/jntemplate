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
using JinianNet.JNTemplate.Exceptions;
using JinianNet.JNTemplate.Nodes;

namespace JinianNet.JNTemplate.Parsers
{
    /// <summary>
    /// The <see cref="FunctionTag"/> registrar
    /// </summary>
    public class FunctionVisitor : TagVisitor<FunctionTag>, ITagVisitor
    {
        /// <inheritdoc />
        public MethodInfo Compile(ITag tag, CompileContext c)
        {
            var getVariableScope = typeof(TemplateContext).GetPropertyGetMethod("TempData");
            var getVariableValue = typeof(IVariableScope).GetMethodInfo("get_Item", new[] { typeof(string) });
            var t = tag as FunctionTag;
            Type baseType;
            MethodInfo method;
            Type[] paramType = new Type[t.Children.Count];
            for (int i = 0; i < t.Children.Count; i++)
            {
                paramType[i] = c.GuessType(t.Children[i]);
                if (paramType[i].FullName == "System.Void")
                {
                    throw new CompileException(tag, "[FunctaionTag]:parameter error");
                }
            }

            MethodInfo childMethd = null;
            FieldInfo field = null;
            if (t.Parent != null)
            {
                baseType = c.GuessType(t.Parent);
                method = baseType.GetMethodInfo(t.Name, paramType, false);
                if (method == null)
                {
                    var property = baseType.GetPropertyInfo(t.Name);
                    Type funcType = null;
                    if (property == null)
                    {
                        field = baseType.GetFieldInfo(t.Name);
                        if (field != null)
                        {
                            funcType = field.FieldType;
                        }
                    }
                    else
                    {
                        funcType = property.PropertyType;
#if NF40 || NF20 || NF35
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
                        throw new CompileException(tag, $"[FunctaionTag]:method \"{t.Name}\" cannot be found!");
                    }
                }
            }
            else
            {
                baseType = c.TempData.GetType(t.Name);
                if (baseType.BaseType.Name != "MulticastDelegate")
                {
                    throw new ArgumentException("[FunctaionTag]:functaion must be delegate"); // Delegate’s actions must be addressed !
                }
                method = baseType.GetMethod("Invoke");
            }
            var isVoid = method.ReturnType.FullName == "System.Void";
            var mb = c.CreateReutrnMethod<FunctionTag>(method.ReturnType);
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
            var ps = method.GetParameters();
            for (int i = 0; i < paramType.Length; i++)
            {
                il.Emit(OpCodes.Ldloc, i + 1);
                if (i < ps.Length && paramType[i] != ps[i].ParameterType)
                {
                    if (ps[i].ParameterType.Name == "Nullable`1")
                    {
                        var genericType =
#if NF40 || NF35 || NF20
                            ps[i].ParameterType.GetGenericArguments()[0]
#else
                            ps[i].ParameterType.GenericTypeArguments[0]
#endif
                            ;

                        if (genericType != paramType[i])
                        {
                            il.ConvertTo(paramType[i], genericType);
                            paramType[i] = genericType;
                        }
                        il.Emit(OpCodes.Newobj, ps[i].ParameterType.GetConstructor(new Type[] { paramType[i] }));
                    }
                    else
                    {
                        il.ConvertTo(paramType[i], ps[i].ParameterType);
                    }
                }

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
        }
        /// <inheritdoc />
        public Type GuessType(ITag tag, CompileContext c)
        {
            var t = tag as FunctionTag;
            if (t.Parent == null)
            {
                var bodyType = c.TempData.GetType(t.Name);
                if (bodyType.BaseType.FullName != "System.MulticastDelegate")
                {
                    throw new CompileException(tag, $"[FunctaionTag]: \"{bodyType.BaseType}\" is not supported.");
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
            var method = parentType.GetMethodInfo(t.Name, types);
            if (method != null)
            {
                return method.ReturnType;
            }
            throw new CompileException(tag, $"[FunctaionTag]: \"{t.Name}\" is not defined");

        }
        /// <inheritdoc />
        public ITag Parse(TemplateParser parser, TokenCollection tc)
        {
            if (tc.First.TokenKind == TokenKind.TextData
                && tc.Count > 2
                && tc[1].TokenKind == TokenKind.LeftParentheses
                && tc.Last.TokenKind == TokenKind.RightParentheses
                )
            {
                var tcs = tc.Split(TokenKind.LeftParentheses, TokenKind.RightParentheses);
                if (tcs.Length != 2
                || tcs[1].Count < 2
                || tcs[1].First.TokenKind != TokenKind.LeftParentheses
                || tcs[1].Last.TokenKind != TokenKind.RightParentheses)
                {
                    return null;
                }

                var tag = new FunctionTag();
                tag.Name = tc.First.Text;
                var ntc = tcs[1].Split(1, tcs[1].Count - 1, TokenKind.Comma);
                for (int i = 0; i < ntc.Length; i++)
                {
                    if (ntc[i].Count == 1 && ntc[i][0].TokenKind == TokenKind.Comma)
                    {
                        continue;
                    }
                    tag.AddChild(parser.ReadSimple(ntc[i]));
                }

                return tag;

            }

            return null;

        }
        /// <inheritdoc />
        public object Excute(ITag tag, TemplateContext context)
        {
            var t = tag as FunctionTag;
            object[] args = new object[t.Children.Count];
            for (int i = 0; i < t.Children.Count; i++)
            {
                args[i] = context.Execute(t.Children[i]);
            }
            Type type = null;
            object parentValue;
            if (t.Parent == null)
            {
                parentValue = context.TempData[t.Name];
            }
            else
            {
                parentValue = context.Execute(t.Parent);
                if (parentValue != null)
                {
                    type = parentValue.GetType();
                }
                else
                {
                    if (t.Parent is VariableTag variable)
                    {
                        type = context.TempData.GetType(variable.Name);
                    }
                }
            }

            if (parentValue == null && type == null)
            {
                return null;
            }
            if (t.Parent == null || (t.Parent != null && string.IsNullOrEmpty(t.Name)))
            {
                //if (parentValue is FuncHandler funcHandler)
                //{
                //    return funcHandler(args);
                //}
                if (parentValue is Delegate func)
                {
                    var ps = func.Method.GetParameters();
                    args = args.ChangeArguments(ps);
                    return func.DynamicInvoke(args);
                }
                return null;
            }

            var result = type.CallMethod(parentValue, t.Name, args);

            if (result != null)
            {
                return result;
            }

            result = parentValue.CallPropertyOrField(t.Name, type);

            if (result != null && result is Delegate)
            {
                return (result as Delegate).DynamicInvoke(args);
            }

            return null;

        }
    }
}