/********************************************************************************
 Copyright (c) jiniannet (http://www.jiniannet.com). All rights reserved.
 Licensed under the MIT license. See licence.txt file in the project root for full license information.
 ********************************************************************************/
using System;

namespace JinianNet.JNTemplate.Common
{
    /// <summary>
    /// 操作符处理类
    /// </summary>
    public class OperatorConvert
    {
        /// <summary>
        /// 将枚举的操作符转换为字符串形式
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        public static String ToString(Operator value)
        {
            switch (value)
            {
                case Operator.Plus:
                    return "+";
                case Operator.Minus:
                    return "-";
                case Operator.Times:
                    return "*";
                case Operator.Percent:
                    return "%";
                case Operator.Divided:
                    return "/";
                case Operator.LogicalOr:
                    return "|";
                case Operator.Or:
                    return "||";
                case Operator.LogicAnd:
                    return "&";
                case Operator.And:
                    return "&&";
                case Operator.GreaterThan:
                    return ">";
                case Operator.GreaterThanOrEqual:
                    return ">=";
                case Operator.LessThan:
                    return "<";
                case Operator.LessThanOrEqual:
                    return "<=";
                case Operator.Equal:
                    return "==";
                case Operator.NotEqual:
                    return "!=";
                case Operator.LeftParentheses:
                    return "(";
                case Operator.RightParentheses:
                    return ")";
                default:
                    return String.Empty;
            }
        }

        /// <summary>
        /// 将操作符转换为枚举形式
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        public static Operator Parse(String value)
        {
            switch (value)
            {
                case "+":
                    return Operator.Plus;
                case "-":
                    return Operator.Minus;
                case "*":
                    return Operator.Times;
                case "%":
                    return Operator.Percent;
                case "/":
                    return Operator.Divided;
                case "|":
                    return Operator.LogicalOr;
                case "||":
                    return Operator.Or;
                case "&":
                    return Operator.LogicAnd;
                case "&&":
                    return Operator.And;
                case ">":
                    return Operator.GreaterThan;
                case ">=":
                    return Operator.GreaterThanOrEqual;
                case "<":
                    return Operator.LessThan;
                case "<=":
                    return Operator.LessThanOrEqual;
                case "==":
                    return Operator.Equal;
                case "!=":
                    return Operator.NotEqual;
                case "(":
                    return Operator.LeftParentheses;
                case ")":
                    return Operator.RightParentheses;
                default:
                    return Operator.None;
            }
        }
    }
}