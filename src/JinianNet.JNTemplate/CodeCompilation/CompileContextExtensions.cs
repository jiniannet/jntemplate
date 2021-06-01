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
            return ctx.TypeGuesser.GetType(tag,ctx);
        }


        /// <summary>
        /// Compiles the specified tag into a method.
        /// </summary>
        /// <param name="tag">The <see cref="ITag"/>.</param>
        /// <param name="ctx">The <see cref="CompileContext"/>.</param>
        /// <returns></returns>
        public static MethodInfo CompileTag(this CompileContext ctx, ITag tag)
        {
            return TagCompiler.Compile(tag, ctx);
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
            il.Call(stringBuilderType, stringBuilderType.GetMethodInfo( "ToString", Type.EmptyTypes));
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
    }
}
