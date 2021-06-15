/********************************************************************************
 Copyright (c) jiniannet (http://www.jiniannet.com). All rights reserved.
 Licensed under the MIT license. See licence.txt file in the project root for full license information.
 ********************************************************************************/
using System;
using System.Collections.Generic;
using System.Reflection;
using System.Reflection.Emit;
using System.Text;
using JinianNet.JNTemplate.CodeCompilation;
using JinianNet.JNTemplate.Dynamic;
using JinianNet.JNTemplate.Exceptions;
using JinianNet.JNTemplate.Nodes;

namespace JinianNet.JNTemplate.Parsers
{
    /// <summary>
    /// The <see cref="LogicTag"/> registrar
    /// </summary>
    public class LogicRegistrar : TagRegistrar<LogicTag>, IRegistrar
    {
        #region
        private static List<Operator> operators = new List<Operator>(new Operator[] { Operator.Equal, Operator.NotEqual, Operator.LessThan, Operator.LessThanOrEqual, Operator.GreaterThan, Operator.GreaterThanOrEqual });
        #endregion
        /// <inheritdoc />
        public override Func<TemplateParser, TokenCollection, ITag> BuildParseMethod()
        {
            return (parser, tc) =>
            {
                if (tc != null
                    && parser != null
                    && tc.Count > 2)
                {
                    var tcs = tc.Split(TokenKind.Logic);
                    if (tcs.Length <= 1)
                    {
                        return null;
                    }

                    var tags = new List<ITag>();
                    for (int i = 0; i < tcs.Length; i++)
                    {
                        if (tcs[i].Count == 1 && tcs[i][0].TokenKind == TokenKind.Logic)
                        {
                            tags.Add(new OperatorTag(tcs[i][0]));
                        }
                        else
                        {
                            tags.Add(parser.Read(tcs[i]));
                        }
                    }

                    if (tags.Count == 1)
                    {
                        return tags[0];
                    }

                    if (tags.Count > 1)
                    {
                        var list = new List<List<ITag>>();
                        ITag t = new LogicTag();
                        var arr = Analysis(tags, new List<Operator>(new Operator[] { Operator.And, Operator.Or }));
                        if (arr.Length == 1)
                        {
                            return arr[0];
                        }
                        AddRange(t, arr);
                        tags.Clear();
                        return t;
                    }

                }

                return null;
            };
        }

