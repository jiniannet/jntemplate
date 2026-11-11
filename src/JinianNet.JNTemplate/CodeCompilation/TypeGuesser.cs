/********************************************************************************
 Copyright (c) jiniannet (http://www.jiniannet.com). All rights reserved.
 Licensed under the MIT license. See licence.txt file in the project root for full license information.
 ********************************************************************************/
using JinianNet.JNTemplate.Dynamic;
using System;

namespace JinianNet.JNTemplate.CodeCompilation;

/// <summary>
/// Type Guess
/// </summary>
public static class TypeGuesser
{
    /// <summary>
    /// Gets the child <see cref="Type"/> with the specified <see cref="Type"/>.
    /// </summary>
    /// <param name="type">Thpe <see cref="Type"/>.</param>
    /// <returns></returns>
    public static Type[] InferChildType(Type type)
    {
        if (type.IsArray)
        {
            var methd = type.GetMethodInfo("Get", [typeof(int)]);
            if (methd != null)
            {
                return [methd.ReturnType];
            }
        }
        var gType = type.GetIEnumerableGenericType();
        if (gType != null)
        {
            return gType.GetGenericArguments();
        }

        if (type == typeof(System.Data.DataTable))
        {
            return [typeof(System.Data.DataRow)];
        }

        var ms = type.GetMethods();
        var objType = typeof(object);
        foreach (var m in ms)
        {
            if (m.Name.Equals("get_Item", StringComparison.CurrentCultureIgnoreCase) &&
                m.ReturnType != objType)
                return [m.ReturnType];
        }
        return [typeof(object)];
    }

    /// <summary>
    /// Get priority
    /// </summary>
    /// <param name="type">TYPE</param>
    /// <returns></returns>
    private static int GetTypeLevel(Type type)
    {
        switch (type.ToTypeCode())
        {
            case TypeCode.Object:
                return 13;
            case TypeCode.String:
                return 12;
            case TypeCode.Boolean:
                return 11;
            case TypeCode.Decimal:
                return 10;
            case TypeCode.Double:
                return 9;
            case TypeCode.Single:
                return 8;
            case TypeCode.Int64:
                return 7;
            case TypeCode.UInt64:
                return 6;
            case TypeCode.Int32:
                return 5;
            case TypeCode.UInt32:
                return 4;
            case TypeCode.Int16:
                return 3;
            case TypeCode.UInt16:
                return 2;
            case TypeCode.SByte:
                return 1;
            case TypeCode.Byte:
                return 0;
            default:
                return -1;
        }
    }

    /// <summary>
    /// Whether the type is unsigned
    /// </summary>
    /// <param name="type">TYPE</param>
    /// <returns></returns>
    private static bool IsMinZero(Type type)
    {
        switch (type.ToTypeCode())
        {
            case TypeCode.UInt16:
            case TypeCode.UInt32:
            case TypeCode.UInt64:
            case TypeCode.Byte:
                return true;
            default:
                return false;
        }
    }

    /// <summary>
    /// The unsigned type is converted to the corresponding signed type
    /// </summary>
    /// <param name="type"></param>
    /// <returns></returns>
    private static Type ChangeType(Type type)
    {
        switch (type.ToTypeCode())
        {
            case TypeCode.UInt16:
                return typeof(Int16);
            case TypeCode.UInt32:
                return typeof(Int32);
            case TypeCode.UInt64:
                return typeof(Int64);
            case TypeCode.SByte:
                return typeof(Byte);
            default:
                return type;
        }
    }

    /// <summary>
    /// Choose the best type
    /// </summary>
    /// <param name="x">The first type.</param>
    /// <param name="y">The second type.</param>
    /// <returns></returns>
    public static Type ChoiceType(Type x, Type y)
    {
        var xCode = x.ToTypeCode();
        var yCode = y.ToTypeCode();
        if ((xCode == TypeCode.Single && (yCode == TypeCode.Int64 || yCode == TypeCode.UInt64))
            || (yCode == TypeCode.Single && (xCode == TypeCode.Int64 || xCode == TypeCode.UInt64)))
        {
            return typeof(double);
        }
        int x1 = GetTypeLevel(x);
        int y1 = GetTypeLevel(y);
        if (IsNumber(x) && IsNumber(y))
        {
            if (x1 > y1)
            {
                if (IsMinZero(x) && !IsMinZero(x))
                {
                    return ChangeType(x);
                }
                return x;
            }
            if (x1 < y1)
            {
                if (!IsMinZero(x) && IsMinZero(x))
                {
                    return ChangeType(y);
                }
                return y;
            }
        }
        else
        {
            if (x1 < y1)
            {
                return y;
            }
        }
        return x;
    }

    /// <summary>
    /// Find the type that matches best.
    /// </summary>
    /// <param name="types">type array</param>
    /// <returns></returns>
    public static Type FindBestType(Type[] types)
    {
        Type type = types[0];
        for (var i = 1; i < types.Length; i++)
        {
            if (types[i] == null)
            {
                continue;
            }
            type = ChoiceType(type, types[i]);
        }
        return type;
    }

    /// <summary>
    /// Gets a value indicating whether the  <see cref="Type"/> is one of the primitive types.
    /// </summary>
    /// <param name="type">The type.</param>
    /// <returns>true if the <see cref="Type"/> is one of the primitive types; otherwise, false.</returns>
    public static bool IsPrimitive(Type type)
    {
        return type.IsPrimitive;
    }
    /// <summary>
    /// 
    /// </summary>
    /// <param name="type"></param>
    /// <returns></returns>
    public static bool CanUseEqual(Type type)
    {
        var code = type.ToTypeCode();
        if (code == TypeCode.Decimal)
            return false;
        return IsNumber(code)
             || code == TypeCode.Boolean
             || type.IsEnum;
    }
    /// <summary>
    /// 
    /// </summary>
    /// <param name="type"></param>
    /// <returns></returns>
    public static bool IsNumber(Type type)
        => IsNumber(type.ToTypeCode());

    /// <summary>
    /// Indicates whether the specified type is categorized as a number.
    /// </summary>
    /// <param name="type">The type.</param>
    /// <returns>true if the <see cref="TypeCode"/> is one of the number types; otherwise, false.</returns>
    public static bool IsNumber(TypeCode type)
    {
        if (type == TypeCode.Decimal
            || type == TypeCode.Double
            || type == TypeCode.Single
            || type == TypeCode.Int64
            || type == TypeCode.UInt64
            || type == TypeCode.UInt32
            || type == TypeCode.Int32
            || type == TypeCode.Int16
            || type == TypeCode.UInt16
            || type == TypeCode.SByte
            || type == TypeCode.Byte)
            return true;
        return false;
    }

}
