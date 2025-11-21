/********************************************************************************
 Copyright (c) jiniannet (http://www.jiniannet.com). All rights reserved.
 Licensed under the MIT license. See licence.txt file in the project root for full license information.
 ********************************************************************************/
using System;
using System.Collections.Generic;
using System.IO;
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
    /// The <see cref="ForTag"/> registrar
    /// </summary>
    public class ForVisitor : TagVisitor<ForTag>, ITagVisitor
    {
        /// <inheritdoc />
        public ITag Parse(TemplateParser parser, TokenCollection tc)
        {

            if (tc.Count > 3
                            && Utility.IsEqual(Const.KEY_FOR, tc.First.Text))
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
                        if (tc[i].TokenKind == TokenKind.Punctuation && tc[i].Text == ";" && pos == 0)
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
                        throw new ParseException($"syntax error near for on {tc.ToString()}", tc.First.BeginLine, tc.First.BeginColumn);
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

                    throw new ParseException($"for is not properly closed by a end tag: {tc.ToString()}", tc.First.BeginLine, tc.First.BeginColumn);
                }
                else
                {
                    throw new ParseException($"syntax error near for: {tc.ToString()}", tc.First.BeginLine, tc.First.BeginColumn);
                }
            }

            return null;

        }

        /// <inheritdoc />
        public MethodInfo Compile(ITag tag, CompileContext context)
        {

            var stringBuilderType = typeof(StringBuilder);
            var t = tag as ForTag;
            var type = context.GuessType(t);
            var templateContextType = typeof(TemplateContext);
            var mb = context.CreateReutrnMethod<ForTag>(type);
            var il = mb.GetILGenerator(); 
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
            il.Emit(OpCodes.Call, templateContextType.GetMethodInfo("CreateContext", new Type[] { templateContextType }));
            il.Emit(OpCodes.Stloc_1);

            il.Emit(OpCodes.Ldarg_0);
            il.Emit(OpCodes.Ldloc_1);
            var m = context.CompileTag(t.Initial);
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
                il.CallTag(context, t.Children[i], (nil, hasReturn, needCall) =>
                {
                    if (hasReturn)
                    {
                        nil.Emit(OpCodes.Ldloc_0);
                    }
                    if (needCall)
                    {
                        nil.Emit(OpCodes.Ldarg_0);
                        nil.Emit(OpCodes.Ldloc_1);
                    }
                }, (nil, returnType) =>
                {
                    if (returnType == null)
                    {
                        return;
                    }
                    nil.StringAppend(context, returnType);
                    nil.Emit(OpCodes.Pop);
                });
            }

            il.Emit(OpCodes.Ldarg_0);
            il.Emit(OpCodes.Ldloc_1);
            m = context.CompileTag(t.Do);
            il.Emit(OpCodes.Call, m);

            if (m.ReturnType.FullName != "System.Void")
            {
                il.DeclareLocal(m.ReturnType);
                il.Emit(OpCodes.Stloc, index); 
            }


            il.MarkLabel(labelNext);

            il.Emit(OpCodes.Ldarg_0);
            il.Emit(OpCodes.Ldloc_1);

            m = context.CompileTag(t.Condition);
            il.Emit(OpCodes.Call, m);

            if (m.ReturnType.Name != "Boolean")
            {
                var cm = typeof(Utility).GetMethodInfo("ToBoolean", new Type[] { m.ReturnType });
                if (cm == null)
                {
                    cm = typeof(Utility).GetMethodInfo("ToBoolean", new Type[] { typeof(object) });
                    if (m.ReturnType.IsValueType)
                    {
                        il.Emit(OpCodes.Box, m.ReturnType);
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
            il.Call(stringBuilderType, stringBuilderType.GetMethodInfo("ToString", Type.EmptyTypes));


            il.Emit(OpCodes.Stloc, 3);
            il.Emit(OpCodes.Ldloc, 3);
            il.Emit(OpCodes.Ret);
            return mb.GetBaseDefinition();

        }
        /// <inheritdoc />
        public Type GuessType(ITag tag, CompileContext context)
        {
            return typeof(string);
        }
        /// <inheritdoc />
        public object Excute(ITag tag, TemplateContext context)
        {
            var t = tag as ForTag;
            context.Execute(t.Initial);
            //如果标签为空，则直接为false,避免死循环以内存溢出
            bool run;

            if (t.Condition == null)
            {
                run = false;
            }
            else
            {
                run = Utility.ToBoolean(context.Execute(t.Condition));
            }
            using (var writer = new StringWriter())
            {
                while (run)
                {
                    for (int i = 0; i < t.Children.Count; i++)
                    {
                        var obj = context.Execute(t.Children[i]);
                        if (obj != null)
                        {
                            writer.Write(obj.ToString());
                        }
                    }

                    if (t.Do != null)
                    {
                        context.Execute(t.Do);
                    }
                    run = Utility.ToBoolean(context.Execute(t.Condition));
                }
                return writer.ToString();
            }
        }
    }
}