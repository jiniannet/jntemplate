/********************************************************************************
 Copyright (c) jiniannet (http://www.jiniannet.com). All rights reserved.
 Licensed under the MIT license. See licence.txt file in the project root for full license information.
 ********************************************************************************/
using JinianNet.JNTemplate.CodeCompilation;
using JinianNet.JNTemplate.Dynamic;
using JinianNet.JNTemplate.Exceptions;
using JinianNet.JNTemplate.Nodes;
using System;
using System.Reflection;
using System.Reflection.Emit;

namespace JinianNet.JNTemplate.Parsers;

/// <summary>
/// The <see cref="SetTag"/> registrar
/// </summary>
public class SetVisitor : TagVisitor<SetTag>, ITagVisitor
{
    /// <inheritdoc />
    public ITag? Parse(TemplateParser parser, TokenCollection tc)
    {
        if (tc.Count > 5
            && Utility.IsEqual(tc.First.Text, Const.KEY_SET)
            && tc[1].TokenKind == TokenKind.LeftParentheses
            && tc[3].Text == "="
            && tc.Last.TokenKind == TokenKind.RightParentheses)
        {
            var tag = new SetTag();
            tag.Name = tc[2].Text;

            tag.Value = parser.ReadSimple(tc[4, -1]);
            return tag;

        }

        if (tc.Count == 2
            && tc.First.TokenKind == TokenKind.TextData
            && tc.Last.TokenKind == TokenKind.Arithmetic
            && (tc.Last.Text == "++" || tc.Last.Text == "--"))
        {
            var tag = new SetTag();
            tag.Name = tc.First.Text;

            var c = new ArithmeticTag();
            c.AddChild(new VariableTag()
            {
                FirstToken = tc.First,
                Name = tc.First.Text
            });
            c.AddChild(new OperatorTag(new Token(TokenKind.Arithmetic, tc.Last.Text[0].ToString())));

            c.AddChild(new NumberTag()
            {
                Value = 1,
                FirstToken = new Token(TokenKind.Number, "1")
            });

            tag.Value = c;
            return tag;
        }

        if (tc.Count > 2
            && tc.First.TokenKind == TokenKind.TextData
            && tc[1].Text == "=")
        {
            var tag = new SetTag();
            tag.Name = tc.First.Text;
            tag.Value = parser.ReadSimple(tc[2, tc.Count]);
            if (tag.Value == null)
                return null;
            return tag;
        }

        return null;
    }
    /// <inheritdoc />
    public MethodInfo? Compile(ITag tag, CompileContext context)
    {
        var getVariableScope = typeof(TemplateContext).GetRequiredPropertyGetMethod("TempData");
        var t = tag as SetTag;
        if (t == null || t.Value == null || t.Name == null) throw new ArgumentNullException(nameof(tag));
        var type = typeof(void);
        var retunType = context.GuessType(t.Value);
        context.Set(t.Name, retunType);
        var mb = context.CreateReturnMethod<SetTag>(type);
        var il = mb.GetILGenerator();
        var labelEnd = il.DefineLabel();
        il.DeclareLocal(retunType);
        il.DeclareLocal(typeof(bool));

        var method = context.CompileRequiredMethod(t.Value);
        il.Emit(OpCodes.Ldarg_0);
        il.Emit(OpCodes.Ldarg_1);
        il.Emit(OpCodes.Call, method);
        il.Emit(OpCodes.Stloc, 0);

        il.Emit(OpCodes.Ldarg_1);
        il.Emit(OpCodes.Callvirt, getVariableScope);
        il.Emit(OpCodes.Ldstr, t.Name);
        il.Emit(OpCodes.Ldloc_S, 0);
        il.Emit(OpCodes.Callvirt, typeof(IVariableScope).GetRequiredGenericMethod("Update", [typeof(string), retunType], [retunType]));

        il.Emit(OpCodes.Ldc_I4_0);
        il.Emit(OpCodes.Ceq);
        il.Emit(OpCodes.Stloc, 1);
        il.Emit(OpCodes.Ldloc, 1);
        il.Emit(OpCodes.Brfalse_S, labelEnd);

        il.Emit(OpCodes.Ldarg_1);
        il.Emit(OpCodes.Callvirt, getVariableScope);
        il.Emit(OpCodes.Ldstr, t.Name);
        il.Emit(OpCodes.Ldloc_S, 0);
        il.Emit(OpCodes.Callvirt, typeof(IVariableScope).GetRequiredGenericMethod("Set", [typeof(string), retunType], [retunType]));


        il.MarkLabel(labelEnd);
        il.Emit(OpCodes.Ret);
        return mb.GetBaseDefinition();

    }
    /// <inheritdoc />
    public Type? GuessType(ITag tag, CompileContext context)
    {
        return typeof(void);
    }
    /// <inheritdoc />
    public object? Execute(ITag tag, TemplateContext context)
    {
        var t = tag as SetTag;
        if (t == null || t.Value == null || t.Name == null || context.TempData == null) return null;
        var value = context.Execute(t.Value);
        if (value != null && !context.TempData.Update(t.Name, value))
        {
            context.TempData.Set(t.Name, value, null);
        }
        return null;
    }
}