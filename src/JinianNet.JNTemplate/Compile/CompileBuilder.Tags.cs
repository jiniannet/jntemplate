using JinianNet.JNTemplate.Dynamic;
using JinianNet.JNTemplate.Exception;
using JinianNet.JNTemplate.Nodes;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Reflection.Emit;
using System.Text;

namespace JinianNet.JNTemplate.Compile
{
    /// <inheritdoc />
    public partial class CompileBuilder
    {

        /// <summary>
        /// 初始化编译方法
        /// </summary>
        public void Initialize()
        {
            #region func
            //BlockTag,
            //ForTag,JsonTag,LayoutTag//
            this.Register<VariableTag>((tag, c) =>
            {
                var t = tag as VariableTag;
                var type = Compiler.TypeGuess.GetType(t, c);
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
                    var parentType = Compiler.TypeGuess.GetType(t.Parent, c);

                    var property = DynamicHelpers.GetPropertyInfo(parentType, t.Name);
                    if (property == null)
                    {
                        var field = DynamicHelpers.GetFieldInfo(parentType, t.Name);
                        if (field == null)
                        {
                            throw new CompileException($"[VariableTag] : {parentType.Name} Cannot find property {t.Name}");
                        }
                        if (!field.IsStatic)
                        {
                            var method = GetCompileMethod(t.Parent, c);
                            il.DeclareLocal(parentType);
                            il.Emit(OpCodes.Ldarg_0);
                            il.Emit(OpCodes.Ldarg_1);
                            il.Emit(OpCodes.Call, method);
                            il.Emit(OpCodes.Stloc, 2);
                            LoadVariable(il, parentType, 2);
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
                            var method = GetCompileMethod(t.Parent, c);
                            il.DeclareLocal(parentType);
                            il.Emit(OpCodes.Ldarg_0);
                            il.Emit(OpCodes.Ldarg_1);
                            il.Emit(OpCodes.Call, method);
                            il.Emit(OpCodes.Stloc, 2);
                            LoadVariable(il, parentType, 2);
                        }
                        il.Call(parentType, getMethod);
                        //il.Emit(OpCodes.Callvirt, property.GetMethod);
                        //il.Emit(OpCodes.Call, property.GetMethod);
                        il.Emit(OpCodes.Stloc, 1);
                    }
                    il.Emit(OpCodes.Br, labelEnd);
                }

                il.MarkLabel(labelInit);
                if (t.Parent == null)
                {
                    var defaultMethod = DynamicHelpers.GetGenericMethod(typeof(CompileBuilder), new Type[] { type }, "GenerateDefaultValue", Type.EmptyTypes);
                    il.Emit(OpCodes.Call, defaultMethod);
                    il.Emit(OpCodes.Stloc, 1);
                }
                il.MarkLabel(labelEnd);
                //if (type.IsValueType)
                //{
                //    il.Emit(OpCodes.Ldloca,1);
                //}
                //else
                //{
                //    il.Emit(OpCodes.Ldloc_1);
                //}
                il.Emit(OpCodes.Ldloc_1);
                il.Emit(OpCodes.Ret);
                return mb.GetBaseDefinition();
            });
            this.Register<IndexValueTag>((tag, c) =>
            {
                var t = tag as IndexValueTag;
                var type = Compiler.TypeGuess.GetType(t, c);
                var mb = c.CreateReutrnMethod<IndexValueTag>(type);
                var il = mb.GetILGenerator();
                var parentType = Compiler.TypeGuess.GetType(t.Parent, c);
                bool toArray = false;
                if (parentType.FullName == "System.String")
                {
                    toArray = true;
                    parentType = typeof(char[]);
                }
                var indexType = Compiler.TypeGuess.GetType(t.Index, c);
                Label labelEnd = il.DefineLabel();
                Label labelInit = il.DefineLabel();
                il.DeclareLocal(parentType);
                il.DeclareLocal(indexType);
                il.DeclareLocal(type);


                il.Emit(OpCodes.Ldarg_0);
                il.Emit(OpCodes.Ldarg_1);
                il.Emit(OpCodes.Call, GetCompileMethod(t.Parent, c));
                if (toArray)
                {
                    il.DeclareLocal(typeof(string));
                    il.Emit(OpCodes.Stloc_3);
                    il.Emit(OpCodes.Ldloc_3);
                    il.Emit(OpCodes.Call, DynamicHelpers.GetMethod(typeof(string), "ToCharArray", Type.EmptyTypes));
                }
                il.Emit(OpCodes.Stloc_0);


                il.Emit(OpCodes.Ldarg_0);
                il.Emit(OpCodes.Ldarg_1);
                il.Emit(OpCodes.Call, GetCompileMethod(t.Index, c));
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
                    var getItem = DynamicHelpers.GetMethod(parentType, "get_Item", new Type[] { indexType });
                    if (getItem == null)
                    {
                        throw new CompileException($"[IndexValutTag] : Cannot not compile.");
                    }
                    il.Emit(OpCodes.Callvirt, getItem);
                    il.Emit(OpCodes.Stloc_2);
                }

                il.Emit(OpCodes.Ldloc_2);
                il.Emit(OpCodes.Ret);
                return mb.GetBaseDefinition();
            });
            this.Register<SetTag>((tag, c) =>
            {
                var t = tag as SetTag;
                var type = typeof(void);
                var retunType = Compiler.TypeGuess.GetType(t.Value, c);
                c.Set(t.Name, retunType);
                var mb = c.CreateReutrnMethod<SetTag>(type);
                var il = mb.GetILGenerator();
                Label labelEnd = il.DefineLabel();
                il.DeclareLocal(retunType);
                il.DeclareLocal(typeof(bool));

                var method = GetCompileMethod(t.Value, c);
                il.Emit(OpCodes.Ldarg_0);
                il.Emit(OpCodes.Ldarg_1);
                il.Emit(OpCodes.Call, method);
                il.Emit(OpCodes.Stloc, 0);
                //il.Emit(OpCodes.Ldloc_0);

                il.Emit(OpCodes.Ldarg_1);
                il.Emit(OpCodes.Callvirt, getVariableScope);
                il.Emit(OpCodes.Ldstr, t.Name);
                il.Emit(OpCodes.Ldloc_S, 0);
                il.Emit(OpCodes.Callvirt, DynamicHelpers.GetGenericMethod(typeof(VariableScope), new Type[] { retunType }, "Update", new Type[] { typeof(string), retunType }));

                il.Emit(OpCodes.Ldc_I4_0);
                il.Emit(OpCodes.Ceq);
                il.Emit(OpCodes.Stloc, 1);
                il.Emit(OpCodes.Ldloc, 1);
                il.Emit(OpCodes.Brfalse_S, labelEnd);

                il.Emit(OpCodes.Ldarg_1);
                il.Emit(OpCodes.Callvirt, getVariableScope);
                il.Emit(OpCodes.Ldstr, t.Name);
                il.Emit(OpCodes.Ldloc_S, 0);
                il.Emit(OpCodes.Callvirt, DynamicHelpers.GetGenericMethod(typeof(VariableScope), new Type[] { retunType }, "Set", new Type[] { typeof(string), retunType }));


                il.MarkLabel(labelEnd);
                il.Emit(OpCodes.Ret);
                return mb.GetBaseDefinition();
            });
            this.Register<BooleanTag>((tag, c) =>
            {
                var t = tag as BooleanTag;
                var type = t.Value.GetType();
                var mb = c.CreateReutrnMethod<BooleanTag>(type);
                var il = mb.GetILGenerator();
                il.DeclareLocal(type);
                il.Emit(OpCodes.Ldc_I4, t.Value ? 1 : 0);
                il.Emit(OpCodes.Stloc_0);
                il.Emit(OpCodes.Ldloc_0);
                il.Emit(OpCodes.Ret);
                return mb.GetBaseDefinition();
            });
            this.Register<EndTag>((tag, c) =>
            {
                var type = typeof(string);
                var mb = c.CreateReutrnMethod<EndTag>(type);
                var il = mb.GetILGenerator();
                il.DeclareLocal(type);
                il.Emit(OpCodes.Ldstr, string.Empty);
                il.Emit(OpCodes.Stloc_0);
                il.Emit(OpCodes.Ldloc_0);
                il.Emit(OpCodes.Ret);
                return mb.GetBaseDefinition();
            });
            this.Register<NumberTag>((tag, c) =>
            {
                var t = tag as NumberTag;
                var type = t.Value.GetType();
                var mb = c.CreateReutrnMethod<NumberTag>(type);
                var il = mb.GetILGenerator();
                il.DeclareLocal(type);
                switch (type.Name)
                {
                    case "Int32":
                        il.Emit(OpCodes.Ldc_I4, (int)t.Value);
                        break;
                    case "Int64":
                        il.Emit(OpCodes.Ldc_I8, (long)t.Value);
                        break;
                    case "Single":
                        il.Emit(OpCodes.Ldc_R4, (float)t.Value);
                        break;
                    case "Double":
                        il.Emit(OpCodes.Ldc_R8, (double)t.Value);
                        break;
                    case "Int16":
                        il.Emit(OpCodes.Ldc_I4, (Int16)t.Value);
                        break;
                    default:
                        throw new NotSupportedException($"[NumberTag] : [{type.FullName}] is not supported");
                }
                il.Emit(OpCodes.Stloc_0);
                il.Emit(OpCodes.Ldloc_0);
                il.Emit(OpCodes.Ret);
                return mb.GetBaseDefinition();
            });
            this.Register<StringTag>((tag, c) =>
            {
                var t = tag as StringTag;
                var type = typeof(string);
                var mb = c.CreateReutrnMethod<StringTag>(type);
                var il = mb.GetILGenerator();
                il.Emit(OpCodes.Ldstr, t.Value);
                il.Emit(OpCodes.Ret);
                return mb.GetBaseDefinition();
            });
            this.Register<TextTag>((tag, c) =>
            {
                var t = tag as TextTag;
                if (!string.IsNullOrEmpty(t.Text))
                {
                    var type = typeof(string);
                    var mb = c.CreateReutrnMethod<TextTag>(type);
                    var il = mb.GetILGenerator();
                    il.Emit(OpCodes.Ldstr, t.Text);
                    il.Emit(OpCodes.Ret);
                    return mb.GetBaseDefinition();
                }
                return null;
            });
            this.Register<FunctaionTag>((tag, c) =>
            {
                var t = tag as FunctaionTag;
                Type baseType;
                MethodInfo method;
                Type[] paramType = new Type[t.Children.Count];
                for (int i = 0; i < t.Children.Count; i++)
                {
                    paramType[i] = Compiler.TypeGuess.GetType(t.Children[i], c);
                    if (paramType[i].FullName == "System.Void")
                    {
                        throw new CompileException("[FunctaionTag]:parameter error");
                    }
                }

                MethodInfo childMethd = null;
                FieldInfo field = null;
                if (t.Parent != null)
                {
                    baseType = Compiler.TypeGuess.GetType(t.Parent, c);
                    method = DynamicHelpers.GetMethod(baseType, t.Name, paramType);
                    if (method == null)
                    {
                        var property = DynamicHelpers.GetPropertyInfo(baseType, t.Name);
                        Type funcType = null;
                        if (property == null)
                        {
                            field = DynamicHelpers.GetFieldInfo(baseType, t.Name);
                            if (field != null)
                            {
                                funcType = field.FieldType;
                            }
                        }
                        else
                        {
                            funcType = property.PropertyType;
#if NET40 || NET20
                            childMethd = property.GetGetMethod();
#else
                            childMethd = property.GetMethod;
#endif
                        }
                        if (funcType != null)
                        {
                            if (funcType.BaseType.Name != "MulticastDelegate")
                            {
                                throw new ArgumentException($"[FunctaionTag]:\"{t.Name}\" must be delegate");
                            }
                            method = funcType.GetMethod("Invoke");
                        }
                        if (method == null)
                        {
                            throw new CompileException($"[FunctaionTag]:method \"{t.Name}\" cannot be found!");
                        }
                    }
                }
                else
                {
                    baseType = c.Data.GetType(t.Name);
                    if (baseType.BaseType.Name != "MulticastDelegate")
                    {
                        throw new ArgumentException("[FunctaionTag]:functaion must be delegate"); // Delegate’s actions must be addressed !
                    }
                    method = baseType.GetMethod("Invoke");
                }
                var isVoid = method.ReturnType.FullName == "System.Void";
                var mb = c.CreateReutrnMethod<FunctaionTag>(method.ReturnType);
                var il = mb.GetILGenerator();
                var len = 1;
                il.DeclareLocal(baseType);
                if (!isVoid)
                {
                    len = 2;
                    il.DeclareLocal(method.ReturnType);
                }
                for (int i = 0; i < paramType.Length; i++)
                {
                    il.DeclareLocal(paramType[i]);
                }
                if (t.Parent != null)
                {
                    if (!method.IsStatic
                    && (childMethd == null || !childMethd.IsStatic)
                    && (field == null || !field.IsStatic))
                    {
                        il.Emit(OpCodes.Nop);
                        var parentMethod = GetCompileMethod(t.Parent, c);
                        il.Emit(OpCodes.Ldarg_0);
                        il.Emit(OpCodes.Ldarg_1);
                        il.Emit(OpCodes.Call, parentMethod);
                        il.Emit(OpCodes.Stloc_0);
                    }

                }
                else
                {
                    il.DeclareLocal(typeof(object));
                    il.Emit(OpCodes.Nop);
                    il.Emit(OpCodes.Ldarg_1);
                    il.Emit(OpCodes.Callvirt, getVariableScope);
                    il.Emit(OpCodes.Ldstr, t.Name);
                    il.Emit(OpCodes.Callvirt, getVariableValue);
                    il.Emit(OpCodes.Stloc, len + paramType.Length);
                    il.Emit(OpCodes.Ldloc, len + paramType.Length);
                    il.Emit(OpCodes.Isinst, baseType);
                    il.Emit(OpCodes.Stloc_0);
                }

                for (int i = 0; i < t.Children.Count; i++)
                {
                    var m = GetCompileMethod(t.Children[i], c);
                    if (m == null)
                    {
                        throw new ArgumentNullException($"[FunctaionTag]:cannot compile functaion \"{t.Name}\" when the children is null.");
                    }
                    il.Emit(OpCodes.Ldarg_0);
                    il.Emit(OpCodes.Ldarg_1);
                    il.Emit(OpCodes.Call, m);
                    il.Emit(OpCodes.Stloc, len + i);
                }
                if (!method.IsStatic
                    && (childMethd == null || !childMethd.IsStatic)
                    && (field == null || !field.IsStatic))
                {
                    LoadVariable(il, baseType, 0);
                }
                if (childMethd != null)
                {
                    il.Emit(OpCodes.Call, childMethd);
                }
                if (field != null)
                {
                    if (field.IsStatic)
                    {
                        il.Emit(OpCodes.Ldsfld, field);
                    }
                    else
                    {
                        il.Emit(OpCodes.Ldfld, field);
                    }
                }
                for (int i = 0; i < paramType.Length; i++)
                {
                    il.Emit(OpCodes.Ldloc, len + i);
                }
                il.Call(baseType, method);
                //il.Emit(OpCodes.Callvirt, method);
                if (!isVoid)
                {
                    il.Emit(OpCodes.Stloc_1);
                    il.Emit(OpCodes.Ldloc_1);
                }
                il.Emit(OpCodes.Ret);
                return mb.GetBaseDefinition();
            });
            this.Register<ReferenceTag>((tag, c) =>
            {
                return GetCompileMethod(((ReferenceTag)tag).Child, c);
            });
            this.Register<ForeachTag>((tag, c) =>
            {
                var t = tag as ForeachTag;
                var sourceType = Compiler.TypeGuess.GetType(t.Source, c);
                if (sourceType.IsArray)
                {
                    return ArrayForeachCompile(t, c, sourceType);
                }
                return EnumerableForeachCompile(t, c, sourceType);
            });
            this.Register<LogicTag>((tag, c) =>
            {
                var t = tag as LogicTag;
                var type = Compiler.TypeGuess.GetType(t, c);
                var mb = c.CreateReutrnMethod<LogicTag>(type);
                var il = mb.GetILGenerator();
                Label labelEnd = il.DefineLabel();
                il.DeclareLocal(type);
                var array = new object[t.Children.Count];
                var types = new List<Type>();
                var opts = new List<Operator>();
                var message = new List<string>();
                for (int i = 0; i < t.Children.Count; i++)
                {
                    var opt = t.Children[i] as OperatorTag;
                    if (opt != null)
                    {
                        if (!opts.Contains(opt.Value))
                        {
                            opts.Add(opt.Value);
                        }
                        array[i] = opt.Value;
                        message.Add(OperatorConvert.ToString(opt.Value));
                    }
                    else
                    {
                        array[i] = t.Children[i];
                        types.Add(Compiler.TypeGuess.GetType(t.Children[i], c));
                        message.Add(types[types.Count - 1].Name);
                    }
                }

                if (opts.Contains(Operator.Or) || opts.Contains(Operator.And))
                {

                    Label labelTrue = il.DefineLabel();
                    Label labelFalse = il.DefineLabel();
                    Operator pre = Operator.None;
                    for (int i = 0; i < t.Children.Count; i++)
                    {
                        var opt = t.Children[i] as OperatorTag;
                        if (opt != null)
                        {
                            pre = opt.Value;
                            continue;
                        }

                        var m = GetCompileMethod(t.Children[i], c);
                        il.Emit(OpCodes.Ldarg_0);
                        il.Emit(OpCodes.Ldarg_1);
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
                        //il.Emit(OpCodes.Stloc_0);
                        if (pre == Operator.None)
                        {
                            pre = (t.Children[i + 1] as OperatorTag).Value;
                        }

                        if (pre == Operator.Or)
                        {
                            il.Emit(OpCodes.Brtrue, labelTrue);
                        }
                        if (pre == Operator.And)
                        {
                            il.Emit(OpCodes.Brfalse, labelFalse);
                        }
                    }

                    if (pre == Operator.Or)
                    {
                        il.Emit(OpCodes.Br, labelEnd);
                    }

                    if (pre == Operator.And)
                    {
                        il.Emit(OpCodes.Br, labelTrue);
                    }
                    il.MarkLabel(labelTrue);
                    il.Emit(OpCodes.Ldc_I4_1);
                    il.Emit(OpCodes.Stloc_0);
                    il.Emit(OpCodes.Br, labelEnd);


                    il.MarkLabel(labelFalse);
                    il.Emit(OpCodes.Ldc_I4_0);
                    il.Emit(OpCodes.Stloc_0);
                }
                else
                {
                    if (t.Children.Count == 1)
                    {
                        var m = GetCompileMethod(t.Children[0], c);
                        il.Emit(OpCodes.Ldarg_0);
                        il.Emit(OpCodes.Ldarg_1);
                        il.Emit(OpCodes.Call, m);
                        il.Emit(OpCodes.Stloc_0);
                    }
                    else if (t.Children.Count == 3)
                    {

                        var bestType = TypeGuess.FindBestType(types.ToArray());
                        var stack = ExpressionEvaluator.ProcessExpression(array);
                        var arr = stack.ToArray();
                        for (var i = arr.Length - 1; i >= 0; i--)
                        {
                            var obj = arr[i];
                            var childTag = obj as ITag;
                            if (childTag != null)
                            {
                                var m = GetCompileMethod(childTag, c);
                                il.Emit(OpCodes.Ldarg_0);
                                il.Emit(OpCodes.Ldarg_1);
                                il.Emit(OpCodes.Call, m);
                                if (m.ReturnType.FullName != bestType.FullName)
                                {
                                    switch (bestType.FullName)
                                    {
                                        case "System.Decimal":
                                            Type cType = m.ReturnType;
                                            switch (cType.FullName)
                                            {
                                                case "System.Int16":
                                                case "System.UInt16":
                                                case "System.Byte":
                                                    il.Emit(OpCodes.Conv_I4);
                                                    break;
                                            }
                                            il.Emit(OpCodes.Call, typeof(decimal).GetConstructor(new Type[] { cType }));
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
                                            if (m.ReturnType.IsValueType)
                                            {
                                                if (bestType.IsValueType)
                                                {
                                                    il.Emit(OpCodes.Isinst, bestType);
                                                }
                                                else
                                                {
                                                    il.Emit(OpCodes.Box, bestType);
                                                }
                                            }
                                            else
                                            {
                                                if (bestType.IsValueType)
                                                {
                                                    il.Emit(OpCodes.Unbox, bestType);
                                                }
                                                else
                                                {
                                                    il.Emit(OpCodes.Castclass, bestType);
                                                }
                                            }
                                            break;
                                    }
                                }
                            }
                            else
                            {
                                switch ((Operator)obj)
                                {
                                    case Operator.GreaterThan:
                                        if (TypeGuess.IsNumber(bestType))
                                        {
                                            il.Emit(OpCodes.Cgt);
                                        }
                                        else
                                        {
                                            var m = DynamicHelpers.GetMethod(bestType, "op_GreaterThan", new Type[] { bestType, bestType });
                                            if (m == null)
                                            {
                                                throw new Exception.TemplateException($"Operator \">\" can not be applied operand \"{bestType.FullName}\" and \"{bestType.FullName}\"");
                                            }
                                            il.Emit(OpCodes.Call, m);
                                        }
                                        break;
                                    case Operator.GreaterThanOrEqual:
                                        if (TypeGuess.IsNumber(bestType))
                                        {
                                            il.Emit(OpCodes.Clt_Un);
                                            il.Emit(OpCodes.Ldc_I4_0);
                                            il.Emit(OpCodes.Ceq);
                                        }
                                        else
                                        {
                                            var m = DynamicHelpers.GetMethod(bestType, "op_GreaterThanOrEqual", new Type[] { bestType, bestType });
                                            if (m == null)
                                            {
                                                throw new Exception.TemplateException($"Operator \">=\" can not be applied operand \"{bestType.FullName}\" and \"{bestType.FullName}\"");
                                            }
                                            il.Emit(OpCodes.Call, m);
                                        }
                                        break;
                                    case Operator.LessThan:
                                        if (TypeGuess.IsNumber(bestType))
                                        {
                                            il.Emit(OpCodes.Clt);
                                        }
                                        else
                                        {
                                            var m = DynamicHelpers.GetMethod(bestType, "op_LessThan", new Type[] { bestType, bestType });
                                            if (m == null)
                                            {
                                                throw new Exception.TemplateException($"Operator \"<\" can not be applied operand \"{bestType.FullName}\" and \"{bestType.FullName}\"");
                                            }
                                            il.Emit(OpCodes.Call, m);
                                        }
                                        break;
                                    case Operator.LessThanOrEqual:
                                        if (TypeGuess.IsNumber(bestType))
                                        {
                                            il.Emit(OpCodes.Cgt_Un);
                                            il.Emit(OpCodes.Ldc_I4_0);
                                            il.Emit(OpCodes.Ceq);
                                        }
                                        else
                                        {
                                            var m = DynamicHelpers.GetMethod(bestType, "op_LessThanOrEqual", new Type[] { bestType, bestType });
                                            if (m == null)
                                            {
                                                throw new Exception.TemplateException($"Operator \"<=\" can not be applied operand \"{bestType.FullName}\" and \"{bestType.FullName}\"");
                                            }
                                            il.Emit(OpCodes.Call, m);
                                        }
                                        break;
                                    case Operator.Equal:
                                        if (TypeGuess.IsNumber(bestType))
                                        {
                                            il.Emit(OpCodes.Ceq);
                                        }
                                        else
                                        {
                                            il.Emit(OpCodes.Call, DynamicHelpers.GetMethod(bestType, "Equals", new Type[] { bestType }));
                                        }
                                        break;
                                    case Operator.NotEqual:
                                        if (TypeGuess.IsNumber(bestType))
                                        {
                                            il.Emit(OpCodes.Ceq);
                                        }
                                        else
                                        {
                                            il.Emit(OpCodes.Call, DynamicHelpers.GetMethod(bestType, "Equals", new Type[] { bestType }));
                                        }
                                        il.Emit(OpCodes.Ldc_I4_0);
                                        il.Emit(OpCodes.Ceq);
                                        break;
                                    default:
                                        throw new Exception.CompileException($"The operator \"{obj}\" is not supported on type  \"{bestType.FullName}\" .");
                                }
                            }
                        }
                        il.Emit(OpCodes.Stloc_0);
                    }
                    else
                    {
                        throw new Exception.CompileException($"[LogicExpressionTag] : The expression \"{string.Concat(message)}\" is not supported .");
                    }
                }

                il.MarkLabel(labelEnd);
                il.Emit(OpCodes.Ldloc_0);
                il.Emit(OpCodes.Ret);
                return mb.GetBaseDefinition();
            });
            this.Register<ArithmeticTag>((tag, c) =>
            {
                var t = tag as ArithmeticTag;
                var type = Compiler.TypeGuess.GetType(t, c);
                var mb = c.CreateReutrnMethod<ArithmeticTag>(type);
                var il = mb.GetILGenerator();
                Label labelEnd = il.DefineLabel();
                il.DeclareLocal(type);
                var array = new object[t.Children.Count];
                var types = new List<Type>();
                var opts = new List<Operator>();
                var message = new List<string>();
                for (int i = 0; i < t.Children.Count; i++)
                {
                    var opt = t.Children[i] as OperatorTag;
                    if (opt != null)
                    {
                        if (!opts.Contains(opt.Value))
                        {
                            opts.Add(opt.Value);
                        }
                        array[i] = opt.Value;
                        message.Add(OperatorConvert.ToString(opt.Value));
                    }
                    else
                    {
                        array[i] = t.Children[i];
                        types.Add(Compiler.TypeGuess.GetType(t.Children[i], c));
                        message.Add(types[types.Count - 1].Name);
                    }
                }
                if (types.Contains(typeof(string)) && opts.Count == 1 && opts[0] == Operator.Add)
                {
                    il.DeclareLocal(stringBuilderType);
                    il.Emit(OpCodes.Newobj, stringBuilderType.GetConstructor(Type.EmptyTypes));
                    il.Emit(OpCodes.Stloc_1);
                    var index = 2;
                    for (int i = 0; i < t.Children.Count; i++)
                    {
                        var opt = t.Children[i] as OperatorTag;
                        if (opt != null)
                        {
                            continue;
                        }
                        il.Emit(OpCodes.Ldloc_1);
                        il.Emit(OpCodes.Ldarg_0);
                        il.Emit(OpCodes.Ldarg_1);
                        var m = GetCompileMethod(t.Children[i], c);
                        il.Emit(OpCodes.Call, m);
                        StringBuilderAppend(il, c, m.ReturnType, ref index);
                        il.Emit(OpCodes.Pop);
                    }

                    il.Emit(OpCodes.Ldloc_1);
                    il.Call(stringBuilderType, DynamicHelpers.GetMethod(typeof(object), "ToString", Type.EmptyTypes));
                    il.Emit(OpCodes.Stloc_0);
                }
                else
                {
                    var bestType = TypeGuess.FindBestType(types.ToArray());
                    var stack = ExpressionEvaluator.ProcessExpression(array);
                    var arr = stack.ToArray();
                    for (var i = arr.Length - 1; i >= 0; i--)
                    {
                        var obj = arr[i];
                        var childTag = obj as ITag;
                        if (childTag != null)
                        {
                            var m = GetCompileMethod(childTag, c);
                            il.Emit(OpCodes.Ldarg_0);
                            il.Emit(OpCodes.Ldarg_1);
                            il.Emit(OpCodes.Call, m);
                            if (m.ReturnType.FullName != bestType.FullName)
                            {
                                switch (bestType.FullName)
                                {
                                    case "System.Decimal":
                                        Type cType = m.ReturnType;
                                        switch (cType.FullName)
                                        {
                                            case "System.Int16":
                                            case "System.UInt16":
                                            case "System.Byte":
                                                il.Emit(OpCodes.Conv_I4);
                                                break;
                                        }
                                        il.Emit(OpCodes.Call, typeof(decimal).GetConstructor(new Type[] { cType }));
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
                                        throw new Exception.CompileException($"[ExpressionTag] : The type \"{bestType.FullName}\" is not supported .");
                                }
                            }
                        }
                        else
                        {
                            switch ((Operator)obj)
                            {
                                case Operator.Add:
                                    il.Emit(OpCodes.Add);
                                    break;
                                case Operator.Subtract:
                                    il.Emit(OpCodes.Sub);
                                    break;
                                case Operator.Multiply:
                                    il.Emit(OpCodes.Mul);
                                    break;
                                case Operator.Divided:
                                    il.Emit(OpCodes.Div);
                                    break;
                                case Operator.Remainder:
                                    il.Emit(OpCodes.Rem);
                                    break;
                                case Operator.GreaterThan:
                                    il.Emit(OpCodes.Cgt);
                                    break;
                                case Operator.GreaterThanOrEqual:
                                    il.Emit(OpCodes.Clt_Un);
                                    il.Emit(OpCodes.Ldc_I4_0);
                                    il.Emit(OpCodes.Ceq);
                                    break;
                                case Operator.LessThan:
                                    il.Emit(OpCodes.Clt);
                                    break;
                                case Operator.LessThanOrEqual:
                                    il.Emit(OpCodes.Cgt_Un);
                                    il.Emit(OpCodes.Ldc_I4_0);
                                    il.Emit(OpCodes.Ceq);
                                    break;
                                case Operator.Equal:
                                    il.Emit(OpCodes.Ceq);
                                    break;
                                case Operator.NotEqual:
                                    il.Emit(OpCodes.Ceq);
                                    il.Emit(OpCodes.Ldc_I4_0);
                                    il.Emit(OpCodes.Ceq);
                                    break;
                                default:
                                    throw new Exception.CompileException($"[ExpressionTag] : The expression \"{string.Concat(message)}\" is not supported .");
                                    //throw new Exception.CompileException($"The operator \"{obj}\" is not supported on type  \"{bestType.FullName}\" .");
                                    //case Operator.Or:
                                    //    il.Emit(OpCodes.Blt);
                                    //    break;
                            }
                        }
                    }
                    il.Emit(OpCodes.Stloc_0);
                }
                il.MarkLabel(labelEnd);
                il.Emit(OpCodes.Ldloc_0);
                il.Emit(OpCodes.Ret);
                return mb.GetBaseDefinition();
            });
            this.Register<ElseifTag>((tag, c) =>
            {
                return IfCompile((ElseifTag)tag, c);
            });
            this.Register<ElseTag>((tag, c) =>
            {
                return IfCompile((ElseTag)tag, c);
            });
            this.Register<IfTag>((tag, c) =>
            {
                var t = tag as IfTag;
                var type = Compiler.TypeGuess.GetType(t, c);
                var mb = c.CreateReutrnMethod<IfTag>(type);
                var il = mb.GetILGenerator();
                Label labelEnd = il.DefineLabel();
                Label labelSuccess = il.DefineLabel();
                var hasReturn = false;
                var index = 0;
                if (type.FullName != "System.Void")
                {
                    il.DeclareLocal(type);
                    hasReturn = true;
                    index++;
                }
                Label[] lables = new Label[t.Children.Count - 1];
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
                        var m = GetCompileMethod(ifTag.Condition, c);
                        il.Emit(OpCodes.Ldarg_0);
                        il.Emit(OpCodes.Ldarg_1);
                        il.Emit(OpCodes.Call, m);
                        if (m.ReturnType.Name != "Boolean")
                        {
                            il.DeclareLocal(m.ReturnType);
                            il.Emit(OpCodes.Stloc, index);
                            LoadVariable(il, m.ReturnType, index);
                            index++;
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
                        il.Emit(OpCodes.Brfalse, lables[i]);
                    }

                    var execute = GetCompileMethod(ifTag, c);
                    il.Emit(OpCodes.Ldarg_0);
                    il.Emit(OpCodes.Ldarg_1);
                    il.Emit(OpCodes.Call, execute);
                    if (execute.ReturnType.FullName != type.FullName)
                    {
                        if (type.FullName == "System.String")
                        {
                            il.DeclareLocal(execute.ReturnType);
                            il.Emit(OpCodes.Stloc, index);
                            LoadVariable(il, execute.ReturnType, index);
                            index++;
                            il.Call(execute.ReturnType, DynamicHelpers.GetMethod(typeof(object), "ToString", Type.EmptyTypes));
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
                        var defaultMethod = DynamicHelpers.GetGenericMethod(typeof(CompileBuilder), new Type[] { type }, "GenerateDefaultValue", Type.EmptyTypes);
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
            });
            this.Register<IncludeTag>((tag, c) =>
            {
                var t = tag as IncludeTag;
                var type = Compiler.TypeGuess.GetType(t, c);
                var mb = c.CreateReutrnMethod<IncludeTag>(type);
                var il = mb.GetILGenerator();
                var strTag = t.Path as StringTag;
                if (strTag != null)
                {
                    var res = c.Load(strTag.Value);
                    if (res != null)
                    {
                        il.Emit(OpCodes.Ldstr, res.Content);
                    }
                    else
                    {
                        il.Emit(OpCodes.Ldstr, $"\"{strTag.Value}\" not found!");
                    }
                }
                else
                {
                    var strType = typeof(string);
                    var resType = typeof(Resources.ResourceInfo);
                    var ctxType = typeof(TemplateContext);
                    Label labelEnd = il.DefineLabel();
                    Label labelSuccess = il.DefineLabel();
                    il.DeclareLocal(strType);
                    il.DeclareLocal(typeof(bool));
                    il.DeclareLocal(resType);
                    il.DeclareLocal(typeof(bool));
                    il.DeclareLocal(type);

                    var m = GetCompileMethod(t.Path, c);

                    il.Emit(OpCodes.Ldarg_0);
                    il.Emit(OpCodes.Ldarg_1);
                    il.Emit(OpCodes.Call, m);
                    if (m.ReturnType.FullName != "System.String")
                    {
                        il.DeclareLocal(m.ReturnType);
                        il.Emit(OpCodes.Stloc, 5);
                        LoadVariable(il, m.ReturnType, 5);
                        il.Call(m.ReturnType, DynamicHelpers.GetMethod(typeof(object), "ToString", Type.EmptyTypes));
                    }
                    il.Emit(OpCodes.Stloc_0);
                    il.Emit(OpCodes.Ldloc_0);
                    il.Emit(OpCodes.Ldnull);
                    il.Emit(OpCodes.Cgt_Un);
                    il.Emit(OpCodes.Stloc_1);
                    il.Emit(OpCodes.Ldloc_1);
                    il.Emit(OpCodes.Brfalse, labelEnd);

                    il.Emit(OpCodes.Ldarg_1);
                    il.Emit(OpCodes.Ldloc_0);
                    il.Emit(OpCodes.Call, DynamicHelpers.GetMethod(typeof(TemplateContextExtensions), "Load", new Type[] { typeof(Context), strType }));
                    il.Emit(OpCodes.Stloc_2);
                    il.Emit(OpCodes.Ldloc_2);
                    il.Emit(OpCodes.Ldnull);
                    il.Emit(OpCodes.Cgt_Un);
                    il.Emit(OpCodes.Stloc, 3);
                    il.Emit(OpCodes.Ldloc, 3);
                    il.Emit(OpCodes.Brfalse, labelEnd);

                    il.Emit(OpCodes.Ldloc_2);
                    il.Emit(OpCodes.Callvirt, DynamicHelpers.GetPropertyGetMethod(resType, "Content"));
                    il.Emit(OpCodes.Stloc, 4);
                    il.Emit(OpCodes.Br, labelSuccess);

                    il.MarkLabel(labelEnd);
                    il.Emit(OpCodes.Ldstr, $"[IncludeTag] : \"{t.Path.ToSource()}\" cannot be found.");
                    il.Emit(OpCodes.Stloc, 4);


                    il.MarkLabel(labelSuccess);
                    il.Emit(OpCodes.Ldloc, 4);
                }
                il.Emit(OpCodes.Ret);
                return mb.GetBaseDefinition();
            });
            this.Register<LoadTag>((tag, c) =>
            {
                var t = tag as LoadTag;
                var type = Compiler.TypeGuess.GetType(t, c);
                var mb = c.CreateReutrnMethod<LoadTag>(type);
                var il = mb.GetILGenerator();
                var strTag = t.Path as StringTag;
                if (strTag != null)
                {
                    var res = c.Load(strTag.Value);
                    if (res == null)
                    {
                        throw new Exception.CompileException($"[LoadTag] : \"{strTag.Value}\" cannot be found.");
                    }
                    var lexer = new TemplateLexer(res.Content);
                    var ts = lexer.Execute();

                    var parser = new TemplateParser(ts);
                    var tags = parser.Execute();

                    BlockCompile(il, c, tags);
                }
                else
                {
                    Label labelEnd = il.DefineLabel();
                    Label labelSuccess = il.DefineLabel();
                    Label labelTry = il.DefineLabel();
                    Label labelFinally = il.DefineLabel();

                    var m = GetCompileMethod(t.Path, c);
                    il.DeclareLocal(typeof(string));
                    il.DeclareLocal(typeof(bool));
                    il.DeclareLocal(typeof(string));

                    il.Emit(OpCodes.Ldarg_0);
                    il.Emit(OpCodes.Ldarg_1);
                    il.Emit(OpCodes.Call, m);
                    if (m.ReturnType.FullName != "System.String")
                    {
                        il.DeclareLocal(m.ReturnType);
                        il.Emit(OpCodes.Stloc, 3);
                        if (m.ReturnType.IsValueType)
                        {
                            il.Emit(OpCodes.Ldloca, 3);
                        }
                        else
                        {
                            il.Emit(OpCodes.Ldloc, 3);
                            il.DeclareLocal(typeof(bool));
                            il.Emit(OpCodes.Ldnull);
                            il.Emit(OpCodes.Cgt_Un);
                            il.Emit(OpCodes.Stloc, 4);
                            il.Emit(OpCodes.Ldloc, 4);
                            il.Emit(OpCodes.Brfalse, labelEnd);

                            il.Emit(OpCodes.Ldloc, 3);
                        }
                        il.Call(m.ReturnType, DynamicHelpers.GetMethod(typeof(object), "ToString", Type.EmptyTypes));
                        il.Emit(OpCodes.Stloc, 0);
                        il.Emit(OpCodes.Ldloc, 0);
                    }
                    else
                    {
                        il.Emit(OpCodes.Stloc, 0);
                        il.Emit(OpCodes.Ldloc, 0);
                    }
                    il.Emit(OpCodes.Ldnull);
                    il.Emit(OpCodes.Cgt_Un);
                    il.Emit(OpCodes.Stloc, 1);
                    il.Emit(OpCodes.Ldloc, 1);
                    il.Emit(OpCodes.Brfalse, labelEnd);

                    il.Emit(OpCodes.Ldloc_0);
                    il.Emit(OpCodes.Ldarg_1);
                    il.Emit(OpCodes.Call, DynamicHelpers.GetMethod(typeof(Engine), "CompileFileAndExec", new Type[] { typeof(string), typeof(TemplateContext) }));
                    il.Emit(OpCodes.Stloc, 2);

                    il.Emit(OpCodes.Br, labelSuccess);

                    il.MarkLabel(labelEnd);
                    il.Emit(OpCodes.Ldstr, $"[LoadTag] : \"{t.Path.ToSource()}\" cannot be found.");
                    //il.Emit(OpCodes.Ldnull);
                    il.Emit(OpCodes.Stloc, 2);

                    il.MarkLabel(labelSuccess);
                    il.Emit(OpCodes.Ldloc, 2);
                }
                il.Emit(OpCodes.Ret);
                return mb.GetBaseDefinition();
            });
            this.Register<CommentTag>((tag, c) =>
            {
                //var t = tag as CommentTag;
                var type = typeof(void);
                var mb = c.CreateReutrnMethod<CommentTag>(type);
                var il = mb.GetILGenerator();
                il.Emit(OpCodes.Ret);
                return mb.GetBaseDefinition();
            });
            this.Register<BodyTag>((tag, c) =>
            {
                var t = tag as BodyTag;
                var type = typeof(string);
                var mb = c.CreateReutrnMethod<CommentTag>(type);
                var il = mb.GetILGenerator();
                BlockCompile(il, c, t.Children.ToArray());
                il.Emit(OpCodes.Ret);
                return mb.GetBaseDefinition();
            });
            this.Register<NullTag>((tag, c) =>
            {
                var type = typeof(object);
                var mb = c.CreateReutrnMethod<NullTag>(type);
                var il = mb.GetILGenerator();
                il.Emit(OpCodes.Ldnull);
                il.Emit(OpCodes.Ret);
                return mb.GetBaseDefinition();
            });
            this.Register<ForTag>((tag, c) =>
            {
                var t = tag as ForTag;
                var type = Compiler.TypeGuess.GetType(t, c);
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
                var m = GetCompileMethod(t.Initial, c);
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
                    CallTag(il, c, t.Children[i], (nil, hasReturn, needCall) =>
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
                        StringBuilderAppend(nil, c, returnType, ref index);
                        nil.Emit(OpCodes.Pop);
                    });
                }

                il.Emit(OpCodes.Ldarg_0);
                il.Emit(OpCodes.Ldloc_1);
                m = GetCompileMethod(t.Do, c);
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

                m = GetCompileMethod(t.Condition, c);
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
            });
            this.Register<LayoutTag>((tag, c) =>
            {
                var t = tag as LayoutTag;
                var type = Compiler.TypeGuess.GetType(t, c);
                var mb = c.CreateReutrnMethod<LayoutTag>(type);
                var il = mb.GetILGenerator();
                var strTag = t.Path as StringTag;
                if (strTag == null)
                {
                    throw new Exception.CompileException($"[LayoutTag] : path must be a string.");
                }
                var res = c.Load(strTag.Value);
                if (res == null)
                {
                    throw new Exception.CompileException($"[LayoutTag] : \"{strTag.Value}\" cannot be found.");
                }
                var lexer = new TemplateLexer(res.Content);
                var ts = lexer.Execute();

                var parser = new TemplateParser(ts);
                var tags = parser.Execute();
                for (int i = 0; i < tags.Length; i++)
                {
                    if (tags[i] is BodyTag)
                    {
                        BodyTag body = (BodyTag)tags[i];
                        for (int j = 0; j < t.Children.Count; j++)
                        {
                            body.AddChild(t.Children[j]);
                        }
                        //t.Children.Clear();
                        //tags[i] = body;
                    }
                }

                BlockCompile(il, c, tags);

                il.Emit(OpCodes.Ret);
                return mb.GetBaseDefinition();
            });
            #endregion

