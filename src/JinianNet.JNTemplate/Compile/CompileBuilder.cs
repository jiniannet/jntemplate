/********************************************************************************
 Copyright (c) jiniannet (http://www.jiniannet.com). All rights reserved.
 Licensed under the MIT license. See licence.txt file in the project root for full license information.
 ********************************************************************************/
using JinianNet.JNTemplate.Dynamic;
using JinianNet.JNTemplate.Exception;
using JinianNet.JNTemplate.Nodes;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Reflection.Emit;

namespace JinianNet.JNTemplate.Compile
{
    /// <summary>
    /// Compiler
    /// </summary>
    public class CompileBuilder
    {
        /// <summary>
        /// 生成默认值
        /// </summary>
        /// <typeparam name="T">Type</typeparam>
        /// <returns>value</returns>
        public static T GenerateDefaultValue<T>()
        {
            return default(T);
        }

        private MethodInfo getVariableScope;
        private MethodInfo getVariableValue;
        private Action<ITag, CompileContext> defaultRender;
        Dictionary<string, Func<ITag, CompileContext, MethodInfo>> returnDict;
        Dictionary<string, Action<ITag, CompileContext>> renderDict;

        /// <summary>
        /// ctor
        /// </summary>
        public CompileBuilder()
        {
            returnDict = new Dictionary<string, Func<ITag, CompileContext, MethodInfo>>();
            renderDict = new Dictionary<string, Action<ITag, CompileContext>>();
            getVariableScope = DynamicHelpers.GetPropertyGetMethod(typeof(TemplateContext), "TempData");
            getVariableValue = typeof(VariableScope).GetMethod("get_Item", new[] { typeof(string) });
            defaultRender = GeneralDefaultRender();

            Initialize();
        }

        private Action<ITag, CompileContext> GeneralDefaultRender()
        {
            return (tag, scope) =>
            {
                var tagType = tag.GetType();
                if (tagType.IsSubclassOf(typeof(TypeTag<>)))
                {
                    var m = tagType.GetProperty("Value");
                    if (m == null)
                    {
                        throw new Exception.CompileException($"cannot compile the tag! type:{tagType.FullName}");
                    }
                    scope.Generator.Emit(OpCodes.Ldarg_1);
                    var result = m.GetValue(tag, null);
                    if (result == null)
                    {
                        scope.Generator.Emit(OpCodes.Ldstr, string.Empty);
                    }
                    else
                    {
                        scope.Generator.Emit(OpCodes.Ldstr, result.ToString());
                    }
                    scope.Generator.Emit(OpCodes.Callvirt, typeof(TextWriter).GetMethod("Write", new Type[] { typeof(string) }));
                }
                else
                {
                    MethodInfo method = null;
                    if (tag is ReferenceTag)
                    {
                        method = GetCompileMethod((tag as ReferenceTag).Child, scope);
                    }
                    else
                    {
                        method = GetCompileMethod(tag, scope);
                    }
                    if (method == null)
                    {
                        throw new Exception.CompileException($"cannot compile the tag! type:{tag?.GetType().FullName}");
                    }
                    else
                    {
                        var type = method.ReturnType;
                        var isAsync = false;
                        var taskType = typeof(System.Threading.Tasks.Task);
                        MethodInfo taskAwaiterMethod = null;
                        MethodInfo resultMethod = null;
                        if (type.FullName == taskType.FullName || type.IsSubclassOf(typeof(System.Threading.Tasks.Task)))
                        {
                            isAsync = true;
                            taskAwaiterMethod = DynamicHelpers.GetMethod(method.ReturnType, "GetAwaiter", Type.EmptyTypes);
                            resultMethod = DynamicHelpers.GetMethod(taskAwaiterMethod.ReturnType, "GetResult", Type.EmptyTypes);
                            type = resultMethod.ReturnType;
                        }
                        if (type.FullName != "System.Void")
                        {
                            bool needChange = false;
                            var mb = CreateRenderMethod(scope, tagType.Name);
                            var il = mb.GetILGenerator();
                            il.DeclareLocal(typeof(string));
                            var index = 1;
                            if (type.FullName != "System.String")
                            {
                                il.DeclareLocal(type);
                                il.DeclareLocal(typeof(bool));
                                index = 3;
                                needChange = true;
                            }
                            il.Emit(OpCodes.Ldarg_0);
                            il.Emit(OpCodes.Ldarg_2);
                            il.Emit(OpCodes.Call, method);

                            if (isAsync)
                            {
                                il.DeclareLocal(taskAwaiterMethod.ReturnType);
                                il.Emit(OpCodes.Callvirt, taskAwaiterMethod);
                                il.Emit(OpCodes.Stloc, index);
                                il.Emit(OpCodes.Ldloca, index);
                                il.Emit(OpCodes.Call, resultMethod);
                                index++;
                            }

                            Label labelPass = il.DefineLabel();
                            if (needChange)
                            {
                                il.Emit(OpCodes.Stloc_1);
                                if (!method.ReturnType.IsValueType)
                                {
                                    il.Emit(OpCodes.Ldloc_1);
                                    il.Emit(OpCodes.Ldnull);
                                    il.Emit(OpCodes.Cgt_Un);
                                    il.Emit(OpCodes.Stloc_2);
                                    il.Emit(OpCodes.Ldloc_2);
                                    il.Emit(OpCodes.Brfalse_S, labelPass);
                                    il.Emit(OpCodes.Ldloc_1);
                                }
                                else
                                {
                                    il.Emit(OpCodes.Ldloca_S, 1);
                                }
                                Call(il, type, method.ReturnType.GetMethod("ToString", Type.EmptyTypes));
                                il.Emit(OpCodes.Stloc_0);
                            }
                            else
                            {
                                il.Emit(OpCodes.Stloc_0);
                            }
                            il.Emit(OpCodes.Ldarg_1);
                            il.Emit(OpCodes.Ldloc_0);
                            il.Emit(OpCodes.Callvirt, typeof(TextWriter).GetMethod("Write", new Type[] { typeof(string) }));
                            il.MarkLabel(labelPass);

                            il.Emit(OpCodes.Ret);
                            scope.Generator.Emit(OpCodes.Ldarg_0);
                            scope.Generator.Emit(OpCodes.Ldarg_1);
                            scope.Generator.Emit(OpCodes.Ldarg_2);
                            scope.Generator.Emit(OpCodes.Call, mb.GetBaseDefinition());
                        }
                        else
                        {
                            if (isAsync)
                            {
                                var mb = CreateRenderMethod(scope, tagType.Name);
                                var il = mb.GetILGenerator();
                                il.DeclareLocal(taskAwaiterMethod.ReturnType);
                                il.Emit(OpCodes.Ldarg_0);
                                il.Emit(OpCodes.Ldarg_2);
                                il.Emit(OpCodes.Call, method);
                                il.Emit(OpCodes.Callvirt, taskAwaiterMethod);
                                il.Emit(OpCodes.Stloc_0);
                                il.Emit(OpCodes.Ldloca, 0);
                                il.Emit(OpCodes.Call, resultMethod);
                                il.Emit(OpCodes.Ret);
                                scope.Generator.Emit(OpCodes.Ldarg_0);
                                scope.Generator.Emit(OpCodes.Ldarg_1);
                                scope.Generator.Emit(OpCodes.Ldarg_2);
                                scope.Generator.Emit(OpCodes.Call, mb.GetBaseDefinition());
                            }
                            else
                            {
                                scope.Generator.Emit(OpCodes.Ldarg_0);
                                scope.Generator.Emit(OpCodes.Ldarg_2);
                                scope.Generator.Emit(OpCodes.Call, method);
                            }
                        }
                    }
                }
            };
        }

