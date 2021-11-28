/********************************************************************************
 Copyright (c) jiniannet (http://www.jiniannet.com). All rights reserved.
 Licensed under the MIT license. See licence.txt file in the project root for full license information.
 ********************************************************************************/
using JinianNet.JNTemplate.Dynamic;
using JinianNet.JNTemplate.Exceptions;
using JinianNet.JNTemplate.Nodes;
using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using System.Reflection.Emit;
using System.Text;

namespace JinianNet.JNTemplate.CodeCompilation
{
    /// <summary>
    /// 
    /// </summary>
    public static class CompileContextExtensions
    {
        /// <summary>
        /// Adds a new method to the type, with the specified name.
        /// </summary>
        /// <typeparam name="T">The type of the tag.</typeparam>
        /// <param name="ctx">The <see cref="CompileContext"/>.</param>
        /// <returns></returns>
        public static MethodBuilder CreateRenderMethod<T>(this CompileContext ctx)
        {
            return CreateRenderMethod(ctx, typeof(T).Name);
        }
        /// <summary>
        /// Adds a new render method to the type, with the specified name.
        /// </summary>
        /// <param name="name">The name of the method.</param>
        /// <param name="ctx">The <see cref="CompileContext"/>.</param>
        /// <returns>The <see cref="MethodBuilder"/>.</returns>
        public static MethodBuilder CreateRenderMethod(this CompileContext ctx, string name)
        {
            return ctx.TypeBuilder.DefineMethod($"Render{name}{ctx.Seed}", MethodAttributes.Public | MethodAttributes.HideBySig, CallingConventions.Standard | CallingConventions.HasThis, typeof(void), new Type[] { typeof(TextWriter), typeof(TemplateContext) });
        }
        /// <summary>
        /// Adds a new method with has return to the type, with the specified name.
        /// </summary>
        /// <typeparam name="T">The type of the tag.</typeparam>
        /// <param name="ctx">The <see cref="CompileContext"/>.</param>
        /// <param name="returnType">The return type</param>
        /// <returns></returns>
        public static MethodBuilder CreateReutrnMethod<T>(this CompileContext ctx, Type returnType)
        {
            if (returnType.FullName == "System.Void")
            {
                return CreateReutrnMethod(ctx.TypeBuilder, $"Execute{typeof(T).Name}{ctx.Seed}", returnType);
            }
            return CreateReutrnMethod(ctx.TypeBuilder, $"Get{typeof(T).Name}{ctx.Seed}", returnType);
        }

        /// <summary>
        /// Adds a new method to the type, with the specified name.
        /// </summary> 
        /// <param name="builder">The <see cref="TypeBuilder"/>.</param>
        /// <param name="name">The name of the method.</param>
        /// <param name="returnType">The return type</param>
        /// <returns></returns>
        private static MethodBuilder CreateReutrnMethod(TypeBuilder builder, string name, Type returnType)
        {
            if (returnType == null)
            {
                returnType = typeof(object);
            }
            return builder.DefineMethod(name, MethodAttributes.Public | MethodAttributes.HideBySig, CallingConventions.Standard | CallingConventions.HasThis, returnType, new Type[] { typeof(TemplateContext) });
        }

        /// <summary>
        /// Gets the <see cref="Type"/> with the specified tag.
        /// </summary>
        /// <param name="tag">The tag of the type to get.</param>
        /// <param name="ctx">The <see cref="CompileContext"/>.</param>
        /// <returns></returns>
        public static Type GuessType(this CompileContext ctx, ITag tag)
        {
            return ctx.TypeGuesser.GetType(tag, ctx);
        }


        /// <summary>
        /// Compiles the specified tag into a method.
        /// </summary>
        /// <param name="tag">The <see cref="ITag"/>.</param>
        /// <param name="ctx">The <see cref="CompileContext"/>.</param>
        /// <returns></returns>
        public static MethodInfo CompileTag(this CompileContext ctx, ITag tag)
        {
            return Compile(ctx, tag);
        }

        #region internal
        /// <summary>
        /// 
        /// </summary>
        /// <param name="il"></param>
        /// <param name="ctx"></param>
        /// <param name="tags"></param>
        /// <returns></returns>
        internal static void BlockCompile(this CompileContext ctx, ILGenerator il, ITag[] tags)
        {
            var stringBuilderType = typeof(StringBuilder);
            il.DeclareLocal(stringBuilderType);
            il.DeclareLocal(typeof(string));
            il.Emit(OpCodes.Newobj, stringBuilderType.GetConstructor(Type.EmptyTypes));
            il.Emit(OpCodes.Stloc_0);
            for (var i = 0; i < tags.Length; i++)
            {
                il.CallTag(ctx, tags[i], (nil, hasReturn, needCall) =>
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
                    nil.StringAppend(ctx, returnType);
                    nil.Emit(OpCodes.Pop);
                });
            }
            il.Emit(OpCodes.Ldloc_0);
            il.Call(stringBuilderType, stringBuilderType.GetMethodInfo("ToString", Type.EmptyTypes));
            il.Emit(OpCodes.Stloc_1);
            il.Emit(OpCodes.Ldloc_1);
        }



