/********************************************************************************
 Copyright (c) jiniannet (http://www.jiniannet.com). All rights reserved.
 Licensed under the MIT license. See licence.txt file in the project root for full license information.
 ********************************************************************************/
using System;
using System.Collections.Generic;
using System.Linq;
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
    /// The <see cref="ArithmeticTag"/> registrar
    /// </summary>
    public class ArithmeticVisitor : TagVisitor<ArithmeticTag>, ITagVisitor
    {
        /// <inheritdoc />
        public ITag Parse(TemplateParser parser, TokenCollection tc)
        {
            if (tc.Count > 2)
            {
                var tcs = tc.Split(TokenKind.Arithmetic);
                if (tcs.Length <= 1)
                {
                    return null;
                }

                var tags = new List<ITag>();
                for (int i = 0; i < tcs.Length; i++)
                {
                    if (tcs[i].Count == 1 && tcs[i][0].TokenKind == TokenKind.Arithmetic)
                    {
                        tags.Add(new OperatorTag(tcs[i][0]));
                    }
                    else
                    {
                        var t = parser.ReadSimple(tcs[i]);
                        if (t == null)
                            return null;
                        tags.Add(t);
                    }
                }

                if (tags.Count == 1)
                {
                    return tags[0];
                }

                if (tags.Count > 1)
                {
                    ITag t = new ArithmeticTag();
                    for (int i = 0; i < tags.Count; i++)
                    {
                        t.AddChild(tags[i]);
                    }
                    tags.Clear();
                    return t;
                }

            }

            return null;
        }

        /// <inheritdoc />
        public MethodInfo Compile(ITag tag, CompileContext c)
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
                il.Call(stringBuilderType, typeof(object).GetMethodInfo("ToString", Type.EmptyTypes));
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
                                    throw new CompileException(tag, $"[ExpressionTag] : The type \"{bestType.FullName}\" is not supported .");
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
                                throw new CompileException(tag, $"[ExpressionTag] : The expression \"{string.Concat(message)}\" is not supported .");
                                //throw new CompileException($"The operator \"{obj}\" is not supported on type  \"{bestType.FullName}\" .");
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
        }

        /// <inheritdoc />
        public Type GuessType(ITag tag, CompileContext c)
        {
            var t = tag as ArithmeticTag;
            var types = new List<Type>();
            var opts = new List<Operator>();
            for (var i = 0; i < t.Children.Count; i++)
            {
                var opt = t.Children[i] as OperatorTag;
                if (opt == null)
                {
                    types.Add(c.GuessType(t.Children[i]));
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

        }
        /// <inheritdoc />
        public object Excute(ITag tag, TemplateContext context)
        {
            var t = tag as ArithmeticTag;
            var parameters = new List<object>();

            for (int i = 0; i < t.Children.Count; i++)
            {
                var opt = t.Children[i] as OperatorTag;
                if (opt != null)
                {
                    parameters.Add(opt.Value);
                }
                else
                {
                    parameters.Add(context.Execute(t.Children[i]));
                }
            }

            var stack = ExpressionEvaluator.ProcessExpression(parameters.ToArray());
            return ExpressionEvaluator.Calculate(stack);
        }
    }
}
