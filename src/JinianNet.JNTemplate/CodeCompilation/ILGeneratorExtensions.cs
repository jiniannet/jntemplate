/********************************************************************************
 Copyright (c) jiniannet (http://www.jiniannet.com). All rights reserved.
 Licensed under the MIT license. See licence.txt file in the project root for full license information.
 ********************************************************************************/
using JinianNet.JNTemplate.Dynamic;
using JinianNet.JNTemplate.Nodes;
using System;
using System.Collections.Generic;
using System.Reflection;
using System.Reflection.Emit;
using System.Text;

namespace JinianNet.JNTemplate.CodeCompilation
{
    /// <summary>
    /// 
    /// </summary>
    public static class ILGeneratorExtensions
    {
        /// <summary>
        /// Converts an object to a specified type.
        /// </summary>
        /// <param name="il">The <see cref="ILGenerator"/></param>
        /// <param name="type">The target type.</param>
        public static void ObjectTo(this ILGenerator il, Type type)
        {
            var m = type.GetMethod("Parse", new[] { typeof(string) });
            if (m != null)
            {
                var toString = typeof(object).GetMethodInfo("ToString", Type.EmptyTypes);
                il.Emit(OpCodes.Callvirt, toString);
                il.Emit(OpCodes.Call, m);
                return;
            }

            if (type.IsValueType)
            {
                il.Emit(OpCodes.Unbox_Any, type);
            }
            else
            {
                il.Emit(OpCodes.Castclass, type);
            }
        }


        /// <summary>
        /// Calls the method.
        /// </summary>
        /// <param name="il">The <see cref="ILGenerator"/></param>
        /// <param name="type">Method belongs to the object type.</param>
        /// <param name="method">The method being called.</param>
        public static void Call(this ILGenerator il, Type type, MethodInfo method)
        {
            OpCode op;
            if (method.IsStatic || (method.DeclaringType.IsValueType))
            {
                op = OpCodes.Call;
            }
            else
            {
                if (method.IsVirtual && type.IsValueType)
                {
                    il.Emit(OpCodes.Constrained, type);
                }
                op = OpCodes.Callvirt;
            }
            il.Emit(op, method);
        }

        /// <summary>
        /// Loading Array Elements
        /// </summary>
        /// <param name="il">The <see cref="ILGenerator"/></param>
        /// <param name="type">The type of the element.</param> 
        public static void Ldelem(this ILGenerator il, Type type)
        {
            switch (type.FullName)
            {
                case "System.Double":
                    il.Emit(OpCodes.Ldelem_R8);
                    break;
                case "System.Single":
                    il.Emit(OpCodes.Ldelem_R4);
                    break;
                case "System.Int64":
                    il.Emit(OpCodes.Ldelem_I8);
                    break;
                case "System.Int32":
                    il.Emit(OpCodes.Ldelem_I4);
                    break;
                case "System.UInt32":
                    il.Emit(OpCodes.Ldelem_U4);
                    break;
                case "System.Int16":
                    il.Emit(OpCodes.Ldelem_I2);
                    break;
                case "System.UInt16":
                case "System.Char":
                    il.Emit(OpCodes.Ldelem_U2);
                    break;
                case "System.Byte":
                    il.Emit(OpCodes.Ldelem_U1);
                    break;
                default:
                    if (type.IsValueType)
                    {
                        il.Emit(OpCodes.Ldelem, type);
                    }
                    else
                    {
                        il.Emit(OpCodes.Ldelem_Ref);
                    }
                    break;
            }
        }

        /// <summary>
        /// calling the type tag.
        /// </summary>
        /// <param name="il">The <see cref="ILGenerator"/></param>
        /// <param name="tag">The <see cref="ITypeTag"/></param>
        /// <returns></returns>
        public static Type CallTypeTag(this ILGenerator il, ITypeTag tag)
        {
            switch (tag.GetType().Name)
            {
                case "BooleanTag":
                    il.Emit(OpCodes.Ldc_I4, (tag as BooleanTag).Value ? 1 : 0);
                    return typeof(bool);
                case "NumberTag":
                    switch (tag.Value.GetType().Name)
                    {
                        case "Int32":
                            il.Emit(OpCodes.Ldc_I4, (int)tag.Value);
                            return typeof(int);
                        case "Int64":
                            il.Emit(OpCodes.Ldc_I8, (long)tag.Value);
                            return typeof(long);
                        case "Single":
                            il.Emit(OpCodes.Ldc_R4, (float)tag.Value);
                            return typeof(float);
                        case "Double":
                            il.Emit(OpCodes.Ldc_R8, (double)tag.Value);
                            return typeof(double);
                        case "Int16":
                            il.Emit(OpCodes.Ldc_I4, (short)tag.Value);
                            return typeof(short);
                        default:
                            throw new NotSupportedException($"[NumberTag] : [{tag.Value}] is not supported");
                    }
                case "OperatorTag":
                case "StringTag":
                default:
                    il.Emit(OpCodes.Ldstr, tag.Value?.ToString() ?? string.Empty);
                    return typeof(string);
            }
        }

