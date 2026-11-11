/********************************************************************************
 Copyright (c) jiniannet (http://www.jiniannet.com). All rights reserved.
 Licensed under the MIT license. See licence.txt file in the project root for full license information.
 ********************************************************************************/
using JinianNet.JNTemplate.Exceptions;
using System;
using System.Collections.Generic;

namespace JinianNet.JNTemplate.Dynamic;


/// <summary>
/// Expression calculation
/// </summary>
public class ExpressionEvaluator
{
    #region Weights Array
    private static readonly TypeCode[] NumberWeights = new TypeCode[] {
            TypeCode.Int16,
             TypeCode.Int32,
             TypeCode.Int64,
             TypeCode.Single,
             TypeCode.Double,
             TypeCode.Decimal};

    private static readonly TypeCode[] UintWeights = new TypeCode[] {
            TypeCode.UInt16,
             TypeCode.UInt32,
             TypeCode.UInt64};
    #endregion

    #region LetterType
    /// <summary>
    /// LetterType
    /// </summary>
    public enum LetterType
    {
        /// <summary>
        /// None
        /// </summary>
        None = 0,
        /// <summary>
        /// Operator
        /// </summary>
        Operator = 1,
        /// <summary>
        /// (
        /// </summary>
        LeftParentheses = 2,
        /// <summary>
        /// ]
        /// </summary>
        RightParentheses = 3,
        /// <summary>
        /// number
        /// </summary>
        Number = 4,
        /// <summary>
        /// other
        /// </summary>
        Other = 5
    }
    #endregion

    #region common function
    private static bool IsOperator(string value)
    {
        switch (value)
        {
            case "||":
            case "|":
            case "&":
            case "&&":
            case ">":
            case ">=":
            case "<":
            case "<=":
            case "==":
            case "!=":
            case "+":
            case "-":
            case "*":
            case "/":
            case "(":
            case ")":
            case "%":
                return true;
            default:
                return false;
        }
    }

    private static int GetPriority(string? c)
    {
        switch (c)
        {
            case "||":
            case "|":
            case "&":
            case "&&":
            case "Or":
            case "LogicalOr":
            case "LogicAnd":
            case "And":
                return 5;
            case ">":
            case ">=":
            case "<":
            case "<=":
            case "==":
            case "!=":

            case "GreaterThan":
            case "GreaterThanOrEqual":
            case "LessThan":
            case "LessThanOrEqual":
            case "Equal":
            case "NotEqual":
                return 6;
            case "+":
            case "-":

            case "Add":
            case "Subtract":
                return 7;
            case "%":
            case "*":
            case "Remainder":
            case "Multiply":
            case "/":
            case "Divided":
                return 8;
            default:
                return 9;
        }
    }

    private static bool IsNumber(TypeCode? type)
    {
        switch (type)
        {
            case TypeCode.Double:
            case TypeCode.Int16:
            case TypeCode.Int32:
            case TypeCode.Int64:
            case TypeCode.UInt16:
            case TypeCode.UInt32:
            case TypeCode.UInt64:
            case TypeCode.Single:
            case TypeCode.Decimal:
            case TypeCode.SByte:
            case TypeCode.Byte:
                return true;
            default:
                return false;
        }
    }
    #endregion

    #region ProcessExpression
    /// <summary>
    /// process expression.
    /// </summary>
    /// <param name="value">The expression.</param>
    /// <returns></returns>
    public static Stack<object?> ProcessExpression(string value)
    {
        value = value.Replace("  ", string.Empty);
        List<object> result = new List<object>();
        int j = 0;
        int i;
        string num;

        for (i = 0; i < value.Length; i++)
        {
            switch (value[i])
            {
                case '+':
                case '-':
                case '*':
                case '/':
                case '(':
                case ')':
                case '%':
                    if (j < i)
                    {
                        num = value.Substring(j, i - j);
                        if (num.IndexOf('.') == -1)
                        {
                            result.Add(int.Parse(value.Substring(j, i - j)));
                        }
                        else
                        {
                            result.Add(Double.Parse(value.Substring(j, i - j)));
                        }
                        j = i;
                    }
                    result.Add(OperatorConvert.Parse(value[i].ToString()));
                    j++;
                    break;
            }
        }
        if (j < i)
        {
            result.Add(Double.Parse(value.Substring(j, i - j)));
        }
        return ProcessExpression([.. result]);

    }

