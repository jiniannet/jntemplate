/********************************************************************************
 Copyright (c) jiniannet (http://www.jiniannet.com). All rights reserved.
 Licensed under the MIT license. See licence.txt file in the project root for full license information.
 ********************************************************************************/
using System;
using JinianNet.JNTemplate.Configuration;
using JinianNet.JNTemplate.Compile;
using JinianNet.JNTemplate.Dynamic;
using System.Collections.Generic;

namespace JinianNet.JNTemplate
{
    /// <summary>
    /// The template engine
    /// </summary>
    public sealed class Engine
    {

        /// <summary>
        /// The engine version.
        /// </summary>
        public static string Version => Field.Version;

        /// <summary>
        /// Enable or disenable the cache.
        /// </summary>
        public static bool EnableCompile
        {
            get { return Runtime.Options.EnableCompile; }
            set { Runtime.Options.EnableCompile = value; }
        }

        /// <summary>
        /// Configuration engine which <see cref="Action{IConfig}"/>.
        /// </summary>
        /// <param name="action">The <see cref="Action{IConfig}"/>.</param>
        public static void Configure(Action<IConfig> action)
        {
            var conf = Configuration.EngineConfig.CreateDefault();
            action?.Invoke(conf);
            Configure(conf);
        }

        /// <summary>
        /// Configuration engine which <see cref="Action{IConfig, VariableScope}"/>.
        /// </summary>
        /// <param name="action">The <see cref="Action{IConfig, VariableScope}"/>.</param>
        public static void Configure(Action<IConfig, VariableScope> action)
        {
            var conf = Configuration.EngineConfig.CreateDefault();
            var score = new VariableScope(null);
            action?.Invoke(conf, score);
            Configure(conf, score);
        }

        /// <summary>
        /// Configuration engine which <see cref="IConfig"/>.
        /// </summary>
        /// <param name="conf">The <see cref="IConfig"/>.</param>
        public static void Configure(IConfig conf)
        {
            Configure(conf, null);
        }


        /// <summary>
        /// Configuration engine which <see cref="IConfig"/>.
        /// </summary>
        /// <param name="conf">The <see cref="IConfig"/>.</param>
        /// <param name="scope">The global <see cref="VariableScope"/>.</param>
        public static void Configure(IConfig conf, VariableScope scope)
        {
            if (conf.Loader != null)
            {
                Runtime.SetLoader(conf.Loader);
            }
            if (conf.ResourceDirectories != null && conf.ResourceDirectories.Count > 0)
            {
                foreach (var path in conf.ResourceDirectories)
                {
                    Runtime.AppendResourcePath(path);
                }
            }
            if (conf.TagParsers != null && conf.TagParsers.Count > 0)
            {

                foreach (var parser in conf.TagParsers)
                {
                    Runtime.RegisterTagParser(parser);
                }
            }
            Runtime.Configure(conf.ToDictionary(), scope);
        }

        /// <summary>
        /// Configuration engine which <see cref="IDictionary{Tkey,TValue}"/>.
        /// </summary>
        /// <param name="conf">The <see cref="IDictionary{Tkey,TValue}"/>.</param>
        /// <param name="scope">The global <see cref="VariableScope"/>.</param>
        public static void Configure(IDictionary<string, string> conf, VariableScope scope)
        {
            Runtime.Configure(conf, scope);
        }

        /// <summary>
        /// Compile a template with a given file
        /// </summary>
        /// <param name="name">Unique key of the template</param>
        /// <param name="path">The fully qualified path of the file to load.</param>
        /// <param name="action">The <see cref="Action{CompileContext}"/>.</param>
        /// <returns></returns>
        public static ICompileTemplate CompileFile(string name, string path, Action<CompileContext> action = null)
        {
            var res = Runtime.Loader.Load(path, Runtime.Encoding, Runtime.ResourceDirectories.ToArray());
            if (res == null)
            {
                throw new Exception.TemplateException($"Path:\"{path}\", the file could not be found.");
            }

            if (string.IsNullOrEmpty(name))
            {
                name = res.FullPath;
            }
            return Runtime.Templates[name] = Compiler.Compile(name, res.Content, action);
        }


        /// <summary>
        /// Compiles and renders a template.
        /// </summary>
        /// <param name="path">The fully qualified path of the file to load.</param>
        /// <param name="context">The <see cref="TemplateContext"/>.</param>
        /// <returns></returns>
        public static string CompileFileAndExec(string path, TemplateContext context)
        {
            var full = context.FindFullPath(path);
            if (full == null)
            {
                throw new Exception.TemplateException($"\"{ path }\" cannot be found.");
            }
            var template = Runtime.Templates[full];
            if (template == null)
            {
                template = CompileFile(full, full, (c) => context.CopyTo(c));
                if (template == null)
                {
                    throw new Exception.TemplateException($"\"{ path }\" compile error.");
                }
            }
            using (var sw = new System.IO.StringWriter())
            {
                template.Render(sw, context);
                return sw.ToString();
            }
        }

        /// <summary>
        /// Compile a template with a given contents
        /// </summary>
        /// <param name="name">Unique key of the template</param>
        /// <param name="content">The template contents.</param>
        /// <param name="action">The <see cref="Action{CompileContext}"/>.</param>
        /// <returns></returns>
        public static ICompileTemplate Compile(string name, string content, Action<CompileContext> action = null)
        {
            if (string.IsNullOrEmpty(content))
            {
                throw new ArgumentNullException(nameof(content));
            }
            if (string.IsNullOrEmpty(name))
            {
                name = content.GetHashCode().ToString();
            }
            return Runtime.Templates[name] = Compiler.Compile(name, content, action);
        }