        /// <inheritdoc />
        public override Func<ITag, CompileContext, MethodInfo> BuildCompileMethod()
        {
            return (tag, c) =>
            {
                var t = tag as LogicTag;
                var type = c.GuessType(t);
                var mb = c.CreateReutrnMethod<LogicTag>(type);
                var il = mb.GetILGenerator();
                Label labelEnd = il.DefineLabel();
                il.DeclareLocal(type);
                var array = new object[t.Children.Count];
                var types = new List<Type>();
                var opts = new List<Operator>();
                var message = new List<string>();
                for (int i = 0; i < t.Children.Count; i++)
                {
                    var opt = t.Children[i] as OperatorTag;
                    if (opt != null)
                    {
                        if (!opts.Contains(opt.Value))
                        {
                            opts.Add(opt.Value);
                        }
                        array[i] = opt.Value;
                        message.Add(OperatorConvert.ToString(opt.Value));
                    }
                    else
                    {
                        array[i] = t.Children[i];
                        types.Add(c.GuessType(t.Children[i]));
                        message.Add(types[types.Count - 1].Name);
                    }
                }

                if (opts.Contains(Operator.Or) || opts.Contains(Operator.And))
                {

                    Label labelTrue = il.DefineLabel();
                    Label labelFalse = il.DefineLabel();
                    Operator pre = Operator.None;
                    for (int i = 0; i < t.Children.Count; i++)
                    {
                        var opt = t.Children[i] as OperatorTag;
                        if (opt != null)
                        {
                            pre = opt.Value;
                            continue;
                        }

                        var m = c.CompileTag(t.Children[i]);
                        il.Emit(OpCodes.Ldarg_0);
                        il.Emit(OpCodes.Ldarg_1);
                        il.Emit(OpCodes.Call, m);
                        if (m.ReturnType.Name != "Boolean")
                        {
                            var cm = typeof(Utility).GetMethodInfo("ToBoolean", new Type[] { m.ReturnType });
                            if (cm == null)
                            {
                                cm = typeof(Utility).GetMethodInfo("ToBoolean", new Type[] { typeof(object) });
                                if (m.ReturnType.IsValueType)
                                {
                                    il.Emit(OpCodes.Box, typeof(object));
                                }
                                else
                                {
                                    il.Emit(OpCodes.Castclass, typeof(object));
                                }
                            }
                            il.Emit(OpCodes.Call, cm);
                        }
                        //il.Emit(OpCodes.Stloc_0);
                        if (pre == Operator.None)
                        {
                            pre = (t.Children[i + 1] as OperatorTag).Value;
                        }

                        if (pre == Operator.Or)
                        {
                            il.Emit(OpCodes.Brtrue, labelTrue);
                        }
                        if (pre == Operator.And)
                        {
                            il.Emit(OpCodes.Brfalse, labelFalse);
                        }
                    }

                    if (pre == Operator.Or)
                    {
                        il.Emit(OpCodes.Br, labelEnd);
                    }

                    if (pre == Operator.And)
                    {
                        il.Emit(OpCodes.Br, labelTrue);
                    }
                    il.MarkLabel(labelTrue);
                    il.Emit(OpCodes.Ldc_I4_1);
                    il.Emit(OpCodes.Stloc_0);
                    il.Emit(OpCodes.Br, labelEnd);


                    il.MarkLabel(labelFalse);
                    il.Emit(OpCodes.Ldc_I4_0);
                    il.Emit(OpCodes.Stloc_0);
                }
                else
                {
                    if (t.Children.Count == 1)
                    {
                        var m = c.CompileTag(t.Children[0]);
                        il.Emit(OpCodes.Ldarg_0);
                        il.Emit(OpCodes.Ldarg_1);
                        il.Emit(OpCodes.Call, m);
                        il.Emit(OpCodes.Stloc_0);
                    }
                    else if (t.Children.Count == 3)
                    {

                        var bestType = TypeGuesser.FindBestType(types.ToArray());
                        var stack = ExpressionEvaluator.ProcessExpression(array);
                        var arr = stack.ToArray();
                        for (var i = arr.Length - 1; i >= 0; i--)
                        {
                            var obj = arr[i];
                            var childTag = obj as ITag;
                            if (childTag != null)
                            {
                                var m = c.CompileTag(childTag);
                                il.Emit(OpCodes.Ldarg_0);
                                il.Emit(OpCodes.Ldarg_1);
                                il.Emit(OpCodes.Call, m);
                                if (m.ReturnType.FullName != bestType.FullName)
                                {
                                    switch (bestType.FullName)
                                    {
                                        case "System.Decimal":
                                            Type cType = m.ReturnType;
                                            switch (cType.FullName)
                                            {
                                                case "System.Int16":
                                                case "System.UInt16":
                                                case "System.Byte":
                                                    il.Emit(OpCodes.Conv_I4);
                                                    break;
                                            }
                                            il.Emit(OpCodes.Call, typeof(decimal).GetConstructor(new Type[] { cType }));
                                            break;
                                        case "System.Double":
                                            il.Emit(OpCodes.Conv_R8);
                                            break;
                                        case "System.Single":
                                            il.Emit(OpCodes.Conv_R4);
                                            break;
                                        case "System.Int64":
                                            il.Emit(OpCodes.Conv_I8);
                                            break;
                                        case "System.UInt64":
                                            il.Emit(OpCodes.Conv_U8);
                                            break;
                                        case "System.Int32":
                                            il.Emit(OpCodes.Conv_I4);
                                            break;
                                        case "System.UInt32":
                                            il.Emit(OpCodes.Conv_U4);
                                            break;
                                        case "System.Int16":
                                            il.Emit(OpCodes.Conv_I2);
                                            break;
                                        case "System.UInt16":
                                            il.Emit(OpCodes.Conv_U2);
                                            break;
                                        case "System.Byte":
                                            il.Emit(OpCodes.Conv_U1);
                                            break;
                                        case "System.String":
                                            if (m.ReturnType.IsValueType)
                                            {
                                                var p = il.DeclareLocal(m.ReturnType);
                                                il.Emit(OpCodes.Stloc, p.LocalIndex);
                                                il.Emit(OpCodes.Ldloca, p.LocalIndex);
                                            }
                                            il.Call(m.ReturnType, typeof(object).GetMethodInfo("ToString", Type.EmptyTypes));
                                            break;
                                        default:
                                            if (m.ReturnType.IsValueType)
                                            {
                                                if (bestType.IsValueType)
                                                {
                                                    il.Emit(OpCodes.Isinst, bestType);
                                                }
                                                else
                                                {
                                                    il.Emit(OpCodes.Box, bestType);
                                                }
                                            }
                                            else
                                            {
                                                if (bestType.IsValueType)
                                                {
                                                    il.Emit(OpCodes.Unbox, bestType);
                                                }
                                                else
                                                {
                                                    il.Emit(OpCodes.Castclass, bestType);
                                                }
                                            }
                                            break;
                                    }
                                }
                            }
                            else
                            {
                                switch ((Operator)obj)
                                {
                                    case Operator.GreaterThan:
                                        if (TypeGuesser.CanUseEqual(bestType))
                                        {
                                            il.Emit(OpCodes.Cgt);
                                        }
                                        else
                                        {
                                            var m = bestType.GetMethodInfo("op_GreaterThan", new Type[] { bestType, bestType });
                                            if (m == null)
                                            {
                                                throw new TemplateException($"Operator \">\" can not be applied operand \"{bestType.FullName}\" and \"{bestType.FullName}\"");
                                            }
                                            il.Emit(OpCodes.Call, m);
                                        }
                                        break;
                                    case Operator.GreaterThanOrEqual:
                                        if (TypeGuesser.CanUseEqual(bestType))
                                        {
                                            il.Emit(OpCodes.Clt_Un);
                                            il.Emit(OpCodes.Ldc_I4_0);
                                            il.Emit(OpCodes.Ceq);
                                        }
                                        else
                                        {
                                            var m = bestType.GetMethodInfo("op_GreaterThanOrEqual", new Type[] { bestType, bestType });
                                            if (m == null)
                                            {
                                                throw new TemplateException($"Operator \">=\" can not be applied operand \"{bestType.FullName}\" and \"{bestType.FullName}\"");
                                            }
                                            il.Emit(OpCodes.Call, m);
                                        }
                                        break;
                                    case Operator.LessThan:
                                        if (TypeGuesser.CanUseEqual(bestType))
                                        {
                                            il.Emit(OpCodes.Clt);
                                        }
                                        else
                                        {
                                            var m = bestType.GetMethodInfo("op_LessThan", new Type[] { bestType, bestType });
                                            if (m == null)
                                            {
                                                throw new TemplateException($"Operator \"<\" can not be applied operand \"{bestType.FullName}\" and \"{bestType.FullName}\"");
                                            }
                                            il.Emit(OpCodes.Call, m);
                                        }
                                        break;
                                    case Operator.LessThanOrEqual:
                                        if (TypeGuesser.CanUseEqual(bestType))
                                        {
                                            il.Emit(OpCodes.Cgt_Un);
                                            il.Emit(OpCodes.Ldc_I4_0);
                                            il.Emit(OpCodes.Ceq);
                                        }
                                        else
                                        {
                                            var m = bestType.GetMethodInfo("op_LessThanOrEqual", new Type[] { bestType, bestType });
                                            if (m == null)
                                            {
                                                throw new TemplateException($"Operator \"<=\" can not be applied operand \"{bestType.FullName}\" and \"{bestType.FullName}\"");
                                            }
                                            il.Emit(OpCodes.Call, m);
                                        }
                                        break;
                                    case Operator.Equal:
                                        if (TypeGuesser.CanUseEqual(bestType))
                                        {
                                            il.Emit(OpCodes.Ceq);
                                        }
                                        else
                                        {
                                            il.EmitEquals(bestType);
                                            //il.Emit(OpCodes.Call, DynamicHelpers.GetMethodInfo(bestType, "Equals", new Type[] { bestType }));
                                        }
                                        break;
                                    case Operator.NotEqual:
                                        if (TypeGuesser.CanUseEqual(bestType))
                                        {
                                            il.Emit(OpCodes.Ceq);
                                        }
                                        else
                                        {
                                            il.EmitEquals(bestType);
                                            //il.Emit(OpCodes.Call, DynamicHelpers.GetMethodInfo(bestType, "Equals", new Type[] { bestType }));
                                        }
                                        il.Emit(OpCodes.Ldc_I4_0);
                                        il.Emit(OpCodes.Ceq);
                                        break;
                                    default:
                                        throw new CompileException(tag, $"The operator \"{obj}\" is not supported on type  \"{bestType.FullName}\" .");
                                }
                            }
                        }
                        il.Emit(OpCodes.Stloc_0);
                    }
                    else
                    {
                        throw new CompileException(tag, $"[LogicExpressionTag] : The expression \"{string.Concat(message)}\" is not supported .");
                    }
                }

                il.MarkLabel(labelEnd);
                il.Emit(OpCodes.Ldloc_0);
                il.Emit(OpCodes.Ret);
                return mb.GetBaseDefinition();
            };
        }
        /// <inheritdoc />
        public override Func<ITag, CompileContext, Type> BuildGuessMethod()
        {
            return (tag, c) =>
            {
                return typeof(bool);
            };
        }
        /// <inheritdoc />
        public override Func<ITag, TemplateContext, object> BuildExcuteMethod()
        {
            return (tag, context) =>
            {
                var t = tag as LogicTag;
                List<object> parameters = new List<object>();

                for (int i = 0; i < t.Children.Count; i++)
                {
                    bool isOperator = t.Children[i] is OperatorTag;
                    object result = TagExecutor.Execute(t.Children[i], context);
                    if (Eval(parameters, isOperator, result))
                    {
                        return parameters[parameters.Count - 1];
                    }
                }

                var stack = ExpressionEvaluator.ProcessExpression(parameters.ToArray());
                return ExpressionEvaluator.Calculate(stack);
            };
        }