        /// <summary>
        /// 
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="tag"></param>
        /// <param name="ctx"></param>
        /// <returns></returns>
        internal static MethodInfo IfCompile<T>(this CompileContext ctx, T tag) where T : ElseifTag
        {
            var stringBuilderType = typeof(StringBuilder);
            var t = tag;
            var type = ctx.GuessType(t);
            var mb = ctx.CreateReutrnMethod<T>(type);
            var il = mb.GetILGenerator();
            var labelEnd = il.DefineLabel();
            il.DeclareLocal(type);
            il.DeclareLocal(stringBuilderType);
            il.Emit(OpCodes.Newobj, stringBuilderType.GetConstructor(Type.EmptyTypes));
            il.Emit(OpCodes.Stloc_1);
            for (var i = 0; i < t.Children.Count; i++)
            {
                il.CallTag(ctx, t.Children[i], (nil, hasReturn, needCall) =>
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
                    nil.StringAppend(ctx, returnType);
                    nil.Emit(OpCodes.Pop);
                });
            }
            il.Emit(OpCodes.Ldloc_1);
            il.Call(stringBuilderType, stringBuilderType.GetMethodInfo("ToString", Type.EmptyTypes));
            il.Emit(OpCodes.Stloc_0);

            il.MarkLabel(labelEnd);
            il.Emit(OpCodes.Ldloc_0);
            il.Emit(OpCodes.Ret);
            return mb.GetBaseDefinition();
        }




        /// <summary>
        /// 
        /// </summary>
        /// <param name="ctx"></param>
        /// <param name="tag"></param>
        /// <returns></returns>
        internal static Type GuessIfType(this CompileContext ctx, ElseifTag tag)
        {
            var types = new List<Type>();
            var hasVoid = false;
            for (var i = 0; i < tag.Children.Count; i++)
            {
                var type = ctx.GuessType(tag.Children[i]);
                if (type.FullName == "System.Void")
                {
                    hasVoid = true;
                }
                else
                {
                    types.Add(type);
                }
            }
            if (types.Count == 1)
            {
                return types[0];
            }
            if (types.Count == 0 && hasVoid)
            {
                return typeof(void);
            }
            return typeof(string);
        }
        #endregion




        /// <summary>
        /// Compile the text into a dynamic class.
        /// </summary>
        /// <param name="content">the context of the text</param>
        /// <param name="ctx">The <see cref="CompileContext"/>.</param>
        /// <returns></returns>
        public static ICompilerResult Compile(this CompileContext ctx, string content)
        {
            var tags = TemplateContextExtensions.Lexer(ctx,content);

            return Compile(ctx, tags);
        }

        /// <summary>
        /// Compile the array into a dynamic class.
        /// </summary>
        /// <param name="tags">The array of the tag.</param>
        /// <param name="ctx">The <see cref="CompileContext"/>.</param>
        /// <returns></returns>
        private static ICompilerResult Compile(this CompileContext ctx, ITag[] tags)
        {
            var baseType = typeof(CompilerResult);
            var typeBuilder = ObjectBuilder.DefineType(baseType.GetInterface(nameof(ICompilerResult)), baseType, $"{baseType.Namespace}.Template{ctx.Name.GetHashCode()}");
            var targetMethod = baseType.GetMethodInfo("Render", new Type[] { typeof(TextWriter), typeof(TemplateContext) });
            var method = typeBuilder.DefineMethod(targetMethod.Name, MethodAttributes.Public | MethodAttributes.Virtual | MethodAttributes.HideBySig, targetMethod.ReturnType, new Type[] { typeof(TextWriter), typeof(TemplateContext) });
            var methodGenerator = method.GetILGenerator();
            ctx.TypeBuilder = typeBuilder;
            var il = ctx.Generator = methodGenerator;
            var labelPass = il.DefineLabel();

            il.Emit(OpCodes.Ldarg_1);
            il.Emit(OpCodes.Ldnull);
            il.Emit(OpCodes.Ceq);
            il.Emit(OpCodes.Brfalse_S, labelPass);
            il.Emit(OpCodes.Ldstr, "writer cannot be null!");
            il.Emit(OpCodes.Newobj, typeof(System.ArgumentNullException).GetConstructor(new Type[] { typeof(string) }));
            il.Emit(OpCodes.Throw);

            il.MarkLabel(labelPass);

            for (var i = 0; i < tags.Length; i++)
            {
                CompileToRender(ctx, tags[i]);
            }

            il.Emit(OpCodes.Nop);
            il.Emit(OpCodes.Ret);


            Type type =
#if NETSTANDARD2_0
            ctx.TypeBuilder.AsType();
#else
            ctx.TypeBuilder.CreateType();
#endif
            if (type == null)
            {
                return null;
            }
            var instance = type.CreateInstance<ICompilerResult>();
            return instance;

        }

        /// <summary>
        /// Compiles the specified tag into a method.
        /// </summary>
        /// <param name="name">The name of the tag.</param>
        /// <param name="tag">The <see cref="ITag"/>.</param>
        /// <param name="context">The <see cref="CompileContext"/>.</param>
        /// <returns></returns>
        public static MethodInfo Compile(this CompileContext context, string name, ITag tag)
        {
            string tagKey = GetTagKey(tag);
            if (tagKey != null
                && context.Methods.TryGetValue(tagKey, out MethodInfo mi))
            {
                return mi;
            }
            var func = context.CompileBuilder.Build(name);
            var method = func(tag, context);
            if (tagKey != null)
            {
                context.Methods[tagKey] = method;
            }
            return method;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="tag"></param>
        /// <param name="context"></param>
        public static void CompileToRender(this CompileContext context, ITag tag)
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
                if (value.Value != null)
                {
                    context.Generator.Emit(OpCodes.Ldarg_1);
                    context.Generator.Emit(OpCodes.Ldstr, value.Value.ToString());
                    context.Generator.Emit(OpCodes.Callvirt, typeof(TextWriter).GetMethod("Write", new Type[] { typeof(string) }));
                }
                return;
            }

            CompileToRender(context, tag, tagType);
        }

        /// <summary>
        /// Compiles the specified tag into a method.
        /// </summary>
        /// <param name="tag">The <see cref="ITag"/>.</param>
        /// <param name="ctx">The <see cref="CompileContext"/>.</param>
        /// <returns></returns>
        public static MethodInfo Compile(this CompileContext ctx, ITag tag)
        {
            return Compile(ctx, tag.GetType().Name, tag);
        }

        private static void CompileToRender(this CompileContext context, ITag tag, Type tagType)
        {
            if (tag == null)
            {
                return;
            }
            MethodInfo method = null;
            try
            {
                if (tag is ReferenceTag)
                {
                    method = Compile(context, (tag as ReferenceTag).Child);
                }
                else
                {
                    method = Compile(context, tag);
                }
                if (method == null)
                {
                    throw new CompileException(tag, $"Cannot compile the {tag.GetType().Name} on {tag.ToSource()}[line:{tag.FirstToken?.BeginLine ?? 0},col:{tag.FirstToken?.BeginColumn ?? 0}]:");
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

            CompileToRender(context, tag, tagType, method);
        }

        private static void CompileToRender(this CompileContext context, ITag tag, Type tagType, MethodInfo method)
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

            il.Emit(OpCodes.Ldarg_0);
            il.Emit(OpCodes.Ldarg_2);
            il.Emit(OpCodes.Ldloc, exceptionVar.LocalIndex);
            il.Emit(OpCodes.Ldc_I4, tag.FirstToken?.BeginLine ?? 0);
            il.Emit(OpCodes.Ldc_I4, tag.FirstToken?.BeginColumn ?? 0);
            if (context.Debug)
            {
                il.Emit(OpCodes.Ldstr, tag.ToSource());
            }
            else
            {
                il.Emit(OpCodes.Ldnull);
            }
            il.Emit(OpCodes.Call, typeof(CompilerResult).GetMethodInfo("ThrowException", null));
            il.EndExceptionBlock();

            il.MarkLabel(labelPass);
            il.Emit(OpCodes.Ret);

            context.Generator.Emit(OpCodes.Ldarg_0);
            context.Generator.Emit(OpCodes.Ldarg_1);
            context.Generator.Emit(OpCodes.Ldarg_2);
            context.Generator.Emit(OpCodes.Call, mb.GetBaseDefinition());
        }
        private static string GetTagKey(ITag tag)
        {
            if (tag is StringTag str)
            {
                return $"{nameof(StringTag)}_{str.Value?.GetHashCode() ?? 0}";
            }
            if (tag is ITypeTag type)
            {
                return $"{tag.GetType().Name}_{type.Value}";
            }
            if (tag is NullTag)
            {
                return $"null";
            }
            return null;
        }
    }

}