        /// <summary>
        /// Compile a template with a given files
        /// </summary> 
        /// <param name="fs">The files.</param>
        /// <param name="action">The <see cref="Action{CompileContext}"/>.</param>
        /// <returns></returns>
        public static void CompileFile(System.IO.FileInfo[] fs, Action<CompileContext> action = null)
        {
            foreach (var f in fs)
            {
                CompileFile(f.FullName, f.FullName, action);
            }
        }

        /// <summary>
        /// Creates template context.
        /// </summary>
        /// <returns>An instance of a <see cref="TemplateContext"/>.</returns>
        public static TemplateContext CreateContext()
        {
            var data = new VariableScope();
            var ctx = new TemplateContext(data);
            return ctx;
        }


        /// <summary>
        /// Creates template with specified text.
        /// </summary>
        /// <param name="text">The template contents.</param>
        /// <returns>An instance of a template.</returns>
        public static ITemplate CreateTemplate(string text)
        {
            return CreateTemplate(null, text);
        }

        /// <summary>
        /// Creates template with specified text.
        /// </summary>
        /// <param name="name">Unique key of the template</param>
        /// <param name="text">The template contents.</param>
        /// <returns>An instance of a template.</returns>
        public static ITemplate CreateTemplate(string name, string text)
        {
            if (string.IsNullOrEmpty(name))
            {
                name = text.GetHashCode().ToString();
            }
            ITemplate template;
            if (EnableCompile)
            {
                template = new CompileTemplate(CreateContext(), text);
            }
            else
            {
                template = new Template(CreateContext(), text);
            }
            template.TemplateKey = name;
            template.Context.CurrentPath = System.IO.Directory.GetCurrentDirectory();
            return template;
        }

        /// <summary>
        /// Loads the template on the specified path.
        /// </summary>
        /// <param name="path">The fully qualified path of the file to load.</param>
        /// <returns>An instance of a template.</returns>
        public static ITemplate LoadTemplate(string path)
        {
            return LoadTemplate(null, path);
        }


        /// <summary>
        /// Loads the template on the specified path.
        /// </summary>
        /// <param name="name">Unique key of the template</param>
        /// <param name="path">The fully qualified path of the file to load.</param>
        /// <returns>An instance of a template.</returns>
        public static ITemplate LoadTemplate(string name, string path)
        {
            if (EnableCompile)
            {
                return LoadCompileTemplate(name, path);
            }

            var ctx = CreateContext();
            var res = ctx.Load(path);
            if (res == null)
            {
                throw new Exception.TemplateException($"Path:\"{path}\", the file could not be found.");
            }

            var t = LoadTemplate<Template>(name, path, CreateContext());
            t.TemplateContent = res.Content;
            return t;
        }

        /// <summary>
        /// Loads the template on the specified path.
        /// </summary>
        /// <param name="name">Unique key of the template</param>
        /// <param name="path">The fully qualified path of the file to load.</param>
        /// <returns>An instance of a template.</returns>
        private static ITemplate LoadCompileTemplate(string name, string path)
        {
            var ctx = CreateContext();
            if (string.IsNullOrEmpty(name))
            {
                name = ctx.FindFullPath(path);
                if (string.IsNullOrEmpty(name))
                {
                    throw new Exception.TemplateException($"Path:\"{path}\", the file could not be found.");
                }
            }
            var template = LoadTemplate<CompileTemplate>(name, path, ctx);
            if (Runtime.Templates.Keys.Contains(name))
            {
                return template;
            }
            var res = ctx.Load(path);
            if (res == null)
            {
                throw new Exception.TemplateException($"Path:\"{path}\", the file could not be found.");
            }
            template.TemplateContent = res.Content;
            return template;
        }

        /// <summary>
        /// Loads the template on the specified path.
        /// </summary>
        /// <typeparam name="T">Type of template. </typeparam>
        /// <param name="name">Unique key of the template</param>
        /// <param name="path">The fully qualified path of the file to load.</param>
        /// <param name="context">The <see cref="TemplateContext"/>.</param>
        /// <returns>An instance of a template.</returns>
        private static T LoadTemplate<T>(string name, string path, TemplateContext context)
            where T : ITemplate, new()
        {
            T template = new T();
            template.Context = context;
            template.TemplateKey = name;
            if (string.IsNullOrEmpty(template.TemplateKey))
            {
                template.TemplateKey = path;
            }
            if (template.Context != null && string.IsNullOrEmpty(template.Context.CurrentPath))
            {
                template.Context.CurrentPath = Runtime.Loader.GetDirectoryName(path);
            }
            return template;
        }

        /// <summary>
        /// Register an new tag.
        /// </summary>
        /// <typeparam name="T">Type of the new tag. </typeparam>
        /// <param name="parser">parser of the new tag.</param>
        /// <param name="compileFunc">compile method of the new tag.</param>
        /// <param name="guessFunc">guess method of the new tag.</param>
        public static void Register<T>(Parsers.ITagParser parser,
            Func<Nodes.ITag, CompileContext, System.Reflection.MethodInfo> compileFunc,
            Func<Nodes.ITag, CompileContext, Type> guessFunc) where T : Nodes.ITag
        {
            Runtime.RegisterTagParser(parser, 0);
            Compiler.Register<T>(compileFunc, guessFunc);
        }


        /// <summary>
        /// Register an new tag.
        /// </summary>
        /// <typeparam name="T">Type of the new tag. </typeparam>
        /// <param name="parser">parser of the new tag.</param>
        /// <param name="func">parse method of the new tag.</param>
        public static void Register<T>(Parsers.ITagParser parser,
            Func<Nodes.ITag, TemplateContext, object> func) where T : Nodes.ITag
        {
            Runtime.RegisterTagParser(parser, 0);
            TagExecutor.Register<T>(func);
        }
    }
}