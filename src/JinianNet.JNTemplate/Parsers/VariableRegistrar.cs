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

namespace JinianNet.JNTemplate.Parsers
{

    /// <summary>
    /// The <see cref="VariableTag"/> registrar
    /// </summary>
    public class VariableRegistrar : TagRegistrar<VariableTag>, IRegistrar
    {
        /// <inheritdoc />
        public override Func<TemplateParser, TokenCollection, ITag> BuildParseMethod()
        {
            return (parser, tc) =>
            {
                if (tc != null
                    && tc.Count == 1
                    && tc.First.TokenKind == TokenKind.TextData)
                {
                    var tag = new VariableTag();
                    tag.Name = tc.First.Text;
                    return tag;
                }
                return null;
            };
        }
        /// <inheritdoc />
        public override Func<ITag, CompileContext, MethodInfo> BuildCompileMethod()
        {
            return (tag, c) =>
            {
                var getVariableScope = typeof(TemplateContext).GetPropertyGetMethod("TempData");
                var getVariableValue = typeof(IVariableScope).GetMethod("get_Item", new[] { typeof(string) });
                var t = tag as VariableTag;
                var type = c.GuessType(t);
                var mb = c.CreateReutrnMethod<VariableTag>(type);
                var il = mb.GetILGenerator();
                Label labelEnd = il.DefineLabel();
                Label labelInit = il.DefineLabel();
                il.DeclareLocal(typeof(object));
                il.DeclareLocal(type);
                if (t.Parent == null)
                {
                    il.DeclareLocal(typeof(bool));
                    il.Emit(OpCodes.Ldarg_1);
                    il.Emit(OpCodes.Callvirt, getVariableScope);
                    il.Emit(OpCodes.Ldstr, t.Name);
                    il.Emit(OpCodes.Callvirt, getVariableValue);
                    il.Emit(OpCodes.Stloc_0);
                    il.Emit(OpCodes.Ldloc_0);
                    il.Emit(OpCodes.Ldnull);
                    il.Emit(OpCodes.Cgt_Un);
                    il.Emit(OpCodes.Stloc_2);
                    il.Emit(OpCodes.Ldloc_2);
                    il.Emit(OpCodes.Brfalse, labelInit);


                    il.Emit(OpCodes.Ldloc_0);
                    il.ObjectTo(type);
                    il.Emit(OpCodes.Stloc_1);
                    il.Emit(OpCodes.Br, labelEnd);
                }
                else
                {
                    var parentType = c.GuessType(t.Parent);

                    var property = parentType.GetPropertyInfo( t.Name);
                    if (property == null)
                    {
                        var field = parentType.GetFieldInfo(t.Name);
                        if (field == null)
                        {
                            throw new CompileException(tag, $"[VariableTag] : {parentType.Name} Cannot find property {t.Name}");
                        }
                        if (!field.IsStatic)
                        {
                            var method = c.CompileTag(t.Parent);
                            il.DeclareLocal(parentType);
                            il.Emit(OpCodes.Ldarg_0);
                            il.Emit(OpCodes.Ldarg_1);
                            il.Emit(OpCodes.Call, method);
                            il.Emit(OpCodes.Stloc, 2);
                            il.LoadVariable(parentType, 2);
                            il.Emit(OpCodes.Ldfld, field);
                        }
                        else
                        {
                            il.Emit(OpCodes.Ldsfld, field);
                        }
                        il.Emit(OpCodes.Stloc, 1);
                    }
                    else
                    {
                        var getMethod =
#if NET40
                        property.GetGetMethod();
#else
                        property.GetMethod;
#endif
                        if (!getMethod.IsStatic)
                        {
                            var method = c.CompileTag(t.Parent);
                            il.DeclareLocal(parentType);
                            il.Emit(OpCodes.Ldarg_0);
                            il.Emit(OpCodes.Ldarg_1);
                            il.Emit(OpCodes.Call, method);
                            il.Emit(OpCodes.Stloc, 2);
                            il.LoadVariable(parentType, 2);
                        }
                        il.Call(parentType, getMethod);
                        il.Emit(OpCodes.Stloc, 1);
                    }
                    il.Emit(OpCodes.Br, labelEnd);
                }

                il.MarkLabel(labelInit);
                if (t.Parent == null)
                {
                    var defaultMethod = typeof(TemplateCompiler).GetGenericMethod(new Type[] { type }, "GenerateDefaultValue", Type.EmptyTypes);
                    il.Emit(OpCodes.Call, defaultMethod);
                    il.Emit(OpCodes.Stloc, 1);
                }
                il.MarkLabel(labelEnd);
                il.Emit(OpCodes.Ldloc_1);
                il.Emit(OpCodes.Ret);
                return mb.GetBaseDefinition();
            };
        }
        /// <inheritdoc />
        public override Func<ITag, CompileContext, Type> BuildGuessMethod()
        {
            return (tag, c) =>
            {
                var t = tag as VariableTag;
                if (t.Parent == null)
                {
                    return c.Data.GetType(t.Name);
                }
                var parentType = c.GuessType(t.Parent);
                var p = parentType.GetPropertyInfo(t.Name);
                if (p != null)
                {
                    return p.PropertyType;
                }
                var f = parentType.GetFieldInfo(t.Name);
                if (f != null)
                {
                    return f.FieldType;
                }
                throw new CompileException(tag, $"[VariableTag]: \"{t.Name}\" is not defined");
            };
        }

        /// <inheritdoc />
        public override Func<ITag, TemplateContext, object> BuildExcuteMethod()
        {
            return ((tag, context) =>
            {
                var t = tag as VariableTag;
                object baseValue = null;
                Type type = null;
                if (t.Parent == null)
                {
                    return context.TempData[t.Name];
                }
                baseValue = TagExecutor.Execute(t.Parent, context);
                if (baseValue == null && t.Parent is VariableTag variable)
                {
                    type = context.TempData.GetType(variable.Name);
                }
                else
                {
                    type = baseValue.GetType();
                }
                if (type == null)
                {
                    return null;
                }
                return baseValue.CallPropertyOrField(t.Name, type);
            });
        }
    }
}