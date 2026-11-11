/********************************************************************************
 Copyright (c) jiniannet (http://www.jiniannet.com). All rights reserved.
 Licensed under the MIT license. See licence.txt file in the project root for full license information.
 ********************************************************************************/
using JinianNet.JNTemplate.CodeCompilation;
using JinianNet.JNTemplate.Dynamic;
using JinianNet.JNTemplate.Nodes;
using JinianNet.JNTemplate.Exceptions;
using System;
using System.Reflection;
using System.Reflection.Emit;

namespace JinianNet.JNTemplate.Parsers;

/// <summary>
/// The <see cref="IfTag"/> registrar
/// </summary>
public class IfVisitor : TagVisitor<IfTag>, ITagVisitor
{
    /// <inheritdoc />
    public ITag? Parse(TemplateParser parser, TokenCollection tc)
    {
        if (tc.Count > 3
            && Utility.IsEqual(tc.First.Text, Const.KEY_IF))
        {

            if (tc[1].TokenKind == TokenKind.LeftParentheses
               && tc.Last.TokenKind == TokenKind.RightParentheses)
            {
                var tag = new IfTag();

                var t = new ElseifTag();
                TokenCollection coll = tc[2, -1];
                t.Condition = parser.ReadSimple(coll);
                if (t.Condition == null)
                    return null;
                t.FirstToken = coll.First;
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
    }

    /// <inheritdoc />
    public MethodInfo? Compile(ITag tag, CompileContext context)
    {
        var t = tag as IfTag;
        if (t == null)
            throw new ArgumentNullException(nameof(tag));
        var type = context.GuessType(t);
        var mb = context.CreateReturnMethod<IfTag>(type);
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
            var ifTag = t.Children[i] as ElseifTag ?? throw new CompileException("the if tag is invalid.");
            if (!(t.Children[i] is ElseTag))
            {
                var m = context.CompileRequiredMethod(ifTag.Condition ?? new BooleanTag() { Value = false });
                il.Emit(OpCodes.Ldarg_0);
                il.Emit(OpCodes.Ldarg_1);
                il.Emit(OpCodes.Call, m);
                if (m.ReturnType.Name != "Boolean")
                {
                    //var localVar = il.DeclareLocal(m.ReturnType);
                    //il.Emit(OpCodes.Stloc, localVar.LocalIndex);
                    //il.LoadVariable(m.ReturnType, localVar.LocalIndex);
                    var cm = typeof(Utility).GetMethodInfo("ToBoolean", [m.ReturnType]);
                    if (cm == null)
                    {
                        cm = typeof(Utility).GetRequiredMethod("ToBoolean", [typeof(object)]);
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
                il.Emit(OpCodes.Brfalse, lables[i]);
            }

            var execute = context.CompileRequiredMethod(ifTag);
            il.Emit(OpCodes.Ldarg_0);
            il.Emit(OpCodes.Ldarg_1);
            il.Emit(OpCodes.Call, execute);
            if (execute.ReturnType.FullName != type.FullName)
            {
                var tCode = type.ToTypeCode();
                var rCode = execute.ReturnType.ToTypeCode();
                if (tCode == TypeCode.String)
                {
                    var localVar = il.DeclareLocal(execute.ReturnType);
                    il.Emit(OpCodes.Stloc, localVar.LocalIndex);
                    il.LoadVariable(execute.ReturnType, localVar.LocalIndex);
                    il.Call(execute.ReturnType, typeof(object).GetRequiredMethod("ToString", Type.EmptyTypes));
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
                            switch (tCode)
                            {
                                case TypeCode.Decimal:
                                {
                                    var ctorType = execute.ReturnType;
                                    switch (rCode)
                                    {
                                        case TypeCode.Int16:
                                        case TypeCode.UInt16:
                                        case TypeCode.SByte:
                                        case TypeCode.Byte:
                                            // decimal 没有 Int16/UInt16/Byte 的构造函数，统一按 Int32 处理
                                            il.Emit(OpCodes.Conv_I4);
                                            ctorType = typeof(int);
                                            break;
                                    }
                                    // 值类型的构造函数需通过 newobj 构造（call 需要栈上已存在地址）
                                    il.Emit(OpCodes.Newobj, typeof(decimal).GetRequiredConstructor([ctorType]));
                                    break;
                                }
                                case TypeCode.Double:
                                    il.Emit(OpCodes.Conv_R8);
                                    break;
                                case TypeCode.Single:
                                    il.Emit(OpCodes.Conv_R4);
                                    break;
                                case TypeCode.Int64:
                                    il.Emit(OpCodes.Conv_I8);
                                    break;
                                case TypeCode.UInt64:
                                    il.Emit(OpCodes.Conv_U8);
                                    break;
                                case TypeCode.Int32:
                                    il.Emit(OpCodes.Conv_I4);
                                    break;
                                case TypeCode.UInt32:
                                    il.Emit(OpCodes.Conv_U4);
                                    break;
                                case TypeCode.Int16:
                                    il.Emit(OpCodes.Conv_I2);
                                    break;
                                case TypeCode.UInt16:
                                    il.Emit(OpCodes.Conv_U2);
                                    break;
                                case TypeCode.Byte:
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
                var defaultMethod = typeof(Utility).GetRequiredGenericMethod("GenerateDefaultValue", [type]);
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
    }
    /// <inheritdoc />
    public Type? GuessType(ITag tag, CompileContext context)
    {
        return typeof(string);
    }
    /// <inheritdoc />
    public object? Execute(ITag tag, TemplateContext context)
    {
        var t = tag as IfTag;
        if (t == null) return null;
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

            var condition = c.Condition == null ? null : context.Execute(c.Condition);
            if (Utility.ToBoolean(condition))
            {
                return context.Execute(t.Children[i]);
            }
        }
        return null;
    }

}