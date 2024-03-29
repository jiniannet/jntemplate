/********************************************************************************
 Copyright (c) jiniannet (http://www.jiniannet.com). All rights reserved.
 Licensed under the MIT license. See licence.txt file in the project root for full license information.
 ********************************************************************************/
using System;

namespace JinianNet.JNTemplate.Dynamic
{
    /// <summary>
    ///The operator convert class.
    /// </summary>
    public class OperatorConvert
    {
        /// <summary>
        /// Converts the pperator to its equivalent string representation.
        /// </summary>
        /// <param name="value">The operator.</param>
        /// <returns>A string.</returns>
        public static string ToString(Operator value)
        {
            switch (value)
            {
                case Operator.Add:
                    return "+";
                case Operator.Subtract:
                    return "-";
                case Operator.Multiply:
                    return "*";
                case Operator.Remainder:
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
                    return string.Empty;
            }
        }

        /// <summary>
        /// Converts the string representation of a operator.
        /// </summary>
        /// <param name="value">A string containing a operator to convert.</param>
        /// <returns>A operator.</returns>
        public static Operator Parse(string value)
        {
            switch (value)
            {
                case "+":
                    return Operator.Add;
                case "-":
                    return Operator.Subtract;
                case "*":
                    return Operator.Multiply;
                case "%":
                    return Operator.Remainder;
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