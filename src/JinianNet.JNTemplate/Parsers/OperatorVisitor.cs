/********************************************************************************
 Copyright (c) jiniannet (http://www.jiniannet.com). All rights reserved.
 Licensed under the MIT license. See licence.txt file in the project root for full license information.
 ********************************************************************************/
using JinianNet.JNTemplate.CodeCompilation;
using JinianNet.JNTemplate.Nodes;
using System;
using System.Reflection;
using System.Reflection.Emit;

namespace JinianNet.JNTemplate.Parsers;

/// <summary>
/// The <see cref="OperatorVisitor"/> registrar
/// </summary>
public class OperatorVisitor : TagVisitor<OperatorTag>, ITagVisitor
{
    /// <inheritdoc />
    public ITag? Parse(TemplateParser parser, TokenCollection tc)
    {
        return null;
    }
    /// <inheritdoc />
    public MethodInfo? Compile(ITag tag, CompileContext context)
    {
        var t = tag as OperatorTag;
        if (t == null) throw new ArgumentNullException(nameof(tag));
        var type = t.Value.GetType();
        var mb = context.CreateReturnMethod<OperatorTag>(type);
        var il = mb.GetILGenerator();
        il.DeclareLocal(type);
        il.CallTypeTag(t);
        il.Emit(OpCodes.Stloc_0);
        il.Emit(OpCodes.Ldloc_0);
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
        var t = tag as OperatorTag;
        return t?.Value;
    }
}