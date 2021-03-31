/********************************************************************************
 Copyright (c) jiniannet (http://www.jiniannet.com). All rights reserved.
 Licensed under the MIT license. See licence.txt file in the project root for full license information.
 ********************************************************************************/
using System;

namespace JinianNet.JNTemplate.Nodes
{
    /// <summary>
    /// LogicTag
    /// </summary>
    [Serializable]
    public class LogicTag : BasisTag
    {

        ///// <summary>
        ///// 
        ///// </summary>
        ///// <param name="list">list</param>
        ///// <param name="isOperator"></param>
        ///// <param name="value"></param>
        ///// <returns></returns>
        //private bool Eval(List<object> list, bool isOperator, object value)
        //{
        //    if (isOperator)
        //    {
        //        Operator op = (Operator)value;
        //        if (op == Operator.Or || op == Operator.And)
        //        {
        //            object result;
        //            bool isTrue;
        //            if (list.Count > 1)
        //            {
        //                var stack = ExpressionEvaluator.ProcessExpression(list.ToArray());
        //                result = ExpressionEvaluator.Calculate(stack);
        //            }
        //            else
        //            {
        //                result = list[0];
        //            }
        //            if (result is bool)
        //            {
        //                isTrue = (bool)result;
        //            }
        //            else
        //            {
        //                isTrue = ExpressionEvaluator.CalculateBoolean(result);
        //            }
        //            if (op == Operator.Or && isTrue)
        //            {
        //                list.Add(true);
        //                return true;
        //            }
        //            if (op == Operator.And && !isTrue)
        //            {
        //                list.Add(false);
        //                return true;
        //            }
        //            list.Clear();
        //            list.Add(isTrue);
        //        }
        //        list.Add(op);
        //    }
        //    else
        //    {
        //        list.Add(value);
        //    }
        //    return false;
        //}
    }
}