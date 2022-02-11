/********************************************************************************
 Copyright (c) jiniannet (http://www.jiniannet.com). All rights reserved.
 Licensed under the MIT license. See licence.txt file in the project root for full license information.
 ********************************************************************************/
using System;
using System.Reflection;
using System.Threading.Tasks;
using System.Collections.Generic;
using JinianNet.JNTemplate.Configuration;
using JinianNet.JNTemplate.CodeCompilation;
using JinianNet.JNTemplate.Hosting;
using JinianNet.JNTemplate.Resources;
using JinianNet.JNTemplate.Caching;
using JinianNet.JNTemplate.Exceptions;
using JinianNet.JNTemplate.Nodes;
using JinianNet.JNTemplate.Runtime;

namespace JinianNet.JNTemplate
{
    /// <summary>
    /// The template engine
    /// </summary>
    public sealed class TemplatingEngine : IEngine
    {
        #region
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
                "JinianNet.JNTemplate.Parsers.ArrayRegistrar",
                "JinianNet.JNTemplate.Parsers.TextRegistrar",
                "JinianNet.JNTemplate.Parsers.OperatorRegistrar",
                "JinianNet.JNTemplate.Parsers.LogicRegistrar",
                "JinianNet.JNTemplate.Parsers.ArithmeticRegistrar",
                "JinianNet.JNTemplate.Parsers.ReferenceRegistrar"};
        #endregion

        #region
        /// <inheritdoc />
        public IHostEnvironment HostEnvironment { get; }
        /// <inheritdoc />
        public EngineMode Mode => HostEnvironment.Options.Mode;

        /// <summary>
        ///
        /// </summary>
        /// Initializes a new instance of the <see cref="TemplatingEngine"/> class
        /// <param name="environment"></param>
        public TemplatingEngine(IHostEnvironment environment = null)
        {
            HostEnvironment = environment
                ?? new DefaultHostEnvironment();
            Initialize();
        }

        /// <inheritdoc />
        public IEngine Configure(Action<IOptions> action)
        {
            var mode = HostEnvironment.Options.Mode;
            action?.Invoke(HostEnvironment.Options);
            if (mode != HostEnvironment.Options.Mode)
            {
                Initialize();
            }
            return this;
        }

        /// <inheritdoc />
        public IEngine Configure(IConfig option)
        {
            HostEnvironment.Options.DisableeLogogram = option.DisableeLogogram;
            HostEnvironment.Options.TagPrefix = option.TagPrefix;
            HostEnvironment.Options.TagSuffix = option.TagSuffix;
            HostEnvironment.Options.TagFlag = option.TagFlag;
            HostEnvironment.Options.Encoding = option.Encoding;
            HostEnvironment.Options.ThrowExceptions = option.ThrowExceptions;
            HostEnvironment.Options.TypeDetectPattern = option.TypeDetectPattern;
            HostEnvironment.Options.OutMode = option.OutMode;
            if (option.ResourceDirectories?.Count > 0)
            {
                foreach (var path in option.ResourceDirectories)
                {
                    AppendResourcePath(path);
                }
            }
            if (option.GlobalData != null && option.GlobalData.Count > 0)
            {
                foreach (var kv in option.GlobalData)
                {
                    if (kv.Value == null)
                    {
                        continue;
                    }
                    HostEnvironment.Options.Data.Set(kv.Key, kv.Value, kv.Value.GetType());
                }
            }
            var mode = option.EnableCompile ? EngineMode.Compiled : EngineMode.Interpreted;

            if (HostEnvironment.Options.TypeDetectPattern == TypeDetect.None && mode == EngineMode.Compiled)
            {
                HostEnvironment.Options.TypeDetectPattern = TypeDetect.Standard;
            }

            if (mode != HostEnvironment.Options.Mode)
            {
                HostEnvironment.Options.Mode = mode;
                Initialize();
            }
            return this;
        }

        /// <inheritdoc />
        public IResult CompileFile(string name, string path, Action<CompileContext> action = null)
        {
            return HostEnvironment.Results[name] = HostEnvironment.CompileFile(name, path, action);
        }


