/********************************************************************************
 Copyright (c) jiniannet (http://www.jiniannet.com). All rights reserved.
 Licensed under the MIT license. See licence.txt file in the project root for full license information.
 ********************************************************************************/
using JinianNet.JNTemplate.CodeCompilation;
using JinianNet.JNTemplate.Dynamic;
using JinianNet.JNTemplate.Nodes;
using System;
using System.Collections.Generic;
using System.Reflection.Emit;
using System.Text;

namespace JinianNet.JNTemplate.Parsers
{
    /// <summary>
    /// The complex tags registrar
    /// </summary>
    public class ComplexRegistrar : IRegistrar
    {
        #region
        private static List<Operator> operators = new List<Operator>(new Operator[] { Operator.Equal, Operator.NotEqual, Operator.LessThan, Operator.LessThanOrEqual, Operator.GreaterThan, Operator.GreaterThanOrEqual });
        #endregion

        #region 
        /// <inheritdoc />
        public void Regiser(IEngine engine)
        {
            engine.Register((parser, tc) =>
            {
                if (tc != null
                    && parser != null
                    && tc.Count > 2)
                {

                    var tags = new List<ITag>();
                    var tcs = tc.Split(0, tc.Count, TokenKind.Dot, TokenKind.Operator);
                    bool isLogical = false;
                    if (tcs.Length == 1)
                    {
                        return null;
                    }
                    for (int i = 0; i < tcs.Length; i++)
                    {
                        if (tcs[i].Count == 1 && tcs[i][0].TokenKind == TokenKind.Dot)
                        {
                            if (tags.Count == 0 || i == tcs[i].Count - 1 || (tcs[i + 1].Count == 1 && (tcs[i + 1][0].TokenKind == TokenKind.Dot || tcs[i + 1][0].TokenKind == TokenKind.Operator)))
                            {
                                throw new Exception.ParseException(string.Concat("syntax error near .:", tc), tcs[i][0].BeginLine, tcs[i][0].BeginColumn);
                            }
                            if (tags[tags.Count - 1] is ReferenceTag)
                            {
                                tags[tags.Count - 1].AddChild(parser.Read(tcs[i + 1]));
                            }
                            else
                            {
                                var t = new ReferenceTag();
                                t.AddChild(tags[tags.Count - 1]);
                                t.AddChild(parser.Read(tcs[i + 1]));
                                tags[tags.Count - 1] = t;
                            }
                            i++;
                        }
                        else if (tcs[i].Count == 1 && tcs[i][0].TokenKind == TokenKind.Operator)
                        {
                            tags.Add(new OperatorTag(tcs[i][0]));

                            Operator op = Dynamic.OperatorConvert.Parse(tcs[i][0].Text);
                            switch (op)
                            {
                                case Operator.Or:
                                case Operator.And:
                                case Operator.LessThan:
                                case Operator.LessThanOrEqual:
                                case Operator.Equal:
                                case Operator.GreaterThan:
                                case Operator.GreaterThanOrEqual:
                                case Operator.NotEqual:
                                    isLogical = true;
                                    break;
                            }
                        }
                        else if (tcs[i][0].TokenKind == TokenKind.LeftBracket)
                        {
                            if (tags[tags.Count - 1] is ReferenceTag)
                            {
                                tags[tags.Count - 1].AddChild(parser.Read(tcs[i]));
                            }
                            else
                            {
                                if (tags.Count == 0)
                                {
                                    throw new Exception.ParseException(string.Concat("syntax error near [:", tc), tcs[i][0].BeginLine, tcs[i][0].BeginColumn);
                                }
                                var t = new ReferenceTag();
                                t.AddChild(tags[tags.Count - 1]);
                                t.AddChild(parser.Read(tcs[i]));
                                tags[tags.Count - 1] = t;
                            }
                        }
                        else if (tcs[i].Count > 0)
                        {
                            if (tcs[i].First.TokenKind == TokenKind.LeftParentheses && tcs[i].Last.TokenKind == TokenKind.RightParentheses)
                            {
                                tcs[i].RemoveAt(0);
                                tcs[i].RemoveAt(tcs[i].Count - 1);
                            }
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
                        ITag t;
                        if (isLogical)
                        {
                            t = new LogicTag();
                            var arr = Analysis(tags, new List<Operator>(new Operator[] { Operator.And, Operator.Or }));
                            if (arr.Length == 1)
                            {
                                return arr[0];
                            }
                            AddRange(t, arr);
                        }
                        else
                        {
                            t = new ArithmeticTag();
                            for (int i = 0; i < tags.Count; i++)
                            {
                                t.AddChild(tags[i]);
                            }
                        }
                        tags.Clear();
                        return t;
                    }
                }
                return null;
            }, -1);

            engine.Register<ReferenceTag>((tag, c) =>
            {
                return c.CompileTag(((ReferenceTag)tag).Child);
            });

            engine.Register<LogicTag>((tag, c) =>
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
                            var cm = DynamicHelpers.GetMethod(typeof(Utility), "ToBoolean", new Type[] { m.ReturnType });
                            if (cm == null)
                            {
                                cm = DynamicHelpers.GetMethod(typeof(Utility), "ToBoolean", new Type[] { typeof(object) });
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
                                            var m = DynamicHelpers.GetMethod(bestType, "op_GreaterThan", new Type[] { bestType, bestType });
                                            if (m == null)
                                            {
                                                throw new Exception.TemplateException($"Operator \">\" can not be applied operand \"{bestType.FullName}\" and \"{bestType.FullName}\"");
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
                                            var m = DynamicHelpers.GetMethod(bestType, "op_GreaterThanOrEqual", new Type[] { bestType, bestType });
                                            if (m == null)
                                            {
                                                throw new Exception.TemplateException($"Operator \">=\" can not be applied operand \"{bestType.FullName}\" and \"{bestType.FullName}\"");
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
                                            var m = DynamicHelpers.GetMethod(bestType, "op_LessThan", new Type[] { bestType, bestType });
                                            if (m == null)
                                            {
                                                throw new Exception.TemplateException($"Operator \"<\" can not be applied operand \"{bestType.FullName}\" and \"{bestType.FullName}\"");
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
                                            var m = DynamicHelpers.GetMethod(bestType, "op_LessThanOrEqual", new Type[] { bestType, bestType });
                                            if (m == null)
                                            {
                                                throw new Exception.TemplateException($"Operator \"<=\" can not be applied operand \"{bestType.FullName}\" and \"{bestType.FullName}\"");
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
                                            //il.Emit(OpCodes.Call, DynamicHelpers.GetMethod(bestType, "Equals", new Type[] { bestType }));
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
                                            //il.Emit(OpCodes.Call, DynamicHelpers.GetMethod(bestType, "Equals", new Type[] { bestType }));
                                        }
                                        il.Emit(OpCodes.Ldc_I4_0);
                                        il.Emit(OpCodes.Ceq);
                                        break;
                                    default:
                                        throw new Exception.CompileException($"The operator \"{obj}\" is not supported on type  \"{bestType.FullName}\" .");
                                }
                            }
                        }
                        il.Emit(OpCodes.Stloc_0);
                    }
                    else
                    {
                        throw new Exception.CompileException($"[LogicExpressionTag] : The expression \"{string.Concat(message)}\" is not supported .");
                    }
                }

                il.MarkLabel(labelEnd);
                il.Emit(OpCodes.Ldloc_0);
                il.Emit(OpCodes.Ret);
                return mb.GetBaseDefinition();
            });

