/********************************************************************************
 Copyright (c) jiniannet (http://www.jiniannet.com). All rights reserved.
 Licensed under the MIT license. See licence.txt file in the project root for full license information.
 ********************************************************************************/
using System;
using JinianNet.JNTemplate.Exceptions;

namespace JinianNet.JNTemplate.Dynamic;

internal static class NumericOps<T> where T : struct
{
    public static readonly Func<T, T, string, object> Calculate = BuildCalculate();
    public static readonly Func<T, T, string, object> CalculateBinary = BuildCalculateBinary();

    private static Func<T, T, string, object> BuildCalculate()
    {
        Type t = typeof(T);
        if (t == typeof(int)) return Cast<int>(CalcInt32);
        if (t == typeof(uint)) return Cast<uint>(CalcUInt32);
        if (t == typeof(long)) return Cast<long>(CalcInt64);
        if (t == typeof(ulong)) return Cast<ulong>(CalcUInt64);
        if (t == typeof(short)) return Cast<short>(CalcInt16);
        if (t == typeof(ushort)) return Cast<ushort>(CalcUInt16);
        if (t == typeof(byte)) return Cast<byte>(CalcByte);
        if (t == typeof(sbyte)) return Cast<sbyte>(CalcSByte);
        if (t == typeof(float)) return Cast<float>(CalcSingle);
        if (t == typeof(double)) return Cast<double>(CalcDouble);
        if (t == typeof(decimal)) return Cast<decimal>(CalcDecimal);

        throw new TemplateException(
            $"Type \"{t.Name}\" is not a supported numeric type for CalculateNumber.");
    }

    private static Func<T, T, string, object> BuildCalculateBinary()
    {
        Type t = typeof(T);
        if (t == typeof(int)) return Cast<int>(CalcBinaryInt32);
        if (t == typeof(uint)) return Cast<uint>(CalcBinaryUInt32);
        if (t == typeof(long)) return Cast<long>(CalcBinaryInt64);
        if (t == typeof(ulong)) return Cast<ulong>(CalcBinaryUInt64);
        if (t == typeof(short)) return Cast<short>(CalcBinaryInt16);
        if (t == typeof(ushort)) return Cast<ushort>(CalcBinaryUInt16);
        if (t == typeof(byte)) return Cast<byte>(CalcBinaryByte);
        if (t == typeof(sbyte)) return Cast<sbyte>(CalcBinarySByte);
        return Calculate;
    }

    private static Func<T, T, string, object> Cast<TA>(Func<TA, TA, string, object> f)
        where TA : struct
        => (Func<T, T, string, object>)(object)f;

    private static TemplateException Unsupported<TA>(string op)
        => new TemplateException(
            $"Operator \"{op}\" can not be applied operand \"{typeof(TA).Name}\" and \"{typeof(TA).Name}\"");

    private static object CalcInt32(int x, int y, string op)
    {
        unchecked
        {
            switch (op)
            {
                case "+": return x + y;
                case "-": return x - y;
                case "*": return x * y;
                case "/": return x / y;
                case "%": return x % y;
                case ">=": return x >= y;
                case "<=": return x <= y;
                case "<": return x < y;
                case ">": return x > y;
                case "==": return x == y;
                case "!=": return x != y;
                default: throw Unsupported<int>(op);
            }
        }
    }

    private static object CalcInt64(long x, long y, string op)
    {
        unchecked
        {
            switch (op)
            {
                case "+": return x + y;
                case "-": return x - y;
                case "*": return x * y;
                case "/": return x / y;
                case "%": return x % y;
                case ">=": return x >= y;
                case "<=": return x <= y;
                case "<": return x < y;
                case ">": return x > y;
                case "==": return x == y;
                case "!=": return x != y;
                default: throw Unsupported<long>(op);
            }
        }
    }

    private static object CalcDouble(double x, double y, string op)
    {
        switch (op)
        {
            case "+": return x + y;
            case "-": return x - y;
            case "*": return x * y;
            case "/": return x / y;
            case "%": return x % y;
            case ">=": return x >= y;
            case "<=": return x <= y;
            case "<": return x < y;
            case ">": return x > y;
            case "==": return x == y;
            case "!=": return x != y;
            default: throw Unsupported<double>(op);
        }
    }

    private static object CalcDecimal(decimal x, decimal y, string op)
    {
        switch (op)
        {
            case "+": return x + y;
            case "-": return x - y;
            case "*": return x * y;
            case "/": return x / y;
            case "%": return x % y;
            case ">=": return x >= y;
            case "<=": return x <= y;
            case "<": return x < y;
            case ">": return x > y;
            case "==": return x == y;
            case "!=": return x != y;
            default: throw Unsupported<decimal>(op);
        }
    }