        /// <inheritdoc />
        public IResult Compile(string name, string content, Action<CompileContext> action = null)
        {
            if (string.IsNullOrEmpty(content))
            {
                throw new ArgumentNullException(nameof(content));
            }
            if (string.IsNullOrEmpty(name))
            {
                name = content.GetHashCode().ToString();
            }
            return HostEnvironment.Results[name] = HostEnvironment.Compile(name, content, action);
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
            return HostEnvironment.CreateContext();
        }


        /// <inheritdoc />
        public string Parse<T>(string text, T data)
        {
            if (text == null)
                throw new ArgumentNullException(nameof(text));
            var name = text.GetHashCode().ToString();
            return Parse<T>(name, new StringReader(text), data);
        }

        /// <inheritdoc /> 
        public string Parse(string text, Action<TemplateContext> action)
        {
            if (text == null)
                throw new ArgumentNullException(nameof(text));
            var name = text.GetHashCode().ToString();
            return Parse(name, new StringReader(text), action);
        }


        /// <inheritdoc /> 
        public string Parse<T>(System.IO.FileInfo file, T data)
        {
            if (file == null)
                throw new ArgumentNullException(nameof(file));
            return Parse<T>(file.FullName, new ResourceReader(file.FullName), data);
        }

        /// <inheritdoc /> 
        public string Parse(System.IO.FileInfo file, Action<TemplateContext> action)
        {
            if (file == null)
                throw new ArgumentNullException(nameof(file));
            return Parse(file.FullName, new ResourceReader(file.FullName), action);
        }

        /// <inheritdoc />
        private string Parse<T>(string name, IReader reader, T data)
        {
            if (data == null)
            {
                return Parse(name, reader, (Action<TemplateContext>)null);
            }
            if (data is System.Collections.IDictionary dict)
            {
                return Parse(name, reader, ctx =>
                {
                    var keys = dict.Keys;
                    foreach (var key in keys)
                    {
                        if (key == null)
                        {
                            continue;
                        }
                        var value = dict[key];
                        ctx.TempData.Set(key.ToString(), value, value?.GetType() ?? typeof(object));
                    }
                });
            }
            return Parse(name, reader, ctx => ctx.TempData.Set<T>("Model", data));
        }

        /// <inheritdoc />
        public string Parse(string name, IReader reader, Action<TemplateContext> action)
        {
            var ctx = CreateContext();
            action?.Invoke(ctx);
            if (Mode == EngineMode.Compiled)
            {
                return CompileParse(name, ctx, reader);
            }
            return InterpretParse(name, ctx, reader);
        }

        private string CompileParse(string name, TemplateContext ctx, IReader reader)
        {
            var result = ctx.CompileTemplate(name, reader);
            using (var write = new System.IO.StringWriter())
            {
                result.Render(write, ctx);
                return write.ToString();
            }
        }

        private string InterpretParse(string name, TemplateContext ctx, IReader reader)
        {
            var result = ctx.InterpretTemplate(name, reader);
            using (var write = new System.IO.StringWriter())
            {
                result.Render(write, ctx);
                return write.ToString();
            }
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

            var reader = new StringReader(text);
            var template = new Template(CreateContext(), reader);
            template.TemplateKey = name;
            template.Context.CurrentPath = HostEnvironment.RootPath;
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
            var ctx = CreateContext();
            if (string.IsNullOrEmpty(name) &&
                string.IsNullOrEmpty(path = name = ctx.FindFullPath(path)))
            {
                throw new TemplateException($"Path:\"{path}\", the file could not be found.");
            }
            var reader = new ResourceReader(path);
            var template = new Template(ctx, reader);
            template.TemplateKey = name;
            if (template.Context != null && string.IsNullOrEmpty(template.Context.CurrentPath))
            {
                template.Context.CurrentPath = HostEnvironment.Loader.GetDirectoryName(path);
            }
            return template;
        }

        /// <inheritdoc />
        public IEngine AppendResourcePath(string path)
        {
            var options = HostEnvironment.Options;
            if (!options.ResourceDirectories.Contains(path))
            {
                options.ResourceDirectories.Add(path);
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

            if (HostEnvironment.EnvironmentVariable.TryGetValue(key, out value))
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
                HostEnvironment.EnvironmentVariable.Remove(key);
            }
            else
            {
                HostEnvironment.EnvironmentVariable[key] = value;
            }
        }

