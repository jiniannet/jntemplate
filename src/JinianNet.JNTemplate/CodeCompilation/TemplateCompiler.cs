/********************************************************************************
 Copyright (c) jiniannet (http://www.jiniannet.com). All rights reserved.
 Licensed under the MIT license. See licence.txt file in the project root for full license information.
 ********************************************************************************/
using JinianNet.JNTemplate.Dynamic;
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
        /// Adds a new property to the type, with the given name, attributes, calling convention, and property signature.
        /// </summary>
        /// <param name="type">The return type of the property.</param>
        /// <param name="typeBuilder">The <see cref="TypeBuilder"/>.</param>
        /// <param name="name">The name of the property. name cannot contain embedded nulls.</param>
        public static void ImplementationProperty(Type type, TypeBuilder typeBuilder, string name)
        {
            FieldBuilder customerNameBldr = typeBuilder.DefineField($"_{name.ToLower()}", type, FieldAttributes.Private);
            PropertyBuilder propertyBuilder = typeBuilder.DefineProperty(name, PropertyAttributes.HasDefault, type, null);

            MethodAttributes getSetAttr = MethodAttributes.Public | MethodAttributes.SpecialName | MethodAttributes.HideBySig;

            MethodBuilder getBuilder = typeBuilder.DefineMethod($"get_{name}", getSetAttr, type, Type.EmptyTypes);

            ILGenerator getIl = getBuilder.GetILGenerator();

            getIl.Emit(OpCodes.Ldarg_0);
            getIl.Emit(OpCodes.Ldfld, customerNameBldr);
            getIl.Emit(OpCodes.Ret);

            MethodBuilder setBuilder = typeBuilder.DefineMethod($"set_{name}", getSetAttr, typeof(void), new Type[] { type });
            ILGenerator setIl = setBuilder.GetILGenerator();

            setIl.Emit(OpCodes.Ldarg_0);
            setIl.Emit(OpCodes.Ldarg_1);
            setIl.Emit(OpCodes.Stfld, customerNameBldr);
            setIl.Emit(OpCodes.Ret);

            propertyBuilder.SetGetMethod(getBuilder);
            propertyBuilder.SetSetMethod(setBuilder);
        }


        /// <summary>
        /// define object
        /// </summary>
        /// <param name="baseType">base type</param>
        /// <returns></returns>
        public static object DefineObjectFrom(Type baseType)
        {
            var name = baseType.FullName.GetHashCode().ToString();
            var typeBuilder = DefineType(null, null, "JinianNet.JNTemplate.DynamicCompile", $"Entity{name}");
            var ps = baseType.GetProperties();
            foreach (var p in ps)
            {
                ImplementationProperty(p.PropertyType, typeBuilder, p.Name);
            }
            var type =
#if NETSTANDARD2_0
            typeBuilder.AsType();
#else
            typeBuilder.CreateType();
#endif
            return type.CreateInstance();
        }

        /// <summary>
        /// copy property
        /// </summary>
        /// <param name="value">ori object</param>
        /// <returns></returns>
        public static object CopyObject(object value)
        {
            return CopyObject(value.GetType(), value);
        }

        /// <summary>
        /// Copies all properties of an object
        /// </summary>
        /// <param name="baseType">type</param>
        /// <param name="value">ori object</param>
        /// <returns></returns>
        public static object CopyObject(Type baseType, object value)
        {
            var result = DefineObjectFrom(baseType);
            var resultType = result.GetType();
            var ps = baseType.GetProperties();
            foreach (var p in ps)
            {
#if NET40 || NET20
                var data = p.GetValue(value,null);
                resultType.GetProperty(p.Name).SetValue(result, data,null);
#else
                var data = p.GetValue(value);
                resultType.GetProperty(p.Name).SetValue(result, data);
#endif
            }
            return result;
        }

        //private static void ImplementationRender(Type type, TypeBuilder typeBuilder)
        //{
        //    var targetMethod = DynamicHelpers.GetMethodInfo(typeof(ITemplate), "Render", new Type[] { typeof(TextWriter) });
        //    MethodBuilder method = typeBuilder.DefineMethod(targetMethod.Name, targetMethod.Attributes & (~MethodAttributes.Abstract), targetMethod.ReturnType, new Type[] { typeof(TextWriter) });
        //    ILGenerator il = method.GetILGenerator();
        //    il.Emit(OpCodes.Ldarg_0);
        //    il.Emit(OpCodes.Ldarg_1);
        //    il.Emit(OpCodes.Ldarg_0);
        //    il.Emit(OpCodes.Call, DynamicHelpers.GetPropertyInfo(typeof(ITemplate), "Context").SetMethod);
        //    il.Emit(OpCodes.Call, DynamicHelpers.GetMethodInfo(typeof(ICompileTemplate), "Render", new Type[] { typeof(TextWriter), typeof(TemplateContext) }));
        //    il.Emit(OpCodes.Ret);
        //}

        /// <summary>
        /// Generate Context
        /// </summary>
        /// <param name="name">template name</param>
        /// <param name="options">The </param>
        /// <returns></returns>
        private static CompileContext GenerateContext(string name, RuntimeOptions options)
        {
            return GenerateContext(name, options, new VariableScope(options.Data, TypeDetect.Absolute));
        }


        /// <summary>
        /// Constructs a TypeBuilder for a private type with the specified name in this module.
        /// </summary>
        /// <param name="interfaceType">The interface that this type implements.</param>
        /// <param name="parent">The type that the defined type extends.</param>
        /// <param name="assemblyName">The display name of the assembly.</param>
        /// <param name="className">The name of the dynamic module.</param>
        /// <returns></returns>
        public static TypeBuilder DefineType(Type interfaceType, Type parent, string assemblyName, string className)
        {
            AssemblyBuilder assemblyBuilder
#if NET40 || NET20
                =AppDomain.CurrentDomain.DefineDynamicAssembly(new AssemblyName(assemblyName), AssemblyBuilderAccess.Run);
#else
                = AssemblyBuilder.DefineDynamicAssembly(new AssemblyName(assemblyName), AssemblyBuilderAccess.Run);
#endif
            ModuleBuilder moduleBuilder = assemblyBuilder.DefineDynamicModule(className);
            TypeBuilder typeBuilder;
            if (parent != null)
            {
                typeBuilder = moduleBuilder.DefineType(assemblyName, TypeAttributes.Public, parent);
            }
            else
            {
                typeBuilder = moduleBuilder.DefineType(assemblyName, TypeAttributes.Public);
            }
            CustomAttributeBuilder customAttributeBuilder = new CustomAttributeBuilder(typeof(SerializableAttribute).GetConstructor(Type.EmptyTypes), new Type[] { });
            typeBuilder.SetCustomAttribute(customAttributeBuilder);
            if (interfaceType != null)
            {
                typeBuilder.AddInterfaceImplementation(interfaceType);
            }
            return typeBuilder;
        }
        /// <summary>
        /// Create a compilation context.
        /// </summary>
        /// <param name="name">Unique key of the template</param>
        /// <param name="scope">The template data.</param>
        /// <param name="options">The options of the engine.</param>
        /// <returns></returns>
        private static CompileContext GenerateContext(string name, RuntimeOptions options, VariableScope scope)
        {
            if (scope == null)
            {
                scope = new VariableScope(options.Data, options.TypeDetectPattern);
            }
            var ctx = new CompileContext();
            ctx.Data = scope;
            ctx.Name = name;
            ctx.Options = options;
            ctx.Charset = options.Encoding;
            ctx.ThrowExceptions = options.ThrowExceptions;
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
            var interfaceType = typeof(ICompilerResult);
            TypeBuilder typeBuilder = DefineType(interfaceType, null, typeof(TemplateCompiler).Namespace, $"Template{ToHashCode(ctx.Name)}");
            var targetMethod = interfaceType.GetMethodInfo("Render", new Type[] { typeof(TextWriter), typeof(TemplateContext) });
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

            var write = typeof(TextWriter).GetMethod("Write", new Type[] { typeof(string) });

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
            return name.GetHashCode().ToString();
        }
    }
}
