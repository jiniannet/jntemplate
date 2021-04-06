/********************************************************************************
 Copyright (c) jiniannet (http://www.jiniannet.com). All rights reserved.
 Licensed under the MIT license. See licence.txt file in the project root for full license information.
 ********************************************************************************/
using System;
using JinianNet.JNTemplate.Configuration;
using JinianNet.JNTemplate.CodeCompilation;
using System.Collections.Generic;
using JinianNet.JNTemplate.Runtime;
using JinianNet.JNTemplate.Resources;
using JinianNet.JNTemplate.Caching;
using System.Reflection;
using JinianNet.JNTemplate.Nodes;
using System.Text;
using System.Linq;

namespace JinianNet.JNTemplate
{
    /// <summary>
    /// The template engine
    /// </summary>
    public sealed class TemplatingEngine : IEngine
    {
        private static string[] registrars = new string[] {
                "JinianNet.JNTemplate.Parsers.CommentRegistrar",
                "JinianNet.JNTemplate.Parsers.BooleanRegistrar",
                "JinianNet.JNTemplate.Parsers.NumberRegistrar",
                "JinianNet.JNTemplate.Parsers.ElseRegistrar",
                "JinianNet.JNTemplate.Parsers.EleseRegistrar",
                "JinianNet.JNTemplate.Parsers.EndRegistrar",
                "JinianNet.JNTemplate.Parsers.BodyRegistrar",
                "JinianNet.JNTemplate.Parsers.NullRegistrar",
                "JinianNet.JNTemplate.Parsers.VariableRegistrar",
                "JinianNet.JNTemplate.Parsers.IndexValueRegistrar",
                "JinianNet.JNTemplate.Parsers.StringRegistrar",
                "JinianNet.JNTemplate.Parsers.ForeachRegistrar",
                "JinianNet.JNTemplate.Parsers.ForRegistrar",
                "JinianNet.JNTemplate.Parsers.SetRegistrar",
                "JinianNet.JNTemplate.Parsers.IfRegistrar",
                "JinianNet.JNTemplate.Parsers.ElseifRegistrar",
                "JinianNet.JNTemplate.Parsers.LayoutRegistrar",
                "JinianNet.JNTemplate.Parsers.LoadRegistrar",
                "JinianNet.JNTemplate.Parsers.IncludeRegistrar",
                "JinianNet.JNTemplate.Parsers.FunctionRegistrar",
                "JinianNet.JNTemplate.Parsers.JsonRegistrar",
                "JinianNet.JNTemplate.Parsers.TextRegistrar",
                "JinianNet.JNTemplate.Parsers.OperatorRegistrar",
                "JinianNet.JNTemplate.Parsers.ComplexRegistrar"}; 
        #region
        /// <summary>
        /// Gets the <see cref="RuntimeOptions"/>.
        /// </summary>
        public RuntimeOptions Options { private set; get; }
        /// <summary>
        /// Initializes a new instance of the <see cref="TemplatingEngine"/> class
        /// </summary>
        public TemplatingEngine()
        {

        }

        /// <inheritdoc />
        public IEngine Configure(Action<IConfig> action)
        {
            var conf = Configuration.EngineConfig.CreateDefault();
            action?.Invoke(conf);
            return Configure(conf, null);
        }

        /// <inheritdoc />
        public IEngine Configure(Action<IConfig, VariableScope> action)
        {
            var conf = Configuration.EngineConfig.CreateDefault();
            var score = new VariableScope(null);
            action?.Invoke(conf, score);
            return Configure(conf, score);
        }

        /// <inheritdoc />
        public IEngine Configure(IDictionary<string, object> dict)
        {
            var props = typeof(RuntimeOptions).GetProperties(BindingFlags.Public | BindingFlags.Instance);
            foreach (var kv in dict)
            {
                if (kv.Value == null)
                {
                    continue;
                }
                var p = props.Where(m => m.Name.Equals(kv.Key, StringComparison.OrdinalIgnoreCase)).FirstOrDefault();
                if (p == null
                    || !p.CanWrite
                    || !Dynamic.DynamicHelpers.IsMatchType(kv.Value.GetType(), p.PropertyType))
                {
                    SetEnvironmentVariable(kv.Key, kv.Value?.ToString());
                    continue;
                }
#if NET20 || NET40
                p.SetValue(Options, kv.Value,null);
#else
                p.SetValue(Options, kv.Value);
#endif
            }
            return this;
        }

