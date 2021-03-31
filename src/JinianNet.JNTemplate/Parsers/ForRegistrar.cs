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
using JinianNet.JNTemplate.Exception;
using JinianNet.JNTemplate.Nodes;

namespace JinianNet.JNTemplate.Parsers
{
    /// <summary>
    /// The <see cref="ForTag"/> registrar
    /// </summary>
    public class ForRegistrar : TagRegistrar<ForTag>, IRegistrar
    {
        /// <inheritdoc />
        public override Func<TemplateParser, TokenCollection, ITag> BuildParseMethod()
        {
            return (parser, tc) =>
            {
                if (tc != null
                                && parser != null
                                && tc.Count > 3
                                && Utility.IsEqual(Field.KEY_FOR, tc.First.Text))
                {

                    if (tc[1].TokenKind == TokenKind.LeftParentheses
                       && tc.Last.TokenKind == TokenKind.RightParentheses)
                    {
                        int pos = 0,
                            start = 2,
                            end;

                        var ts = new List<ITag>(3);

                        var tag = new ForTag();
                        for (int i = 2; i < tc.Count - 1; i++)
                        {
                            end = i;
                            if (tc[i].TokenKind == TokenKind.Punctuation && tc[i].Text == ";")
                            {
                                if (pos == 0)
                                {
                                    var coll = tc[start, end];
                                    if (coll.Count > 0)
                                    {
                                        ts.Add(parser.Read(coll));
                                    }
                                    else
                                    {
                                        ts.Add(null);
                                    }
                                    start = i + 1;
                                    continue;
                                }
                            }

                            if (tc[i].TokenKind == TokenKind.LeftParentheses)
                            {
                                pos++;
                            }
                            else if (tc[i].TokenKind == TokenKind.RightParentheses)
                            {
                                pos--;
                            }
                            if (i == tc.Count - 2)
                            {
                                var coll = tc[start, end + 1];
                                if (coll.Count > 0)
                                {
                                    ts.Add(parser.Read(coll));
                                }
                                else
                                {
                                    ts.Add(null);
                                }
                            }
                        }

                        if (ts.Count != 3)
                        {
                            throw new Exception.ParseException(string.Concat("syntax error near for:", tc), tc.First.BeginLine, tc.First.BeginColumn);
                        }

                        tag.Initial = ts[0];
                        tag.Condition = ts[1];
                        tag.Do = ts[2];

                        while (parser.MoveNext())
                        {
                            tag.Children.Add(parser.Current);
                            if (parser.Current is EndTag)
                            {
                                return tag;
                            }
                        }

                        throw new Exception.ParseException(string.Concat("for is not properly closed by a end tag:", tc), tc.First.BeginLine, tc.First.BeginColumn);
                    }
                    else
                    {
                        throw new Exception.ParseException(string.Concat("syntax error near for:", tc), tc.First.BeginLine, tc.First.BeginColumn);
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
                var stringBuilderType = typeof(StringBuilder);
                var t = tag as ForTag;
                var type = c.GuessType(t);
                var templateContextType = typeof(TemplateContext);
                var mb = c.CreateReutrnMethod<ForTag>(type);
                var il = mb.GetILGenerator();
                //Label labelEnd = il.DefineLabel();
                Label labelNext = il.DefineLabel();
                Label labelStart = il.DefineLabel();
                il.DeclareLocal(stringBuilderType);
                il.DeclareLocal(templateContextType);
                il.DeclareLocal(typeof(bool));
                il.DeclareLocal(typeof(string));
                var index = 4;
                il.Emit(OpCodes.Nop);
                il.Emit(OpCodes.Newobj, stringBuilderType.GetConstructor(Type.EmptyTypes));
                il.Emit(OpCodes.Stloc_0);
                il.Emit(OpCodes.Ldarg_1);
                il.Emit(OpCodes.Call, DynamicHelpers.GetMethod(templateContextType, "CreateContext", new Type[] { templateContextType }));
                il.Emit(OpCodes.Stloc_1);

                il.Emit(OpCodes.Ldarg_0);
                il.Emit(OpCodes.Ldloc_1);
                var m = c.CompileTag(t.Initial);
                il.Emit(OpCodes.Call, m);
                if (m.ReturnType.FullName != "System.Void")
                {
                    il.DeclareLocal(m.ReturnType);
                    il.Emit(OpCodes.Stloc, index);
                    index++;
                }

                il.Emit(OpCodes.Br, labelNext);

                il.MarkLabel(labelStart);
                for (var i = 0; i < t.Children.Count; i++)
                {
                    il.CallTag(c, t.Children[i], (nil, hasReturn, needCall) =>
                    {
                        if (hasReturn)
                        {
                            nil.Emit(OpCodes.Ldloc_0);
                        }
                        if (needCall)
                        {
                            nil.Emit(OpCodes.Ldarg_0);
                            nil.Emit(OpCodes.Ldarg_1);
                        }
                    }, (nil, returnType) =>
                    {
                        if (returnType == null)
                        {
                            return;
                        }
                        nil.StringAppend(c, returnType);
                        nil.Emit(OpCodes.Pop);
                    });
                }

                il.Emit(OpCodes.Ldarg_0);
                il.Emit(OpCodes.Ldloc_1);
                m = c.CompileTag(t.Do);
                il.Emit(OpCodes.Call, m);

                if (m.ReturnType.FullName != "System.Void")
                {
                    il.DeclareLocal(m.ReturnType);
                    il.Emit(OpCodes.Stloc, index);
                    index++;
                }


                il.MarkLabel(labelNext);

                il.Emit(OpCodes.Ldarg_0);
                il.Emit(OpCodes.Ldloc_1);

                m = c.CompileTag(t.Condition);
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

                il.Emit(OpCodes.Stloc, 2);
                il.Emit(OpCodes.Ldloc, 2);
                il.Emit(OpCodes.Brtrue, labelStart);


                il.Emit(OpCodes.Ldloc, 0);
                il.Call(stringBuilderType, DynamicHelpers.GetMethod(stringBuilderType, "ToString", Type.EmptyTypes));


                il.Emit(OpCodes.Stloc, 3);
                il.Emit(OpCodes.Ldloc, 3);
                il.Emit(OpCodes.Ret);
                return mb.GetBaseDefinition();
            };
        }
        /// <inheritdoc />
        public override Func<ITag, CompileContext, Type> BuildGuessMethod()
        {
            return (tag, c) =>
            {
                return typeof(string);
            };
        }
    }

}