        /// <inheritdoc />
        public IEngine UseLoader(IResourceLoader loader)
        {
            if (loader == null)
            {
                throw new ArgumentNullException(nameof(loader));
            }
            HostEnvironment.Loader = loader;
            return this;
        }

        /// <inheritdoc />
        public IEngine UseScopeProvider(IScopeProvider provider)
        {
            if (provider == null)
            {
                throw new ArgumentNullException(nameof(provider));
            }
            HostEnvironment.ScopeProvider = provider;
            var data = provider.CreateScope();
            var original = HostEnvironment.Options.Data;
            foreach (KeyValuePair<string, object> kv in data)
            {
                data.Set(kv.Key, kv.Value, data.GetType(kv.Key));
            }

            return this;
        }

        /// <inheritdoc />
        public IEngine UseCache(ICache cache)
        {
            if (cache == null)
            {
                throw new ArgumentNullException(nameof(cache));
            }
            HostEnvironment.Cache = cache;
            return this;
        }

        /// <inheritdoc />
        public IEngine UseOptions(IOptions options)
        {
            if (HostEnvironment.Options != options)
            {
                HostEnvironment.Options = options;
                Initialize();
            }
            return this;
        }

        private void Initialize()
        {
            lock (this)
            {
                Reset();
                for (var i = 0; i < registrars.Length; i++)
                {
                    Dynamic
                        .ReflectionExtensions
                        .CreateInstance<IRegistrar>(registrars[i])
                        ?.Regiser(this);
                }
            }
        }

        /// <inheritdoc />
        public IEngine UseDefaultOptions()
        {
            return UseOptions(new RuntimeOptions());
        }

        /// <inheritdoc />
        public IEngine UseCompileEngine()
        {
            HostEnvironment.Options.Mode = EngineMode.Compiled;
            Initialize();
            return this;
        }

        /// <inheritdoc />
        public IEngine UseInterpretationEngine()
        {
            HostEnvironment.Options.Mode = EngineMode.Interpreted;
            Initialize();
            return this;
        }

        /// <inheritdoc />
        public void Clean()
        {
            HostEnvironment.Results?.Clear();
            //HostEnvironment.Cache?.Clear();
        }

        /// <inheritdoc />
        public void Register<T>(Func<TemplateParser, TokenCollection, ITag> parseMethod,
            Func<ITag, CompileContext, MethodInfo> compileMethod,
            Func<ITag, CompileContext, Type> guessMethod,
            int index = 0) where T : ITag
        {
            HostEnvironment.Parser.Register(parseMethod, index);
            HostEnvironment.Builder?.Register<T>(compileMethod);
            HostEnvironment.Guesser?.Register<T>(guessMethod);
        }

        /// <inheritdoc />
        public void RegisterParseFunc(Func<TemplateParser, TokenCollection, ITag> func,
            int index = 0)
        {
            HostEnvironment.Parser.Register(func, index);
        }

        /// <inheritdoc />
        public void RegisterCompileFunc<T>(Func<ITag, CompileContext, MethodInfo> func)
            where T : ITag
        {
            HostEnvironment.Builder?.Register<T>(func);
        }

        /// <inheritdoc />
        public void RegisterGuessFunc<T>(Func<ITag, CompileContext, Type> func)
            where T : ITag
        {
            HostEnvironment.Guesser?.Register<T>(func);
        }

        /// <inheritdoc />
        public void RegisterExecuteFunc<T>(Func<ITag, TemplateContext, object> func)
           where T : ITag
        {
            HostEnvironment.ExecutorBuilder?.Register<T>(func);
        }

        /// <inheritdoc />
        public void Reset()
        {
            HostEnvironment.ExecutorBuilder.Clear();
            HostEnvironment.Guesser.Clear();
            HostEnvironment.Builder.Clear();
            HostEnvironment.Parser.Clear();
            Clean();
        }

        #endregion
    }
}