        /// <inheritdoc />
        public IEngine Configure(IConfig conf, VariableScope scope)
        {
            if (conf.ResourceDirectories != null && conf.ResourceDirectories.Count > 0)
            {
                foreach (var path in conf.ResourceDirectories)
                {
                    AppendResourcePath(path);
                }
            }
            if (scope != null && scope.Count > 0)
            {
                Options.Data = scope;
            }
            if (!string.IsNullOrEmpty(conf.Charset))
            {
                Options.Encoding = Encoding.GetEncoding(conf.Charset);
            }
            this.Options.DisableeLogogram = conf.DisableeLogogram;
            this.Options.EnableTemplateCache = conf.EnableTemplateCache;
            this.Options.StripWhiteSpace = conf.StripWhiteSpace;
            if (conf.TagFlag != '\0')
            {
                this.Options.TagFlag = conf.TagFlag;
            }
            if (!string.IsNullOrEmpty(conf.TagPrefix))
            {
                this.Options.TagPrefix = conf.TagPrefix;
            }
            if (!string.IsNullOrEmpty(conf.TagSuffix))
            {
                this.Options.TagSuffix = conf.TagSuffix;
            }
            this.Options.ThrowExceptions = conf.ThrowExceptions;
            return this;
        }

        /// <inheritdoc />
        public ICompilerResult CompileFile(string name, string path, Action<CompileContext> action = null)
        {
            var res = Options.Loader.Load(path, Options.Encoding, Options.ResourceDirectories.ToArray());
            if (res == null)
            {
                throw new Exception.TemplateException($"Path:\"{path}\", the file could not be found.");
            }

            if (string.IsNullOrEmpty(name))
            {
                name = res.FullPath;
            }
            return Options.CompilerResults[name] = TemplateCompiler.Compile(name, res.Content, Options, action);
        }


        /// <inheritdoc />
        public ICompilerResult Compile(string name, string content, Action<CompileContext> action = null)
        {
            if (string.IsNullOrEmpty(content))
            {
                throw new ArgumentNullException(nameof(content));
            }
            if (string.IsNullOrEmpty(name))
            {
                name = content.GetHashCode().ToString();
            }
            return Options.CompilerResults[name] = TemplateCompiler.Compile(name, content, Options, action);
        }

        /// <inheritdoc />
        public void CompileFile(System.IO.FileInfo[] fs, Action<CompileContext> action = null)
        {
            foreach (var f in fs)
            {
                this.CompileFile(f.FullName, f.FullName, action);
            }
        }


        /// <inheritdoc />
        public TemplateContext CreateContext()
        {
            var data = new VariableScope(Options.Data);
            var ctx = new TemplateContext(data);
            ctx.Options = Options;
            ctx.Charset = Options.Encoding;
            ctx.EnableTemplateCache = Options.EnableTemplateCache && !Options.EnableCompile;
            ctx.StripWhiteSpace = Options.StripWhiteSpace;
            ctx.ThrowExceptions = Options.ThrowExceptions;
            return ctx;
        }

        /// <inheritdoc />
        public ITemplate CreateTemplate(string text)
        {
            return CreateTemplate(null, text);
        }

