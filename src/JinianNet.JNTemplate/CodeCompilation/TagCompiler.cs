/********************************************************************************
 Copyright (c) jiniannet (http://www.jiniannet.com). All rights reserved.
 Licensed under the MIT license. See licence.txt file in the project root for full license information.
 ********************************************************************************/
using JinianNet.JNTemplate.Dynamic;
using JinianNet.JNTemplate.Nodes;
using JinianNet.JNTemplate.Exceptions;
using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using System.Reflection.Emit;

namespace JinianNet.JNTemplate.CodeCompilation
{
    /// <summary>
    /// 
    /// </summary>
    public class TagCompiler
    {
        /// <summary>
        /// Compiles the specified tag into a method.
        /// </summary>
        /// <param name="tag">The <see cref="ITag"/>.</param>
        /// <param name="ctx">The <see cref="CompileContext"/>.</param>
        /// <returns></returns>
        public static MethodInfo Compile(ITag tag, CompileContext ctx)
        {
            return Compile(tag.GetType().Name, tag, ctx);
        }
        private static string GetTagKey(ITag tag)
        {
            if (tag is StringTag str)
            {
                return $"s{str.Value?.GetHashCode() ?? 0}";
            }
            if (tag is NumberTag num)
            {
                return $"n{num.Value}";
            }
            if (tag is BooleanTag b)
            {
                return $"b{b.Value}";
            }
            if (tag is OperatorTag opt)
            {
                return $"o{opt.Value.ToString()}";
            }
            if (tag is NullTag)
            {
                return $"null";
            }
            return null;
        }

        /// <summary>
        /// Compiles the specified tag into a method.
        /// </summary>
        /// <param name="name">The name of the tag.</param>
        /// <param name="tag">The <see cref="ITag"/>.</param>
        /// <param name="context">The <see cref="CompileContext"/>.</param>
        /// <returns></returns>
        public static MethodInfo Compile(string name, ITag tag, CompileContext context)
        {
            string tagKey = GetTagKey(tag);
            if (tagKey != null
                && context.Methods.TryGetValue(tagKey, out MethodInfo mi))
            {
                return mi;
            }
            var func = context.CompileBuilder.Build(name);
            return func(tag, context);
        }


        private static void CompileToRender(ITag tag, CompileContext context, Type tagType, MethodInfo method)
        {
            var type = method.ReturnType;
            var isAsync = false;
            var taskType = typeof(System.Threading.Tasks.Task);
            MethodInfo taskAwaiterMethod = null;
            MethodInfo resultMethod = null;

            var mb = context.CreateRenderMethod(tagType.Name);
            var il = mb.GetILGenerator();
            var exBlock = il.BeginExceptionBlock();
            //var lableThrow = il.DefineLabel();
            var labelPass = il.DefineLabel();

            if (type.IsMatchType(taskType))
            {
                isAsync = true;
                taskAwaiterMethod = method.ReturnType.GetMethodInfo("GetAwaiter", Type.EmptyTypes);
                resultMethod = taskAwaiterMethod.ReturnType.GetMethodInfo("GetResult", Type.EmptyTypes);
                type = resultMethod.ReturnType;
            }
            if (type.FullName != "System.Void")
            {
                il.DeclareLocal(type);
                il.Emit(OpCodes.Ldarg_0);
                il.Emit(OpCodes.Ldarg_2);
                il.Emit(OpCodes.Call, method);

                if (isAsync)
                {
                    var returnVar = il.DeclareLocal(taskAwaiterMethod.ReturnType);
                    il.Emit(OpCodes.Callvirt, taskAwaiterMethod);
                    il.Emit(OpCodes.Stloc, returnVar.LocalIndex);
                    il.Emit(OpCodes.Ldloca, returnVar.LocalIndex);
                    il.Emit(OpCodes.Call, resultMethod);
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
                }
                else
                {
                    il.Emit(OpCodes.Ldarg_0);
                    il.Emit(OpCodes.Ldarg_2);
                    il.Emit(OpCodes.Call, method);
                }
            }
            var exceptionVar = il.DeclareLocal(typeof(System.Exception));
            //il.DeclareLocal(typeof(bool));

            il.BeginCatchBlock(typeof(System.Exception));
            il.Emit(OpCodes.Stloc, exceptionVar.LocalIndex);
            //il.Emit(OpCodes.Ldarg_2);
            //il.Emit(OpCodes.Callvirt, typeof(Context).GetPropertyGetMethod("ThrowExceptions"));
            //il.Emit(OpCodes.Stloc, index + 1);
            //il.Emit(OpCodes.Ldloc, index + 1);
            //il.Emit(OpCodes.Brfalse, lableThrow);

            //il.Emit(OpCodes.Ldloc, index);
            //il.Emit(OpCodes.Throw);

            if (context.Debug)
            {
                //{tag.ToSource()} 
                il.Emit(OpCodes.Ldstr, $"{{0}} on [line:{tag.FirstToken?.BeginLine},col:{tag.FirstToken?.BeginColumn}]");
            }
            else
            {
                il.Emit(OpCodes.Ldstr, $"{{0}} [line:{tag.FirstToken?.BeginLine},col:{tag.FirstToken?.BeginColumn}]");
            }
            il.Emit(OpCodes.Ldloc, exceptionVar.LocalIndex);
            il.Emit(OpCodes.Callvirt, typeof(System.Exception).GetPropertyGetMethod("Message"));
            il.Emit(OpCodes.Call, typeof(string).GetMethodInfo("Format",new Type[] { typeof(string),typeof(object) }));
            var msgVar = il.DeclareLocal(typeof(string));
            il.Emit(OpCodes.Stloc, msgVar.LocalIndex);
            il.Emit(OpCodes.Ldloc, msgVar.LocalIndex);
            il.Emit(OpCodes.Ldloc, exceptionVar.LocalIndex);
            var exceptionType = typeof(RuntimeException);
            il.Emit(OpCodes.Newobj, exceptionType.GetConstructor(new Type[] { typeof(string), typeof(System.Exception) }));

            var newExVar = il.DeclareLocal(exceptionType);
            il.Emit(OpCodes.Stloc, newExVar.LocalIndex);
            if (tag.FirstToken != null)
            {
                il.Emit(OpCodes.Ldloc, newExVar.LocalIndex);
                il.Emit(OpCodes.Ldc_I4, tag.FirstToken.BeginLine);
                il.Emit(OpCodes.Callvirt, exceptionType.GetPropertySetMethod("Line"));
                il.Emit(OpCodes.Ldloc, newExVar.LocalIndex);
                il.Emit(OpCodes.Ldc_I4, tag.FirstToken.BeginColumn);
                il.Emit(OpCodes.Callvirt, exceptionType.GetPropertySetMethod("Column"));
            }

            //call string[System.Runtime] System.String::Format(string, object)
            //callvirt instance string [System.Runtime]System.Exception::get_Message()
            //il.MarkLabel(lableThrow);
            il.Emit(OpCodes.Ldarg_2);
            il.Emit(OpCodes.Ldloc, newExVar.LocalIndex);
            il.Emit(OpCodes.Callvirt, typeof(TemplateContext).GetMethodInfo("AddError", new Type[] { typeof(System.Exception) }));
            //il.Emit(OpCodes.Leave_S, labelPass);
            il.EndExceptionBlock();

            il.MarkLabel(labelPass);
            il.Emit(OpCodes.Ret);

            context.Generator.Emit(OpCodes.Ldarg_0);
            context.Generator.Emit(OpCodes.Ldarg_1);
            context.Generator.Emit(OpCodes.Ldarg_2);
            context.Generator.Emit(OpCodes.Call, mb.GetBaseDefinition());
        }


