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
            var type = builder.CreateType();
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
            return GenerateContext(name, new VariableScope(Runtime.Data));
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
                scope = new VariableScope(Runtime.Data);
            }
            var type = typeof(ICompileTemplate);
            TypeBuilder typeBuilder = DefineType(type, null, typeof(Compiler).Namespace, $"Template{ToHashCode(name)}");
            var targetMethod = DynamicHelpers.GetMethod(type, "Render", new Type[] { typeof(TextWriter), typeof(TemplateContext) });

            MethodBuilder method = typeBuilder.DefineMethod(targetMethod.Name, MethodAttributes.Public | MethodAttributes.Virtual | MethodAttributes.HideBySig, targetMethod.ReturnType, new Type[] { typeof(TextWriter), typeof(TemplateContext) });
            //MethodBuilder method = typeBuilder.DefineMethod(targetMethod.Name, targetMethod.Attributes & (~MethodAttributes.Abstract), targetMethod.ReturnType, new Type[] { typeof(TextWriter), typeof(TemplateContext) });
            //MethodBuilder method = typeBuilder.DefineMethod(targetMethod.Name, targetMethod.Attributes & (~MethodAttributes.Abstract) | MethodAttributes.Final, targetMethod.ReturnType, new Type[] { typeof(TextWriter), typeof(TemplateContext) });
            ILGenerator methodGenerator = method.GetILGenerator();
            var ctx = new CompileContext();
            ctx.Data = scope;
            ctx.Generator = methodGenerator;
            ctx.TypeBuilder = typeBuilder;
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
            var il = ctx.Generator;
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
                if (tags[i] is TextTag)
                {
                    var t = tags[i] as TextTag;
                    if (!string.IsNullOrEmpty(t.Text))
                    {
                        il.Emit(OpCodes.Ldarg_1);
                        if (ctx.StripWhiteSpace)
                        {
                            il.Emit(OpCodes.Ldstr, t.Text.Trim());
                        }
                        else
                        {
                            il.Emit(OpCodes.Ldstr, t.Text);
                        }
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


            Type type = ctx.TypeBuilder.CreateType();
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
        /// 编译模板文件
        /// </summary>
        /// <param name="absFileName">Absolute file path</param>
        /// <param name="ctx">Compile Context</param>
        /// <returns></returns>
        private static ICompileTemplate CompileFile(string absFileName, CompileContext ctx)
        {
            var res = Runtime.Loader.Load(absFileName, ctx.Charset, new string[0]);
            if (res != null)
            {
                return Compile(res.Content, ctx);
                //return Compile(absFileName, res.Content);
            }
            throw new Exception.CompileException($"cannot load resource:\"{absFileName}\".");
        }

        /// <summary>
        /// 获取HASHCODE
        /// </summary>
        /// <param name="name">名称</param>
        /// <returns></returns>
        private static string ToHashCode(string name)
        {
            return name.GetHashCode().ToString();
        }


        /// <summary>
        /// 生成编译模板
        /// </summary> 
        /// <param name="name">模板名 唯一</param>  
        /// <param name="fileName">模板文件</param>
        /// <param name="action">action</param>
        /// <returns></returns>
        public static ICompileTemplate CompileFile(string name, string fileName, Action<CompileContext> action = null)
        {
            CompileContext ctx;
            if (string.IsNullOrEmpty(name))
            {
                ctx = GenerateContext(fileName);
            }
            else
            {
                ctx = GenerateContext(name);
            }
            if (action != null)
            {
                action(ctx);
            }
            var full = CompileBuilder.FindPath(fileName, ctx);
            if (string.IsNullOrEmpty(full))
            {
                throw new Exception.CompileException($"\"{fileName}\" cannot be found.");
            }
            if (ctx.Name != name)
            {
                ctx.Name = full;
            }
            if (string.IsNullOrEmpty(ctx.CurrentPath))
            {
                ctx.CurrentPath = Runtime.Loader.GetDirectoryName(full);
            }
            return CompileFile(full, ctx);
        }



        /// <summary>
        /// 生成编译模板
        /// </summary> 
        /// <param name="fileName">文件</param>
        /// <param name="context">模板内容</param>
        /// <returns></returns>
        public static ICompileTemplate GenerateCompileTemplate(string fileName, TemplateContext context)
        {
            return GenerateCompileTemplate(fileName, (ctx) =>
            {
                ctx.Data = context.TempData;
                ctx.CurrentPath = context.CurrentPath;
                ctx.Charset = context.Charset;
                ctx.ResourceDirectories.AddRange(context.ResourceDirectories);
                ctx.StripWhiteSpace = context.StripWhiteSpace;
                ctx.ThrowExceptions = context.ThrowExceptions;
            });
        }

        /// <summary>
        /// 生成编译模板
        /// </summary>
        /// <param name="name">模板名 唯一</param>
        /// <param name="content">模板内容</param>
        /// <param name="action">ACTION</param>
        /// <returns></returns>
        public static ICompileTemplate GenerateCompileTemplate(string name, string content, Action<CompileContext> action = null)
        {
            if (string.IsNullOrEmpty(name))
            {
                name = ToHashCode(content);
            }
            var ctx = GenerateContext(name);
            if (action != null)
            {
                action(ctx);
            }
            var t = Runtime.Templates[ctx.Name];
            if (t != null)
            {
                return t as ICompileTemplate;
            }
            return Runtime.Templates[ctx.Name] = Compile(content, ctx);
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

        /// <summary>
        /// 生成编译模板
        /// </summary>
        /// <param name="fileName">模板文件</param>
        /// <param name="action">ACTION</param>
        /// <returns></returns>
        public static ICompileTemplate GenerateCompileTemplate(string fileName, Action<CompileContext> action = null)
        {
            var name = fileName;

            var ctx = GenerateContext(name);
            if (action != null)
            {
                action(ctx);
            }
            var full = CompileBuilder.FindPath(fileName, ctx);
            if (string.IsNullOrEmpty(full))
            {
                throw new Exception.CompileException($"\"{fileName}\" cannot be found.");
            }
            ctx.Name = full;
            if (string.IsNullOrEmpty(ctx.CurrentPath))
            {
                ctx.CurrentPath = Runtime.Loader.GetDirectoryName(full);
            }
            var t = Runtime.Templates[ctx.Name];
            if (t != null)
            {
                return t as ICompileTemplate;
            }
            return Runtime.Templates[ctx.Name] = CompileFile(full, ctx);
        }
    }
}