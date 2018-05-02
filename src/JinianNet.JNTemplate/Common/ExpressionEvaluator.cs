/********************************************************************************
 Copyright (c) jiniannet (http://www.jiniannet.com). All rights reserved.
 Licensed under the MIT license. See licence.txt file in the project root for full license information.
 ********************************************************************************/
using System;
using System.Collections.Generic;

namespace JinianNet.JNTemplate.Common
{

    /// <summary>
    /// 计算器
    /// </summary>
    public class ExpressionEvaluator
    {
        #region Weights Array
        private static readonly String[] _numberWeights = new String[] {
                "System.Int16",
                 "System.Int32",
                 "System.Int64",
                 "System.Single",
                 "System.Double",
                 "System.Decimal"};

        private static readonly String[] _uintWeights = new String[] {
                "System.UInt16",
                 "System.UInt32",
                 "System.UInt64"};
        #endregion

        #region LetterType
        /// <summary>
        /// 字符类型
        /// </summary>
        public enum LetterType
        {
            /// <summary>
            /// 无
            /// </summary>
            None = 0,
            /// <summary>
            /// 操作符
            /// </summary>
            Operator = 1,
            /// <summary>
            /// 左圆括号
            /// </summary>
            LeftParentheses = 2,
            /// <summary>
            /// 右中括号
            /// </summary>
            RightParentheses = 3,
            /// <summary>
            /// 数字
            /// </summary>
            Number = 4,
            /// <summary>
            /// 其它
            /// </summary>
            Other = 5
        }
        #endregion

        #region common function
        private static Boolean IsOperator(String value)
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

        private static Int32 GetPriority(String c)
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

