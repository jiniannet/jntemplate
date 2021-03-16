/********************************************************************************
 Copyright (c) jiniannet (http://www.jiniannet.com). All rights reserved.
 Licensed under the MIT license. See licence.txt file in the project root for full license information.
 ********************************************************************************/
using JinianNet.JNTemplate.Dynamic;
using JinianNet.JNTemplate.Nodes;
using System;
using System.IO;
using System.Reflection;
using System.Reflection.Emit;

namespace JinianNet.JNTemplate.Compile
{
    /// <summary>
    /// 编译
    /// </summary>
    public class Compiler
    {
        #region filed
        private static Lazy<TypeGuess> typeGuess;
        private static Lazy<CompileBuilder> builder;
        #endregion
        #region
        /// <summary>
        /// Compile builder
        /// </summary>
        public static CompileBuilder Builder
        {
            get { return builder.Value; }
        }

        /// <summary>
        /// Type guess
        /// </summary>
        public static TypeGuess TypeGuess
        {
            get { return typeGuess.Value; }
        }
        #endregion

        /// <summary>
        /// ctor
        /// </summary>
        static Compiler()
        {
            builder = new Lazy<CompileBuilder>(() =>
            {
                return new CompileBuilder();
            }, true);
            typeGuess = new Lazy<TypeGuess>(() =>
            {
                return new TypeGuess();
            }, true);
        }

        /// <summary>
        /// define property
        /// </summary>
        /// <param name="type">base type</param>
        /// <param name="typeBuilder">type builder</param>
        /// <param name="name">property name</param>
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
            var builder = DefineType(null, null, "JinianNet.JNTemplate.DynamicCompile", $"Entity{name}");
            var ps = baseType.GetProperties();
            foreach (var p in ps)
            {
                ImplementationProperty(p.PropertyType, builder, p.Name);
            }
            var type =
#if NETSTANDARD2_0
            builder.AsType();
#else
            builder.CreateType();
#endif
            return DynamicHelpers.CreateInstance(type);
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
        /// copy property
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
        //    var targetMethod = DynamicHelpers.GetMethod(typeof(ITemplate), "Render", new Type[] { typeof(TextWriter) });
        //    MethodBuilder method = typeBuilder.DefineMethod(targetMethod.Name, targetMethod.Attributes & (~MethodAttributes.Abstract), targetMethod.ReturnType, new Type[] { typeof(TextWriter) });
        //    ILGenerator il = method.GetILGenerator();
        //    il.Emit(OpCodes.Ldarg_0);
        //    il.Emit(OpCodes.Ldarg_1);
        //    il.Emit(OpCodes.Ldarg_0);
        //    il.Emit(OpCodes.Call, DynamicHelpers.GetPropertyInfo(typeof(ITemplate), "Context").SetMethod);
        //    il.Emit(OpCodes.Call, DynamicHelpers.GetMethod(typeof(ICompileTemplate), "Render", new Type[] { typeof(TextWriter), typeof(TemplateContext) }));
        //    il.Emit(OpCodes.Ret);
        //}

        /// <summary>
        /// Generate Context
        /// </summary>
        /// <param name="name">template name</param>
        /// <returns></returns>
        private static CompileContext GenerateContext(string name)
        {
            return GenerateContext(name, new VariableScope());
        }


        /// <summary>
        /// Define Type
        /// </summary>
        /// <param name="interfaceType">interface</param>
        /// <param name="parent">parent</param>
        /// <param name="assemblyName">assembly Name</param>
        /// <param name="className">class Name</param>
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
        /// Generate Context
        /// </summary>
        /// <param name="name">template name</param>
        /// <param name="scope">data</param>
        /// <returns></returns>
        private static CompileContext GenerateContext(string name, VariableScope scope)
        {
            if (scope == null)
            {
                scope = new VariableScope();
            }
            var ctx = new CompileContext();
            ctx.Data = scope;
            ctx.Name = name;
            return ctx;

        }

        /// <summary>
        /// Compile tag
        /// </summary>
        /// <param name="tags">tag</param>
        /// <param name="ctx">Context</param>
        /// <returns></returns>
        private static ICompileTemplate Compile(ITag[] tags, CompileContext ctx)
        {
            var interfaceType = typeof(ICompileTemplate);
            TypeBuilder typeBuilder = DefineType(interfaceType, null, typeof(Compiler).Namespace, $"Template{ToHashCode(ctx.Name)}");
            var targetMethod = DynamicHelpers.GetMethod(interfaceType, "Render", new Type[] { typeof(TextWriter), typeof(TemplateContext) });
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
                if (tags[i] is TextTag text)
                {
                    if (!string.IsNullOrEmpty(text.Text))
                    {
                        il.Emit(OpCodes.Ldarg_1);
                        if (ctx.StripWhiteSpace)
                        {
                            il.Emit(OpCodes.Ldstr, text.Text.Trim());
                        }
                        else
                        {
                            il.Emit(OpCodes.Ldstr, text.Text);
                        }
                        il.Emit(OpCodes.Callvirt, write);
                    }
                }
                else if (tags[i] is ITypeTag value)
                {
                    if (value != null)
                    {
                        il.Emit(OpCodes.Ldarg_1);
                        il.Emit(OpCodes.Ldstr, value.Value.ToString());
                        il.Emit(OpCodes.Callvirt, write);
                    }
                }
                else
                {
                    Builder.Build(tags[i])?.Invoke(tags[i], ctx);
                }
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
            var instance = DynamicHelpers.CreateInstance<ICompileTemplate>(type);
            return instance;

        }

        /// <summary>
        /// Compile content
        /// </summary>
        /// <param name="content">content</param>
        /// <param name="name">template name</param>
        /// <param name="action"></param>
        /// <returns></returns>
        public static ICompileTemplate Compile(string name, string content, Action<CompileContext> action = null)
        {
            if (string.IsNullOrEmpty(name))
            {
                name = content.GetHashCode().ToString();
            }
            var ctx = GenerateContext(name);
            if (action != null)
            {
                action(ctx);
            }
            return Compile(content, ctx);
        }

        /// <summary>
        /// Compile content
        /// </summary>
        /// <param name="content">content</param>
        /// <param name="ctx">Context</param>
        /// <returns></returns>
        public static ICompileTemplate Compile(string content, CompileContext ctx)
        {
            var lexer = new TemplateLexer(content);
            var ts = lexer.Execute();

            var parser = new TemplateParser(ts);
            var tags = parser.Execute();

            return Compile(tags, ctx);
        }


        /// <summary>
        /// 获取HASHCODE
        /// </summary>
        /// <param name="name">名称</param>
        /// <returns></returns>
        public static string ToHashCode(string name)
        {
            return name.GetHashCode().ToString();
        }

        /// <summary>
        /// Register tag
        /// </summary>
        /// <typeparam name="T">ITag</typeparam> 
        /// <param name="compile"></param>
        /// <param name="guess"></param>
        public static void Register<T>(Func<Nodes.ITag, CompileContext, System.Reflection.MethodInfo> compile,
            Func<Nodes.ITag, CompileContext, Type> guess) where T : Nodes.ITag
        {
            Builder.Register<T>(compile);
            TypeGuess.Register<T>(guess);
        }

    }
}