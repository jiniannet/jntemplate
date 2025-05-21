/********************************************************************************
 Copyright (c) jiniannet (http://www.jiniannet.com). All rights reserved.
 Licensed under the MIT license. See licence.txt file in the project root for full license information.
 ********************************************************************************/
using System;
using System.Collections.Generic;
using JinianNet.JNTemplate.Exceptions;
using System.Collections.ObjectModel;

namespace JinianNet.JNTemplate.Dynamic
{

    /// <summary>
    /// Expression calculation
    /// </summary>
    public class ExpressionEvaluator
    {
        #region Weights Array
        private static readonly string[] numberWeights = new string[] {
                "System.Int16",
                 "System.Int32",
                 "System.Int64",
                 "System.Single",
                 "System.Double",
                 "System.Decimal"};

        private static readonly string[] uintWeights = new string[] {
                "System.UInt16",
                 "System.UInt32",
                 "System.UInt64"};
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

        private static int GetPriority(string c)
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

        private static bool IsNumber(string fullName)
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
        /// process expression.
        /// </summary>
        /// <param name="value">The expression.</param>
        /// <returns></returns>
        public static Stack<object> ProcessExpression(string value)
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
            return ProcessExpression(result.ToArray());

        }

        /// <summary>
        /// process expression.
        /// </summary>
        /// <param name="value">The expression.</param>
        /// <returns></returns>
        public static Stack<object> ProcessExpression(object[] value)
        {
            Stack<object> post = new Stack<object>();
            Stack<object> stack = new Stack<object>();

            for (int i = 0; i < value.Length; i++)
            {
                string fullName;
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
                            //object eX = stack.Peek();
                            //object eY = value[i];
                            //if (GetPriority(eY.ToString()) > GetPriority(eX.ToString()))
                            //{
                            //    stack.Push(eY);
                            //}
                            //else
                            //{

                            //    if (eX.ToString() != "(")
                            //    {
                            //        post.Push(stack.Pop());
                            //    }
                            //    stack.Push(eY);
                            //}

                            object eX = stack.Peek();
                            object eY = value[i];
                            int pY = GetPriority(eY.ToString());
                            int pX = GetPriority(eX.ToString());
                            /*
                            if (eX.ToString() == "(" && eY.ToString() != ")")
                            {
                                stack.Push(eY);
                            }
                            else if (eY.ToString() == ")")
                            {
                                while (eX.ToString() != "(")
                                {
                                    post.Push(stack.Pop());
                                }
                                stack.Pop();
                            }
                            else */
                            
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

                                if (eX.ToString() != "(")
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
        /// <summary>
        /// Calculates the value of objects
        /// </summary>
        /// <param name="x">The first value. </param>
        /// <param name="y">The second value.</param>
        /// <param name="value">The result of expression.</param>
        /// <returns></returns>
        public static object Calculate(object x, object y, string value)
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
                    int i, j;
                    if (tX.Name[0] == 'U' && tY.Name[0] == 'U')
                    {
                        i = Array.IndexOf<string>(uintWeights, tX.FullName);
                        j = Array.IndexOf<string>(uintWeights, tY.FullName);
                    }
                    else
                    {
                        if (tX.Name[0] == 'U')
                        {
                            tX = Type.GetType($"System.{tX.Name.Remove(0, 1)}");
                        }

                        if (tY.Name[0] == 'U')
                        {
                            tY = Type.GetType($"System.{tY.Name.Remove(0, 1)}");
                        }

                        i = Array.IndexOf<string>(numberWeights, tX.FullName);
                        j = Array.IndexOf<string>(numberWeights, tY.FullName);
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
                return Calculate((bool)x, (bool)y, value);
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
                    return $"{x.ToString()}{y.ToString()}";
                case ">=":
                case ">":
                case "<=":
                case "<":
                    if (x != null && y != null)
                    {
                        string strX, strY;
                        Single fx, fy;
                        if (!string.IsNullOrEmpty(strX = x.ToString())
                            && !string.IsNullOrEmpty(strY = y.ToString())
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
                    throw new TemplateException($"Operator \"{value}\" can not be applied operand \"object\" and \"object\"");
            }
        }

        private static object CalculateAnd(object x, object y, string value)
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

        private static object CalculateOr(object x, object y, string value)
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

        internal static bool CalculateBoolean(object value)
        {
            if (value == null)
                return false;
            switch (value.GetType().FullName)
            {
                case "System.Boolean":
                    return (bool)value;
                case "System.String":
                    return !string.IsNullOrEmpty(value.ToString());
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

        private static bool Equals(object x, object y, Type tX, Type tY)
        {
            if (x == null || y == null)
            {
                return x == null && y == null;
            }
            if (tX.FullName == tX.FullName)
            {
                if (tX.IsEnum)
                {
                    return Equals(x,y);
                }
                return x == y;
            }
            return false;
        }

        /// <summary>
        /// Evaluating reverse polish notation
        /// </summary>
        /// <param name="value">The reverse polish notation.</param>
        /// <returns></returns>
        public static object Calculate(Stack<object> value)
        {
            Stack<object> post = new Stack<object>();
            while (value.Count > 0)
            {
                post.Push(value.Pop());
            }
            Stack<object> stack = new Stack<object>();

            while (post.Count > 0)
            {
                object obj = post.Pop();
                if (obj != null && obj is Operator o)
                {
                    object y = stack.Pop();
                    object x = stack.Pop();
                    stack.Push(Calculate(x, y, OperatorConvert.ToString(o)));
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
        public static object Calculate(object[] value)
        {
            Stack<object> stack = ProcessExpression(value);

            return Calculate(stack);
        }

        /// <summary>
        /// Computational mathematical expression.
        /// </summary>
        /// <param name="value">The expression.</param>
        /// <returns></returns>
        public static object Calculate(string value)
        {
            Stack<object> stack = ProcessExpression(value);

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

        /// <summary>
        /// Calculates the value of objects
        /// </summary>
        /// <param name="x">The first value. </param>
        /// <param name="y">The second value.</param>
        /// <param name="value">The operator.</param> 
        /// <returns>The result of expression.</returns>
        public static object Calculate(Double x, Double y, string value)
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
                    throw new TemplateException($"Operator \"{value}\" can not be applied operand \"Double\" and \"Double\"");
            }
        }

        /// <summary>
        /// Calculates the value of objects
        /// </summary>
        /// <param name="x">The first value. </param>
        /// <param name="y">The second value.</param>
        /// <param name="value">The operator.</param> 
        /// <returns>The result of expression.</returns>
        public static object Calculate(Single x, Single y, string value)
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
                    throw new TemplateException($"Operator \"{value}\" can not be applied operand \"Single\" and \"Single\"");
            }
        }

        /// <summary>
        /// Calculates the value of objects
        /// </summary>
        /// <param name="x">The first value. </param>
        /// <param name="y">The second value.</param>
        /// <param name="value">The operator.</param> 
        /// <returns>The result of expression.</returns>
        public static object Calculate(Decimal x, Decimal y, string value)
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
                    throw new TemplateException($"Operator \"{value}\" can not be applied operand \"Decimal\" and \"Decimal\"");
            }
        }

        /// <summary>
        /// Calculates the value of objects
        /// </summary>
        /// <param name="x">The first value. </param>
        /// <param name="y">The second value.</param>
        /// <param name="value">The operator.</param> 
        /// <returns>The result of expression.</returns>
        public static object Calculate(int x, int y, string value)
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
                    throw new TemplateException($"Operator \"{value}\" can not be applied operand \"int\" and \"int\"");
            }
        }

        /// <summary>
        /// Calculates the value of objects
        /// </summary>
        /// <param name="x">The first value. </param>
        /// <param name="y">The second value.</param>
        /// <param name="value">The operator.</param> 
        /// <returns>The result of expression.</returns>
        public static object Calculate(Int64 x, Int64 y, string value)
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
                    throw new TemplateException($"Operator \"{value}\" can not be applied operand \"Int64\" and \"Int64\"");
            }
        }

        /// <summary>
        /// Calculates the value of objects
        /// </summary>
        /// <param name="x">The first value. </param>
        /// <param name="y">The second value.</param>
        /// <param name="value">The operator.</param> 
        /// <returns>The result of expression.</returns>
        public static object Calculate(Int16 x, Int16 y, string value)
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
                    throw new TemplateException($"Operator \"{value}\" can not be applied operand \"Int16\" and \"Int16\"");
            }
        }
        #region 
        /*
        
        /// <summary>
        /// Calculates the value of objects
        /// </summary>
        /// <param name="x">The first value. </param>
        /// <param name="y">The second value.</param>
        /// <param name="value">The operator.</param> 
        /// <returns>The result of expression.</returns>
        public static object Calculate(UInt32 x, UInt32 y, string value)
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
                    throw new TemplateException(string.Concat("Operator \"", value, "\" can not be applied operand \"UInt32\" and \"UInt32\""));
            }
        }
        
        /// <summary>
        /// Calculates the value of objects
        /// </summary>
        /// <param name="x">The first value. </param>
        /// <param name="y">The second value.</param>
        /// <param name="value">The operator.</param> 
        /// <returns>The result of expression.</returns>
        public static object Calculate(UInt64 x, UInt64 y, string value)
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
                    throw new TemplateException(string.Concat("Operator \"", value, "\" can not be applied operand \"UInt64\" and \"UInt64\""));
            }
        }
        
        /// <summary>
        /// Calculates the value of objects
        /// </summary>
        /// <param name="x">The first value. </param>
        /// <param name="y">The second value.</param>
        /// <param name="value">The operator.</param> 
        /// <returns>The result of expression.</returns>
        public static object Calculate(UInt16 x, UInt16 y, string value)
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
                    throw new TemplateException(string.Concat("Operator \"", value, "\" can not be applied operand \"UInt16\" and \"UInt16\""));
            }
        }

        */
        #endregion
        #endregion

    }
}