        private static void CompileToRender(ITag tag, CompileContext context, Type tagType)
        {
            MethodInfo method = null;
            try
            {
                if (tag is ReferenceTag)
                {
                    method = Compile((tag as ReferenceTag).Child, context);
                }
                else
                {
                    method = Compile(tag, context);
                }
                if (method == null)
                {
                    throw new CompileException(tag,$"cannot compile the tag! type:{tag?.GetType().FullName}");
                }
            }
            catch (System.Exception exception)
            {
                if (context.ThrowExceptions)
                {
                    throw;
                }
                context.Generator.Emit(OpCodes.Ldarg_2);
                context.Generator.Emit(OpCodes.Ldstr, exception.ToString());
                context.Generator.Emit(OpCodes.Newobj, typeof(CompileException).GetConstructor(new Type[] { typeof(string) }));
                //context.Generator.Emit(OpCodes.Throw);
                context.Generator.Emit(OpCodes.Callvirt, typeof(TemplateContext).GetMethod("AddError", new Type[] { typeof(System.Exception) }));
                return;
            }

            CompileToRender(tag, context, tagType, method);
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="tag"></param>
        /// <param name="context"></param>
        public static void CompileToRender(ITag tag, CompileContext context)
        {
            var tagType = tag.GetType();
            if (tag is EndTag _
            || tag is CommentTag _)
            {
                return;
            }

            if (tag is TextTag textTag)
            {
                var text = textTag.ToString(context.OutMode); 
                if (!string.IsNullOrEmpty(text))
                {
                    context.Generator.Emit(OpCodes.Ldarg_1);
                    context.Generator.Emit(OpCodes.Ldstr, text);
                    context.Generator.Emit(OpCodes.Callvirt, typeof(TextWriter).GetMethod("Write", new Type[] { typeof(string) }));
                }
                return;
            }

            if (tag is ITypeTag value)
            {
                context.Generator.Emit(OpCodes.Ldarg_1);
                if (value.Value == null)
                {
                    context.Generator.Emit(OpCodes.Ldstr, string.Empty);
                }
                else
                {
                    context.Generator.Emit(OpCodes.Ldstr, value.Value.ToString());
                }
                context.Generator.Emit(OpCodes.Callvirt, typeof(TextWriter).GetMethod("Write", new Type[] { typeof(string) }));
                return;
            }

            CompileToRender(tag, context, tagType);
        }
    }
}