        /// <summary>
        /// 创建VOID的呈现方法
        /// </summary>
        /// <typeparam name="T">ITag</typeparam>
        /// <param name="ctx">CompileContext</param>
        /// <returns>MethodBuilder</returns>
        private MethodBuilder CreateRenderMethod<T>(CompileContext ctx)
        {
            return CreateRenderMethod(ctx, typeof(T).Name);
        }
        /// <summary>
        /// 创建VOID的呈现方法
        /// </summary>
        /// <param name="name">name</param>
        /// <param name="ctx">CompileContext</param>
        /// <returns>MethodBuilder</returns>
        private MethodBuilder CreateRenderMethod(CompileContext ctx, string name)
        {
            return ctx.TypeBuilder.DefineMethod($"Render{name}{ctx.Seed}", MethodAttributes.Public | MethodAttributes.HideBySig, CallingConventions.Standard | CallingConventions.HasThis, typeof(void), new Type[] { typeof(TextWriter), typeof(TemplateContext) });
        }
        /// <summary>
        /// 创建有返回类型的方法
        /// </summary>
        /// <typeparam name="T">ITAG</typeparam>
        /// <param name="ctx">CompileContext</param>
        /// <param name="returnType">return type</param>
        /// <returns></returns>
        private MethodBuilder CreateReutrnMethod<T>(CompileContext ctx, Type returnType)
        {
            if (returnType.FullName == "System.Void")
            {
                return CreateReutrnMethod(ctx.TypeBuilder, $"Execute{typeof(T).Name}{ctx.Seed}", returnType);
            }
            return CreateReutrnMethod(ctx.TypeBuilder, $"Get{typeof(T).Name}{ctx.Seed}", returnType);
        }

        /// <summary>
        /// 创建有返回类型的方法
        /// </summary> 
        /// <param name="builder">TypeBuilder</param>
        /// <param name="name">Method name</param>
        /// <param name="returnType">return type</param>
        /// <returns></returns>
        private MethodBuilder CreateReutrnMethod(TypeBuilder builder, string name, Type returnType)
        {
            if (returnType == null)
            {
                returnType = typeof(object);
            }
            return builder.DefineMethod(name, MethodAttributes.Public | MethodAttributes.HideBySig, CallingConventions.Standard | CallingConventions.HasThis, returnType, new Type[] { typeof(TemplateContext) });
        }

        /// <summary>
        /// object to type
        /// </summary>
        /// <param name="il">ILGenerator</param>
        /// <param name="type">目标类型</param>
        private void ChangeType(ILGenerator il, Type type)
        {
            switch (type.FullName)
            {
                case "System.Int32":
                    il.Emit(OpCodes.Callvirt, typeof(object).GetMethod("ToString"));
                    il.Emit(OpCodes.Call, typeof(Int32).GetMethod("Parse", new[] { typeof(string) }));
                    break;
                case "System.Int64":
                    il.Emit(OpCodes.Callvirt, typeof(object).GetMethod("ToString"));
                    il.Emit(OpCodes.Call, typeof(Int64).GetMethod("Parse", new[] { typeof(string) }));
                    break;
                case "System.Int16":
                    il.Emit(OpCodes.Callvirt, typeof(object).GetMethod("ToString"));
                    il.Emit(OpCodes.Call, typeof(Int16).GetMethod("Parse", new[] { typeof(string) }));
                    break;
                case "System.Decimal":
                    il.Emit(OpCodes.Callvirt, typeof(object).GetMethod("ToString"));
                    il.Emit(OpCodes.Call, typeof(Decimal).GetMethod("Parse", new[] { typeof(string) }));
                    break;
                case "System.Single":
                    il.Emit(OpCodes.Callvirt, typeof(object).GetMethod("ToString"));
                    il.Emit(OpCodes.Call, typeof(Single).GetMethod("Parse", new[] { typeof(string) }));
                    break;
                case "System.Double":
                    il.Emit(OpCodes.Callvirt, typeof(object).GetMethod("ToString"));
                    il.Emit(OpCodes.Call, typeof(Double).GetMethod("Parse", new[] { typeof(string) }));
                    break;
                case "System.Boolean":
                    il.Emit(OpCodes.Callvirt, typeof(object).GetMethod("ToString"));
                    il.Emit(OpCodes.Call, typeof(Boolean).GetMethod("Parse", new[] { typeof(string) }));
                    break;
                case "System.String":
                    il.Emit(OpCodes.Callvirt, typeof(object).GetMethod("ToString"));
                    break;
                default:
                    if (type.IsValueType)
                    {
                        il.Emit(OpCodes.Unbox_Any, type);
                    }
                    else
                    {
                        il.Emit(OpCodes.Castclass, type);
                    }
                    break;
            }
        }

        /// <summary>
        /// 获取编译方法
        /// </summary>
        /// <param name="tag">标签</param>
        /// <param name="scope">编译上下文</param>
        /// <returns></returns>
        public MethodInfo GetCompileMethod(ITag tag, CompileContext scope)
        {
            return GetCompileMethod(tag.GetType().Name, tag, scope);
        }


        /// <summary>
        /// 获取编译方法
        /// </summary>
        /// <param name="name">方法名</param>
        /// <param name="tag">标签</param>
        /// <param name="scope">编译上下文</param>
        /// <returns></returns>
        public MethodInfo GetCompileMethod(string name, ITag tag, CompileContext scope)
        {
            if (returnDict.TryGetValue(name, out var func))
            {
                return func(tag, scope);
            }
            throw new Exception.CompileException($"The tag \"{name}\" is not supported .");
        }

        /// <summary>
        /// 构建一个Render
        /// </summary>
        /// <param name="tag">TAG</param>
        /// <returns></returns>
        public Action<ITag, CompileContext> Build(ITag tag)
        {
            return Build(tag.GetType().Name);
        }

        /// <summary>
        /// 构建一个Render
        /// </summary>
        /// <typeparam name="T">ITag</typeparam>
        /// <returns></returns>
        public Action<ITag, CompileContext> Build<T>() where T : ITag
        {
            return Build(typeof(T).Name);
        }

        /// <summary>
        /// 构建一个Render
        /// </summary>
        /// <param name="name">TAG NAME</param>
        /// <returns></returns>
        public Action<ITag, CompileContext> Build(string name)
        {
            if (renderDict.TryGetValue(name, out var func))
            {
                return func;
            }
            return defaultRender;
            // throw new Exception.CompileException($"The tag \"{name}\" is not supported .");
        }

        /// <summary>
        /// 设置Render
        /// </summary>
        /// <typeparam name="T">ITag</typeparam>
        /// <param name="action">action</param>
        public void SetRenderFunc<T>(Action<ITag, CompileContext> action) where T : ITag
        {
            SetRenderFunc(typeof(T).Name, action);
        }
        /// <summary>
        /// 设置Default Render
        /// </summary>
        /// <typeparam name="T">ITag</typeparam> 
        public void SetRenderFunc<T>() where T : ITag
        {
            SetRenderFunc(typeof(T).Name);
        }
        /// <summary>
        /// 设置Render
        /// </summary>
        /// <param name="name">tag name</param>
        /// <param name="action">action</param>
        public void SetRenderFunc(string name, Action<ITag, CompileContext> action)
        {
            renderDict[name] = action;
        }
        /// <summary>
        /// 设置Default Render
        /// </summary>
        /// <param name="name">tag name</param>
        public void SetRenderFunc(string name)
        {
            renderDict[name] = (tag, scope) =>
            {
                var tagType = tag.GetType();
                if (tagType.IsSubclassOf(typeof(TypeTag<>)))
                {
                    var m = tagType.GetProperty("Value");
                    if (m == null)
                    {
                        throw new Exception.CompileException($"cannot compile the tag! type:{tagType.FullName}");
                    }
                    scope.Generator.Emit(OpCodes.Ldarg_1);
                    var result = m.GetValue(tag, null);
                    if (result == null)
                    {
                        scope.Generator.Emit(OpCodes.Ldstr, string.Empty);
                    }
                    else
                    {
                        scope.Generator.Emit(OpCodes.Ldstr, result.ToString());
                    }
                    scope.Generator.Emit(OpCodes.Callvirt, typeof(TextWriter).GetMethod("Write", new Type[] { typeof(string) }));
                }
                else
                {
                    MethodInfo method = null;
                    if (tag is ReferenceTag)
                    {
                        method = GetCompileMethod((tag as ReferenceTag).Child, scope);
                    }
                    else
                    {
                        method = GetCompileMethod(tag, scope);
                    }
                    if (method == null)
                    {
                        throw new Exception.CompileException($"cannot compile the tag! type:{tag?.GetType().FullName}");
                    }
                    else
                    {
                        var type = method.ReturnType;
                        if (type.FullName != "System.Void")
                        {
                            bool needChange = false;
                            var mb = CreateRenderMethod(scope, name);
                            var il = mb.GetILGenerator();
                            il.DeclareLocal(typeof(string));
                            if (type.FullName != "System.String")
                            {
                                il.DeclareLocal(type);
                                il.DeclareLocal(typeof(bool));
                                needChange = true;
                            }
                            il.Emit(OpCodes.Ldarg_0);
                            il.Emit(OpCodes.Ldarg_2);
                            il.Emit(OpCodes.Call, method);

                            Label labelPass = il.DefineLabel();
                            if (needChange)
                            {
                                il.Emit(OpCodes.Stloc_1);
                                if (!method.ReturnType.IsValueType)
                                {
                                    il.Emit(OpCodes.Ldloc_1);
                                    il.Emit(OpCodes.Ldnull);
                                    il.Emit(OpCodes.Cgt_Un);
                                    il.Emit(OpCodes.Stloc_2);
                                    il.Emit(OpCodes.Ldloc_2);
                                    il.Emit(OpCodes.Brfalse_S, labelPass);
                                    il.Emit(OpCodes.Ldloc_1);
                                }
                                else
                                {
                                    il.Emit(OpCodes.Ldloca_S, 1);
                                }
                                Call(il, type, method.ReturnType.GetMethod("ToString", Type.EmptyTypes));
                                il.Emit(OpCodes.Stloc_0);
                            }
                            else
                            {
                                il.Emit(OpCodes.Stloc_0);
                            }
                            il.Emit(OpCodes.Ldarg_1);
                            il.Emit(OpCodes.Ldloc_0);
                            il.Emit(OpCodes.Callvirt, typeof(TextWriter).GetMethod("Write", new Type[] { typeof(string) }));
                            il.MarkLabel(labelPass);

                            il.Emit(OpCodes.Ret);
                            scope.Generator.Emit(OpCodes.Ldarg_0);
                            scope.Generator.Emit(OpCodes.Ldarg_1);
                            scope.Generator.Emit(OpCodes.Ldarg_2);
                            scope.Generator.Emit(OpCodes.Call, mb.GetBaseDefinition());
                        }
                        else
                        {
                            scope.Generator.Emit(OpCodes.Ldarg_0);
                            scope.Generator.Emit(OpCodes.Ldarg_2);
                            scope.Generator.Emit(OpCodes.Call, method);
                        }
                    }
                }
            };
        }
        /// <summary>
        /// 加载变量
        /// </summary>
        /// <param name="il">ILGenerator</param>
        /// <param name="type">Type</param>
        /// <param name="index">变量所在索引</param>
        public void LoadVariable(ILGenerator il, Type type, int index)
        {
            if (type.IsValueType)
            {
                il.Emit(OpCodes.Ldloca, index);
            }
            else
            {
                il.Emit(OpCodes.Ldloc, index);
            }
        }