                case "Plus":
                case "Minus":
                    return 7;
                case "%":
                case "*":
                case "Percent":
                case "Times":
                case "/":
                case "Divided":
                    return 8;
                default:
                    return 9;
            }
        }

        private static Boolean IsNumber(String fullName)
        {
            switch (fullName)
            {
                case "System.Double":
                case "System.Int16":
                case "System.Int32":
                case "System.Int64":
                case "System.UInt16":
                case "System.UInt32":
                case "System.UInt64":
                case "System.Single":
                case "System.Decimal":
                    return true;
                default:
                    return false;
            }
        }
        #endregion

        #region ProcessExpression
        /// <summary>
        /// 处理表达式
        /// </summary>
        /// <param name="value">表达式</param>
        /// <returns></returns>
        public static Stack<Object> ProcessExpression(String value)
        {
            value = value.Replace("  ", String.Empty);
            List<Object> result = new List<Object>();
            Int32 j = 0;
            Int32 i;
            String num;

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
                                result.Add(Int32.Parse(value.Substring(j, i - j)));
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
            return ProcessExpression(result.ToArray());

        }

        /// <summary>
        /// 处理表达式
        /// </summary>
        /// <param name="value">表达式</param>
        /// <returns></returns>
        public static Stack<Object> ProcessExpression(Object[] value)
        {
            Stack<Object> post = new Stack<Object>();
            Stack<Object> stack = new Stack<Object>();

            for (Int32 i = 0; i < value.Length; i++)
            {
                String fullName;
                if (value[i] != null)
                {
                    fullName = value[i].GetType().FullName;
                }
                else
                {
                    fullName = "System.Object";
                    value[i] = null;
                }
                if (fullName != "JinianNet.JNTemplate.Operator")
                {
                    post.Push(value[i]);
                    continue;
                }
                switch (value[i].ToString())
                {
                    case "(":
                    case "LeftParentheses":
                        stack.Push("(");
                        break;
                    case ")":
                    case "RightParentheses":
                        while (stack.Count > 0)
                        {
                            object op;
                            if ((op = stack.Pop()).ToString() == "(")
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
                    case "Plus":
                    case "Minus":
                    case "Times":
                    case "Percent":
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
                            Object eX = stack.Peek();
                            Object eY = value[i];
                            if (GetPriority(eY.ToString()) > GetPriority(eX.ToString()))
                            {
                                stack.Push(eY);
                            }
                            else
                            {

                                if (eX.ToString() != "(")
                                {
                                    post.Push(stack.Pop());
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
        /// 获取类型
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        private static Type GetType(Object value)
        {
            if (value == null)
            {
                return typeof(Object);
            }
            return value.GetType();
        }
        /// <summary>
        /// 计算结果
        /// </summary>
        /// <param name="x">值一</param>
        /// <param name="y">值二</param>
        /// <param name="value">操作符</param>
        /// <returns></returns>
        public static Object Calculate(Object x, Object y, String value)
        {
            if (value == "||")
            {
                return CalculateOr(x, y, value);
            }

            if (value == "&&")
            {
                return CalculateAnd(x, y, value);
            }

            Type tX = GetType(x);
            Type tY = GetType(y); ;

            if (IsNumber(tX.FullName) && IsNumber(tY.FullName))
            {
                Type t;
                if (tX == tY)
                {
                    t = tX;
                }
                else
                {
                    Int32 i, j;
                    if (tX.Name[0] == 'U' && tY.Name[0] == 'U')
                    {
                        i = Array.IndexOf<String>(_uintWeights, tX.FullName);
                        j = Array.IndexOf<String>(_uintWeights, tY.FullName);
                    }
                    else
                    {
                        if (tX.Name[0] == 'U')
                        {
                            tX = Type.GetType(String.Concat("System.", tX.Name.Remove(0, 1)));
                        }

                        if (tY.Name[0] == 'U')
                        {
                            tY = Type.GetType(String.Concat("System.", tY.Name.Remove(0, 1)));
                        }

                        i = Array.IndexOf<String>(_numberWeights, tX.FullName);
                        j = Array.IndexOf<String>(_numberWeights, tY.FullName);
                    }
                    if (i > j)
                    {
                        t = tX;
                    }
                    else
                    {
                        t = tY;
                    }
                }
                switch (t.FullName)
                {
                    case "System.Double":
                        return Calculate(Convert.ToDouble(x.ToString()), Convert.ToDouble(y.ToString()), value);
                    case "System.Int16":
                        return Calculate(Convert.ToInt16(x.ToString()), Convert.ToInt16(y.ToString()), value);
                    case "System.Int32":
                        return Calculate(Convert.ToInt32(x.ToString()), Convert.ToInt32(y.ToString()), value);
                    case "System.Int64":
                        return Calculate(Convert.ToInt64(x.ToString()), Convert.ToInt64(y.ToString()), value);
                    /*
                    case "System.UInt16":
                        return Calculate(Convert.ToUInt16(x.ToString()), Convert.ToUInt16(y.ToString()), value);
                    case "System.UInt32":
                        return Calculate(Convert.ToUInt32(x.ToString()), Convert.ToUInt32(y.ToString()), value);
                    case "System.UInt64":
                        return Calculate(Convert.ToUInt64(x.ToString()), Convert.ToUInt64(y.ToString()), value);
                    */
                    case "System.Single":
                        return Calculate(Convert.ToSingle(x.ToString()), Convert.ToSingle(y.ToString()), value);
                    case "System.Decimal":
                        return Calculate(Convert.ToDecimal(x.ToString()), Convert.ToDecimal(y.ToString()), value);
                    default:
                        return null;
                }
            }

            if (tX.FullName == "System.Boolean" && tY.FullName == "System.Boolean")
                return Calculate((Boolean)x, (Boolean)y, value);
            if (tX.FullName == "System.String" && tY.FullName == "System.String")
                return Calculate(x.ToString(), y.ToString(), value);
            if (tX.FullName == "System.DateTime" && tY.FullName == "System.DateTime")
                return Calculate((DateTime)x, (DateTime)y, value);

            switch (value)
            {
                case "==":
                    return Equals(x, y, tX, tY);
                case "!=":
                    return !Equals(x, y, tX, tY);
                case "+":
                    return String.Concat(x.ToString(), y.ToString());
                case ">=":
                case ">":
                case "<=":
                case "<":
                    if (x != null && y != null)
                    {
                        string strX, strY;
                        Single fx, fy;
                        if (!String.IsNullOrEmpty(strX = x.ToString())
                            && !String.IsNullOrEmpty(strY = y.ToString())
                            && Single.TryParse(strX, out fx)
                            && Single.TryParse(strY, out fy))
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
                    throw new Exception.TemplateException(String.Concat("Operator \"", value, "\" can not be applied operand \"Object\" and \"Object\""));
            }
        }

        private static Object CalculateAnd(Object x, Object y, String value)
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

        private static Object CalculateOr(Object x, Object y, String value)
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

        internal static Boolean CalculateBoolean(Object value)
        {
            if (value == null)
                return false;
            switch (value.GetType().FullName)
            {
                case "System.Boolean":
                    return (Boolean)value;
                case "System.String":
                    return !String.IsNullOrEmpty(value.ToString());
                case "System.UInt16":
                case "System.UInt32":
                case "System.UInt64":
                case "System.Int16":
                case "System.Int32":
                case "System.Int64":
                    return value.ToString() != "0";
                case "System.Decimal":
                    return (Decimal)value != 0;
                case "System.Double":
                    return (Double)value != 0;
                case "System.Single":
                    return (Single)value != 0;
                default:
                    return value != null;
            }
        }

        private static Boolean Equals(Object x, Object y, Type tX, Type tY)
        {
            if (x == null || x == null)
            {
                return x == null && y == null;
            }
            if (tX.FullName == tX.FullName)
            {
                //if(tX.FullName)
                return x == y;
            }
            return false;
        }

        /// <summary>
        /// 计算后缀表达式
        /// </summary>
        /// <param name="value">后缀表达式</param>
        /// <returns></returns>
        public static Object Calculate(Stack<Object> value)
        {
            Stack<Object> post = new Stack<Object>();
            while (value.Count > 0)
            {
                post.Push(value.Pop());
            }
            Stack<Object> stack = new Stack<Object>();

            while (post.Count > 0)
            {
                Object obj = post.Pop();
                if (obj != null && obj.GetType().FullName == "JinianNet.JNTemplate.Operator")
                {
                    Object y = stack.Pop();
                    Object x = stack.Pop();
                    stack.Push(Calculate(x, y, OperatorConvert.ToString((Operator)obj)));
                }
                else
                {
                    stack.Push(obj);
                }
            }

            return stack.Pop();
        }

        /// <summary>
        /// 计算表达式
        /// </summary>
        /// <param name="value">表达式</param>
        /// <returns></returns>
        public static Object Calculate(Object[] value)
        {
            Stack<Object> stack = ProcessExpression(value);

            return Calculate(stack);
        }

        /// <summary>
        /// 计算表达式
        /// </summary>
        /// <param name="value">表达式</param>
        /// <returns></returns>
        public static Object Calculate(String value)
        {
            Stack<Object> stack = ProcessExpression(value);

            return Calculate(stack);
        }

        #endregion

        #region  Calculate

        /// <summary>
        /// 计算结果
        /// </summary>
        /// <param name="x">值一</param>
        /// <param name="y">值二</param>
        /// <param name="value">操作符</param>
        /// <returns></returns>
        public static Object Calculate(Boolean x, Boolean y, String value)
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
                    throw new Exception.TemplateException(String.Concat("Operator \"", value, "\" can not be applied operand \"Boolean\" and \"Boolean\""));
            }
        }
        /// <summary>
        /// 计算结果
        /// </summary>
        /// <param name="x">值一</param>
        /// <param name="y">值二</param>
        /// <param name="value">操作符</param>
        /// <returns></returns>
        public static Object Calculate(String x, String y, String value)
        {
            switch (value)
            {
                case "==":
                    return x.Equals(y, Engine.Runtime.ComparisonIgnoreCase);
                case "!=":
                    return !x.Equals(y, Engine.Runtime.ComparisonIgnoreCase);
                case "+":
                    return String.Concat(x, y);
                default:
                    throw new Exception.TemplateException(String.Concat("Operator \"", value, "\" can not be applied operand \"String\" and \"String\""));
            }
        }

        /// <summary>
        /// 计算结果
        /// </summary>
        /// <param name="x">值一</param>
        /// <param name="y">值二</param>
        /// <param name="value">操作符</param>
        /// <returns></returns>
        public static Object Calculate(DateTime x, DateTime y, String value)
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
                    throw new Exception.TemplateException(String.Concat("Operator \"", value, "\" can not be applied operand \"DateTime\" and \"DateTime\""));
            }
        }
        /// <summary>
        /// 计算结果
        /// </summary>
        /// <param name="x">值一</param>
        /// <param name="y">值二</param>
        /// <param name="value">操作符</param>
        /// <returns></returns>
        public static Object Calculate(Double x, Double y, String value)
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
                    throw new Exception.TemplateException(String.Concat("Operator \"", value, "\" can not be applied operand \"Double\" and \"Double\""));
            }
        }

        /// <summary>
        /// 计算结果
        /// </summary>
        /// <param name="x">值一</param>
        /// <param name="y">值二</param>
        /// <param name="value">操作符</param>
        /// <returns></returns>
        public static Object Calculate(Single x, Single y, String value)
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
                    throw new Exception.TemplateException(String.Concat("Operator \"", value, "\" can not be applied operand \"Single\" and \"Single\""));
            }
        }

        /// <summary>
        /// 计算结果
        /// </summary>
        /// <param name="x">值一</param>
        /// <param name="y">值二</param>
        /// <param name="value">操作符</param>
        /// <returns></returns>
        public static Object Calculate(Decimal x, Decimal y, String value)
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
                    throw new Exception.TemplateException(String.Concat("Operator \"", value, "\" can not be applied operand \"Decimal\" and \"Decimal\""));
            }
        }


        /// <summary>
        /// 计算结果
        /// </summary>
        /// <param name="x">值一</param>
        /// <param name="y">值二</param>
        /// <param name="value">操作符</param>
        /// <returns></returns>
        public static Object Calculate(Int32 x, Int32 y, String value)
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
                case "|":
                    return x | y;
                case "&":
                    return x & y;

                default:
                    throw new Exception.TemplateException(String.Concat("Operator \"", value, "\" can not be applied operand \"Int32\" and \"Int32\""));
            }
        }

        /// <summary>
        /// 计算结果
        /// </summary>
        /// <param name="x">值一</param>
        /// <param name="y">值二</param>
        /// <param name="value">操作符</param>
        /// <returns></returns>
        public static Object Calculate(Int64 x, Int64 y, String value)
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
                case "|":
                    return x | y;
                case "&":
                    return x & y;

                default:
                    throw new Exception.TemplateException(String.Concat("Operator \"", value, "\" can not be applied operand \"Int64\" and \"Int64\""));
            }
        }

        /// <summary>
        /// 计算结果
        /// </summary>
        /// <param name="x">值一</param>
        /// <param name="y">值二</param>
        /// <param name="value">操作符</param>
        /// <returns></returns>
        public static Object Calculate(Int16 x, Int16 y, String value)
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
                case "|":
                    return x | y;
                case "&":
                    return x & y;

                default:
                    throw new Exception.TemplateException(String.Concat("Operator \"", value, "\" can not be applied operand \"Int16\" and \"Int16\""));
            }
        }
        #region 以下参数类型因不符合CLS，故取消
        /*
        /// <summary>
        /// 计算结果
        /// </summary>
        /// <param name="x">值一</param>
        /// <param name="y">值二</param>
        /// <param name="value">操作符</param>
        /// <returns></returns>
        public static Object Calculate(UInt32 x, UInt32 y, String value)
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
                    throw new Exception.TemplateException(String.Concat("Operator \"", value, "\" can not be applied operand \"UInt32\" and \"UInt32\""));
            }
        }

        /// <summary>
        /// 计算结果
        /// </summary>
        /// <param name="x">值一</param>
        /// <param name="y">值二</param>
        /// <param name="value">操作符</param>
        /// <returns></returns>
        public static Object Calculate(UInt64 x, UInt64 y, String value)
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
                    throw new Exception.TemplateException(String.Concat("Operator \"", value, "\" can not be applied operand \"UInt64\" and \"UInt64\""));
            }
        }

        /// <summary>
        /// 计算结果
        /// </summary>
        /// <param name="x">值一</param>
        /// <param name="y">值二</param>
        /// <param name="value">操作符</param>
        /// <returns></returns>
        public static Object Calculate(UInt16 x, UInt16 y, String value)
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
                    throw new Exception.TemplateException(String.Concat("Operator \"", value, "\" can not be applied operand \"UInt16\" and \"UInt16\""));
            }
        }

        */
        #endregion
        #endregion

    }
}