    /// <summary>
    /// process expression.
    /// </summary>
    /// <param name="value">The expression.</param>
    /// <returns></returns>
    public static Stack<object?> ProcessExpression(object?[] value)
    {
        var post = new Stack<object?>();
        var stack = new Stack<object?>();

        for (int i = 0; i < value.Length; i++)
        {
            if (!(value[i] is Operator _))
            {
                post.Push(value[i]);
                continue;
            }
            switch (value[i]?.ToString())
            {
                //case ")":
                //case "RightParentheses":
                //case "(":
                //case "LeftParentheses":

                case "(":
                case "LeftParentheses":
                    stack.Push("(");
                    break;
                case ")":
                case "RightParentheses":
                    while (stack.Count > 0)
                    {
                        var op = stack.Pop();
                        if (op?.ToString() == "(")
                        {
                            break;
                        }
                        else
                        {
                            post.Push(op);
                        }
                    }
                    break;
                case "+":
                case "-":
                case "*":
                case "%":
                case "/":
                case "||":
                case "|":
                case "&&":
                case "&":
                case ">":
                case ">=":
                case "<":
                case "<=":
                case "==":
                case "!=":
                case "Add":
                case "Subtract":
                case "Multiply":
                case "Remainder":
                case "Divided":
                case "LogicalOr":
                case "Or":
                case "LogicAnd":
                case "And":
                case "GreaterThan":
                case "GreaterThanOrEqual":
                case "LessThan":
                case "LessThanOrEqual":
                case "Equal":
                case "NotEqual":
                    if (stack.Count == 0)
                    {
                        stack.Push(value[i]);
                    }
                    else
                    {
                        var eX = stack.Peek();
                        var eY = value[i];
                        int pY = GetPriority(eY?.ToString());
                        int pX = GetPriority(eX?.ToString());

                        if (pY > pX)
                        {
                            stack.Push(eY);
                        }
                        else if (pY == pX)
                        {
                            post.Push(stack.Pop());
                            stack.Push(eY);
                        }
                        else if (pY < pX)
                        {

                            if (eX?.ToString() != "(")
                            {
                                while (stack.Count > 0)
                                {
                                    post.Push(stack.Pop());
                                }
                            }
                            stack.Push(eY);
                        }
                    }
                    break;
                default:
                    post.Push(value[i]);
                    break;
            }
        }

        while (stack.Count > 0)
        {
            post.Push(stack.Pop());
        }

        return post;
    }



    #endregion

    #region Calculate
    /// <summary>
    /// Gets the type of the specified object.
    /// </summary>
    /// <param name="value">The object.</param>
    /// <returns>A type.</returns>
    private static Type GetType(object value)
    {
        if (value == null)
        {
            return typeof(object);
        }
        return value.GetType();
    }

    private static object CalculateAnd(object? x, object? y)
    {
        if (!CalculateBoolean(x))
        {
            return false;
        }
        if (!CalculateBoolean(y))
        {
            return false;
        }
        return true;
    }

    private static object CalculateOr(object? x, object? y)
    {
        if (CalculateBoolean(x))
        {
            return true;
        }
        if (CalculateBoolean(y))
        {
            return true;
        }
        return false;
    }

    internal static bool CalculateBoolean(object? value)
    {
        if (value == null)
            return false;
        switch (value.GetType().ToTypeCode())
        {
            case TypeCode.Boolean:
                return (bool)value;
            case TypeCode.String:
                return !string.IsNullOrEmpty(value.ToString());
            case TypeCode.UInt16:
            case TypeCode.UInt32:
            case TypeCode.UInt64:
            case TypeCode.Int16:
            case TypeCode.Int32:
            case TypeCode.Int64:
                return value.ToString() != "0";
            case TypeCode.Decimal:
                return (decimal)value != 0;
            case TypeCode.Double:
                return !0.Equals((double)value);
            case TypeCode.Single:
                return !0.Equals((float)value);
            default:
                return value != null;
        }
    }

