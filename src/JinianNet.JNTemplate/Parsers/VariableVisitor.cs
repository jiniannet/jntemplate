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
    public class VariableVisitor : TagVisitor<VariableTag>, ITagVisitor
    {
        /// <inheritdoc />
        public ITag Parse(TemplateParser parser, TokenCollection tc)
        {
            if (tc.Count == 1
                && tc.First.TokenKind == TokenKind.TextData)
            {
                var tag = new VariableTag();
                tag.Name = tc.First.Text;
                return tag;
            }
            return null;
        }
        /// <inheritdoc />
        public MethodInfo Compile(ITag tag, CompileContext c)
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
                if (type != typeof(object))
                {
                    if (type.IsValueType)
                    {
                        il.Emit(OpCodes.Unbox_Any, type);
                    }
                    else
                    {
                        il.Emit(OpCodes.Castclass, type);
                    }
                }
                //il.ObjectTo(type);
                il.Emit(OpCodes.Stloc_1);
                il.Emit(OpCodes.Br, labelEnd);
            }
            else
            {
                var parentType = c.GuessType(t.Parent);

                var property = parentType.GetPropertyInfo(t.Name);
                if (property == null)
                {
                    var field = parentType.GetFieldInfo(t.Name);
                    if (field == null)
                    {
                        var indexMethod = parentType.GetMethodInfo("get_Item", new Type[] { typeof(string) });
                        if (indexMethod == null)
                        {
                            throw new CompileException(tag, $"[VariableTag] : {parentType.Name} Cannot find property {t.Name}");
                        }
                        var method = c.CompileTag(t.Parent);
                        il.DeclareLocal(parentType);
                        il.Emit(OpCodes.Ldarg_0);
                        il.Emit(OpCodes.Ldarg_1);
                        il.Emit(OpCodes.Call, method);
                        il.Emit(OpCodes.Stloc, 2);
                        il.LoadVariable(parentType, 2);
                        il.Emit(OpCodes.Ldstr, t.Name);
                        il.Call(parentType, indexMethod);
                    }
                    else
                    {
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
                    }
                    il.Emit(OpCodes.Stloc, 1);
                }
                else
                {
                    var getMethod =
#if NF40 || NF35 || NF20
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
                if (type.IsClass)
                {
                    il.Emit(OpCodes.Ldnull);
                    il.Emit(OpCodes.Stloc, 1);
                }
                else
                {
                    switch (type.Name)
                    {
                        case "Boolean":
                        case "Int16":
                        case "Int32":
                            il.Emit(OpCodes.Ldc_I4, 0);
                            break;
                        case "Int64":
                            il.Emit(OpCodes.Ldc_I8, 0L);
                            break;
                        case "Single":
                            il.Emit(OpCodes.Ldc_R4, 0F);
                            break;
                        case "Double":
                            il.Emit(OpCodes.Ldc_R8, 0D);
                            break;
                        default:
                            var defaultMethod = typeof(Utility).GetGenericMethod(new Type[] { type }, "GenerateDefaultValue", Type.EmptyTypes);
                            il.Emit(OpCodes.Call, defaultMethod);
                            break;
                    }
                    il.Emit(OpCodes.Stloc, 1);
                }
            }
            il.MarkLabel(labelEnd);
            il.Emit(OpCodes.Ldloc_1);
            il.Emit(OpCodes.Ret);
            return mb.GetBaseDefinition();
        }
        /// <inheritdoc />
        public Type GuessType(ITag tag, CompileContext c)
        {
            var t = tag as VariableTag;
            if (t.Parent == null)
            {
                return c.TempData.GetType(t.Name);
            }
            var parentType = c.GuessType(t.Parent);
            if (parentType == typeof(System.Data.DataRow))
                return typeof(object);
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
            var m = parentType.GetMethodInfo("get_Item", new Type[] { typeof(string) });
            if (m != null)
            {
                return m.ReturnType;
            }
            throw new CompileException(tag, $"[VariableTag]: \"{t.Name}\" is not defined");
        }

        /// <inheritdoc />
        public object Excute(ITag tag, TemplateContext context)
        {
            var t = tag as VariableTag;
            object baseValue = null;
            Type type = null;
            if (t.Parent == null)
            {
                return context.TempData[t.Name];
            }
            baseValue = context.Execute(t.Parent);
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
                throw new CompileException(tag, $"[VariableTag]: \"{t.Name}\" is not defined");
            }
            var data = baseValue.CallPropertyOrField(t.Name, type);
            if (data == null && baseValue != null)
            {
                return baseValue.CallIndexValue(t.Name);
            }
            return data;
        }
    }
}