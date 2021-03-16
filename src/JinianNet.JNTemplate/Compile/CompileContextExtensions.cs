/********************************************************************************
 Copyright (c) jiniannet (http://www.jiniannet.com). All rights reserved.
 Licensed under the MIT license. See licence.txt file in the project root for full license information.
 ********************************************************************************/
using System;
using System.IO;
using System.Reflection;
using System.Reflection.Emit;

namespace JinianNet.JNTemplate.Compile
{
    /// <summary>
    /// 
    /// </summary>
    public static class CompileContextExtensions
    {
        /// <summary>
        /// 创建VOID的呈现方法
        /// </summary>
        /// <typeparam name="T">ITag</typeparam>
        /// <param name="ctx">CompileContext</param>
        /// <returns>MethodBuilder</returns>
        public static MethodBuilder CreateRenderMethod<T>(this CompileContext ctx)
        {
            return CreateRenderMethod(ctx, typeof(T).Name);
        }
        /// <summary>
        /// 创建VOID的呈现方法
        /// </summary>
        /// <param name="name">name</param>
        /// <param name="ctx">CompileContext</param>
        /// <returns>MethodBuilder</returns>
        public static MethodBuilder CreateRenderMethod(this CompileContext ctx, string name)
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
        public static MethodBuilder CreateReutrnMethod<T>(this CompileContext ctx, Type returnType)
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
        private static MethodBuilder CreateReutrnMethod(TypeBuilder builder, string name, Type returnType)
        {
            if (returnType == null)
            {
                returnType = typeof(object);
            }
            return builder.DefineMethod(name, MethodAttributes.Public | MethodAttributes.HideBySig, CallingConventions.Standard | CallingConventions.HasThis, returnType, new Type[] { typeof(TemplateContext) });
        }
    }
}
