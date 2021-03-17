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
    public partial class CompileBuilder
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
        private Type stringBuilderType;

        /// <summary>
        /// ctor
        /// </summary>
        public CompileBuilder()
        {
            returnDict = new Dictionary<string, Func<ITag, CompileContext, MethodInfo>>(StringComparer.OrdinalIgnoreCase);
            renderDict = new Dictionary<string, Action<ITag, CompileContext>>(StringComparer.OrdinalIgnoreCase);
            getVariableScope = DynamicHelpers.GetPropertyGetMethod(typeof(TemplateContext), "TempData");
            getVariableValue = typeof(VariableScope).GetMethod("get_Item", new[] { typeof(string) });
            defaultRender = GeneralDefaultRender();
            stringBuilderType = typeof(System.Text.StringBuilder);

            Initialize();
        }

        private void DefineRender(ITag tag, CompileContext context, Type tagType)
        {
            MethodInfo method = null;
            if (tag is ReferenceTag)
            {
                method = GetCompileMethod((tag as ReferenceTag).Child, context);
            }
            else
            {
                method = GetCompileMethod(tag, context);
            }
            if (method == null)
            {
                throw new Exception.CompileException($"cannot compile the tag! type:{tag?.GetType().FullName}");
            }

            var type = method.ReturnType;
            var isAsync = false;
            var taskType = typeof(System.Threading.Tasks.Task);
            MethodInfo taskAwaiterMethod = null;
            MethodInfo resultMethod = null;

            var mb = context.CreateRenderMethod(tagType.Name);
            var il = mb.GetILGenerator();
            var exBlock = il.BeginExceptionBlock();
            var lableThrow = il.DefineLabel();
            var labelPass = il.DefineLabel();

            var index = 0;


            if (DynamicHelpers.IsMatchType(type, taskType))
            {
                isAsync = true;
                taskAwaiterMethod = DynamicHelpers.GetMethod(method.ReturnType, "GetAwaiter", Type.EmptyTypes);
                resultMethod = DynamicHelpers.GetMethod(taskAwaiterMethod.ReturnType, "GetResult", Type.EmptyTypes);
                type = resultMethod.ReturnType;
            }
            if (type.FullName != "System.Void")
            {
                il.DeclareLocal(type);
                index = 1;
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
                il.Emit(OpCodes.Stloc_0);
                il.Emit(OpCodes.Ldarg_1);
                MethodInfo writeMethod;
                switch (type.FullName)
                {
                    case "System.Object":
                    case "System.String":
                    case "System.Decimal":
                    case "System.Single":
                    case "System.UInt64":
                    case "System.Int64":
                    case "System.UInt32":
                    case "System.Int32":
                    case "System.Boolean":
                    case "System.Double":
                    case "System.Char":
                        il.Emit(OpCodes.Ldloc_0);
                        writeMethod = typeof(TextWriter).GetMethod("Write", new Type[] { type });
                        break;
                    default:
                        if (type.IsValueType)
                        {
                            il.Emit(OpCodes.Ldloca_S, 0);
                            il.Call(type, typeof(object).GetMethod("ToString", Type.EmptyTypes));
                            writeMethod = typeof(TextWriter).GetMethod("Write", new Type[] { typeof(string) });
                        }
                        else
                        {
                            il.Emit(OpCodes.Ldloc_0);
                            writeMethod = typeof(TextWriter).GetMethod("Write", new Type[] { typeof(object) });
                        }
                        break;

                }
                il.Emit(OpCodes.Callvirt, writeMethod);
            }
            else
            {
                if (isAsync)
                {
                    il.DeclareLocal(taskAwaiterMethod.ReturnType);
                    il.Emit(OpCodes.Ldarg_0);
                    il.Emit(OpCodes.Ldarg_2);
                    il.Emit(OpCodes.Call, method);
                    il.Emit(OpCodes.Callvirt, taskAwaiterMethod);
                    il.Emit(OpCodes.Stloc_0);
                    il.Emit(OpCodes.Ldloca, 0);
                    il.Emit(OpCodes.Call, resultMethod);
                    index = 1;
                }
                else
                {
                    il.Emit(OpCodes.Ldarg_0);
                    il.Emit(OpCodes.Ldarg_2);
                    il.Emit(OpCodes.Call, method);
                }
            }

            il.DeclareLocal(typeof(System.Exception));
            il.DeclareLocal(typeof(bool));

            il.BeginCatchBlock(typeof(System.Exception));
            il.Emit(OpCodes.Stloc, index);
            il.Emit(OpCodes.Ldarg_2);
            il.Emit(OpCodes.Callvirt, DynamicHelpers.GetPropertyGetMethod(typeof(Context), "ThrowExceptions"));
            il.Emit(OpCodes.Stloc, index + 1);
            il.Emit(OpCodes.Ldloc, index + 1);
            il.Emit(OpCodes.Brfalse, lableThrow);

            il.Emit(OpCodes.Ldloc, index);
            il.Emit(OpCodes.Throw);

            il.MarkLabel(lableThrow);
            il.Emit(OpCodes.Ldarg_2);
            il.Emit(OpCodes.Ldloc, index);
            il.Emit(OpCodes.Callvirt, DynamicHelpers.GetMethod(typeof(TemplateContext), "AddError", new Type[] { typeof(System.Exception) }));
            //il.Emit(OpCodes.Leave_S, labelPass);
            il.EndExceptionBlock();

            il.MarkLabel(labelPass);
            il.Emit(OpCodes.Ret);

            context.Generator.Emit(OpCodes.Ldarg_0);
            context.Generator.Emit(OpCodes.Ldarg_1);
            context.Generator.Emit(OpCodes.Ldarg_2);
            context.Generator.Emit(OpCodes.Call, mb.GetBaseDefinition());
        }

        private Action<ITag, CompileContext> GeneralDefaultRender()
        {
            return (tag, scope) =>
            {
                var tagType = tag.GetType();
                if (tagType is ITypeTag value)
                {
                    scope.Generator.Emit(OpCodes.Ldarg_1);
                    if (value.Value == null)
                    {
                        scope.Generator.Emit(OpCodes.Ldstr, string.Empty);
                    }
                    else
                    {
                        scope.Generator.Emit(OpCodes.Ldstr, value.Value.ToString());
                    }
                    scope.Generator.Emit(OpCodes.Callvirt, typeof(TextWriter).GetMethod("Write", new Type[] { typeof(string) }));
                }
                else
                {
                    DefineRender(tag, scope, tagType);
                }
            };
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
        private string GetTagKey(ITag tag)
        {
            //if (tag is VariableTag variable)
            //{
            //    if (variable.Parent == null)
            //    {
            //        return $"var_{variable.Name}";
            //    }
            //}
            if (tag is StringTag str)
            {
                return $"str_{str.Value?.GetHashCode() ?? 0}";
            }
            if (tag is NumberTag num)
            {
                return $"num_{num.Value}";
            }
            if (tag is BooleanTag b)
            {
                return $"bool_{b.Value}";
            }
            if (tag is OperatorTag opt)
            {
                return $"opt_{opt.Value.ToString()}";
            }
            if (tag is NullTag)
            {
                return $"null";
            }
            return null;
        }


        /// <summary>
        /// 获取编译方法
        /// </summary>
        /// <param name="name">方法名</param>
        /// <param name="tag">标签</param>
        /// <param name="context">编译上下文</param>
        /// <returns></returns>
        public MethodInfo GetCompileMethod(string name, ITag tag, CompileContext context)
        {
            string tagKey = GetTagKey(tag);
            if (tagKey != null
                && context.Methods.TryGetValue(tagKey, out MethodInfo mi))
            {
                return mi;
            }
            if (returnDict.TryGetValue(name, out var func))
            {
                var m = func(tag, context);
                if (m != null)
                {
                    if (tagKey != null)
                    {
                        context.Methods[tagKey] = m;
                    }
                    return m;
                }
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
        /// 设置Render
        /// </summary>
        /// <param name="name">tag name</param>
        /// <param name="action">action</param>
        public void SetRenderFunc(string name, Action<ITag, CompileContext> action)
        {
            renderDict[name] = action;
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
    }
}