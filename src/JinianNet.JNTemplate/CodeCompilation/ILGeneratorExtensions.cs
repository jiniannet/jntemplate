/********************************************************************************
 Copyright (c) jiniannet (http://www.jiniannet.com). All rights reserved.
 Licensed under the MIT license. See licence.txt file in the project root for full license information.
 ********************************************************************************/
using JinianNet.JNTemplate.Dynamic;
using JinianNet.JNTemplate.Exceptions;
using JinianNet.JNTemplate.Nodes;
using System;
using System.Collections.Generic;
using System.Reflection;
using System.Reflection.Emit;
using System.Text;

namespace JinianNet.JNTemplate.CodeCompilation;

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
        var m = type.GetRequiredMethod("Parse", [typeof(string)]);
        if (m != null)
        {
            var toString = typeof(object).GetRequiredMethod("ToString", Type.EmptyTypes);
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
        if (method.IsStatic || (method.DeclaringType != null && method.DeclaringType.IsValueType))
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
        switch (type.ToTypeCode())
        {
            case TypeCode.Double:
                il.Emit(OpCodes.Ldelem_R8);
                break;
            case TypeCode.Single:
                il.Emit(OpCodes.Ldelem_R4);
                break;
            case TypeCode.Int64:
                il.Emit(OpCodes.Ldelem_I8);
                break;
            case TypeCode.Int32:
                il.Emit(OpCodes.Ldelem_I4);
                break;
            case TypeCode.UInt32:
                il.Emit(OpCodes.Ldelem_U4);
                break;
            case TypeCode.Int16:
                il.Emit(OpCodes.Ldelem_I2);
                break;
            case TypeCode.UInt16:
            case TypeCode.Char:
                il.Emit(OpCodes.Ldelem_U2);
                break;
            case TypeCode.Byte:
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
        switch (tag)
        {
            case BooleanTag boolTag:
                il.Emit(OpCodes.Ldc_I4, boolTag.Value ? 1 : 0);
                return typeof(bool);
            case NumberTag numberTag:
                switch (numberTag.Value)
                {
                    case int iValue:
                        il.Emit(OpCodes.Ldc_I4, iValue);
                        return typeof(int);
                    case long lValue:
                        il.Emit(OpCodes.Ldc_I8, lValue);
                        return typeof(long);
                    case float fValue:
                        il.Emit(OpCodes.Ldc_R4, fValue);
                        return typeof(float);
                    case double dValue:
                        il.Emit(OpCodes.Ldc_R8, dValue);
                        return typeof(double);
                    case short sValue:
                        il.Emit(OpCodes.Ldc_I4, sValue);
                        return typeof(short);
                    default:
                        throw new NotSupportedException($"[NumberTag] : [{numberTag.Value}] is not supported");
                }
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
        Action<ILGenerator, Type?> completed)
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
            if (setTag.Name == null || setTag.Value == null) throw new ArgumentException(nameof(tag));
            ctx.Set(setTag.Name, ctx.GuessType(setTag.Value));
        }
        var m = ctx.CompileRequiredMethod(tag);
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
        var equals = type.GetMethodInfo("op_Equality", [type]);
        if (equals == null)
            equals = type.GetRequiredMethod("Equals", [type]);
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
        MethodInfo? appendMethod = null;
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
                il.Call(returnType, typeof(object).GetRequiredMethod("ToString", Type.EmptyTypes));
                appendMethod = stringBuilderType.GetRequiredMethod("Append", [typeof(string)]);
            }
            else
            {
                appendMethod = stringBuilderType.GetRequiredMethod("Append", [typeof(object)]);
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
        switch (target.ToTypeCode())
        {
            case TypeCode.Decimal:
                switch (source.ToTypeCode())
                {
                    case TypeCode.Int16:
                    case TypeCode.UInt16:
                    case TypeCode.Byte:
                        il.Emit(OpCodes.Conv_I4);
                        break;
                }
                il.Emit(OpCodes.Newobj, typeof(decimal).GetRequiredConstructor([source]));
                break;
            case TypeCode.Double:
                il.Emit(OpCodes.Conv_R8);
                break;
            case TypeCode.Single:
                il.Emit(OpCodes.Conv_R4);
                break;
            case TypeCode.Int64:
                il.Emit(OpCodes.Conv_I8);
                break;
            case TypeCode.UInt64:
                il.Emit(OpCodes.Conv_U8);
                break;
            case TypeCode.Int32:
                il.Emit(OpCodes.Conv_I4);
                break;
            case TypeCode.UInt32:
                il.Emit(OpCodes.Conv_U4);
                break;
            case TypeCode.Int16:
                il.Emit(OpCodes.Conv_I2);
                break;
            case TypeCode.UInt16:
                il.Emit(OpCodes.Conv_U2);
                break;
            case TypeCode.Byte:
                il.Emit(OpCodes.Conv_U1);
                break;
            case TypeCode.String:
                if (source.IsValueType)
                {
                    var p = il.DeclareLocal(source);
                    il.Emit(OpCodes.Stloc, p.LocalIndex);
                    il.Emit(OpCodes.Ldloca, p.LocalIndex);
                }
                il.Call(source, typeof(object).GetRequiredMethod("ToString", Type.EmptyTypes));
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