    private static bool Equals(object? x, object? y, Type tX, Type tY)
    {
        if (x == null || y == null)
        {
            return x == null && y == null;
        }
        if (tX.FullName == tY.FullName)
        {
            if (tX.IsEnum)
            {
                return Equals(x, y);
            }
            return x == y;
        }
        return false;
    }
    /// <summary>
    /// Calculates the value of objects
    /// </summary>
    /// <param name="x">The first value. </param>
    /// <param name="y">The second value.</param>
    /// <param name="value">The result of expression.</param>
    /// <returns></returns>
    public static object? Calculate(object? x, object? y, string value)
    {
        if (value == "||")
        {
            return CalculateOr(x, y);
        }

        if (value == "&&")
        {
            return CalculateAnd(x, y);
        }

        Type tX, tY;
        if (x == null || y == null)
        {
            if (x != null)
                tX = tY = GetType(x);
            else if (y != null)
                tX = tY = GetType(y);
            else
                tX = tY = typeof(object);
        }
        else
        {
            tX = GetType(x);
            tY = GetType(y);
        }

        var xCode = tX.ToTypeCode();
        var yCode = tY.ToTypeCode();

        if (IsNumber(xCode) && IsNumber(yCode))
        {
            TypeCode t;
            if (xCode == yCode)
            {
                t = xCode;
            }
            else
            {
                int i, j;
                if (tX.Name[0] == 'U' && tY.Name[0] == 'U')
                {
                    i = Array.IndexOf<TypeCode>(UintWeights, xCode);
                    j = Array.IndexOf<TypeCode>(UintWeights, yCode);
                    t = i > j ? xCode : yCode;
                }
                else
                {
                    if (tX.Name[0] == 'U')
                    {
                        xCode = tX.ToTypeCode();
                        t = ChoiceBestType(xCode, yCode);
                    }
                    else if (tY.Name[0] == 'U')
                    {
                        yCode = tY.ToTypeCode();
                        t = ChoiceBestType(xCode, yCode);
                    }
                    else
                    {
                        i = Array.IndexOf<TypeCode>(NumberWeights, xCode);
                        j = Array.IndexOf<TypeCode>(NumberWeights, yCode);
                        t = i > j ? xCode : yCode;
                    }
                }
            }
            if (t == TypeCode.Byte || t == TypeCode.SByte)
            {
                t = TypeCode.Int32;
            }
            switch (t)
            {
                case TypeCode.Double:
                    return CalculateBinaryNumber(x.ToRequiredDouble(0), y.ToRequiredDouble(0), value);
                case TypeCode.Int16:
                    return CalculateBinaryNumber(x.ToRequiredShort(0), y.ToRequiredShort(0), value);
                case TypeCode.Int32:
                    return CalculateBinaryNumber(x.ToRequiredInt(0), y.ToRequiredInt(0), value);
                case TypeCode.Int64:
                    return CalculateBinaryNumber(x.ToRequiredLong(0), y.ToRequiredLong(0), value);
                case TypeCode.UInt32:
                    return CalculateBinaryNumber(x.ToRequiredUInt(0), y.ToRequiredUInt(0), value);
                case TypeCode.UInt16:
                    return CalculateBinaryNumber(x.ToRequiredUShort(0), y.ToRequiredUShort(0), value);
                case TypeCode.UInt64:
                    return CalculateBinaryNumber(x.ToRequiredULong(0), y.ToRequiredULong(0), value);
                case TypeCode.Single:
                    return CalculateBinaryNumber(x.ToRequiredFloat(0), y.ToRequiredFloat(0), value);
                case TypeCode.Decimal:
                    return CalculateNumber(x.ToRequiredDecimal(0), y.ToRequiredDecimal(0), value);
                default:
                    return null;
            }
        }