        /// <summary>
        /// calling the tag.
        /// </summary>
        /// <param name="il">The <see cref="ILGenerator"/></param>
        /// <param name="ctx">The <see cref="CompileContext"/></param>
        /// <param name="tag">The <see cref="ITag"/></param>
        /// <param name="before">The action.</param>
        /// <param name="completed">The action of the completed.</param>
        public static void CallTag(this ILGenerator il,
            CompileContext ctx,
            ITag tag,
            Action<ILGenerator, bool, bool> before,//hasReturn,call
            Action<ILGenerator, Type> completed)
        {
            if (tag is EndTag _
                || tag is CommentTag _)
            {
                return;
            }
            if (tag is TextTag textTag)
            {
                if (string.IsNullOrEmpty(textTag.Text))
                {
                    return;
                }
                before?.Invoke(il, true, false);
                var text = textTag.ToString(ctx.OutMode);
                il.Emit(OpCodes.Ldstr, text);
                completed?.Invoke(il, typeof(string));
                return;
            }
            if (tag is ITypeTag typeTag)
            {
                if (typeTag.Value == null)
                {
                    return;
                }
                before?.Invoke(il, true, false);
                Type returnType = il.CallTypeTag(typeTag);
                completed?.Invoke(il, returnType);
                return;
            }
            if (tag is SetTag setTag)
            {
                ctx.Set(setTag.Name, ctx.GuessType(setTag.Value));
            }
            var m = ctx.CompileTag(tag);
            if (m.ReturnType.FullName != "System.Void")
            {
                before?.Invoke(il, true, true);
                il.Emit(OpCodes.Call, m);
                completed?.Invoke(il, m.ReturnType);
            }
            else
            {
                before?.Invoke(il, false, true);
                il.Emit(OpCodes.Call, m);
                completed?.Invoke(il, null);
            }
        }


        /// <summary>
        /// Load the variables
        /// </summary>
        /// <param name="il">The <see cref="ILGenerator"/></param>
        /// <param name="type">The variable type.</param>
        /// <param name="index"></param>
        public static void LoadVariable(this ILGenerator il, Type type, int index)
        {
            if (type.IsValueType)
            {
                il.Emit(OpCodes.Ldloca, index);
            }
            else
            {
                il.Emit(OpCodes.Ldloc, index);
            }
        }
        /// <summary>
        /// Equals
        /// </summary>
        /// <param name="il">The <see cref="ILGenerator"/></param>
        /// <param name="type">The type.</param>
        public static void EmitEquals(this ILGenerator il, Type type)
        {
            var equals = type.GetMethodInfo("Equals", new Type[] { type });
            //var ps = equals.GetParameters();
            //if (ps.Length == 1
            //    && ps[0].ParameterType != type)
            //{
            //    if (ps[0].ParameterType.IsValueType)
            //    {
            //        if (type.IsValueType)
            //        {
            //            il.Emit(OpCodes.Isinst, ps[0].ParameterType);
            //        }
            //        else
            //        {
            //            il.Emit(OpCodes.Unbox_Any, ps[0].ParameterType);
            //        }
            //    }
            //    else
            //    {
            //        if (type.IsValueType)
            //        {
            //            il.Emit(OpCodes.Box, ps[0].ParameterType);
            //        }
            //        else
            //        {
            //            il.Emit(OpCodes.Castclass, ps[0].ParameterType);
            //        }
            //    }
            //}
            il.Emit(OpCodes.Call, equals);
        }
        /// <summary>
        /// Appends the string representation of a specified object to <see cref="StringBuilder"/>.
        /// </summary>
        /// <param name="il">The <see cref="ILGenerator"/></param>
        /// <param name="context">The <see cref="CompileContext"/></param>
        /// <param name="tags">The <see cref="IList{ITag}"/></param>
        /// <param name="stringBuildIndex">The index on which <see cref="StringBuilder"/> resides.</param>
        /// <param name="contextIndex">The index on which <see cref="CompileContext"/> resides.</param>
        public static void StringAppend(this ILGenerator il, CompileContext context, TagCollection tags, int stringBuildIndex, int contextIndex)
        {
            for (var i = 0; i < tags.Count; i++)
            {
                il.CallTag(context, tags[i], (nil, hasReturn, needCall) =>
                 {
                     if (hasReturn)
                     {
                         nil.Emit(OpCodes.Ldloc, stringBuildIndex);
                     }
                     if (needCall)
                     {
                         nil.Emit(OpCodes.Ldarg_0);
                         nil.Emit(OpCodes.Ldloc, contextIndex);
                     }
                 }, (nil, returnType) =>
                 {
                     if (returnType == null)
                     {
                         return;
                     }
                     nil.StringAppend(context, returnType);
                     nil.Emit(OpCodes.Pop);
                 });
            }
        }



