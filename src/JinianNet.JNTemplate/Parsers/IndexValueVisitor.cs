/********************************************************************************
 Copyright (c) jiniannet (http://www.jiniannet.com). All rights reserved.
 Licensed under the MIT license. See licence.txt file in the project root for full license information.
 ********************************************************************************/
using System;
using System.Reflection;
using System.Reflection.Emit;
using JinianNet.JNTemplate.CodeCompilation;
using JinianNet.JNTemplate.Dynamic;
using JinianNet.JNTemplate.Exceptions;
using JinianNet.JNTemplate.Nodes;

namespace JinianNet.JNTemplate.Parsers;

/// <summary>
/// The <see cref="IndexValueTag"/> registrar
/// </summary>
public class IndexValueVisitor : TagVisitor<IndexValueTag>, ITagVisitor
{
    /// <inheritdoc />
    public ITag? Parse(TemplateParser parser, TokenCollection tc)
    {
        if (tc != null
                       && parser != null
                       && tc.Count >= 3
                       && tc[0].TokenKind == TokenKind.LeftBracket
                       && tc.Last.TokenKind == TokenKind.RightBracket)
        {
            int y = tc.Count - 1;
            int x = -1;
            int pos = 0;

            for (int i = y; i >= 0; i--)
            {
                if (tc[i].TokenKind == TokenKind.Dot && pos == 0)
                {
                    return null;
                }
                if (tc[i].TokenKind == TokenKind.RightBracket)
                {
                    pos++;
                    continue;
                }
                if (tc[i].TokenKind == TokenKind.LeftBracket)
                {
                    pos--;
                    if (pos == 0 && x == -1)
                    {
                        x = i;
                    }
                }
            }

            if (x == -1)
            {
                return null;
            }
            var tag = new IndexValueTag();
            tag.Index = parser.ReadSimple(tc[x + 1, y]);
            if (tag.Index == null)
                return null;
            return tag;
        }

        return null;
    }
    /// <inheritdoc />
    public MethodInfo? Compile(ITag tag, CompileContext context)
    {
        var t = tag as IndexValueTag;
        if (t == null || t.Index == null) throw new ArgumentNullException(nameof(tag));
        var type = context.GuessType(t);
        var mb = context.CreateReturnMethod<IndexValueTag>(type);
        var il = mb.GetILGenerator();
        if (t.Parent == null)
        {
            throw new CompileException(tag, $"[IndexValutTag] : tag error on {t.ToSource()}.");
        }
        var parentType = context.GuessType(t.Parent);
        bool toArray = false;
        if (parentType.ToTypeCode() == TypeCode.String)
        {
            toArray = true;
            parentType = typeof(char[]);
        }
        var indexType = context.GuessType(t.Index);
        il.DefineLabel();
        il.DefineLabel();
        il.DeclareLocal(parentType);
        il.DeclareLocal(indexType);
        il.DeclareLocal(type);


        il.Emit(OpCodes.Ldarg_0);
        il.Emit(OpCodes.Ldarg_1);
        il.Emit(OpCodes.Call, context.CompileRequiredMethod(t.Parent));
        if (toArray)
        {
            il.DeclareLocal(typeof(string));
            il.Emit(OpCodes.Stloc_3);
            il.Emit(OpCodes.Ldloc_3);
            il.Emit(OpCodes.Call, typeof(string).GetRequiredMethod("ToCharArray", Type.EmptyTypes));
        }
        il.Emit(OpCodes.Stloc_0);


        il.Emit(OpCodes.Ldarg_0);
        il.Emit(OpCodes.Ldarg_1);
        il.Emit(OpCodes.Call, context.CompileRequiredMethod(t.Index));
        il.Emit(OpCodes.Stloc_1);


        il.Emit(OpCodes.Ldloc, 0);
        il.Emit(OpCodes.Ldloc, 1);
        if (parentType.IsArray)
        {
            il.Ldelem(type);
            il.Emit(OpCodes.Stloc_2);
        }
        else
        {
            var getItem = parentType.GetRequiredPropertyGetMethod("Item", [indexType]);
            ParameterInfo[]? ps = getItem.GetParameters();
            if (ps == null || ps.Length != 1)
            {
                throw new CompileException(tag, $"[IndexValutTag] : Cannot not compile.");
            }
            if (indexType != ps[0].ParameterType)
            {
                il.ConvertTo(indexType, ps[0].ParameterType);
            }
            il.Emit(OpCodes.Callvirt, getItem);
            il.Emit(OpCodes.Stloc_2);
        }

        il.Emit(OpCodes.Ldloc_2);
        il.Emit(OpCodes.Ret);
        return mb.GetBaseDefinition();
    }
    /// <inheritdoc />
    public Type? GuessType(ITag tag, CompileContext context)
    {
        var t = tag as IndexValueTag;
        if (t == null || t.Index == null) throw new ArgumentNullException(nameof(tag));
        if (t.Parent == null)
        {
            throw new CompileException(tag, "[IndexValueTag] : Parent cannot be null");
        }
        var parentType = context.GuessType(t.Parent);
        if (parentType.ToTypeCode() == TypeCode.String)
        {
            return typeof(char);
        }
        var indexType = context.GuessType(t.Index);
        if (parentType.IsArray)
        {
            var method = parentType.GetMethodInfo("get", [indexType]);
            if (method != null)
            {
                return method.ReturnType;
            }
        }
        var m = parentType.GetMethodInfo("get_Item", [indexType]);
        if (m != null)
        {
            return m.ReturnType;
        }

        throw new CompileException(tag, $"[IndexValueTag]: \"{tag.ToSource()}\" is not defined");
    }
    /// <inheritdoc />
    public object? Execute(ITag tag, TemplateContext context)
    {
        var t = tag as IndexValueTag;
        if (t == null || t.Parent == null || t.Index == null) return null;
        var obj = context.Execute(t.Parent);
        var index = context.Execute(t.Index);
        return obj == null || index == null ? null : obj.CallIndexValue(index);
    }
}
