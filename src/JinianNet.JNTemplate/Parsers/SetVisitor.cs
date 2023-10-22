/********************************************************************************
 Copyright (c) jiniannet (http://www.jiniannet.com). All rights reserved.
 Licensed under the MIT license. See licence.txt file in the project root for full license information.
 ********************************************************************************/
using JinianNet.JNTemplate.CodeCompilation;
using JinianNet.JNTemplate.Dynamic;
using JinianNet.JNTemplate.Nodes;
using System;
using System.Reflection;
using System.Reflection.Emit;

namespace JinianNet.JNTemplate.Parsers
{
    /// <summary>
    /// The <see cref="SetTag"/> registrar
    /// </summary>
    public class SetVisitor : TagVisitor<SetTag>, ITagVisitor
    {
        /// <inheritdoc />
        public ITag Parse(TemplateParser parser, TokenCollection tc)
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

                //c.AddChild(new TextTag()
                //{
                //    FirstToken = new Token(TokenKind.Operator, tc.Last.Text[0].ToString())
                //});

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
        public MethodInfo Compile(ITag tag, CompileContext c)
        {
            var getVariableScope = typeof(TemplateContext).GetPropertyGetMethod("TempData");
            //var getVariableValue = typeof(VariableScope).GetMethod("get_Item", new[] { typeof(string) });
            var t = tag as SetTag;
            var type = typeof(void);
            var retunType = c.GuessType(t.Value);
            c.Set(t.Name, retunType);
            var mb = c.CreateReutrnMethod<SetTag>(type);
            var il = mb.GetILGenerator();
            var labelEnd = il.DefineLabel();
            il.DeclareLocal(retunType);
            il.DeclareLocal(typeof(bool));

            var method = c.CompileTag(t.Value);
            il.Emit(OpCodes.Ldarg_0);
            il.Emit(OpCodes.Ldarg_1);
            il.Emit(OpCodes.Call, method);
            il.Emit(OpCodes.Stloc, 0);
            //il.Emit(OpCodes.Ldloc_0);

            il.Emit(OpCodes.Ldarg_1);
            il.Emit(OpCodes.Callvirt, getVariableScope);
            il.Emit(OpCodes.Ldstr, t.Name);
            il.Emit(OpCodes.Ldloc_S, 0);
            il.Emit(OpCodes.Callvirt, typeof(IVariableScope).GetGenericMethod(new Type[] { retunType }, "Update", new Type[] { typeof(string), retunType }));

            il.Emit(OpCodes.Ldc_I4_0);
            il.Emit(OpCodes.Ceq);
            il.Emit(OpCodes.Stloc, 1);
            il.Emit(OpCodes.Ldloc, 1);
            il.Emit(OpCodes.Brfalse_S, labelEnd);

            il.Emit(OpCodes.Ldarg_1);
            il.Emit(OpCodes.Callvirt, getVariableScope);
            il.Emit(OpCodes.Ldstr, t.Name);
            il.Emit(OpCodes.Ldloc_S, 0);
            il.Emit(OpCodes.Callvirt, typeof(IVariableScope).GetGenericMethod(new Type[] { retunType }, "Set", new Type[] { typeof(string), retunType }));


            il.MarkLabel(labelEnd);
            il.Emit(OpCodes.Ret);
            return mb.GetBaseDefinition();

        }
        /// <inheritdoc />
        public Type GuessType(ITag tag, CompileContext c)
        {
            return typeof(void);
        }
        /// <inheritdoc />
        public object Excute(ITag tag, TemplateContext context)
        {
            var t = tag as SetTag;
            object value = context.Execute(t.Value);
            if (value != null)
            {
                if (!context.TempData.Update(t.Name, value))
                {
                    context.TempData.Set(t.Name, value, null);
                }
            }
            return null;
        }
    }
}