            #region  render
            this.SetRenderFunc<TextTag>((tag, c) =>
            {
                var t = tag as TextTag;
                var text = t.Text;
                if (c.StripWhiteSpace)
                {
                    text = text?.Trim();
                }
                if (!string.IsNullOrEmpty(text))
                {
                    c.Generator.Emit(OpCodes.Ldarg_1);
                    c.Generator.Emit(OpCodes.Ldstr, text);
                    c.Generator.Emit(OpCodes.Callvirt, typeof(TextWriter).GetMethod("Write", new Type[] { typeof(string) }));
                }
            });
            #endregion
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="il"></param>
        /// <param name="c"></param>
        /// <param name="tags"></param>
        /// <returns></returns>
        private void BlockCompile(ILGenerator il, CompileContext c, ITag[] tags)
        {
            il.DeclareLocal(stringBuilderType);
            il.DeclareLocal(typeof(string));
            il.Emit(OpCodes.Newobj, stringBuilderType.GetConstructor(Type.EmptyTypes));
            il.Emit(OpCodes.Stloc_0);
            var index = 2;
            for (var i = 0; i < tags.Length; i++)
            {
                CallTag(il, c, tags[i], (nil, hasReturn, needCall) =>
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
                    StringBuilderAppend(nil, c, returnType, ref index);
                    nil.Emit(OpCodes.Pop);
                });
            }
            il.Emit(OpCodes.Ldloc_0);
            il.Call(stringBuilderType, DynamicHelpers.GetMethod(stringBuilderType, "ToString", Type.EmptyTypes));
            il.Emit(OpCodes.Stloc_1);
            il.Emit(OpCodes.Ldloc_1);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="tag"></param>
        /// <param name="c"></param>
        /// <returns></returns>
        private MethodInfo IfCompile<T>(T tag, CompileContext c) where T : ElseifTag
        {
            var t = tag;
            var type = Compiler.TypeGuess.GetType(t, c);
            var mb = c.CreateReutrnMethod<T>(type);
            var il = mb.GetILGenerator();
            var labelEnd = il.DefineLabel();
            il.DeclareLocal(type);
            il.DeclareLocal(stringBuilderType);
            il.Emit(OpCodes.Newobj, stringBuilderType.GetConstructor(Type.EmptyTypes));
            il.Emit(OpCodes.Stloc_1);
            var index = 2;
            for (var i = 0; i < t.Children.Count; i++)
            {
                CallTag(il, c, t.Children[i], (nil, hasReturn, needCall) =>
                {
                    if (hasReturn)
                    {
                        nil.Emit(OpCodes.Ldloc_1);
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
                    StringBuilderAppend(nil, c, returnType, ref index);
                    nil.Emit(OpCodes.Pop);
                });
            }
            il.Emit(OpCodes.Ldloc_1);
            il.Call(stringBuilderType, DynamicHelpers.GetMethod(stringBuilderType, "ToString", Type.EmptyTypes));
            il.Emit(OpCodes.Stloc_0);

            il.MarkLabel(labelEnd);
            il.Emit(OpCodes.Ldloc_0);
            il.Emit(OpCodes.Ret);
            return mb.GetBaseDefinition();
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="tag"></param>
        /// <param name="c"></param>
        /// <param name="sourceType"></param>
        /// <returns></returns>
        private MethodInfo EnumerableForeachCompile(ForeachTag tag, CompileContext c, Type sourceType)
        {
            var t = tag;
            var type = Compiler.TypeGuess.GetType(t, c);
            var variableScopeType = typeof(VariableScope);
            var templateContextType = typeof(TemplateContext);
            var childType = TypeGuess.InferChildType(sourceType);
            var enumerableType = sourceType.GetInterface("IEnumerable`1");// typeof(System.Collections.IEnumerable);
            Type enumeratorType;
            bool mustTo = false;
            if (enumerableType == null)
            {
                enumerableType = typeof(System.Collections.IEnumerable);
                enumeratorType = typeof(System.Collections.IEnumerator);
                mustTo = true;
            }
            else
            {
                enumeratorType = typeof(IEnumerator<>).MakeGenericType(childType);
            }
            if (childType.Length != 1)
            {
                throw new CompileException("[ForeachTag]:source error.");
            }
            var old = c.Data;
            var scope = new VariableScope(old);
            c.Data = scope;
            c.Set(t.Name, childType[0]);
            c.Set("foreachIndex", typeof(int));
            var mb = c.CreateReutrnMethod<ForeachTag>(type);
            var il = mb.GetILGenerator();
            Label labelEnd = il.DefineLabel();
            Label labelNext = il.DefineLabel();
            Label labelStart = il.DefineLabel();
            il.DeclareLocal(stringBuilderType);
            il.DeclareLocal(enumerableType);
            il.DeclareLocal(templateContextType);
            il.DeclareLocal(typeof(bool));
            il.DeclareLocal(enumeratorType);
            il.DeclareLocal(typeof(Int32));
            il.DeclareLocal(childType[0]);
            il.DeclareLocal(typeof(bool));
            il.DeclareLocal(typeof(string));
            var index = 9;
            var types = new Type[t.Children.Count];
            for (var i = 0; i < t.Children.Count; i++)
            {
                types[i] = Compiler.TypeGuess.GetType(t.Children[i], c);
                if (t.Children[i] is SetTag setTag)
                {
                    c.Set(setTag.Name, Compiler.TypeGuess.GetType(setTag.Value, c));
                }
                if (types[i].FullName == "System.Void" || t.Children[i] is TextTag)
                {
                    continue;
                }
                //il.DeclareLocal(types[i]);
                //index++;
            }
            il.Emit(OpCodes.Nop);
            il.Emit(OpCodes.Newobj, stringBuilderType.GetConstructor(Type.EmptyTypes));
            il.Emit(OpCodes.Stloc_0);
            il.Emit(OpCodes.Ldarg_0);
            il.Emit(OpCodes.Ldarg_1);
            var method = GetCompileMethod(t.Source, c);
            il.Emit(OpCodes.Call, method);
            il.Emit(OpCodes.Stloc_1);
            il.Emit(OpCodes.Ldloc_1);
            il.Emit(OpCodes.Ldnull);
            il.Emit(OpCodes.Cgt_Un);
            il.Emit(OpCodes.Stloc_3);
            il.Emit(OpCodes.Ldloc_3);
            il.Emit(OpCodes.Brfalse, labelEnd);
            il.Emit(OpCodes.Nop);
            il.Emit(OpCodes.Ldloc_1);
            il.Emit(OpCodes.Callvirt, DynamicHelpers.GetMethod(enumerableType, "GetEnumerator", Type.EmptyTypes));
            il.Emit(OpCodes.Stloc_S, 4);
            il.Emit(OpCodes.Ldarg_1);
            il.Emit(OpCodes.Call, DynamicHelpers.GetMethod(templateContextType, "CreateContext", new Type[] { templateContextType }));
            il.Emit(OpCodes.Stloc_2);
            il.Emit(OpCodes.Ldc_I4, 0);
            il.Emit(OpCodes.Stloc_S, 5);
            il.Emit(OpCodes.Br, labelNext);
            il.MarkLabel(labelStart);
            il.Emit(OpCodes.Nop);
            il.Emit(OpCodes.Ldloc_S, 5);
            il.Emit(OpCodes.Ldc_I4, 1);
            il.Emit(OpCodes.Add);
            il.Emit(OpCodes.Stloc_S, 5);
            il.Emit(OpCodes.Ldloc_S, 4);
            il.Emit(OpCodes.Callvirt, DynamicHelpers.GetPropertyGetMethod(enumeratorType, "Current"));
            if (mustTo)
            {
                if (childType[0].IsValueType)
                {
                    il.Emit(OpCodes.Unbox_Any, childType[0]);
                }
                else
                {
                    il.Emit(OpCodes.Isinst, childType[0]);
                }
            }
            il.Emit(OpCodes.Stloc_S, 6);
            il.Emit(OpCodes.Ldloc, 2);
            il.Emit(OpCodes.Callvirt, getVariableScope);
            il.Emit(OpCodes.Ldstr, t.Name);
            il.Emit(OpCodes.Ldloc_S, 6);
            il.Emit(OpCodes.Callvirt, DynamicHelpers.GetGenericMethod(variableScopeType, childType, "Set", new Type[] { typeof(string), childType[0] }));

            il.Emit(OpCodes.Nop);
            il.Emit(OpCodes.Ldloc, 2);
            il.Emit(OpCodes.Callvirt, getVariableScope);
            il.Emit(OpCodes.Ldstr, "foreachIndex");
            il.Emit(OpCodes.Ldloc_S, 5);
            il.Emit(OpCodes.Callvirt, DynamicHelpers.GetGenericMethod(variableScopeType, new Type[] { typeof(int) }, "Set", new Type[] { typeof(string), typeof(int) }));
            il.Emit(OpCodes.Nop);

            StringBuilderAppend(il, c, t.Children, 0, 2, ref index);

            il.Emit(OpCodes.Nop);
            il.MarkLabel(labelNext);
            il.Emit(OpCodes.Ldloc_S, 4);
            if (!mustTo)
            {
                il.Emit(OpCodes.Callvirt, DynamicHelpers.GetMethod(typeof(System.Collections.IEnumerator), "MoveNext", Type.EmptyTypes));
            }
            else
            {
                il.Emit(OpCodes.Callvirt, DynamicHelpers.GetMethod(enumeratorType, "MoveNext", Type.EmptyTypes));
            }
            il.Emit(OpCodes.Stloc_S, 7);
            il.Emit(OpCodes.Ldloc_S, 7);
            il.Emit(OpCodes.Brtrue, labelStart);
            il.MarkLabel(labelEnd);
            il.Emit(OpCodes.Ldloc_0);
            il.Call(stringBuilderType, DynamicHelpers.GetMethod(typeof(object), "ToString", Type.EmptyTypes));
            il.Emit(OpCodes.Stloc_S, 8);
            il.Emit(OpCodes.Ldloc_S, 8);
            il.Emit(OpCodes.Ret);
            c.Data = old;
            return mb.GetBaseDefinition();
        }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="tag"></param>
        /// <param name="c"></param>
        /// <param name="sourceType"></param>
        /// <returns></returns>
        private MethodInfo ArrayForeachCompile(ForeachTag tag, CompileContext c, Type sourceType)
        {
            //var t = tag;
            //var type = Compiler.TypeGuess.GetType(t, c);
            //var childType = TypeGuess.InferChildType(sourceType);
            //var templateContextType = typeof(TemplateContext);
            //var variableScopeType = typeof(VariableScope);
            //if (childType.Length != 1)
            //{
            //    throw new CompileException("[ForeachTag]:source error.");
            //}

            //var old = c.Data;
            //var scope = new VariableScope(old);
            //c.Data = scope;
            //c.Set(t.Name, childType[0]);
            //c.Set("foreachIndex", typeof(int));

            //var mb = c.CreateReutrnMethod<ForeachTag>(type);
            //var il = mb.GetILGenerator();
            //var labelNext = il.DefineLabel();
            //var labelStart = il.DefineLabel();
            //var labelEnd = il.DefineLabel();
            //var vars = new LocalVar(il);
            //vars.Declare("sb", stringBuilderType);
            //vars.Declare("source", sourceType);
            //vars.Declare("isNull", typeof(bool));
            //vars.Declare("newContext", templateContextType);
            //vars.Declare("foreachIndex", typeof(int));
            //vars.Declare("arr", sourceType);
            //vars.Declare("i", typeof(int));
            //vars.Declare("node", childType[0]);
            //vars.Declare("result", childType[0]);

            //var types = new Type[t.Children.Count];
            //for (var i = 0; i < t.Children.Count; i++)
            //{
            //    types[i] = Compiler.TypeGuess.GetType(t.Children[i], c);
            //    if (t.Children[i] is SetTag setTag)
            //    {
            //        c.Set(setTag.Name, Compiler.TypeGuess.GetType(setTag.Value, c));
            //    }
            //    if (types[i].FullName == "System.Void" || t.Children[i] is TextTag)
            //    {
            //        continue;
            //    }
            //    vars.Declare(types[i]);
            //}

            //il.Emit(OpCodes.Newobj, stringBuilderType.GetConstructor(Type.EmptyTypes));
            //il.Emit(OpCodes.Stloc, vars["sb"]);

            //il.Emit(OpCodes.Ldarg_0);
            //il.Emit(OpCodes.Ldarg_1);
            //var method = GetCompileMethod(t.Source, c);
            //il.Emit(OpCodes.Call, method);

            //il.Emit(OpCodes.Stloc, vars["source"]);
            //il.Emit(OpCodes.Ldloc, vars["source"]);
            //il.Emit(OpCodes.Ldnull);
            //il.Emit(OpCodes.Cgt_Un);
            //il.Emit(OpCodes.Stloc, vars["isNull"]);
            //il.Emit(OpCodes.Ldloc, vars["isNull"]);
            //il.Emit(OpCodes.Brfalse, labelEnd);


            //il.Emit(OpCodes.Ldarg_1);
            //il.Emit(OpCodes.Call, DynamicHelpers.GetMethod(templateContextType, "CreateContext", new Type[] { templateContextType }));
            //il.Emit(OpCodes.Stloc, vars["newContext"]);
            //il.Emit(OpCodes.Ldc_I4_0);
            //il.Emit(OpCodes.Stloc, vars["foreachIndex"]);
            //il.Emit(OpCodes.Ldloc, vars["source"]);
            //il.Emit(OpCodes.Stloc, vars["arr"]);
            //il.Emit(OpCodes.Ldc_I4_0);
            //il.Emit(OpCodes.Stloc, vars["i"]);
            //il.Emit(OpCodes.Br, labelStart);

            //// loop start
            //il.MarkLabel(labelNext);
            //il.Emit(OpCodes.Ldloc, vars["arr"]);
            //il.Emit(OpCodes.Ldloc, vars["i"]);
            //Ldelem(il, childType[0]);
            //il.Emit(OpCodes.Stloc, vars["node"]);
            //il.Emit(OpCodes.Ldloc, vars["foreachIndex"]);
            //il.Emit(OpCodes.Ldc_I4_1);
            //il.Emit(OpCodes.Add);
            //il.Emit(OpCodes.Stloc, vars["foreachIndex"]);
            //il.Emit(OpCodes.Ldloc, vars["newContext"]);
            //il.Emit(OpCodes.Callvirt, getVariableScope);
            //il.Emit(OpCodes.Ldstr, t.Name);
            //il.Emit(OpCodes.Ldloc, vars["node"]);
            //il.Emit(OpCodes.Callvirt, DynamicHelpers.GetGenericMethod(variableScopeType, childType, "Set", new Type[] { typeof(string), childType[0] }));
            //il.Emit(OpCodes.Ldloc, vars["newContext"]);
            //il.Emit(OpCodes.Callvirt, getVariableScope);
            //il.Emit(OpCodes.Ldstr, "foreachIndex");
            //il.Emit(OpCodes.Ldloc, vars["foreachIndex"]);
            //il.Emit(OpCodes.Callvirt, DynamicHelpers.GetGenericMethod(variableScopeType, new Type[] { typeof(int) }, "Set", new Type[] { typeof(string), typeof(int) }));

            //var index = 0;
            //StringBuilderAppend(il, c, t.Children, vars["sb"], vars["newContext"], ref index);

            //il.Emit(OpCodes.Ldloc, vars["i"]);
            //il.Emit(OpCodes.Ldc_I4_1);
            //il.Emit(OpCodes.Add);
            //il.Emit(OpCodes.Stloc, vars["i"]);
            //il.MarkLabel(labelStart);
            //il.Emit(OpCodes.Ldloc, vars["i"]);
            //il.Emit(OpCodes.Ldloc, vars["arr"]);
            //il.Emit(OpCodes.Ldlen);
            //il.Emit(OpCodes.Conv_I4);
            //il.Emit(OpCodes.Blt, labelNext);
            //// end loop

            //il.MarkLabel(labelEnd);
            //il.Emit(OpCodes.Ldloc, vars["sb"]);
            //il.Call( stringBuilderType, DynamicHelpers.GetMethod(typeof(object), "ToString", Type.EmptyTypes));
            //il.Emit(OpCodes.Stloc, vars["result"]);
            //il.Emit(OpCodes.Ldloc, vars["result"]);
            //il.Emit(OpCodes.Ret);

            //return mb.GetBaseDefinition();

            var t = tag;
            var type = Compiler.TypeGuess.GetType(t, c);
            var childType = TypeGuess.InferChildType(sourceType);
            var templateContextType = typeof(TemplateContext);
            var variableScopeType = typeof(VariableScope);
            if (childType.Length != 1)
            {
                throw new CompileException("[ForeachTag]:source error.");
            }

            var old = c.Data;
            var scope = new VariableScope(old);
            c.Data = scope;
            c.Set(t.Name, childType[0]);
            c.Set("foreachIndex", typeof(int));

            var mb = c.CreateReutrnMethod<ForeachTag>(type);
            var il = mb.GetILGenerator();
            var labelNext = il.DefineLabel();
            var labelStart = il.DefineLabel();
            var labelEnd = il.DefineLabel();
            il.DeclareLocal(stringBuilderType);
            il.DeclareLocal(sourceType);
            il.DeclareLocal(typeof(bool));
            il.DeclareLocal(templateContextType);
            il.DeclareLocal(typeof(int));
            il.DeclareLocal(sourceType);
            il.DeclareLocal(typeof(int));
            il.DeclareLocal(childType[0]);
            il.DeclareLocal(typeof(string));

            var index = 9;

            var types = new Type[t.Children.Count];
            for (var i = 0; i < t.Children.Count; i++)
            {
                types[i] = Compiler.TypeGuess.GetType(t.Children[i], c);
                if (t.Children[i] is SetTag setTag)
                {
                    c.Set(setTag.Name, Compiler.TypeGuess.GetType(setTag.Value, c));
                }
                if (types[i].FullName == "System.Void" || t.Children[i] is TextTag)
                {
                    continue;
                }
                il.DeclareLocal(types[i]);
                index++;
            }


            il.Emit(OpCodes.Newobj, stringBuilderType.GetConstructor(Type.EmptyTypes));
            il.Emit(OpCodes.Stloc, 0);

            il.Emit(OpCodes.Ldarg_0);
            il.Emit(OpCodes.Ldarg_1);
            var method = GetCompileMethod(t.Source, c);
            il.Emit(OpCodes.Call, method);

            il.Emit(OpCodes.Stloc_1);
            il.Emit(OpCodes.Ldloc_1);
            il.Emit(OpCodes.Ldnull);
            il.Emit(OpCodes.Cgt_Un);
            il.Emit(OpCodes.Stloc_2);
            il.Emit(OpCodes.Ldloc_2);
            il.Emit(OpCodes.Brfalse, labelEnd);


            il.Emit(OpCodes.Ldarg_1);
            il.Emit(OpCodes.Call, DynamicHelpers.GetMethod(templateContextType, "CreateContext", new Type[] { templateContextType }));
            il.Emit(OpCodes.Stloc_3);
            il.Emit(OpCodes.Ldc_I4_0);
            il.Emit(OpCodes.Stloc, 4);
            il.Emit(OpCodes.Ldloc_1);
            il.Emit(OpCodes.Stloc, 5);
            il.Emit(OpCodes.Ldc_I4_0);
            il.Emit(OpCodes.Stloc, 6);
            il.Emit(OpCodes.Br, labelStart);

            // loop start
            il.MarkLabel(labelNext);
            il.Emit(OpCodes.Ldloc, 5);
            il.Emit(OpCodes.Ldloc, 6);
            il.Ldelem(childType[0]);
            il.Emit(OpCodes.Stloc, 7);
            il.Emit(OpCodes.Ldloc, 4);
            il.Emit(OpCodes.Ldc_I4_1);
            il.Emit(OpCodes.Add);
            il.Emit(OpCodes.Stloc, 4);
            il.Emit(OpCodes.Ldloc, 3);
            il.Emit(OpCodes.Callvirt, getVariableScope);
            il.Emit(OpCodes.Ldstr, t.Name);
            il.Emit(OpCodes.Ldloc, 7);
            il.Emit(OpCodes.Callvirt, DynamicHelpers.GetGenericMethod(variableScopeType, childType, "Set", new Type[] { typeof(string), childType[0] }));
            il.Emit(OpCodes.Ldloc, 3);
            il.Emit(OpCodes.Callvirt, getVariableScope);
            il.Emit(OpCodes.Ldstr, "foreachIndex");
            il.Emit(OpCodes.Ldloc, 4);
            il.Emit(OpCodes.Callvirt, DynamicHelpers.GetGenericMethod(variableScopeType, new Type[] { typeof(int) }, "Set", new Type[] { typeof(string), typeof(int) }));

            StringBuilderAppend(il, c, t.Children, 0, 3, ref index);

            il.Emit(OpCodes.Ldloc, 6);
            il.Emit(OpCodes.Ldc_I4_1);
            il.Emit(OpCodes.Add);
            il.Emit(OpCodes.Stloc, 6);
            il.MarkLabel(labelStart);
            il.Emit(OpCodes.Ldloc, 6);
            il.Emit(OpCodes.Ldloc, 5);
            il.Emit(OpCodes.Ldlen);
            il.Emit(OpCodes.Conv_I4);
            il.Emit(OpCodes.Blt, labelNext);
            // end loop

            il.MarkLabel(labelEnd);
            il.Emit(OpCodes.Ldloc_0);
            il.Call(stringBuilderType, DynamicHelpers.GetMethod(typeof(object), "ToString", Type.EmptyTypes));
            il.Emit(OpCodes.Stloc, 8);
            il.Emit(OpCodes.Ldloc, 8);
            il.Emit(OpCodes.Ret);

            return mb.GetBaseDefinition();

        }
    }
}