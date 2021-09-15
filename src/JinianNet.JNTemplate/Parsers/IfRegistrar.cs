/********************************************************************************
 Copyright (c) jiniannet (http://www.jiniannet.com). All rights reserved.
 Licensed under the MIT license. See licence.txt file in the project root for full license information.
 ********************************************************************************/
using JinianNet.JNTemplate.CodeCompilation;
using JinianNet.JNTemplate.Dynamic;
using JinianNet.JNTemplate.Nodes;
using JinianNet.JNTemplate.Exceptions;
using System;
using System.Collections.Generic;
using System.Reflection;
using System.Reflection.Emit;

namespace JinianNet.JNTemplate.Parsers
{
    /// <summary>
    /// The <see cref="IfTag"/> registrar
    /// </summary>
    public class IfRegistrar : TagRegistrar<IfTag>, IRegistrar
    {
        /// <inheritdoc />
        public override Func<TemplateParser, TokenCollection, ITag> BuildParseMethod()
        {
            return (parser, tc) =>
            {
                if (tc != null
                    && parser != null
                    && tc.Count > 3
                    && Utility.IsEqual(tc.First.Text, Const.KEY_IF))
                {

                    if (tc[1].TokenKind == TokenKind.LeftParentheses
                       && tc.Last.TokenKind == TokenKind.RightParentheses)
                    {
                        var tag = new IfTag();

                        var t = new ElseifTag();
                        TokenCollection coll = tc[2, -1];
                        t.Condition = parser.Read(coll);
                        t.FirstToken = coll.First;
                        //t.LastToken = coll.Last;
                        tag.AddChild(t);

                        while (parser.MoveNext())
                        {
                            if (parser.Current is EndTag)
                            {
                                tag.AddChild(parser.Current);
                                return tag;
                            }
                            else if (parser.Current is ElseifTag
                                || parser.Current is ElseTag)
                            {
                                tag.AddChild(parser.Current);
                            }
                            else
                            {
                                tag.Children[tag.Children.Count - 1].AddChild(parser.Current);
                            }
                        }

                        throw new ParseException($"if is not properly closed by a end tag: {tc.ToString()}", tc.First.BeginLine, tc.First.BeginColumn);
                    }
                    else
                    {
                        throw new ParseException($"syntax error near if: {tc.ToString()}", tc.First.BeginLine, tc.First.BeginColumn);
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
                var t = tag as IfTag;
                var type = c.GuessType(t);
                var mb = c.CreateReutrnMethod<IfTag>(type);
                var il = mb.GetILGenerator();
                var labelEnd = il.DefineLabel();
                var labelSuccess = il.DefineLabel();
                var hasReturn = type.FullName != "System.Void";
                if (type.FullName != "System.Void")
                {
                    il.DeclareLocal(type);
                    hasReturn = true;
                }
                var lables = new Label[t.Children.Count - 1];
                for (var i = 0; i < lables.Length; i++)
                {
                    lables[i] = il.DefineLabel();
                }
                for (var i = 0; i < t.Children.Count; i++)
                {
                    if (t.Children[i] is EndTag)
                    {
                        continue;
                    }
                    var ifTag = t.Children[i] as ElseifTag;

                    if (!(t.Children[i] is ElseTag))
                    {
                        var m = c.CompileTag(ifTag.Condition);
                        il.Emit(OpCodes.Ldarg_0);
                        il.Emit(OpCodes.Ldarg_1);
                        il.Emit(OpCodes.Call, m);
                        if (m.ReturnType.Name != "Boolean")
                        {
                            var localVar = il.DeclareLocal(m.ReturnType);
                            il.Emit(OpCodes.Stloc, localVar.LocalIndex);
                            il.LoadVariable(m.ReturnType, localVar.LocalIndex);
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
                        il.Emit(OpCodes.Brfalse, lables[i]);
                    }

                    var execute = c.CompileTag(ifTag);
                    il.Emit(OpCodes.Ldarg_0);
                    il.Emit(OpCodes.Ldarg_1);
                    il.Emit(OpCodes.Call, execute);
                    if (execute.ReturnType.FullName != type.FullName)
                    {
                        if (type.FullName == "System.String")
                        {
                            var localVar = il.DeclareLocal(execute.ReturnType);
                            il.Emit(OpCodes.Stloc, localVar.LocalIndex);
                            il.LoadVariable(execute.ReturnType, localVar.LocalIndex);
                            il.Call(execute.ReturnType, typeof(object).GetMethodInfo("ToString", Type.EmptyTypes));
                        }
                        else
                        {
                            if (execute.ReturnType.IsValueType)
                            {
                                if (!type.IsValueType)
                                {
                                    il.Emit(OpCodes.Box, type);
                                }
                                else
                                {
                                    switch (type.FullName)
                                    {
                                        case "System.Decimal":
                                            switch (execute.ReturnType.FullName)
                                            {
                                                case "System.Int16":
                                                case "System.UInt16":
                                                case "System.Byte":
                                                    il.Emit(OpCodes.Conv_I4);
                                                    break;
                                            }
                                            il.Emit(OpCodes.Call, typeof(decimal).GetConstructor(new Type[] { execute.ReturnType }));
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
                                            il.Emit(OpCodes.Isinst, type);
                                            break;
                                    }
                                }
                            }
                            else
                            {
                                if (type.IsValueType)
                                {
                                    il.Emit(OpCodes.Unbox, type);
                                }
                                else
                                {
                                    il.Emit(OpCodes.Castclass, type);
                                }
                            }
                        }
                    }
                    if (hasReturn)
                    {
                        il.Emit(OpCodes.Stloc_0);
                        il.Emit(OpCodes.Br, labelSuccess);// lables[lables.Length - 1]
                    }
                    else
                    {
                        il.Emit(OpCodes.Br, labelEnd);
                    }

                    il.MarkLabel(lables[i]);
                }

                if (hasReturn)
                {
                    if (type.IsValueType)
                    {
                        var defaultMethod = typeof(Utility).GetGenericMethod(new Type[] { type }, "GenerateDefaultValue", Type.EmptyTypes);
                        il.Emit(OpCodes.Call, defaultMethod);
                        il.Emit(OpCodes.Stloc_0);
                    }
                    else
                    {
                        il.Emit(OpCodes.Ldnull);
                        il.Emit(OpCodes.Stloc_0);
                    }
                }

                il.MarkLabel(labelSuccess);
                il.Emit(OpCodes.Ldloc_0);
                il.MarkLabel(labelEnd);
                il.Emit(OpCodes.Ret);
                return mb.GetBaseDefinition();
            };
        }
        /// <inheritdoc />
        public override Func<ITag, CompileContext, Type> BuildGuessMethod()
        {
            return (tag, c) =>
            {
                var t = tag as IfTag;
                Type type = null;
                for (var i = 0; i < t.Children.Count; i++)
                {
                    if (t.Children[i] is EndTag)
                    {
                        continue;
                    }
                    var cType = c.GuessType(t.Children[i]);
                    if (type == null)
                    {
                        type = cType;
                    }
                    else
                    {
                        if (cType == null || type.FullName != cType.FullName)
                        {
                            return typeof(string);
                        }
                    }
                }
                return type;
            };
        }
        /// <inheritdoc />
        public override Func<ITag, TemplateContext, object> BuildExcuteMethod()
        {
            return (tag, context) =>
            {
                var t = tag as IfTag;
                for (int i = 0; i < t.Children.Count - 1; i++)
                {
                    var c = (ElseifTag)t.Children[i];
                    if (c == null)
                    {
                        continue;
                    }
                    if (t.Children[i] is ElseTag)
                    {
                        return context.Execute(t.Children[i]);
                    }

                    var condition = context.Execute(c.Condition);
                    if (Utility.ToBoolean(condition))
                    {
                        return context.Execute(t.Children[i]);
                    }
                }
                return null;
            };
        }

    }
}