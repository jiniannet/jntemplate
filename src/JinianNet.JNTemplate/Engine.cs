/********************************************************************************
 Copyright (c) jiniannet (http://www.jiniannet.com). All rights reserved.
 Licensed under the MIT license. See licence.txt file in the project root for full license information.
 ********************************************************************************/
using System;
using JinianNet.JNTemplate.Configuration;
using JinianNet.JNTemplate.CodeCompilation;
using JinianNet.JNTemplate.Resources;

namespace JinianNet.JNTemplate
{
    /// <summary>
    /// The template engine
    /// </summary>
    public static class Engine
    {

        private static readonly object State = new object();
        private static IEngine engine;
        private static string engineVersion;

        /// <summary>
        /// Gets or sets the engine instance.
        /// </summary>
        public static IEngine Current
        {
            get
            {
                if (engine == null)
                {
                    lock (State)
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
        }

        /// <summary>
        /// The engine version.
        /// </summary>
        public static string Version
        {
            get
            {
                if (string.IsNullOrEmpty(engineVersion))
                {
                    var ver = typeof(Engine).Assembly.GetName().Version;
                    engineVersion = $"{ver.Major}.{ver.Minor}.{ver.Build}";
                }
                return engineVersion;
            }
        }

        /// <summary>
        /// 
        /// </summary>
        public static EngineMode Mode => Current.Mode;

        /// <summary>
        /// Configuration engine which <see cref="Action{T}"/>.
        /// </summary>
        /// <param name="action">The <see cref="Action{T}"/>.</param>
        public static IEngine Configure(Action<Runtime.IOptions> action)
        {
            return Current.Configure(action);
        }

        /// <summary>
        /// Configuration engine which <see cref="Runtime.IOptions"/>.
        /// </summary>
        /// <param name="option">The <see cref="Runtime.IOptions"/>.</param>
        public static IEngine Configure(IConfig option)
        {
            return Current.Configure(option);
        }

        /// <summary>
        /// Compile a template with a given contents
        /// </summary>
        /// <param name="name">Unique key of the template</param>
        /// <param name="content">The template contents.</param>
        /// <param name="action">The <see cref="Action{CompileContext}"/>.</param>
        /// <returns></returns>
        public static ITemplateResult Compile(string name, string content, Action<CompileContext> action = null)
        {
            return Current.Compile(name, content, action);
        }


        /// <summary>
        /// Compile a template with a given file
        /// </summary>
        /// <param name="name">Unique key of the template</param>
        /// <param name="path">The fully qualified path of the file to load.</param>
        /// <param name="action">The <see cref="Action{CompileContext}"/>.</param>
        /// <returns></returns>
        public static ITemplateResult CompileFile(string name, string path, Action<CompileContext> action = null)
        {
            return Current.CompileFile(name, path, action);
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
        public static TemplateContext CreateContext()
        {
            return Current.CreateContext();
        }

        /// <summary>
        /// 
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="text"></param>
        /// <param name="data"></param>
        /// <returns></returns>

        /// <inheritdoc />
        public static string Parse<T>(string text, T data)
        {
            return Current.Parse<T>(text, data);
        }


        /// <summary>
        /// 
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="file"></param>
        /// <param name="data"></param>
        /// <returns></returns>
        public static string Parse<T>(System.IO.FileInfo file, T data)
        {
            return Current.Parse<T>(file, data);
        }


        /// <summary>
        /// Register an new excute method.
        /// </summary>
        /// <param name="visitor"></param>
        public static void Register(Parsers.ITagVisitor visitor)
        {
            Current.Register(visitor);
        }

        /// <summary>
        /// Enable compilation mode.
        /// </summary>
        /// <returns></returns>
        public static IEngine UseCompileEngine()
        {
            if (engine == null)
            {
                engine = new EngineBuilder().UseCompileEngine().Build();
            }
            else
            {
                engine.UseCompileEngine();
            }
            return engine;
        }

        /// <summary>
        /// Enable compilation mode.
        /// </summary>
        /// <returns></returns>
        public static IEngine UseInterpretationEngine()
        {
            if (engine == null)
            {
                engine = new EngineBuilder().UseInterpretationEngine().Build();
            }
            else
            {
                engine.UseInterpretationEngine();
            }
            return engine;
        }

        /// <summary>
        /// Appends the specified directory name to the resource path list.
        /// </summary>
        /// <param name="path">The name of the directory to be appended to the resource path.</param>
        public static IEngine AppendResourcePath(string path)
        {
            return Current.AppendResourcePath(path);
        }

        /// <summary>
        /// Sets an <see cref="IResourceLoader"/> values from engine.
        /// </summary>
        /// <param name="loader">The <see cref="IResourceLoader"/> to add set.</param> 
        public static IEngine UseLoader(IResourceLoader loader)
        {
            return Current.UseLoader(loader);
        }

        /// <summary>
        /// enable template file Watcher.
        /// </summary>
        /// <returns></returns>
        public static IEngine EnableFileWatcher()
        {
            return Current.EnableFileWatcher();
        }

        /// <summary>
        /// disable template file Watcher.
        /// </summary>
        /// <returns></returns>
        public static IEngine DisabledFileWatcher()
        {
            return Current.EnableFileWatcher();
        }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="provider"></param>
        /// <returns></returns>
        public static IEngine UseWatcherProvider(ITemplateWatcherProvider provider)
        {
            return Current.UseWatcherProvider(provider);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="action"></param>
        /// <returns></returns>
        public static IEngine Use(Action<Hosting.IHostEnvironment> action)
        {
            return Current.Use(action);
        }

        /// <summary>
        /// Clear compiled object and cache.
        /// </summary>
        public static void Clean()
        {
            Current.Clean();
        }

        /// <summary>
        /// 
        /// </summary>
        public static void Unload()
        {
            var temp = engine;
            engine = null;
            temp.Dispose();
        }
    }
}