    private static object CalcUInt32(uint x, uint y, string op)
    {
        switch (op)
        {
            case "+": return x + y;
            case "-": return x - y;
            case "*": return x * y;
            case "/": return x / y;
            case "%": return x % y;
            case ">=": return x >= y;
            case "<=": return x <= y;
            case "<": return x < y;
            case ">": return x > y;
            case "==": return x == y;
            case "!=": return x != y;
            default: throw Unsupported<uint>(op);
        }
    }
    private static object CalcUInt64(ulong x, ulong y, string op)
    {
        switch (op)
        {
            case "+": return x + y;
            case "-": return x - y;
            case "*": return x * y;
            case "/": return x / y;
            case "%": return x % y;
            case ">=": return x >= y;
            case "<=": return x <= y;
            case "<": return x < y;
            case ">": return x > y;
            case "==": return x == y;
            case "!=": return x != y;
            default: throw Unsupported<ulong>(op);
        }
    }

    private static object CalcInt16(short x, short y, string op)
    {
        switch (op)
        {
            case "+": return x + y;
            case "-": return x - y;
            case "*": return x * y;
            case "/": return x / y;
            case "%": return x % y;
            case ">=": return x >= y;
            case "<=": return x <= y;
            case "<": return x < y;
            case ">": return x > y;
            case "==": return x == y;
            case "!=": return x != y;
            default: throw Unsupported<short>(op);
        }
    }
    private static object CalcUInt16(ushort x, ushort y, string op)
    {
        switch (op)
        {
            case "+": return x + y;
            case "-": return x - y;
            case "*": return x * y;
            case "/": return x / y;
            case "%": return x % y;
            case ">=": return x >= y;
            case "<=": return x <= y;
            case "<": return x < y;
            case ">": return x > y;
            case "==": return x == y;
            case "!=": return x != y;
            default: throw Unsupported<ushort>(op);
        }
    }
    private static object CalcByte(byte x, byte y, string op)
    {
        switch (op)
        {
            case "+": return x + y;
            case "-": return x - y;
            case "*": return x * y;
            case "/": return x / y;
            case "%": return x % y;
            case ">=": return x >= y;
            case "<=": return x <= y;
            case "<": return x < y;
            case ">": return x > y;
            case "==": return x == y;
            case "!=": return x != y;
            default: throw Unsupported<byte>(op);
        }
    }

    private static object CalcSByte(sbyte x, sbyte y, string op)
    {
        switch (op)
        {
            case "+": return x + y;
            case "-": return x - y;
            case "*": return x * y;
            case "/": return x / y;
            case "%": return x % y;
            case ">=": return x >= y;
            case "<=": return x <= y;
            case "<": return x < y;
            case ">": return x > y;
            case "==": return x == y;
            case "!=": return x != y;
            default: throw Unsupported<sbyte>(op);
        }
    }
    private static object CalcSingle(float x, float y, string op)
    {
        switch (op)
        {
            case "+": return x + y;
            case "-": return x - y;
            case "*": return x * y;
            case "/": return x / y;
            case "%": return x % y;
            case ">=": return x >= y;
            case "<=": return x <= y;
            case "<": return x < y;
            case ">": return x > y;
            case "==": return x == y;
            case "!=": return x != y;
            default: throw Unsupported<float>(op);
        }
    }

    private static object CalcBinaryInt32(int x, int y, string op)
    {
        unchecked
        {
            switch (op)
            {
                case "|": return x | y;
                case "&": return x & y;
                default: return CalcInt32(x, y, op);
            }
        }
    }

    private static object CalcBinaryInt64(long x, long y, string op)
    {
        unchecked
        {
            switch (op)
            {
                case "|": return x | y;
                case "&": return x & y;
                default: return CalcInt64(x, y, op);
            }
        }
    }
    private static object CalcBinaryUInt32(uint x, uint y, string op)
    {
        unchecked
        {
            switch (op)
            {
                case "|": return x | y;
                case "&": return x & y;
                default: return CalcUInt32(x, y, op);
            }
        }
    }
    private static object CalcBinaryUInt64(ulong x, ulong y, string op)
    {
        unchecked
        {
            switch (op)
            {
                case "|": return x | y;
                case "&": return x & y;
                default: return CalcUInt64(x, y, op);
            }
        }
    }
    private static object CalcBinaryInt16(short x, short y, string op)
    {
        unchecked
        {
            switch (op)
            {
                case "|": return x | y;
                case "&": return x & y;
                default: return CalcInt16(x, y, op);
            }
        }
    }
    private static object CalcBinaryUInt16(ushort x, ushort y, string op)
    {
        unchecked
        {
            switch (op)
            {
                case "|": return x | y;
                case "&": return x & y;
                default: return CalcUInt16(x, y, op);
            }
        }
    }
    private static object CalcBinaryByte(byte x, byte y, string op)
    {
        unchecked
        {
            switch (op)
            {
                case "|": return x | y;
                case "&": return x & y;
                default: return CalcByte(x, y, op);
            }
        }
    }
    private static object CalcBinarySByte(sbyte x, sbyte y, string op)
    {
        unchecked
        {
            switch (op)
            {
                case "|": return x | y;
                case "&": return x & y;
                default: return CalcSByte(x, y, op);
            }
        }
    }
}
