/********************************************************************************
 Copyright (c) jiniannet (http://www.jiniannet.com). All rights reserved.
 Licensed under the MIT license. See licence.txt file in the project root for full license information.
 ********************************************************************************/
using JinianNet.JNTemplate.Dynamic;
using JinianNet.JNTemplate.Exceptions;
using JinianNet.JNTemplate.Nodes;
using JinianNet.JNTemplate.Runtime;
using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using System.Reflection.Emit;

namespace JinianNet.JNTemplate.CodeCompilation
{
    /// <summary>
    /// TemplateCompiler
    /// </summary>
    public class TemplateCompiler
    {

        /// <summary>
        /// Generates default values based on the specified type
        /// </summary>
        /// <typeparam name="T">the Type of the object.</typeparam>
        /// <returns>The efault values.</returns>
        public static T GenerateDefaultValue<T>()
        {
            return default(T);
        }

        
        /// <summary>
        /// Generate Context
        /// </summary>
        /// <param name="name">template name</param>
        /// <param name="options">The </param>
        /// <returns></returns>
        private static CompileContext GenerateContext(string name, RuntimeOptions options)
        {
            return GenerateContext(name, options, null);
        }

        /// <summary>
        /// Create a compilation context.
        /// </summary>
        /// <param name="name">Unique key of the template</param>
        /// <param name="scope">The template data.</param>
        /// <param name="options">The options of the engine.</param>
        /// <returns></returns>
        private static CompileContext GenerateContext(string name, RuntimeOptions options, IVariableScope scope)
        {
            var ctx = new CompileContext();
            ctx.Name = name;
            ctx.Options = options;
            ctx.Charset = options.Encoding;
            ctx.ThrowExceptions = options.ThrowExceptions;
            ctx.OutMode = options.OutMode;
            ctx.Data = scope ?? ctx.CreateVariableScope();
            return ctx;

        }

        /// <summary>
        /// Compile the array into a dynamic class.
        /// </summary>
        /// <param name="tags">The array of the tag.</param>
        /// <param name="ctx">The <see cref="CompileContext"/>.</param>
        /// <returns></returns>
        private static ICompilerResult Compile(ITag[] tags, CompileContext ctx)
        {
            var baseType = typeof(CompilerResult);
            TypeBuilder typeBuilder = ObjectBuilder.DefineType(baseType.GetInterface(nameof(ICompilerResult)), baseType, $"{baseType.Namespace}.Template{ToHashCode(ctx.Name)}");
            var targetMethod = baseType.GetMethodInfo("Render", new Type[] { typeof(TextWriter), typeof(TemplateContext) });
            MethodBuilder method = typeBuilder.DefineMethod(targetMethod.Name, MethodAttributes.Public | MethodAttributes.Virtual | MethodAttributes.HideBySig, targetMethod.ReturnType, new Type[] { typeof(TextWriter), typeof(TemplateContext) });
            ILGenerator methodGenerator = method.GetILGenerator();
            ctx.TypeBuilder = typeBuilder;
            var il = ctx.Generator = methodGenerator;
            Label labelPass = il.DefineLabel();

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
                TagCompiler.CompileToRender(tags[i], ctx);
            }

            il.Emit(OpCodes.Nop);
            il.Emit(OpCodes.Ret);


            Type type =
#if NETSTANDARD2_0
            ctx.TypeBuilder.AsType();
#else
            ctx.TypeBuilder.CreateType();
#endif
            ctx.Dispose();

            if (type == null)
            {
                return null;
            }
            var instance = type.CreateInstance<ICompilerResult>();
            return instance;

        }

        /// <summary>
        /// Compile the text into a dynamic class.
        /// </summary>
        /// <param name="path"></param>
        /// <param name="name">Unique key of the template</param>
        /// <param name="action">The parameter setting method.</param>
        /// <param name="options">The options of the engine.</param>
        /// <returns></returns>
        public static ICompilerResult CompileFile(string name, string path, RuntimeOptions options, Action<CompileContext> action = null)
        {
            var ctx = GenerateContext(name, options);
            if (string.IsNullOrEmpty(path))
            {
                throw new ArgumentNullException(nameof(path));
            }
            if (action != null)
            {
                action(ctx);
            }
            var res = options.Loader.Load(ctx, path);
            if (res == null)
            {
                throw new TemplateException($"Path:\"{path}\", the file could not be found.");
            }
            if (string.IsNullOrEmpty(ctx.Name))
            {
                ctx.Name = res.FullPath;
            }
            return Compile(res.Content, ctx);
        }

        /// <summary>
        /// Compile the text into a dynamic class.
        /// </summary>
        /// <param name="content">the context of the text</param>
        /// <param name="name">Unique key of the template</param>
        /// <param name="action">The parameter setting method.</param>
        /// <param name="options">The options of the engine.</param>
        /// <returns></returns>
        public static ICompilerResult Compile(string name, string content, RuntimeOptions options, Action<CompileContext> action = null)
        {
            if (string.IsNullOrEmpty(name))
            {
                name = content.GetHashCode().ToString();
            }
            var ctx = GenerateContext(name, options);
            if (action != null)
            {
                action(ctx);
            }
            return Compile(content, ctx);
        }

        /// <summary>
        /// Compile the text into a dynamic class.
        /// </summary>
        /// <param name="content">the context of the text</param>
        /// <param name="ctx">The <see cref="CompileContext"/>.</param>
        /// <returns></returns>
        public static ICompilerResult Compile(string content, CompileContext ctx)
        {
            var lexer = ctx.CreateTemplateLexer(content);
            var ts = lexer.Execute();

            var parser = ctx.CreateTemplateParser(ts);
            var tags = parser.Execute();

            return Compile(tags, ctx);
        }


        /// <summary>
        /// Returns the hash code for specified string.
        /// </summary>
        /// <param name="name">The string.</param>
        /// <returns></returns>
        public static string ToHashCode(string name)
        {
            return name?.GetHashCode().ToString() ?? "0";
        }
    }
}