        /// <summary>
        /// eval expression
        /// </summary>
        /// <param name="list">list</param>
        /// <param name="isOperator"></param>
        /// <param name="value"></param>
        /// <returns></returns>
        private static bool Eval(List<object> list, bool isOperator, object value)
        {
            if (isOperator)
            {
                Operator op = (Operator)value;
                if (op == Operator.Or || op == Operator.And)
                {
                    object result;
                    bool isTrue;
                    if (list.Count > 1)
                    {
                        var stack = ExpressionEvaluator.ProcessExpression(list.ToArray());
                        result = ExpressionEvaluator.Calculate(stack);
                    }
                    else
                    {
                        result = list[0];
                    }
                    if (result is bool)
                    {
                        isTrue = (bool)result;
                    }
                    else
                    {
                        isTrue = ExpressionEvaluator.CalculateBoolean(result);
                    }
                    if (op == Operator.Or && isTrue)
                    {
                        list.Add(true);
                        return true;
                    }
                    if (op == Operator.And && !isTrue)
                    {
                        list.Add(false);
                        return true;
                    }
                    list.Clear();
                    list.Add(isTrue);
                }
                list.Add(op);
            }
            else
            {
                list.Add(value);
            }
            return false;
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="tags"></param>
        /// <param name="opt"></param>
        /// <returns></returns>
        private static ITag[] Analysis(IList<ITag> tags, IList<Operator> opt)
        {
            var result = new List<ITag>();
            var temp = new List<ITag>();

            var isLogical = false;
            for (var i = 0; i < tags.Count; i++)
            {
                var o = tags[i] as OperatorTag;
                if (o != null)
                {
                    if (opt.Contains(o.Value))
                    {
                        result.Add(Analysis(temp, isLogical));
                        result.Add(tags[i]);
                        isLogical = false;
                        temp.Clear();
                    }
                    else
                    {
                        if (operators.Contains(o.Value))
                        {
                            isLogical = true;
                        }
                        temp.Add(tags[i]);
                    }
                    //result.Add(new Tem);
                }
                else
                {
                    temp.Add(tags[i]);
                }
            }

            if (temp.Count > 0)
            {
                result.Add(Analysis(temp, isLogical));
            }

            return result.ToArray();
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="tags"></param>
        /// <param name="isLogical"></param>
        /// <returns></returns>
        private static ITag Analysis(IList<ITag> tags, bool isLogical)
        {
            if (tags.Count > 1)
            {
                ITag t;
                if (isLogical)
                {
                    t = new LogicTag();
                    AddRange(t, Analysis(tags, operators));
                }
                else
                {
                    t = new ArithmeticTag();
                    AddRange(t, tags);
                }
                return t;
            }
            else
            {
                return tags[0];
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="tag"></param>
        /// <param name="list"></param>
        private static void AddRange(ITag tag, IList<ITag> list)
        {
            for (var i = 0; i < list.Count; i++)
            {
                tag.AddChild(list[i]);
            }
        }

    }
}