        /// <summary>
        /// 调用方法
        /// </summary>
        /// <param name="il"></param>
        /// <param name="type"></param>
        /// <param name="method"></param>
        public void Call(ILGenerator il, Type type, MethodInfo method)
        {
            if (method.IsStatic || (!method.IsAbstract && !method.IsVirtual) || type.IsValueType)
            {
                il.Emit(OpCodes.Call, method);
            }
            else
            {
                if (method.IsVirtual && type.IsValueType && !method.DeclaringType.IsValueType)
                {
                    il.Emit(OpCodes.Constrained, type);
                }
                il.Emit(OpCodes.Callvirt, method);
            }
        }

        /// <summary>
        /// 设置编译方法
        /// </summary>
        /// <typeparam name="T">ITag</typeparam>
        /// <param name="func">编译方法</param>
        public void Register<T>(Func<ITag, CompileContext, MethodInfo> func) where T : ITag
        {
            Register(typeof(T).Name, func);
        }
        /// <summary>
        /// 设置编译方法
        /// </summary>
        /// <param name="name">标签名</param>
        /// <param name="func">编译方法</param>
        public void Register(string name, Func<ITag, CompileContext, MethodInfo> func)
        {
            returnDict[name] = func;
        }

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
                var mb = this.CreateReutrnMethod<VariableTag>(c, type);
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
                    ChangeType(il, type);
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
                        Call(il, parentType, getMethod);
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
                var mb = this.CreateReutrnMethod<IndexValueTag>(c, type);
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
                    switch (type.FullName)
                    {
                        case "System.Double":
                            il.Emit(OpCodes.Ldelem_R8);
                            break;
                        case "System.Single":
                            il.Emit(OpCodes.Ldelem_R4);
                            break;
                        case "System.Int64":
                            il.Emit(OpCodes.Ldelem_I8);
                            break;
                        case "System.Int32":
                            il.Emit(OpCodes.Ldelem_I4);
                            break;
                        case "System.UInt32":
                            il.Emit(OpCodes.Ldelem_U4);
                            break;
                        case "System.Int16":
                            il.Emit(OpCodes.Ldelem_I2);
                            break;
                        case "System.UInt16":
                        case "System.Char":
                            il.Emit(OpCodes.Ldelem_U2);
                            break;
                        case "System.Byte":
                            il.Emit(OpCodes.Ldelem_U1);
                            break;
                        default:
                            //il.Emit(OpCodes.Ldelem);
                            if (type.IsValueType)
                            {

                                il.Emit(OpCodes.Ldelem, type);
                                //il.Emit(OpCodes.Ldelema,type);
                            }
                            else
                            {
                                il.Emit(OpCodes.Ldelem_Ref);
                            }
                            break;
                    }
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
                var mb = this.CreateReutrnMethod<SetTag>(c, type);
                var il = mb.GetILGenerator();
                il.DeclareLocal(retunType);

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
                il.Emit(OpCodes.Callvirt, DynamicHelpers.GetGenericMethod(typeof(VariableScope), new Type[] { retunType }, "Set", new Type[] { typeof(string), retunType }));
                il.Emit(OpCodes.Ret);
                return mb.GetBaseDefinition();
            });
            this.Register<BooleanTag>((tag, c) =>
            {
                var t = tag as BooleanTag;
                var type = t.Value.GetType();
                var mb = this.CreateReutrnMethod<BooleanTag>(c, type);
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
                var mb = this.CreateReutrnMethod<EndTag>(c, type);
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
                var mb = this.CreateReutrnMethod<NumberTag>(c, type);
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
                var mb = this.CreateReutrnMethod<StringTag>(c, type);
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
                    var mb = this.CreateReutrnMethod<TextTag>(c, type);
                    var il = mb.GetILGenerator();
                    il.Emit(OpCodes.Ldstr, t.Text);
                    il.Emit(OpCodes.Ret);
                    return mb.GetBaseDefinition();
                }
                return null;
            });
            this.Register<FunctaionTag>((tag, c) =>
            {
#region
                /*
                var t = tag as FunctaionTag;
                var bodyType = c.Data.GetType(t.Name);
                if (bodyType.BaseType.Name != "MulticastDelegate")
                {
                    throw new ArgumentException("must delegate!");
                }
                var method = bodyType.GetMethod("Invoke");
                var type = method.ReturnType;
                var mb = this.CreateReutrnMethod<FunctaionTag>(c, type);
                var il = mb.GetILGenerator();

                il.DeclareLocal(typeof(object));
                il.DeclareLocal(bodyType);
                il.DeclareLocal(type);
                Type[] paramType = new Type[t.Children.Count];
                for (int i = 0; i < t.Children.Count; i++)
                {
                    paramType[i] = Compiler.TypeGuess.GetType(t.Children[i], c);
                    il.DeclareLocal(paramType[i]);
                }

                il.Emit(OpCodes.Nop);
                il.Emit(OpCodes.Ldarg_1);
                il.Emit(OpCodes.Callvirt, getVariableScope);
                il.Emit(OpCodes.Ldstr, t.Name);
                il.Emit(OpCodes.Callvirt, getVariableValue);
                il.Emit(OpCodes.Stloc_0);
                il.Emit(OpCodes.Ldloc_0);
                il.Emit(OpCodes.Isinst, bodyType);
                il.Emit(OpCodes.Stloc_1);

                for (int i = 0; i < t.Children.Count; i++)
                {
                    if (!returnDict.TryGetValue(t.Children[i].GetType().Name, out var func))
                    {
                        throw new ArgumentNullException("compile error");
                    }
                    il.Emit(OpCodes.Ldarg_0);
                    il.Emit(OpCodes.Ldarg_1);
                    il.Emit(OpCodes.Call, func(t.Children[i], c));
                    il.Emit(OpCodes.Stloc, 3 + i);
                }

                il.Emit(OpCodes.Ldloc, 1);
                for (int i = 0; i < t.Children.Count; i++)
                {
                    il.Emit(OpCodes.Ldloc, 3 + i);
                }
                il.Emit(OpCodes.Callvirt, method);
                //il.Emit(OpCodes.Ldloc_1);
                //il.Emit(OpCodes.Ldc_I4, t.Children.Count);
                //il.Emit(OpCodes.Newarr, typeof(object));


                //for (int i = 0; i < t.Children.Count; i++)
                //{
                //    il.Emit(OpCodes.Dup);
                //    il.Emit(OpCodes.Ldc_I4, i);
                //    il.Emit(OpCodes.Ldloc,2+i);
                //    if (paramType[i].IsValueType)
                //    {
                //        il.Emit(OpCodes.Unbox_Any, paramType[i]);
                //    }
                //    il.Emit(OpCodes.Stelem_Ref);
                //}
                //il.Emit(OpCodes.Callvirt, typeof(Delegate).GetMethod("DynamicInvoke",new[] { typeof(object[]) }));

                //ChangeType(il,type);

                il.Emit(OpCodes.Ret);
                return mb.GetBaseDefinition(); 
                 */
#endregion

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
                if (t.Parent != null)
                {
                    baseType = Compiler.TypeGuess.GetType(t.Parent, c);
                    method = DynamicHelpers.GetMethod(baseType, t.Name, paramType);
                    if (method == null)
                    {
                        throw new CompileException($"[FunctaionTag]:method \"{t.Name}\" cannot be found!");
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
                var mb = this.CreateReutrnMethod<FunctaionTag>(c, method.ReturnType);
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
                    if (!method.IsStatic)
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
                if (!method.IsStatic)
                {
                    LoadVariable(il, baseType, 0);
                }
                for (int i = 0; i < paramType.Length; i++)
                {
                    il.Emit(OpCodes.Ldloc, len + i);
                }
                Call(il, baseType, method);
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
                var t = tag as ReferenceTag;
                var type = Compiler.TypeGuess.GetType(t, c);
                var mb = this.CreateReutrnMethod<ReferenceTag>(c, type);
                var il = mb.GetILGenerator();
                var method = GetCompileMethod(t.Child, c);
                il.Emit(OpCodes.Ldarg_0);
                il.Emit(OpCodes.Ldarg_1);
                il.Emit(OpCodes.Call, method);
                il.Emit(OpCodes.Ret);
                return mb.GetBaseDefinition();
            });
            this.Register<ForeachTag>((tag, c) =>
            {
                var t = tag as ForeachTag;
                var type = Compiler.TypeGuess.GetType(t, c);
                var stringBuilderType = typeof(System.Text.StringBuilder);
                var enumerableType = typeof(System.Collections.IEnumerable);
                var variableScopeType = typeof(VariableScope);
                var enumeratorType = typeof(System.Collections.IEnumerator);
                var templateContextType = typeof(TemplateContext);
                var childType = TypeGuess.InferChildType(Compiler.TypeGuess.GetType(t.Source, c));
                if (childType.Length != 1)
                {
                    throw new CompileException("[ForeachTag]:source error.");
                }
                var old = c.Data;
                var scope = new VariableScope(old);
                scope.SetElement(t.Name, new VariableElement(childType[0], null));
                scope.SetElement("foreachIndex", new VariableElement(typeof(int), null));
                c.Data = scope;
                var mb = this.CreateReutrnMethod<ForeachTag>(c, type);
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
                var types = new Type[t.Children.Count];
                for (var i = 0; i < t.Children.Count; i++)
                {
                    types[i] = Compiler.TypeGuess.GetType(t.Children[i], c);
                    if (types[i].FullName == "System.Void" || t.Children[i] is TextTag)
                    {
                        continue;
                    }
                    il.DeclareLocal(types[i]);
                }
                il.DeclareLocal(typeof(bool));
                il.DeclareLocal(typeof(string));
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
                if (childType[0].IsValueType)
                {
                    il.Emit(OpCodes.Unbox_Any, childType[0]);
                }
                else
                {
                    il.Emit(OpCodes.Isinst, childType[0]);
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
                int paramIndex = 0;
                for (var i = 0; i < t.Children.Count; i++)
                {
                    if (t.Children[i] is EndTag)
                    {
                        continue;
                    }
                    if (t.Children[i] is TextTag)
                    {
                        var textTag = t.Children[i] as TextTag;
                        il.Emit(OpCodes.Ldloc, 0);
                        if (c.StripWhiteSpace)
                        {
                            il.Emit(OpCodes.Ldstr, textTag.Text.Trim());
                        }
                        else
                        {
                            il.Emit(OpCodes.Ldstr, textTag.Text);
                        }
                        il.Emit(OpCodes.Callvirt, DynamicHelpers.GetMethod(stringBuilderType, "Append", new Type[] { typeof(string) }));
                        il.Emit(OpCodes.Pop);
                    }
                    else
                    {
                        var m = GetCompileMethod(t.Children[i], c);
                        if (m.ReturnType.FullName != "System.Void")
                        {
                            il.Emit(OpCodes.Ldloc, 0);
                            il.Emit(OpCodes.Ldarg_0);
                            il.Emit(OpCodes.Ldloc_2);
                            il.Emit(OpCodes.Call, m);

                            il.Emit(OpCodes.Stloc_S, 7 + paramIndex);

                            LoadVariable(il, m.ReturnType, 7 + paramIndex);

                            if (m.ReturnType.FullName != "System.String")
                            {

                                Call(il, m.ReturnType, DynamicHelpers.GetMethod(m.ReturnType, "ToString", Type.EmptyTypes));
                                //il.Emit(OpCodes.Call, DynamicHelpers.GetMethod(m.ReturnType, "ToString", Type.EmptyTypes));
                            }
                            il.Emit(OpCodes.Callvirt, DynamicHelpers.GetMethod(stringBuilderType, "Append", new Type[] { typeof(string) }));
                            il.Emit(OpCodes.Pop);
                            paramIndex++;
                        }
                        else
                        {
                            il.Emit(OpCodes.Ldarg_0);
                            il.Emit(OpCodes.Ldloc_2);
                            il.Emit(OpCodes.Call, m);
                        }
                    }
                }
                il.Emit(OpCodes.Nop);
                il.MarkLabel(labelNext);
                il.Emit(OpCodes.Ldloc_S, 4);
                il.Emit(OpCodes.Callvirt, DynamicHelpers.GetMethod(enumeratorType, "MoveNext", Type.EmptyTypes));
                il.Emit(OpCodes.Stloc_S, 7 + paramIndex);
                il.Emit(OpCodes.Ldloc_S, 7 + paramIndex);
                il.Emit(OpCodes.Brtrue, labelStart);
                il.MarkLabel(labelEnd);
                il.Emit(OpCodes.Ldloc_0);
                Call(il, stringBuilderType, DynamicHelpers.GetMethod(stringBuilderType, "ToString", Type.EmptyTypes));
                //il.Emit(OpCodes.Callvirt, DynamicHelpers.GetMethod(stringBuilderType, "ToString", Type.EmptyTypes));
                il.Emit(OpCodes.Stloc_S, 7 + paramIndex + 1);
                il.Emit(OpCodes.Ldloc_S, 7 + paramIndex + 1);
                il.Emit(OpCodes.Ret);
                c.Data = old;
                return mb.GetBaseDefinition();
            });
            this.Register<LogicTag>((tag, c) =>
            {
                var t = tag as LogicTag;
                var type = Compiler.TypeGuess.GetType(t, c);
                var mb = this.CreateReutrnMethod<LogicTag>(c, type);
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
                var mb = this.CreateReutrnMethod<ArithmeticTag>(c, type);
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
                    var stringBuilderType = typeof(System.Text.StringBuilder);
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
                        if (m.ReturnType.FullName != "System.String")
                        {
                            il.DeclareLocal(m.ReturnType);
                            il.Emit(OpCodes.Stloc, index);
                            LoadVariable(il, m.ReturnType, index);
                            index++;
                            Call(il, m.ReturnType, DynamicHelpers.GetMethod(m.ReturnType, "ToString", Type.EmptyTypes));
                        }
                        il.Emit(OpCodes.Callvirt, DynamicHelpers.GetMethod(stringBuilderType, "Append", new Type[] { typeof(string) }));
                        il.Emit(OpCodes.Pop);
                    }

                    il.Emit(OpCodes.Ldloc_1);
                    Call(il, stringBuilderType, DynamicHelpers.GetMethod(stringBuilderType, "ToString", Type.EmptyTypes));
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
                var t = tag as ElseifTag;
                var type = Compiler.TypeGuess.GetType(t, c);
                var stringBuilderType = typeof(System.Text.StringBuilder);
                var mb = this.CreateReutrnMethod<ElseifTag>(c, type);
                var il = mb.GetILGenerator();
                //var method = GetCompileMethod(t.Condition, c);
                Label labelEnd = il.DefineLabel();
                il.DeclareLocal(type);
                var index = 1;
                if (t.Children.Count != 1)
                {
                    il.DeclareLocal(stringBuilderType);
                    il.Emit(OpCodes.Newobj, stringBuilderType.GetConstructor(Type.EmptyTypes));
                    il.Emit(OpCodes.Stloc_1);
                    index++;
                }
                //il.Emit(OpCodes.Ldarg_0);
                //il.Emit(OpCodes.Ldarg_1);
                //il.Emit(OpCodes.Call, method);
                //if (method.ReturnType.Name != "Boolean")
                //{
                //    var cm = DynamicHelpers.GetMethod(typeof(Utility), "ToBoolean", new Type[] { method.ReturnType });
                //    if (cm == null)
                //    {
                //        cm = DynamicHelpers.GetMethod(typeof(Utility), "ToBoolean", new Type[] { typeof(object) });
                //        if (method.ReturnType.IsValueType)
                //        {
                //            il.Emit(OpCodes.Box, typeof(object));
                //        }
                //        else
                //        {
                //            il.Emit(OpCodes.Castclass, typeof(object));
                //        }
                //    }
                //    il.Emit(OpCodes.Callvirt, cm);
                //}
                //il.Emit(OpCodes.Ldc_I4_1);
                //il.Emit(OpCodes.Brfalse, labelEnd);
                if (t.Children.Count == 1)
                {
                    var childMethod = GetCompileMethod(t.Children[0], c);
                    il.Emit(OpCodes.Ldarg_0);
                    il.Emit(OpCodes.Ldarg_1);
                    il.Emit(OpCodes.Call, childMethod);
                    il.Emit(OpCodes.Stloc_0);
                }
                else
                {
                    for (var i = 0; i < t.Children.Count; i++)
                    {
                        if (t.Children[i] is EndTag)
                        {
                            continue;
                        }
                        if (t.Children[i] is TextTag)
                        {
                            var textTag = t.Children[i] as TextTag;
                            il.Emit(OpCodes.Ldloc_1);
                            if (c.StripWhiteSpace)
                            {
                                il.Emit(OpCodes.Ldstr, textTag.Text.Trim());
                            }
                            else
                            {
                                il.Emit(OpCodes.Ldstr, textTag.Text);
                            }
                            il.Emit(OpCodes.Callvirt, DynamicHelpers.GetMethod(stringBuilderType, "Append", new Type[] { typeof(string) }));
                            il.Emit(OpCodes.Pop);
                        }
                        else
                        {
                            var m = GetCompileMethod(t.Children[i], c);
                            if (m.ReturnType.FullName != "System.Void")
                            {
                                il.Emit(OpCodes.Ldloc_1);
                                il.Emit(OpCodes.Ldarg_0);
                                il.Emit(OpCodes.Ldarg_1);
                                il.Emit(OpCodes.Call, m);
                                if (m.ReturnType.FullName != "System.String")
                                {
                                    il.DeclareLocal(m.ReturnType);
                                    il.Emit(OpCodes.Stloc, index);
                                    LoadVariable(il, m.ReturnType, index);
                                    index++;
                                    //il.Emit(OpCodes.Call, DynamicHelpers.GetMethod(m.ReturnType, "ToString", Type.EmptyTypes));
                                    Call(il, m.ReturnType, DynamicHelpers.GetMethod(m.ReturnType, "ToString", Type.EmptyTypes));
                                }
                                il.Emit(OpCodes.Callvirt, DynamicHelpers.GetMethod(stringBuilderType, "Append", new Type[] { typeof(string) }));
                                il.Emit(OpCodes.Pop);
                            }
                            else
                            {
                                il.Emit(OpCodes.Ldarg_0);
                                il.Emit(OpCodes.Ldarg_1);
                                il.Emit(OpCodes.Call, m);
                            }
                        }
                    }
                    il.Emit(OpCodes.Ldloc_1);
                    //il.Emit(OpCodes.Callvirt, DynamicHelpers.GetMethod(stringBuilderType, "ToString", Type.EmptyTypes));
                    Call(il, stringBuilderType, DynamicHelpers.GetMethod(stringBuilderType, "ToString", Type.EmptyTypes));
                    il.Emit(OpCodes.Stloc_0);
                }
                il.MarkLabel(labelEnd);
                il.Emit(OpCodes.Ldloc_0);
                il.Emit(OpCodes.Ret);
                return mb.GetBaseDefinition();
            });
            this.Register<ElseTag>((tag, c) =>
            {
                var t = tag as ElseTag;
                var type = Compiler.TypeGuess.GetType(t, c);
                var stringBuilderType = typeof(System.Text.StringBuilder);
                var mb = this.CreateReutrnMethod<ElseTag>(c, type);
                var il = mb.GetILGenerator();
                //var method = GetCompileMethod(t.Condition, c);
                Label labelEnd = il.DefineLabel();
                il.DeclareLocal(type);
                var index = 1;
                if (t.Children.Count != 1)
                {
                    il.DeclareLocal(stringBuilderType);
                    il.Emit(OpCodes.Newobj, stringBuilderType.GetConstructor(Type.EmptyTypes));
                    il.Emit(OpCodes.Stloc_1);
                    index++;
                }
                if (t.Children.Count == 1)
                {
                    var childMethod = GetCompileMethod(t.Children[0], c);
                    il.Emit(OpCodes.Ldarg_0);
                    il.Emit(OpCodes.Ldarg_1);
                    il.Emit(OpCodes.Call, childMethod);
                    il.Emit(OpCodes.Stloc_0);
                }
                else
                {
                    for (var i = 0; i < t.Children.Count; i++)
                    {
                        if (t.Children[i] is EndTag)
                        {
                            continue;
                        }
                        if (t.Children[i] is TextTag)
                        {
                            var textTag = t.Children[i] as TextTag;
                            il.Emit(OpCodes.Ldloc_1);
                            if (c.StripWhiteSpace)
                            {
                                il.Emit(OpCodes.Ldstr, textTag.Text.Trim());
                            }
                            else
                            {
                                il.Emit(OpCodes.Ldstr, textTag.Text);
                            }
                            il.Emit(OpCodes.Callvirt, DynamicHelpers.GetMethod(stringBuilderType, "Append", new Type[] { typeof(string) }));
                            il.Emit(OpCodes.Pop);
                        }
                        else
                        {
                            var m = GetCompileMethod(t.Children[i], c);
                            if (m.ReturnType.FullName != "System.Void")
                            {
                                il.Emit(OpCodes.Ldloc_1);
                                il.Emit(OpCodes.Ldarg_0);
                                il.Emit(OpCodes.Ldarg_1);
                                il.Emit(OpCodes.Call, m);
                                if (m.ReturnType.FullName != "System.String")
                                {
                                    il.DeclareLocal(m.ReturnType);
                                    il.Emit(OpCodes.Stloc, index);
                                    LoadVariable(il, m.ReturnType, index);
                                    index++;
                                    il.Emit(OpCodes.Call, DynamicHelpers.GetMethod(m.ReturnType, "ToString", Type.EmptyTypes));
                                }
                                il.Emit(OpCodes.Callvirt, DynamicHelpers.GetMethod(stringBuilderType, "Append", new Type[] { typeof(string) }));
                                il.Emit(OpCodes.Pop);
                            }
                            else
                            {
                                il.Emit(OpCodes.Ldarg_0);
                                il.Emit(OpCodes.Ldarg_1);
                                il.Emit(OpCodes.Call, m);
                            }
                        }
                    }
                    il.Emit(OpCodes.Ldloc_1);
                    il.Emit(OpCodes.Callvirt, DynamicHelpers.GetMethod(stringBuilderType, "ToString", Type.EmptyTypes));
                    il.Emit(OpCodes.Stloc_0);
                }
                il.MarkLabel(labelEnd);
                il.Emit(OpCodes.Ldloc_0);
                il.Emit(OpCodes.Ret);
                return mb.GetBaseDefinition();
            });
            this.Register<IfTag>((tag, c) =>
            {
#region
                //var t = tag as IfTag;
                //var type = Compiler.TypeGuess.GetType(t, c);
                //var mb = this.CreateReutrnMethod<IfTag>(c, type);
                //var il = mb.GetILGenerator();
                //Label labelEnd = il.DefineLabel();
                //var hasReturn = false;
                //if (type.FullName != "System.Void")
                //{
                //    il.DeclareLocal(type);
                //    hasReturn = true;
                //}
                //Label[] lables = new Label[t.Children.Count - 1];
                //for (var i = 0; i < lables.Length; i++)
                //{
                //    lables[i] = il.DefineLabel();
                //}
                //for (var i = 0; i < t.Children.Count; i++)
                //{
                //    if (t.Children[i] is EndTag)
                //    {
                //        continue;
                //    }
                //    var ifTag = t.Children[i] as ElseifTag;
                //    if (!(t.Children[i] is ElseTag))
                //    {
                //        var m = GetCompileMethod(ifTag.Condition, c);
                //        il.Emit(OpCodes.Ldarg_0);
                //        il.Emit(OpCodes.Ldarg_1);
                //        il.Emit(OpCodes.Call, m);
                //        if (m.ReturnType.Name != "Boolean")
                //        {
                //            var cm = DynamicHelpers.GetMethod(typeof(Utility), "ToBoolean", new Type[] { m.ReturnType });
                //            if (cm == null)
                //            {
                //                cm = DynamicHelpers.GetMethod(typeof(Utility), "ToBoolean", new Type[] { typeof(object) });
                //                if (m.ReturnType.IsValueType)
                //                {
                //                    il.Emit(OpCodes.Box, typeof(object));
                //                }
                //                else
                //                {
                //                    il.Emit(OpCodes.Castclass, typeof(object));
                //                }
                //            }
                //            il.Emit(OpCodes.Callvirt, cm);
                //        }
                //        il.Emit(OpCodes.Brfalse, lables[i]);
                //    }

                //    var execute = GetCompileMethod(ifTag, c);
                //    il.Emit(OpCodes.Ldarg_0);
                //    il.Emit(OpCodes.Ldarg_1);
                //    il.Emit(OpCodes.Call, execute);

                //    if (hasReturn)
                //    {
                //        il.Emit(OpCodes.Stloc_0);
                //        il.Emit(OpCodes.Br, lables[lables.Length - 1]);
                //    }
                //    else
                //    {
                //        il.Emit(OpCodes.Br, labelEnd);
                //    }

                //    il.MarkLabel(lables[i]);
                //}
                //il.Emit(OpCodes.Ldloc_0);
                //il.MarkLabel(labelEnd);
                //il.Emit(OpCodes.Ret);
                //return mb.GetBaseDefinition();
#endregion
                var t = tag as IfTag;
                var type = Compiler.TypeGuess.GetType(t, c);
                var mb = this.CreateReutrnMethod<IfTag>(c, type);
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
                            //il.DeclareLocal(typeof(bool));
                            //il.Emit(OpCodes.Ldnull);
                            //il.Emit(OpCodes.Ceq);
                            //il.Emit(OpCodes.Stloc, index);
                            //il.Emit(OpCodes.Ldloc, index);
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
                            //il.Emit(OpCodes.Callvirt, DynamicHelpers.GetMethod(execute.ReturnType, "ToString", Type.EmptyTypes));
                            Call(il, execute.ReturnType, DynamicHelpers.GetMethod(execute.ReturnType, "ToString", Type.EmptyTypes));
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
                var mb = this.CreateReutrnMethod<IncludeTag>(c, type);
                var il = mb.GetILGenerator();
                var strTag = t.Path as StringTag;
                if (strTag != null)
                {
                    var res = Runtime.Loader.Load(strTag.Value, Runtime.Encoding,
#if NETCOREAPP || NETSTANDARD
                        c.GetResourceDirectories()
#else
                        TemplateContextExtensions.GetResourceDirectories(c)
#endif
                        );
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
                    var strArrType = typeof(string[]);
                    var resType = typeof(Resources.ResourceInfo);
                    var ctxType = typeof(TemplateContext);
                    Label labelEnd = il.DefineLabel();
                    Label labelSuccess = il.DefineLabel();
                    il.DeclareLocal(strType);
                    il.DeclareLocal(typeof(bool));
                    il.DeclareLocal(strArrType);
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
                        il.Emit(OpCodes.Stloc, 6);
                        LoadVariable(il, m.ReturnType, 6);
                        //il.Emit(OpCodes.Call, DynamicHelpers.GetMethod(m.ReturnType, "ToString", Type.EmptyTypes));
                        Call(il, m.ReturnType, DynamicHelpers.GetMethod(m.ReturnType, "ToString", Type.EmptyTypes));
                    }
                    il.Emit(OpCodes.Stloc_0);
                    il.Emit(OpCodes.Ldloc_0);
                    il.Emit(OpCodes.Ldnull);
                    il.Emit(OpCodes.Cgt_Un);
                    il.Emit(OpCodes.Stloc_1);
                    il.Emit(OpCodes.Ldloc_1);
                    il.Emit(OpCodes.Brfalse, labelEnd);

                    il.Emit(OpCodes.Ldarg_1);
                    il.Emit(OpCodes.Call, DynamicHelpers.GetMethod(typeof(TemplateContextExtensions), "GetResourceDirectories", new Type[] { ctxType }));
                    il.Emit(OpCodes.Stloc_2);
                    il.Emit(OpCodes.Ldarg_1);
                    il.Emit(OpCodes.Callvirt, DynamicHelpers.GetPropertyGetMethod(ctxType, "Loader"));
                    il.Emit(OpCodes.Ldloc_0);
                    il.Emit(OpCodes.Ldarg_1);
                    il.Emit(OpCodes.Callvirt, DynamicHelpers.GetPropertyGetMethod(ctxType, "Charset"));
                    il.Emit(OpCodes.Ldloc_2);
                    il.Emit(OpCodes.Callvirt, DynamicHelpers.GetMethod(typeof(Resources.IResourceLoader), "Load", new Type[] { strType, typeof(System.Text.Encoding), strArrType }));
                    il.Emit(OpCodes.Stloc_3);
                    il.Emit(OpCodes.Ldloc_3);
                    il.Emit(OpCodes.Ldnull);
                    il.Emit(OpCodes.Cgt_Un);
                    il.Emit(OpCodes.Stloc, 4);
                    il.Emit(OpCodes.Ldloc, 4);
                    il.Emit(OpCodes.Brfalse, labelEnd);

                    il.Emit(OpCodes.Ldloc_3);
                    il.Emit(OpCodes.Callvirt, DynamicHelpers.GetPropertyGetMethod(resType, "Content"));
                    il.Emit(OpCodes.Stloc, 5);
                    il.Emit(OpCodes.Br, labelSuccess);

                    il.MarkLabel(labelEnd);
                    il.Emit(OpCodes.Ldnull);
                    il.Emit(OpCodes.Stloc, 5);


                    il.MarkLabel(labelSuccess);
                    il.Emit(OpCodes.Ldloc, 5);
                }
                il.Emit(OpCodes.Ret);
                return mb.GetBaseDefinition();
            });
            this.Register<LoadTag>((tag, c) =>
            {
                var t = tag as LoadTag;
                var type = Compiler.TypeGuess.GetType(t, c);
                var mb = this.CreateReutrnMethod<LoadTag>(c, type);
                var il = mb.GetILGenerator();
                var strTag = t.Path as StringTag;
                if (strTag != null)
                {
                    var stringBuilderType = typeof(System.Text.StringBuilder);
                    il.DeclareLocal(stringBuilderType);
                    il.DeclareLocal(type);
                    var res = c.Load(strTag.Value);
                    if (res == null)
                    {
                        throw new Exception.CompileException($"[LoadTag] : \"{strTag.Value}\" cannot be found.");
                    }
                    var lexer = new TemplateLexer(res.Content);
                    var ts = lexer.Execute();

                    var parser = new TemplateParser(ts);
                    var tags = parser.Execute();
                    il.Emit(OpCodes.Newobj, stringBuilderType.GetConstructor(Type.EmptyTypes));
                    il.Emit(OpCodes.Stloc_0);
                    var index = 2;
                    for (var i = 0; i < tags.Length; i++)
                    {
                        if (tags[i] is EndTag)
                        {
                            continue;
                        }
                        if (tags[i] is TextTag)
                        {
                            var textTag = tags[i] as TextTag;
                            il.Emit(OpCodes.Ldloc_0);
                            if (c.StripWhiteSpace)
                            {
                                il.Emit(OpCodes.Ldstr, textTag.Text.Trim());
                            }
                            else
                            {
                                il.Emit(OpCodes.Ldstr, textTag.Text);
                            }
                            il.Emit(OpCodes.Callvirt, DynamicHelpers.GetMethod(stringBuilderType, "Append", new Type[] { typeof(string) }));
                            il.Emit(OpCodes.Pop);
                        }
                        else
                        {
                            var m = GetCompileMethod(tags[i], c);
                            if (m.ReturnType.FullName != "System.Void")
                            {
                                il.Emit(OpCodes.Ldloc_0);
                                il.Emit(OpCodes.Ldarg_0);
                                il.Emit(OpCodes.Ldarg_1);
                                il.Emit(OpCodes.Call, m);
                                if (m.ReturnType.FullName != "System.String")
                                {
                                    il.DeclareLocal(m.ReturnType);
                                    il.Emit(OpCodes.Stloc, index);
                                    LoadVariable(il, m.ReturnType, index);
                                    index++;
                                    //il.Emit(OpCodes.Call, DynamicHelpers.GetMethod(m.ReturnType, "ToString", Type.EmptyTypes));
                                    Call(il, m.ReturnType, DynamicHelpers.GetMethod(m.ReturnType, "ToString", Type.EmptyTypes));
                                }
                                il.Emit(OpCodes.Callvirt, DynamicHelpers.GetMethod(stringBuilderType, "Append", new Type[] { typeof(string) }));
                                il.Emit(OpCodes.Pop);
                            }
                            else
                            {
                                il.Emit(OpCodes.Ldarg_0);
                                il.Emit(OpCodes.Ldarg_1);
                                il.Emit(OpCodes.Call, m);
                            }
                        }
                    }

                    il.Emit(OpCodes.Ldloc_0);
                    //il.Emit(OpCodes.Callvirt, DynamicHelpers.GetMethod(stringBuilderType, "ToString", Type.EmptyTypes));
                    Call(il, stringBuilderType, DynamicHelpers.GetMethod(stringBuilderType, "ToString", Type.EmptyTypes));
                    il.Emit(OpCodes.Stloc_1);
                    il.Emit(OpCodes.Ldloc_1);
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
                    il.DeclareLocal(typeof(ICompileTemplate));
                    il.DeclareLocal(typeof(bool));
                    il.DeclareLocal(typeof(System.IO.StringWriter));
                    il.DeclareLocal(typeof(string));

                    il.Emit(OpCodes.Ldarg_0);
                    il.Emit(OpCodes.Ldarg_1);
                    il.Emit(OpCodes.Call, m);
                    if (m.ReturnType.FullName != "System.String")
                    {
                        il.DeclareLocal(m.ReturnType);
                        il.Emit(OpCodes.Stloc, 6);
                        if (m.ReturnType.IsValueType)
                        {
                            il.Emit(OpCodes.Ldloca, 6);
                        }
                        else
                        {
                            il.Emit(OpCodes.Ldloc, 6);
                            il.DeclareLocal(typeof(bool));
                            il.Emit(OpCodes.Ldnull);
                            il.Emit(OpCodes.Cgt_Un);
                            il.Emit(OpCodes.Stloc, 7);
                            il.Emit(OpCodes.Ldloc, 7);
                            il.Emit(OpCodes.Brfalse, labelEnd);

                            il.Emit(OpCodes.Ldloc, 6);
                        }

                        //il.Emit(OpCodes.Call, DynamicHelpers.GetMethod(m.ReturnType, "ToString", Type.EmptyTypes));
                        Call(il, m.ReturnType, DynamicHelpers.GetMethod(m.ReturnType, "ToString", Type.EmptyTypes));
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
                    il.Emit(OpCodes.Call, DynamicHelpers.GetMethod(typeof(Compiler), "GenerateCompileTemplate", new Type[] { typeof(string), typeof(TemplateContext) }));
                    il.Emit(OpCodes.Stloc, 2);
                    il.Emit(OpCodes.Ldloc, 2);
                    il.Emit(OpCodes.Ldnull);
                    il.Emit(OpCodes.Cgt_Un);
                    il.Emit(OpCodes.Stloc, 3);
                    il.Emit(OpCodes.Ldloc, 3);
                    il.Emit(OpCodes.Brfalse, labelEnd);

                    il.Emit(OpCodes.Newobj, typeof(System.IO.StringWriter).GetConstructor(Type.EmptyTypes));
                    il.Emit(OpCodes.Stloc, 4);

                    il.BeginExceptionBlock();

                    il.Emit(OpCodes.Ldloc, 2);
                    il.Emit(OpCodes.Ldloc, 4);
                    il.Emit(OpCodes.Ldarg_1);
                    il.Emit(OpCodes.Call, DynamicHelpers.GetMethod(typeof(ICompileTemplate), "Render", new Type[] { typeof(TextWriter), typeof(TemplateContext) }));

                    il.Emit(OpCodes.Ldloc, 4);
                    //il.Emit(OpCodes.Callvirt, DynamicHelpers.GetMethod(typeof(object), "ToString", Type.EmptyTypes));
                    Call(il, typeof(object), DynamicHelpers.GetMethod(typeof(object), "ToString", Type.EmptyTypes));
                    il.Emit(OpCodes.Stloc, 5);
                    //il.Emit(OpCodes.Leave, labelSuccess);

                    il.BeginFinallyBlock();

                    //il.Emit(OpCodes.Ldloc, 4);
                    //il.Emit(OpCodes.Brfalse, labelEnd);

                    il.Emit(OpCodes.Ldloc, 4);
                    il.Emit(OpCodes.Callvirt, DynamicHelpers.GetMethod(typeof(System.IO.StringWriter), "Dispose", Type.EmptyTypes));

                    //il.Emit(OpCodes.Endfinally);

                    il.EndExceptionBlock();

                    il.Emit(OpCodes.Br, labelSuccess);

                    il.MarkLabel(labelEnd);
                    il.Emit(OpCodes.Ldnull);
                    il.Emit(OpCodes.Stloc, 5);


                    il.MarkLabel(labelSuccess);
                    il.Emit(OpCodes.Ldloc, 5);
                }
                il.Emit(OpCodes.Ret);
                return mb.GetBaseDefinition();
            });
            this.Register<CommentTag>((tag, c) =>
            {
                //var t = tag as CommentTag;
                var type = typeof(void);
                var mb = this.CreateReutrnMethod<CommentTag>(c, type);
                var il = mb.GetILGenerator();
                il.Emit(OpCodes.Ret);
                return mb.GetBaseDefinition();
            });
            this.Register<BodyTag>((tag, c) =>
            {
                var t = tag as BodyTag;
                var type = typeof(string);
                var mb = this.CreateReutrnMethod<CommentTag>(c, type);
                var il = mb.GetILGenerator();
                var stringBuilderType = typeof(System.Text.StringBuilder);
                il.DeclareLocal(stringBuilderType);
                il.DeclareLocal(type);

                il.Emit(OpCodes.Newobj, stringBuilderType.GetConstructor(Type.EmptyTypes));
                il.Emit(OpCodes.Stloc_0);
                var index = 2;
                for (var i = 0; i < t.Children.Count; i++)
                {
                    if (t.Children[i] is EndTag)
                    {
                        continue;
                    }
                    if (t.Children[i] is TextTag)
                    {
                        var textTag = t.Children[i] as TextTag;
                        il.Emit(OpCodes.Ldloc_0);
                        if (c.StripWhiteSpace)
                        {
                            il.Emit(OpCodes.Ldstr, textTag.Text.Trim());
                        }
                        else
                        {
                            il.Emit(OpCodes.Ldstr, textTag.Text);
                        }
                        il.Emit(OpCodes.Callvirt, DynamicHelpers.GetMethod(stringBuilderType, "Append", new Type[] { typeof(string) }));
                        il.Emit(OpCodes.Pop);
                    }
                    else
                    {
                        var m = GetCompileMethod(t.Children[i], c);
                        if (m.ReturnType.FullName != "System.Void")
                        {
                            il.Emit(OpCodes.Ldloc_0);
                            il.Emit(OpCodes.Ldarg_0);
                            il.Emit(OpCodes.Ldarg_1);
                            il.Emit(OpCodes.Call, m);
                            if (m.ReturnType.FullName != "System.String")
                            {
                                il.DeclareLocal(m.ReturnType);
                                il.Emit(OpCodes.Stloc, index);
                                LoadVariable(il, m.ReturnType, index);
                                index++;
                                //il.Emit(OpCodes.Call, DynamicHelpers.GetMethod(m.ReturnType, "ToString", Type.EmptyTypes));
                                Call(il, m.ReturnType, DynamicHelpers.GetMethod(m.ReturnType, "ToString", Type.EmptyTypes));
                            }
                            il.Emit(OpCodes.Callvirt, DynamicHelpers.GetMethod(stringBuilderType, "Append", new Type[] { typeof(string) }));
                            il.Emit(OpCodes.Pop);
                        }
                        else
                        {
                            il.Emit(OpCodes.Ldarg_0);
                            il.Emit(OpCodes.Ldarg_1);
                            il.Emit(OpCodes.Call, m);
                        }
                    }
                }

                il.Emit(OpCodes.Ldloc_0);
                //il.Emit(OpCodes.Callvirt, DynamicHelpers.GetMethod(stringBuilderType, "ToString", Type.EmptyTypes));
                Call(il, stringBuilderType, DynamicHelpers.GetMethod(stringBuilderType, "ToString", Type.EmptyTypes));
                il.Emit(OpCodes.Stloc_1);
                il.Emit(OpCodes.Ldloc_1);

                il.Emit(OpCodes.Ret);
                return mb.GetBaseDefinition();
            });
            this.Register<NullTag>((tag, c) =>
            {
                var type = typeof(object);
                var mb = this.CreateReutrnMethod<NullTag>(c, type);
                var il = mb.GetILGenerator();
                il.Emit(OpCodes.Ldnull);
                il.Emit(OpCodes.Ret);
                return mb.GetBaseDefinition();
            });
            this.Register<ForTag>((tag, c) =>
            {
                var t = tag as ForTag;
                var type = Compiler.TypeGuess.GetType(t, c);
                var stringBuilderType = typeof(System.Text.StringBuilder);
                var templateContextType = typeof(TemplateContext);
                var mb = this.CreateReutrnMethod<ForTag>(c, type);
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
                    if (t.Children[i] is EndTag)
                    {
                        continue;
                    }
                    if (t.Children[i] is TextTag)
                    {
                        var textTag = t.Children[i] as TextTag;
                        il.Emit(OpCodes.Ldloc, 0);
                        if (c.StripWhiteSpace)
                        {
                            il.Emit(OpCodes.Ldstr, textTag.Text.Trim());
                        }
                        else
                        {
                            il.Emit(OpCodes.Ldstr, textTag.Text);
                        }
                        il.Emit(OpCodes.Callvirt, DynamicHelpers.GetMethod(stringBuilderType, "Append", new Type[] { typeof(string) }));
                        il.Emit(OpCodes.Pop);
                    }
                    else
                    {
                        m = GetCompileMethod(t.Children[i], c);
                        if (m.ReturnType.FullName != "System.Void")
                        {
                            il.Emit(OpCodes.Ldloc, 0);
                            il.Emit(OpCodes.Ldarg_0);
                            il.Emit(OpCodes.Ldloc_1);
                            il.Emit(OpCodes.Call, m);
                            if (m.ReturnType.FullName != "System.String")
                            {
                                il.DeclareLocal(m.ReturnType);
                                il.Emit(OpCodes.Stloc, index);
                                LoadVariable(il, m.ReturnType, index);
                                index++;
                                //il.Emit(OpCodes.Call, DynamicHelpers.GetMethod(m.ReturnType, "ToString", Type.EmptyTypes));
                                Call(il, m.ReturnType, DynamicHelpers.GetMethod(m.ReturnType, "ToString", Type.EmptyTypes));
                            }
                            il.Emit(OpCodes.Callvirt, DynamicHelpers.GetMethod(stringBuilderType, "Append", new Type[] { typeof(string) }));
                            il.Emit(OpCodes.Pop);
                        }
                        else
                        {
                            il.Emit(OpCodes.Ldarg_0);
                            il.Emit(OpCodes.Ldloc_1);
                            il.Emit(OpCodes.Call, m);
                        }
                    }
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
                //il.Emit(OpCodes.Call, DynamicHelpers.GetMethod(stringBuilderType, "ToString", Type.EmptyTypes));
                Call(il, stringBuilderType, DynamicHelpers.GetMethod(stringBuilderType, "ToString", Type.EmptyTypes));


                il.Emit(OpCodes.Stloc, 3);
                il.Emit(OpCodes.Ldloc, 3);
                il.Emit(OpCodes.Ret);
                return mb.GetBaseDefinition();
            });
            this.Register<LayoutTag>((tag, c) =>
            {
                var t = tag as LayoutTag;
                var type = Compiler.TypeGuess.GetType(t, c);
                var mb = this.CreateReutrnMethod<LayoutTag>(c, type);
                var il = mb.GetILGenerator();
                var strTag = t.Path as StringTag;
                if (strTag == null)
                {
                    throw new Exception.CompileException($"[LayoutTag] : path must be a string.");
                }
                var stringBuilderType = typeof(System.Text.StringBuilder);
                il.DeclareLocal(stringBuilderType);
                il.DeclareLocal(type);
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
                il.Emit(OpCodes.Newobj, stringBuilderType.GetConstructor(Type.EmptyTypes));
                il.Emit(OpCodes.Stloc_0);
                var index = 2;
                for (var i = 0; i < tags.Length; i++)
                {
                    if (tags[i] is EndTag)
                    {
                        continue;
                    }
                    if (tags[i] is TextTag)
                    {
                        var textTag = tags[i] as TextTag;
                        il.Emit(OpCodes.Ldloc_0);
                        if (c.StripWhiteSpace)
                        {
                            il.Emit(OpCodes.Ldstr, textTag.Text.Trim());
                        }
                        else
                        {
                            il.Emit(OpCodes.Ldstr, textTag.Text);
                        }
                        il.Emit(OpCodes.Callvirt, DynamicHelpers.GetMethod(stringBuilderType, "Append", new Type[] { typeof(string) }));
                        il.Emit(OpCodes.Pop);
                    }
                    else
                    {
                        var m = GetCompileMethod(tags[i], c);
                        if (m.ReturnType.FullName != "System.Void")
                        {
                            il.Emit(OpCodes.Ldloc_0);
                            il.Emit(OpCodes.Ldarg_0);
                            il.Emit(OpCodes.Ldarg_1);
                            il.Emit(OpCodes.Call, m);
                            if (m.ReturnType.FullName != "System.String")
                            {
                                il.DeclareLocal(m.ReturnType);
                                il.Emit(OpCodes.Stloc, index);
                                LoadVariable(il, m.ReturnType, index);
                                index++;
                                //il.Emit(OpCodes.Call, DynamicHelpers.GetMethod(m.ReturnType, "ToString", Type.EmptyTypes));
                                Call(il, m.ReturnType, DynamicHelpers.GetMethod(m.ReturnType, "ToString", Type.EmptyTypes));
                            }
                            il.Emit(OpCodes.Callvirt, DynamicHelpers.GetMethod(stringBuilderType, "Append", new Type[] { typeof(string) }));
                            il.Emit(OpCodes.Pop);
                        }
                        else
                        {
                            il.Emit(OpCodes.Ldarg_0);
                            il.Emit(OpCodes.Ldarg_1);
                            il.Emit(OpCodes.Call, m);
                        }
                    }
                }
                il.Emit(OpCodes.Ldloc_0);
                Call(il, stringBuilderType, DynamicHelpers.GetMethod(stringBuilderType, "ToString", Type.EmptyTypes));
                il.Emit(OpCodes.Stloc_1);
                il.Emit(OpCodes.Ldloc_1);

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
            this.SetRenderFunc("Default");
#endregion
        }

        /// <summary>
        /// find path
        /// </summary>
        /// <param name="path">filename</param>
        /// <param name="ctx">context</param>
        /// <returns></returns>
        public static string FindPath(string path, CompileContext ctx)
        {
#if NETCOREAPP || NETSTANDARD
            string[] paths = ctx.GetResourceDirectories();
#else
            string[] paths = TemplateContextExtensions.GetResourceDirectories(ctx);
#endif
            return Runtime.Loader.FindFullPath(path, paths);
        }
    }
}