        /// <inheritdoc />
        public ITemplate CreateTemplate(string name, string text)
        {
            if (string.IsNullOrEmpty(name))
            {
                name = text.GetHashCode().ToString();
            }
            ITemplate template;
            if (Options.EnableCompile)
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

        /// <inheritdoc />
        public ITemplate LoadTemplate(string path)
        {
            return LoadTemplate(null, path);
        }

        /// <inheritdoc />
        public ITemplate LoadTemplate(string name, string path)
        {
            if (Options.EnableCompile)
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
        private ITemplate LoadCompileTemplate(string name, string path)
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
            //template.CompileDelegate = (t) =>
            //{
            //    Options.Templates.GetOrAdd()
            //}
            if (Options.CompilerResults.Keys.Contains(name))
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
        private T LoadTemplate<T>(string name, string path, TemplateContext context)
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
                template.Context.CurrentPath = Options.Loader.GetDirectoryName(path);
            }
            return template;
        }



        /// <inheritdoc />
        public IEngine AppendResourcePath(string path)
        {
            if (!Options.ResourceDirectories.Contains(path))
            {
                Options.ResourceDirectories.Add(path);
            }
            return this;
        }

        /// <inheritdoc />
        public string GetEnvironmentVariable(string key)
        {
            if (key == null)
            {
                throw new ArgumentNullException($"\"{nameof(key)}\" cannot be null.");
            }
            string value;

            if (Options.Variable.TryGetValue(key, out value))
            {
                return value;
            }
            return null;
        }

        /// <inheritdoc />
        public void SetEnvironmentVariable(string key, string value)
        {
            if (key == null)
            {
                throw new ArgumentNullException($"\"{nameof(key)}\" cannot be null.");
            }
            if (value == null)
            {
                Options.Variable.Remove(key);
            }
            else
            {
                Options.Variable[key] = value;
            }
        }

        /// <inheritdoc />
        public IEngine UseLoader(IResourceLoader loader)
        {
            if (loader == null)
            {
                throw new ArgumentNullException(nameof(loader));
            }
            Options.Loader = loader;
            return this;
        }

        /// <inheritdoc />
        public IEngine UseCache(ICache cache)
        {
            if (cache == null)
            {
                throw new ArgumentNullException(nameof(cache));
            }
            Options.Cache = cache;
            return this;
        }

        /// <inheritdoc />
        public IEngine UseOptions(RuntimeOptions options)
        {
            Options = options;
            for (var i = 0; i < registrars.Length; i++)
            {
                Dynamic
                    .DynamicHelpers
                    .CreateInstance<IRegistrar>(registrars[i])
                    ?.Regiser(this);
            }
            return this;
        }

        /// <inheritdoc />
        public IEngine UseDefaultOptions()
        {
            return UseOptions(new RuntimeOptions());
        }

        /// <summary>
        /// Clear compiled object or cache.
        /// </summary>
        public void Clean()
        {
            if (Options.EnableCompile)
            {
                Options.CompilerResults.Clear();
            }
            else
            {
                Options.Cache.Clear();
            }
        }

        /// <inheritdoc />
        public void Register<T>(Func<TemplateParser, TokenCollection, ITag> parseMethod,
            Func<ITag, CompileContext, MethodInfo> compileMethod,
            Func<ITag, CompileContext, Type> guessMethod,
            int index = 0) where T : ITag
        {
            Options.Parser.Register(parseMethod, index);
            Options.Builder?.Register<T>(compileMethod);
            Options.Guesser?.Register<T>(guessMethod);
        }

        /// <inheritdoc />
        public void RegisterParseFunc(Func<TemplateParser, TokenCollection, ITag> func,
            int index = 0)
        {
            this.Options.Parser.Register(func, index);
        }

        /// <inheritdoc />
        public void RegisterCompileFunc<T>(Func<ITag, CompileContext, MethodInfo> func)
            where T : ITag
        {
            this.Options.Builder?.Register<T>(func);
        }

        /// <inheritdoc />
        public void RegisterGuessFunc<T>(Func<ITag, CompileContext, Type> func)
            where T : ITag
        {
            this.Options.Guesser?.Register<T>(func);
        }

        /// <inheritdoc />
        public void RegisterExecuteFunc<T>(Func<ITag, TemplateContext, object> func)
           where T : ITag
        {
            this.Options.ExecutorBuilder?.Register<T>(func);
        }
        #endregion
    }
}