        if (xCode == TypeCode.Boolean && yCode == TypeCode.Boolean)
            return Calculate(x.ToRequiredBoolean(false), y.ToRequiredBoolean(false), value);
        if (xCode == TypeCode.String && yCode == TypeCode.String)
            return Calculate(x?.ToString()!, y?.ToString()!, value);
        if (xCode == TypeCode.DateTime && yCode == TypeCode.DateTime)
            return Calculate((DateTime?)x ?? DateTime.MinValue, (DateTime?)y ?? DateTime.MinValue, value);

        switch (value)
        {
            case "==":
                return Equals(x, y, tX, tY);
            case "!=":
                return !Equals(x, y, tX, tY);
            case "+":
                return $"{x?.ToString()}{y?.ToString()}";
            case ">=":
            case ">":
            case "<=":
            case "<":
                if (x != null && y != null)
                {
                    string? strX = x.ToString();
                    string? strY = y.ToString();
                    float fx, fy;
                    if (!string.IsNullOrEmpty(strX)
                        && !string.IsNullOrEmpty(strY)
                        && float.TryParse(strX, out fx)
                        && float.TryParse(strY, out fy))
                    {
                        return Calculate(fx, fy, value);
                    }
                }
                if (value.Length > 1)
                {
                    return Equals(x, y, tX, tY);
                }
                return false;
            default:
                throw new TemplateException($"Operator \"{value}\" can not be applied operand \"object\" and \"object\"");
        }
    }
    /// <summary>
    /// Evaluating reverse polish notation
    /// </summary>
    /// <param name="value">The reverse polish notation.</param>
    /// <returns></returns>
    public static object? Calculate(Stack<object?> value)
    {
        var post = new Stack<object?>();
        while (value.Count > 0)
        {
            post.Push(value.Pop());
        }
        var stack = new Stack<object?>();

        while (post.Count > 0)
        {
            var obj = post.Pop();
            if (obj is Operator o)
            {
                var y = stack.Pop();
                var x = stack.Pop();
                stack.Push(Calculate(x, y, OperatorConvert.ToString(o)) ?? throw new NotSupportedException());
            }
            else
            {
                stack.Push(obj);
            }
        }

        return stack.Pop();
    }

    /// <summary>
    /// Computational mathematical expression.
    /// </summary>
    /// <param name="value">The expression.</param>
    /// <returns></returns>
    public static object? Calculate(object?[] value)
    {
        var stack = ProcessExpression(value);

        return Calculate(stack);
    }

    /// <summary>
    /// Computational mathematical expression.
    /// </summary>
    /// <param name="value">The expression.</param>
    /// <returns></returns>
    public static object? Calculate(string value)
    {
        var stack = ProcessExpression(value);

        return Calculate(stack);
    }

    #endregion

    #region  Calculate
    /// <summary>
    /// Calculates the value of objects
    /// </summary>
    /// <param name="x">The first value. </param>
    /// <param name="y">The second value.</param>
    /// <param name="value">The operator.</param> 
    /// <returns>The result of expression.</returns>
    public static object Calculate(bool x, bool y, string value)
    {
        switch (value)
        {
            case "==":
                return x == y;
            case "!=":
                return x != y;
            case "||":
                return x || y;
            case "&&":
                return x && y;
            default:
                throw new TemplateException($"Operator \"{value}\" can not be applied operand \"bool\" and \"bool\"");
        }
    }

    /// <summary>
    /// Calculates the value of objects
    /// </summary>
    /// <param name="x">The first value. </param>
    /// <param name="y">The second value.</param>
    /// <param name="value">The operator.</param> 
    /// <returns>The result of expression.</returns>
    public static object Calculate(string x, string y, string value)
    {
        switch (value)
        {
            case "==":
                return x.Equals(y, StringComparison.OrdinalIgnoreCase);
            case "!=":
                return !x.Equals(y, StringComparison.OrdinalIgnoreCase);
            case "+":
                return $"{x}{y}";
            default:
                throw new TemplateException($"Operator \"{value}\" can not be applied operand \"string\" and \"string\"");
        }
    }

