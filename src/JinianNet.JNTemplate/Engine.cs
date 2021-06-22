/********************************************************************************
 Copyright (c) jiniannet (http://www.jiniannet.com). All rights reserved.
 Licensed under the MIT license. See licence.txt file in the project root for full license information.
 ********************************************************************************/
using System;
using JinianNet.JNTemplate.Configuration;
using JinianNet.JNTemplate.CodeCompilation;

namespace JinianNet.JNTemplate
{
    /// <summary>
    /// The template engine
    /// </summary>
    public sealed class Engine
    {
        private static volatile object state = new object();
        private static IEngine engine;

        /// <summary>
        /// Gets or sets the engine instance.
        /// </summary>
        public static IEngine Current
        {
            get
            {
                if (engine == null)
                {
                    lock (state)
                    {
                        if (engine == null)
                        {
                            engine = new EngineBuilder()
                                .Build();
                        }
                    }
                }
                return engine;
            }
            set { engine = value; }
        }

        /// <summary>
        /// The engine version.
        /// </summary>
        public static string Version => Field.Version;

        /// <summary>
        /// Enable or disenable the compile mode.
        /// </summary>
        public static bool EnableCompile => Current.Options.EnableCompile;

        /// <summary>
        /// Configuration engine which <see cref="Action{T}"/>.
        /// </summary>
        /// <param name="action">The <see cref="Action{T}"/>.</param>
        public static void Configure(Action<Runtime.IOptions> action)
        {
            Current.Configure(action);
        }

        /// <summary>
        /// Configuration engine which <see cref="Action{TOptions, TVariable}"/>.
        /// </summary>
        /// <param name="action">The <see cref="Action{TOptions, TVariable}"/>.</param>
        public static void Configure(Action<Runtime.IOptions, IVariableScope> action)
        {
            action?.Invoke(Current.Options, Current.Options.Data);
        }

        /// <summary>
        /// Configuration engine which <see cref="Runtime.IOptions"/>.
        /// </summary>
        /// <param name="option">The <see cref="Runtime.IOptions"/>.</param>
        public static void Configure(Runtime.IOptions option)
        {
            Current.Configure(option);
        }


        /// <summary>
        /// Configuration engine which <see cref="Runtime.IOptions"/>.
        /// </summary>
        /// <param name="option">The <see cref="Runtime.IOptions"/>.</param>
        /// <param name="scope">The global <see cref="IVariableScope"/>.</param>
        [Obsolete("please use Configure(Action<IOptions>)")]
        public static void Configure(Runtime.IOptions option, IVariableScope scope)
        {
            if(scope!=null && scope.Count > 0)
            {
                var keys = scope.Keys;
                foreach(var key in keys)
                {
                    option.Data?.Set(key, scope[key], scope.GetType(key));
                }
            }
            Current.Configure(option);
        }

        /// <summary>
        /// Compile a template with a given file
        /// </summary>
        /// <param name="name">Unique key of the template</param>
        /// <param name="path">The fully qualified path of the file to load.</param>
        /// <param name="action">The <see cref="Action{CompileContext}"/>.</param>
        /// <returns></returns>
        public static ICompilerResult CompileFile(string name, string path, Action<CompileContext> action = null)
        {
            return Current.CompileFile(name, path, action);
        }

        /// <summary>
        /// Compile a template with a given contents
        /// </summary>
        /// <param name="name">Unique key of the template</param>
        /// <param name="content">The template contents.</param>
        /// <param name="action">The <see cref="Action{CompileContext}"/>.</param>
        /// <returns></returns>
        public static ICompilerResult Compile(string name, string content, Action<CompileContext> action = null)
        {
            return Current.Compile(name, content, action);
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
        /// Creates template with specified text.
        /// </summary>
        /// <param name="text">The template contents.</param>
        /// <returns>An instance of a template.</returns>
        public static ITemplate CreateTemplate(string text)
        {
            return Current.CreateTemplate(null, text);
        }

        /// <summary>
        /// Creates template with specified text.
        /// </summary>
        /// <param name="name">Unique key of the template</param>
        /// <param name="text">The template contents.</param>
        /// <returns>An instance of a template.</returns>
        public static ITemplate CreateTemplate(string name, string text)
        {
            return Current.CreateTemplate(name, text);
        }

        /// <summary>
        /// Loads the template on the specified path.
        /// </summary>
        /// <param name="path">The fully qualified path of the file to load.</param>
        /// <returns>An instance of a template.</returns>
        public static ITemplate LoadTemplate(string path)
        {
            return Current.LoadTemplate(path);
        }


        /// <summary>
        /// Loads the template on the specified path.
        /// </summary>
        /// <param name="name">Unique key of the template</param>
        /// <param name="path">The fully qualified path of the file to load.</param>
        /// <returns>An instance of a template.</returns>
        public static ITemplate LoadTemplate(string name, string path)
        {
            return Current.LoadTemplate(name, path);
        }

        /// <summary>
        /// Creates template context.
        /// </summary>
        /// <returns>An instance of a <see cref="TemplateContext"/>.</returns>
        public TemplateContext CreateContext()
        {
            return Current.CreateContext();
        }

        /// <summary>
        /// Register an new tag.
        /// </summary>
        /// <typeparam name="T">Type of the new tag. </typeparam>
        /// <param name="parser">parser of the new tag.</param>
        /// <param name="compileFunc">compile method of the new tag.</param>
        /// <param name="guessFunc">guess method of the new tag.</param>
        [Obsolete]
        public static void Register<T>(Parsers.ITagParser parser,
            Func<Nodes.ITag, CompileContext, System.Reflection.MethodInfo> compileFunc,
            Func<Nodes.ITag, CompileContext, Type> guessFunc) where T : Nodes.ITag
        {
            Current.Register<T>((p, tc) => parser.Parse(p, tc), compileFunc, guessFunc);
        }

        /// <summary>
        /// Register an new tag.
        /// </summary>
        /// <typeparam name="T">Type of the new tag. </typeparam>
        /// <param name="parseMethod">parser of the new tag.</param>
        /// <param name="compileMethod">compile method of the new tag.</param>
        /// <param name="guessMethod">guess method of the new tag.</param>
        /// <param name="index">The zero-based index.</param>
        public static void Register<T>(Func<TemplateParser, Nodes.TokenCollection, Nodes.ITag> parseMethod,
           Func<Nodes.ITag, CompileContext, System.Reflection.MethodInfo> compileMethod,
           Func<Nodes.ITag, CompileContext, Type> guessMethod,
           int index = 0) where T : Nodes.ITag
        {
            Current.Register<T>(parseMethod, compileMethod, guessMethod);
        }
    }
}