        /// <summary>
        /// Appends the string representation of a specified object to <see cref="StringBuilder"/>.
        /// </summary>
        /// <param name="il">The <see cref="ILGenerator"/></param>
        /// <param name="c">The <see cref="CompileContext"/></param>
        /// <param name="returnType"></param>\
        public static void StringAppend(this ILGenerator il, CompileContext c, Type returnType)
        {
            var stringBuilderType = typeof(StringBuilder);
            MethodInfo appendMethod = null;
            var ms = stringBuilderType.GetCacheMethods("Append");
            foreach (var m in ms)
            {
                var ps = m.GetParameters();
                if (ps.Length == 1 && returnType.FullName == ps[0].ParameterType.FullName && m.IsPublic)
                {
                    appendMethod = m;
                    break;
                }
            }
            if (appendMethod == null)
            {
                if (returnType.IsValueType)
                {
                    var p = il.DeclareLocal(returnType);
                    il.Emit(OpCodes.Stloc, p.LocalIndex);
                    LoadVariable(il, returnType, p.LocalIndex);
                    il.Call(returnType, typeof(object).GetMethodInfo("ToString", Type.EmptyTypes));
                    appendMethod = stringBuilderType.GetMethodInfo("Append", new Type[] { typeof(string) });
                }
                else
                {
                    appendMethod = stringBuilderType.GetMethodInfo("Append", new Type[] { typeof(object) });
                }
            }
            il.Emit(OpCodes.Callvirt, appendMethod);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="il"></param>
        /// <param name="source"></param>
        /// <param name="target"></param>
        public static void ConvertTo(this ILGenerator il, Type source, Type target)
        {
            var op = Dynamic.ReflectionExtensions.GetExplicitOrImplicit(source, target);
            if (op != null)
            {
                il.Emit(OpCodes.Call, op);
            }
            else
            {
                ObviousConvertTo(il, source, target);
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="il"></param>
        /// <param name="source"></param>
        /// <param name="target"></param>
        public static void ObviousConvertTo(this ILGenerator il, Type source, Type target)
        {
            switch (target.FullName)
            {
                case "System.Decimal":
                    switch (source.FullName)
                    {
                        case "System.Int16":
                        case "System.UInt16":
                        case "System.Byte":
                            il.Emit(OpCodes.Conv_I4);
                            break;
                    }
                    il.Emit(OpCodes.Newobj, typeof(decimal).GetConstructor(new Type[] { source }));
                    break;
                case "System.Double":
                    il.Emit(OpCodes.Conv_R8);
                    break;
                case "System.Single":
                    il.Emit(OpCodes.Conv_R4);
                    break;
                case "System.Int64":
                    il.Emit(OpCodes.Conv_I8);
                    break;
                case "System.UInt64":
                    il.Emit(OpCodes.Conv_U8);
                    break;
                case "System.Int32":
                    il.Emit(OpCodes.Conv_I4);
                    break;
                case "System.UInt32":
                    il.Emit(OpCodes.Conv_U4);
                    break;
                case "System.Int16":
                    il.Emit(OpCodes.Conv_I2);
                    break;
                case "System.UInt16":
                    il.Emit(OpCodes.Conv_U2);
                    break;
                case "System.Byte":
                    il.Emit(OpCodes.Conv_U1);
                    break;
                case "System.String":
                    if (source.IsValueType)
                    {
                        var p = il.DeclareLocal(source);
                        il.Emit(OpCodes.Stloc, p.LocalIndex);
                        il.Emit(OpCodes.Ldloca, p.LocalIndex);
                    }
                    il.Call(source, typeof(object).GetMethodInfo("ToString", Type.EmptyTypes));
                    break;
                default:
                    if (source.IsValueType)
                    {
                        if (target.IsValueType)
                        {
                            il.Emit(OpCodes.Isinst, target);
                        }
                        else
                        {
                            il.Emit(OpCodes.Box, source);
                        }
                    }
                    else
                    {
                        if (target.IsValueType)
                        {
                            il.Emit(OpCodes.Unbox, target);
                        }
                        else
                        {
                            il.Emit(OpCodes.Castclass, target);
                        }
                    }
                    break;
            }
        }
    }
}
