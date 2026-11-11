/********************************************************************************
 Copyright (c) jiniannet (http://www.jiniannet.com). All rights reserved.
 Licensed under the MIT license. See licence.txt file in the project root for full license information.
 ********************************************************************************/

using System;
using System.Collections.Generic;

namespace JinianNet.JNTemplate;

/// <summary>
/// 
/// </summary>
public static class Extensions
{

    #region float
    /// <summary>
    /// 
    /// </summary>
    /// <param name="value"></param>
    /// <param name="defaultValue"></param>
    /// <returns></returns>
    public static float ToRequiredFloat(this object? value, float defaultValue)
    {
        return ToFloat(value) ?? defaultValue;
    }
    /// <summary>
    /// 
    /// </summary>
    /// <param name="value"></param>
    /// <returns></returns>
    public static float ToRequiredFloat(this object value)
    {
        return ToFloat(value) ?? throw new NullReferenceException();
    }
    /// <summary>
    /// 
    /// </summary>
    /// <param name="value"></param> 
    /// <returns></returns>
    public static float? ToFloat(this object? value)
    {
        if (value == null)
            return null;
        return value switch
        {
            int tValue => (float)tValue,
            short tValue => (float)tValue,
            long tValue => (float)tValue,
            double tValue => (float)tValue,
            byte tValue => (float)tValue,
            decimal tValue => (float)tValue,
            sbyte tValue => (float)tValue,
            ushort tValue => (float)tValue,
            uint tValue => (float)tValue,
            ulong tValue => (float)tValue,
            float tValue => tValue,
            bool tValue => tValue ? 1f : 0f,
            string tValue => ToFloat(tValue, null),
            _ => ToFloat(value.ToString(), null)
        };
    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="value"></param>
    /// <param name="defaultValue"></param>
    /// <returns></returns>
    public static float? ToFloat(this string? value, float? defaultValue = null)
    {
        if (value != null && float.TryParse(value, out var dValue))
            return dValue;
        return defaultValue;
    }
    #endregion

    #region double
    /// <summary>
    /// 
    /// </summary>
    /// <param name="value"></param>
    /// <param name="defaultValue"></param>
    /// <returns></returns>
    public static double ToRequiredDouble(this object? value, double defaultValue)
    {
        return ToDouble(value) ?? defaultValue;
    }
    /// <summary>
    /// 
    /// </summary>
    /// <param name="value"></param>
    /// <returns></returns>
    public static double ToRequiredDouble(this object value)
    {
        return ToDouble(value) ?? throw new NullReferenceException();
    }
    /// <summary>
    /// 
    /// </summary>
    /// <param name="value"></param> 
    /// <returns></returns>
    public static double? ToDouble(this object? value)
    {
        if (value == null)
            return null;
        return value switch
        {
            int tValue => (double)tValue,
            short tValue => (double)tValue,
            long tValue => (double)tValue,
            double tValue => tValue,
            byte tValue => (double)tValue,
            decimal tValue => (double)tValue,
            sbyte tValue => (double)tValue,
            ushort tValue => (double)tValue,
            uint tValue => (double)tValue,
            ulong tValue => (double)tValue,
            float tValue => (double)tValue,
            bool tValue => tValue ? 1d : 0d,
            string tValue => ToDouble(tValue, null),
            _ => ToDouble(value.ToString(), null)
        };
    }
    /// <summary>
    /// 
    /// </summary>
    /// <param name="value"></param>
    /// <param name="defaultValue"></param>
    /// <returns></returns>
    public static double? ToDouble(this string? value, double? defaultValue = null)
    {
        if (value != null && double.TryParse(value, out var dValue))
            return dValue;
        return defaultValue;
    }

    #endregion

    #region decimal

    /// <summary>
    /// 
    /// </summary>
    /// <param name="value"></param>
    /// <param name="defaultValue"></param>
    /// <returns></returns>
    public static decimal ToRequiredDecimal(this object? value, decimal defaultValue)
    {
        return ToDecimal(value) ?? defaultValue;
    }
    /// <summary>
    /// 
    /// </summary>
    /// <param name="value"></param>
    /// <returns></returns>
    public static decimal ToRequiredDecimal(this object? value)
    {
        return ToDecimal(value) ?? throw new NullReferenceException();
    }
    /// <summary>
    /// 
    /// </summary>
    /// <param name="value"></param>
    /// <returns></returns>
    public static decimal? ToDecimal(this object? value)
    {
        if (value == null)
            return null;
        return value switch
        {
            int tValue => (decimal)tValue,
            short tValue => (decimal)tValue,
            long tValue => (decimal)tValue,
            double tValue => (decimal)tValue,
            byte tValue => (decimal)tValue,
            decimal tValue => tValue,
            sbyte tValue => (decimal)tValue,
            ushort tValue => (decimal)tValue,
            uint tValue => (decimal)tValue,
            ulong tValue => (decimal)tValue,
            float tValue => (decimal)tValue,
            bool tValue => tValue ? 1M : 0M,
            string tValue => ToDecimal(tValue, null),
            _ => ToDecimal(value.ToString(), null)
        };
    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="value"></param>
    /// <param name="defaultValue"></param>
    /// <returns></returns>
    public static decimal? ToDecimal(this string? value, decimal? defaultValue)
    {
        if (value != null && decimal.TryParse(value, out var dValue))
            return dValue;
        return defaultValue;
    }
    #endregion

    #region  bool 
    /// <summary>
    /// 
    /// </summary>
    /// <param name="value"></param>
    /// <returns></returns>
    public static bool ToRequiredBoolean(this object value)
    {
        return ToBoolean(value) ?? throw new NullReferenceException();
    }
    /// <summary>
    /// 
    /// </summary>
    /// <param name="value"></param>
    /// <param name="defaultValue"></param>s
    /// <returns></returns>
    public static bool ToRequiredBoolean(this object? value, bool defaultValue)
    {
        return ToBoolean(value) ?? defaultValue;
    }
    /// <summary>
    /// 
    /// </summary>
    /// <param name="value"></param>
    /// <returns></returns>
    public static bool? ToBoolean(this object? value)
    {

        if (value == null)
            return null;

        return value switch
        {
            int tValue => tValue != 0,
            short tValue => tValue != 0,
            long tValue => tValue != 0,
            double tValue => tValue > 0 || tValue < 0,
            byte tValue => tValue != 0,
            decimal tValue => tValue != 0,
            sbyte tValue => tValue != 0,
            ushort tValue => tValue != 0,
            uint tValue => tValue != 0,
            ulong tValue => tValue != 0,
            float tValue => tValue > 0 || tValue < 0,
            bool tValue => tValue,
            string tValue => ToBoolean(tValue, null),
            _ => ToBoolean(value.ToString(), null)
        };
    }
    /// <summary>
    /// 
    /// </summary>
    /// <param name="value"></param>
    /// <param name="defaultValue"></param>
    /// <returns></returns>
    public static bool? ToBoolean(this string? value, bool? defaultValue = null)
    {
        if (value == null)
            return defaultValue;

        if (bool.TryParse(value, out var dValue))
            return dValue;

        if ("true".Equals(value, StringComparison.OrdinalIgnoreCase) || ToDecimal(value) == 1M)
            return true;

        return defaultValue;
    }

    #endregion

    #region int32

    /// <summary>
    /// 
    /// </summary>
    /// <param name="value"></param>
    /// <param name="defaultValue"></param>
    /// <returns></returns>
    public static int ToRequiredInt(this object? value, int defaultValue)
    {
        return ToInt(value) ?? defaultValue;
    }
    /// <summary>
    /// 
    /// </summary>
    /// <param name="value"></param>
    /// <returns></returns>
    public static int ToRequiredInt(this object value)
    {
        return ToInt(value) ?? throw new NullReferenceException();
    }
    /// <summary>
    /// 
    /// </summary>
    /// <param name="value"></param> 
    /// <returns></returns>
    public static int? ToInt(this object? value)
    {
        if (value == null)
            return null;

        return value switch
        {
            int tValue => tValue,
            short tValue => (int)tValue,
            long tValue => (int)tValue,
            double tValue => (int)tValue,
            byte tValue => (int)tValue,
            decimal tValue => (int)tValue,
            sbyte tValue => (int)tValue,
            ushort tValue => (int)tValue,
            uint tValue => (int)tValue,
            ulong tValue => (int)tValue,
            float tValue => (int)tValue,
            bool tValue => tValue ? 1 : 0,
            string tValue => ToInt(tValue, null),
            _ => ToInt(value.ToString(), null)
        };
    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="value"></param>
    /// <param name="defaultValue"></param>
    /// <returns></returns>
    public static int? ToInt(this string? value, int? defaultValue = null)
    {
        if (value != null && int.TryParse(value, out var dValue))
            return dValue;
        return defaultValue;
    }
    #endregion

    #region int16
    /// <summary>
    /// 
    /// </summary>
    /// <param name="value"></param>
    /// <param name="defaultValue"></param>
    /// <returns></returns>
    public static short ToRequiredShort(this object? value, short defaultValue)
    {
        return ToShort(value) ?? defaultValue;
    }
    /// <summary>
    /// 
    /// </summary>
    /// <param name="value"></param>
    /// <returns></returns>
    public static short ToRequiredShort(this object value)
    {
        return ToShort(value) ?? throw new NullReferenceException();
    }
    /// <summary>
    /// 
    /// </summary>
    /// <param name="value"></param>
    /// <returns></returns>
    public static short? ToShort(this object? value)
    {
        if (value == null)
            return null;

        return value switch
        {
            int tValue => (short)tValue,
            short tValue => tValue,
            long tValue => (short)tValue,
            double tValue => (short)tValue,
            byte tValue => (short)tValue,
            decimal tValue => (short)tValue,
            sbyte tValue => (short)tValue,
            ushort tValue => (short)tValue,
            uint tValue => (short)tValue,
            ulong tValue => (short)tValue,
            float tValue => (short)tValue,
            bool tValue => tValue ? (short)1 : (short)0,
            string tValue => ToShort(tValue, null),
            _ => ToShort(value.ToString(), null)
        };
    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="value"></param>
    /// <param name="defaultValue"></param>
    /// <returns></returns>
    public static short? ToShort(this string? value, short? defaultValue = null)
    {
        if (value != null && short.TryParse(value, out var dValue))
            return dValue;
        return defaultValue;
    }

    #endregion

    #region int64
    /// <summary>
    /// 
    /// </summary>
    /// <param name="value"></param>
    /// <param name="defaultValue"></param>
    /// <returns></returns>
    public static long ToRequiredLong(this object? value, long defaultValue)
    {
        return ToLong(value) ?? defaultValue;
    }
    /// <summary>
    /// 
    /// </summary>
    /// <param name="value"></param>
    /// <returns></returns>
    public static long ToRequiredLong(this object value)
    {
        return ToLong(value) ?? throw new NullReferenceException();
    }
    /// <summary>
    /// 
    /// </summary>
    /// <param name="gid"></param>
    /// <returns></returns>
    public static long ToRequiredLong(this Guid gid)
    {
        var bs = gid.ToByteArray();
        return BitConverter.ToInt64(bs, 0);
    }
    /// <summary>
    /// 
    /// </summary>
    /// <param name="value"></param>
    /// <returns></returns>
    public static long? ToLong(this object? value)
    {
        if (value == null)
            return null;

        return value switch
        {
            int tValue => (long)tValue,
            short tValue => (long)tValue,
            long tValue => tValue,
            double tValue => (long)tValue,
            byte tValue => (long)tValue,
            decimal tValue => (long)tValue,
            sbyte tValue => (long)tValue,
            ushort tValue => (long)tValue,
            uint tValue => (long)tValue,
            ulong tValue => (long)tValue,
            float tValue => (long)tValue,
            bool tValue => tValue ? 1L : 0L,
            string tValue => ToLong(tValue, null),
            _ => ToLong(value.ToString(), null)
        };
    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="value"></param>
    /// <param name="defaultValue"></param>
    /// <returns></returns>
    public static long? ToLong(this string? value, long? defaultValue = null)
    {
        if (value != null && long.TryParse(value, out var dValue))
            return dValue;
        return defaultValue;
    }
    #endregion

    #region uint32

    /// <summary>
    /// 
    /// </summary>
    /// <param name="value"></param>
    /// <param name="defaultValue"></param>
    /// <returns></returns>
    public static uint ToRequiredUInt(this object? value, uint defaultValue)
    {
        return ToUInt(value) ?? defaultValue;
    }
    /// <summary>
    /// 
    /// </summary>
    /// <param name="value"></param>
    /// <returns></returns>
    public static uint ToRequiredUInt(this object value)
    {
        return ToUInt(value) ?? throw new NullReferenceException();
    }
    /// <summary>
    /// 
    /// </summary>
    /// <param name="value"></param>
    /// <returns></returns>
    public static uint? ToUInt(this object? value)
    {
        if (value == null)
            return null;
        return value switch
        {
            int tValue => (uint)tValue,
            short tValue => (uint)tValue,
            long tValue => (uint)tValue,
            double tValue => (uint)tValue,
            byte tValue => (uint)tValue,
            decimal tValue => (uint)tValue,
            sbyte tValue => (uint)tValue,
            ushort tValue => (uint)tValue,
            uint tValue => tValue,
            ulong tValue => (uint)tValue,
            float tValue => (uint)tValue,
            bool tValue => tValue ? 1u : 0u,
            string tValue => ToUInt(tValue, null),
            _ => ToUInt(value.ToString(), null)
        };
    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="value"></param>
    /// <param name="defaultValue"></param>
    /// <returns></returns>
    public static uint? ToUInt(this string? value, uint? defaultValue)
    {
        if (value != null && uint.TryParse(value, out var dValue))
            return dValue;
        return defaultValue;
    }
    #endregion

    #region ushort

    /// <summary>
    /// 
    /// </summary>
    /// <param name="value"></param>
    /// <param name="defaultValue"></param>
    /// <returns></returns>
    public static ushort ToRequiredUShort(this object? value, ushort defaultValue)
    {
        return ToUShort(value) ?? defaultValue;
    }
    /// <summary>
    /// 
    /// </summary>
    /// <param name="value"></param>
    /// <returns></returns>
    public static ushort ToRequiredUShort(this object value)
    {
        return ToUShort(value) ?? throw new NullReferenceException();
    }
    /// <summary>
    /// 
    /// </summary>
    /// <param name="value"></param>
    /// <returns></returns>
    public static ushort? ToUShort(this object? value)
    {
        if (value == null)
            return null;

        return value switch
        {
            int tValue => (ushort)tValue,
            short tValue => (ushort)tValue,
            long tValue => (ushort)tValue,
            double tValue => (ushort)tValue,
            byte tValue => (ushort)tValue,
            decimal tValue => (ushort)tValue,
            sbyte tValue => (ushort)tValue,
            ushort tValue => tValue,
            uint tValue => (ushort)tValue,
            ulong tValue => (ushort)tValue,
            float tValue => (ushort)tValue,
            bool tValue => (ushort)(tValue ? 1 : 0),
            string tValue => ToUShort(tValue, null),
            _ => ToUShort(value.ToString(), null)
        };
    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="value"></param>
    /// <param name="defaultValue"></param>
    /// <returns></returns>
    public static ushort? ToUShort(this string? value, ushort? defaultValue)
    {
        if (value != null && ushort.TryParse(value, out var dValue))
            return dValue;
        return defaultValue;
    }

    #endregion

    #region ulong 
    /// <summary>
    /// 
    /// </summary>
    /// <param name="value"></param>
    /// <param name="defaultValue"></param>
    /// <returns></returns>
    public static ulong ToRequiredULong(this object? value, ulong defaultValue)
    {
        return ToULong(value) ?? defaultValue;
    }
    /// <summary>
    /// 
    /// </summary>
    /// <param name="value"></param>
    /// <returns></returns>
    public static ulong ToRequiredULong(this object? value)
    {
        return ToULong(value) ?? throw new NullReferenceException();
    }
    /// <summary>
    /// 
    /// </summary>
    /// <param name="value"></param> 
    /// <returns></returns>
    public static ulong? ToULong(this object? value)
    {
        if (value == null)
            return null;

        return value switch
        {
            int tValue => (ulong)tValue,
            short tValue => (ulong)tValue,
            long tValue => (ulong)tValue,
            double tValue => (ulong)tValue,
            byte tValue => (ulong)tValue,
            decimal tValue => (ulong)tValue,
            sbyte tValue => (ulong)tValue,
            ushort tValue => (ulong)tValue,
            uint tValue => (ulong)tValue,
            ulong tValue => tValue,
            float tValue => (ulong)tValue,
            bool tValue => tValue ? 1ul : 0ul,
            string tValue => ToULong(tValue, null),
            _ => ToULong(value.ToString(), null)
        };
    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="value"></param>
    /// <param name="defaultValue"></param>
    /// <returns></returns>
    public static ulong? ToULong(this string? value, ulong? defaultValue)
    {
        if (value != null && ulong.TryParse(value, out var dValue))
            return dValue;
        return defaultValue;
    }
    #endregion

    #region byte
    /// <summary>
    /// 
    /// </summary>
    /// <param name="value"></param>
    /// <param name="defaultValue"></param>
    /// <returns></returns>
    public static byte ToRequiredByte(this object? value, byte defaultValue)
    {
        return ToByte(value) ?? defaultValue;
    }
    /// <summary>
    /// 
    /// </summary>
    /// <param name="value"></param>
    /// <returns></returns>
    public static byte ToRequiredByte(this object value)
    {
        return ToByte(value) ?? throw new NullReferenceException();
    }
    /// <summary>
    /// 
    /// </summary>
    /// <param name="value"></param> 
    /// <returns></returns>
    public static byte? ToByte(this object? value)
    {
        if (value == null)
            return null;
        return value switch
        {
            int tValue => (byte)tValue,
            short tValue => (byte)tValue,
            long tValue => (byte)tValue,
            double tValue => (byte)tValue,
            byte tValue => tValue,
            decimal tValue => (byte)tValue,
            sbyte tValue => (byte)tValue,
            ushort tValue => (byte)tValue,
            uint tValue => (byte)tValue,
            ulong tValue => (byte)tValue,
            float tValue => (byte)tValue,
            bool tValue => (byte)(tValue ? 1 : 0),
            string tValue => ToByte(tValue, null),
            _ => ToByte(value.ToString(), null)
        };
    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="value"></param>
    /// <param name="defaultValue"></param>
    /// <returns></returns>
    public static byte? ToByte(this string? value, byte? defaultValue)
    {
        if (value != null && byte.TryParse(value, out var dValue))
            return dValue;
        return defaultValue;
    }
    #endregion

    #region common
    /// <summary>
    /// 
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="list"></param>
    /// <param name="value"></param>
    public static void AddNotNullObject<T>(this IList<T> list, T? value)
    {
        if (value != null)
            list.Add(value);
    }
    /// <summary>
    /// 
    /// </summary>
    /// <param name="type"></param>
    /// <returns></returns>
    public static TypeCode ToTypeCode(this Type type)
    {
        if (type.IsEnum)
            return TypeCode.Empty;
        var typeCode = Type.GetTypeCode(type);
        if (typeCode == TypeCode.Object && type != typeof(Object))
            return TypeCode.Empty;
        return typeCode;
    }
    #endregion
}