    /// <summary>
    /// Calculates the value of objects
    /// </summary>
    /// <param name="x">The first value. </param>
    /// <param name="y">The second value.</param>
    /// <param name="value">The operator.</param> 
    /// <returns>The result of expression.</returns>
    public static object Calculate(DateTime x, DateTime y, string value)
    {
        switch (value)
        {
            case "==":
                return x == y;
            case "!=":
                return x != y;
            case ">":
                return x > y;
            case ">=":
                return x >= y;
            case "<":
                return x < y;
            case "<=":
                return x <= y;
            default:
                throw new TemplateException($"Operator \"{value}\" can not be applied operand \"DateTime\" and \"DateTime\"");
        }
    }
#if NFW || NT5 || NT6 || NC || NS
    /// <summary>
    /// Calculates the value of objects
    /// </summary>
    /// <param name="x">The first value. </param>
    /// <param name="y">The second value.</param>
    /// <param name="value">The operator.</param> 
    /// <returns>The result of expression.</returns>
    public static object CalculateNumber<T>(T x, T y, string value) where T : struct
        => NumericOps<T>.Calculate(x, y, value);
    /// <summary>
    /// Calculates the value of objects
    /// </summary>
    /// <param name="x">The first value. </param>
    /// <param name="y">The second value.</param>
    /// <param name="value">The operator.</param> 
    /// <returns>The result of expression.</returns>
    public static object CalculateBinaryNumber<T>(T x, T y, string value) where T : struct
                    => NumericOps<T>.CalculateBinary(x, y, value);
#else
    /// <summary>
    /// Calculates the value of objects
    /// </summary>
    /// <param name="x">The first value. </param>
    /// <param name="y">The second value.</param>
    /// <param name="value">The operator.</param> 
    /// <returns>The result of expression.</returns>
    public static object CalculateNumber<T>(T x, T y, string value) where T : struct, System.Numerics.INumber<T>
    {
        switch (value)
        {
            case "+":
                return x + y;
            case "-":
                return x - y;
            case "*":
                return x * y;
            case "/":
                return x / y;
            case "%":
                return x % y;
            case ">=":
                return x >= y;
            case "<=":
                return x <= y;
            case "<":
                return x < y;
            case ">":
                return x > y;
            case "==":
                return x == y;
            case "!=":
                return x != y;
            default:
                {
                    var name = typeof(T).Name;
                    throw new TemplateException($"Operator \"{value}\" can not be applied operand \"{name}\" and \"{name}\"");
                }
        }
    }

    /// <summary>
    /// Calculates the value of objects
    /// </summary>
    /// <param name="x">The first value. </param>
    /// <param name="y">The second value.</param>
    /// <param name="value">The operator.</param> 
    /// <returns>The result of expression.</returns>
    public static object CalculateBinaryNumber<T>(T x, T y, string value) where T : struct, System.Numerics.IBinaryNumber<T>
    {
        switch (value)
        {
            case "|":
                return x | y;
            case "&":
                return x & y;
            default:
                return CalculateNumber<T>(x, y, value);
        }
    }
#endif



    /// <summary>
    /// 
    /// </summary>
    /// <param name="xCode"></param>
    /// <param name="yCode"></param>
    /// <returns></returns>
    private static TypeCode ChoiceBestType(TypeCode xCode, TypeCode yCode)
    {
        switch (xCode)
        {
            case TypeCode.UInt16:
                xCode = TypeCode.Int32;
                break;
            case TypeCode.UInt32:
                xCode = TypeCode.Int64;
                break;
            case TypeCode.UInt64:
                xCode = TypeCode.Int64;
                break;
            default:
                if (yCode == TypeCode.UInt16 || yCode == TypeCode.UInt32 || yCode == TypeCode.UInt64)
                    return ChoiceBestType(yCode, xCode);
                throw new NotSupportedException();
        }
        var i = Array.IndexOf<TypeCode>(NumberWeights, xCode);
        var j = Array.IndexOf<TypeCode>(NumberWeights, yCode);
        return i > j ? xCode : yCode;

    }
    #endregion

}