            engine.Register<ArithmeticTag>((tag, c) =>
            {
                var stringBuilderType = typeof(StringBuilder);
                var t = tag as ArithmeticTag;
                var type = c.GuessType(t);
                var mb = c.CreateReutrnMethod<ArithmeticTag>(type);
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
                if (types.Contains(typeof(string)) && opts.Count == 1 && opts[0] == Operator.Add)
                {
                    il.DeclareLocal(stringBuilderType);
                    il.Emit(OpCodes.Newobj, stringBuilderType.GetConstructor(Type.EmptyTypes));
                    il.Emit(OpCodes.Stloc_1);
                    for (int i = 0; i < t.Children.Count; i++)
                    {
                        var opt = t.Children[i] as OperatorTag;
                        if (opt != null)
                        {
                            continue;
                        }
                        il.Emit(OpCodes.Ldloc_1);
                        il.Emit(OpCodes.Ldarg_0);
                        il.Emit(OpCodes.Ldarg_1);
                        var m = c.CompileTag(t.Children[i]);
                        il.Emit(OpCodes.Call, m);
                        il.StringAppend(c, m.ReturnType);
                        il.Emit(OpCodes.Pop);
                    }

                    il.Emit(OpCodes.Ldloc_1);
                    il.Call(stringBuilderType, DynamicHelpers.GetMethod(typeof(object), "ToString", Type.EmptyTypes));
                    il.Emit(OpCodes.Stloc_0);
                }
                else
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
                                    default:
                                        throw new Exception.CompileException($"[ExpressionTag] : The type \"{bestType.FullName}\" is not supported .");
                                }
                            }
                        }
                        else
                        {
                            switch ((Operator)obj)
                            {
                                case Operator.Add:
                                    il.Emit(OpCodes.Add);
                                    break;
                                case Operator.Subtract:
                                    il.Emit(OpCodes.Sub);
                                    break;
                                case Operator.Multiply:
                                    il.Emit(OpCodes.Mul);
                                    break;
                                case Operator.Divided:
                                    il.Emit(OpCodes.Div);
                                    break;
                                case Operator.Remainder:
                                    il.Emit(OpCodes.Rem);
                                    break;
                                case Operator.GreaterThan:
                                    il.Emit(OpCodes.Cgt);
                                    break;
                                case Operator.GreaterThanOrEqual:
                                    il.Emit(OpCodes.Clt_Un);
                                    il.Emit(OpCodes.Ldc_I4_0);
                                    il.Emit(OpCodes.Ceq);
                                    break;
                                case Operator.LessThan:
                                    il.Emit(OpCodes.Clt);
                                    break;
                                case Operator.LessThanOrEqual:
                                    il.Emit(OpCodes.Cgt_Un);
                                    il.Emit(OpCodes.Ldc_I4_0);
                                    il.Emit(OpCodes.Ceq);
                                    break;
                                case Operator.Equal:
                                    il.Emit(OpCodes.Ceq);
                                    break;
                                case Operator.NotEqual:
                                    il.Emit(OpCodes.Ceq);
                                    il.Emit(OpCodes.Ldc_I4_0);
                                    il.Emit(OpCodes.Ceq);
                                    break;
                                default:
                                    throw new Exception.CompileException($"[ExpressionTag] : The expression \"{string.Concat(message)}\" is not supported .");
                                    //throw new Exception.CompileException($"The operator \"{obj}\" is not supported on type  \"{bestType.FullName}\" .");
                                    //case Operator.Or:
                                    //    il.Emit(OpCodes.Blt);
                                    //    break;
                            }
                        }
                    }
                    il.Emit(OpCodes.Stloc_0);
                }
                il.MarkLabel(labelEnd);
                il.Emit(OpCodes.Ldloc_0);
                il.Emit(OpCodes.Ret);
                return mb.GetBaseDefinition();
            });

            engine.Register<ReferenceTag>((tag, c) =>
            {
                return c.GuessType(((ReferenceTag)tag).Child);
            });

            engine.Register<LogicTag>((tag, ctx) =>
            {
                return typeof(bool);
            });

            engine.Register<ArithmeticTag>((tag, ctx) =>
            {
                var t = tag as ArithmeticTag;
                var types = new List<Type>();
                var opts = new List<Operator>();
                for (var i = 0; i < t.Children.Count; i++)
                {
                    var opt = t.Children[i] as OperatorTag;
                    if (opt == null)
                    {
                        types.Add(ctx.GuessType(t.Children[i]));
                    }
                }
                if (types.Count == 1)
                {
                    return types[0];
                }
                if (types.Contains(typeof(string)))
                {
                    return typeof(string);
                }
                if (types.Count > 0)
                {
                    return TypeGuesser.FindBestType(types.ToArray());
                }

                return typeof(int);
            });
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="tags"></param>
        /// <param name="opt"></param>
        /// <returns></returns>
        public static ITag[] Analysis(IList<ITag> tags, IList<Operator> opt)
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
        public static ITag Analysis(IList<ITag> tags, bool isLogical)
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
        public static void AddRange(ITag tag, IList<ITag> list)
        {
            for (var i = 0; i < list.Count; i++)
            {
                tag.AddChild(list[i]);
            }
        }

        #endregion
    }
}