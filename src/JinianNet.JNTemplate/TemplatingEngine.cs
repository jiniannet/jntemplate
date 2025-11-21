/********************************************************************************
 Copyright (c) jiniannet (http://www.jiniannet.com). All rights reserved.
 Licensed under the MIT license. See licence.txt file in the project root for full license information.
 ********************************************************************************/
using System;
using System.Reflection;
using System.Collections.Generic;
using JinianNet.JNTemplate.Configuration;
using JinianNet.JNTemplate.CodeCompilation;
using JinianNet.JNTemplate.Hosting;
using JinianNet.JNTemplate.Resources;
using JinianNet.JNTemplate.Exceptions;
using JinianNet.JNTemplate.Nodes;
using JinianNet.JNTemplate.Runtime;
using JinianNet.JNTemplate.Parsers;



#if !NF35 && !NF20
using System.Threading.Tasks;
#endif
namespace JinianNet.JNTemplate
{
    /// <summary>
    /// The template engine
    /// </summary>
    public sealed class TemplatingEngine : IEngine
    {
        #region Registrar 
        private static ITagVisitor[] LoadDefaultVisitor()
        {
            return new ITagVisitor[] {
                LoadVisitor<Parsers.CommentVisitor>(),
                LoadVisitor<Parsers.BooleanVisitor>(),
                LoadVisitor<Parsers.NumberVisitor>(),
                LoadVisitor<Parsers.ElseVisitor>(),
                LoadVisitor<Parsers.EndVisitor>(),
                LoadVisitor<Parsers.BodyVisitor>(),
                LoadVisitor<Parsers.NullVisitor>(),
                LoadVisitor<Parsers.VariableVisitor>(),
                LoadVisitor<Parsers.IndexValueVisitor>(),
                LoadVisitor<Parsers.StringVisitor>(),
                LoadVisitor<Parsers.ForeachVisitor>(),
                LoadVisitor<Parsers.ForVisitor>(),
                LoadVisitor<Parsers.SetVisitor>(),
                LoadVisitor<Parsers.IfVisitor>(),
                LoadVisitor<Parsers.ElseifVisitor>(),
                LoadVisitor<Parsers.LayoutVisitor>(),
                LoadVisitor<Parsers.LoadVisitor>(),
                LoadVisitor<Parsers.IncludeVisitor>(),
                LoadVisitor<Parsers.FunctionVisitor>(),
                LoadVisitor<Parsers.ArrayVisitor>(),
                LoadVisitor<Parsers.TextVisitor>(),
                LoadVisitor<Parsers.OperatorVisitor>(),
                LoadVisitor<Parsers.LogicVisitor>(),
                LoadVisitor<Parsers.ArithmeticVisitor>(),
                LoadVisitor<Parsers.JsonVisitor>(),
                LoadVisitor<Parsers.ReferenceVisitor>()
            };
        }

        private static ITagVisitor LoadVisitor<T>()
            where T : ITagVisitor, new()
        {
            return new T();
        }
        #endregion

        #region
        private readonly object locker;
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
            locker = new object();
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

            if (mode != HostEnvironment.Options.Mode)
            {
                HostEnvironment.Options.Mode = mode;
                Initialize();
            }
            return this;
        }


        /// <inheritdoc />
        public ITemplateResult Compile(string name, string content, Action<CompileContext> action = null)
        {
            if (string.IsNullOrEmpty(content))
            {
                throw new ArgumentNullException(nameof(content));
            }
            if (string.IsNullOrEmpty(name))
            {
                name = Utility.ContentToTemplateName(content);
            }

            using (var ctx = HostEnvironmentExtensions.GenerateContext(HostEnvironment, name))
            {
                if (action != null)
                {
                    action(ctx);
                }
                return HostEnvironment.Cache[name] = ctx.Compile(content);
            }
        }


        /// <inheritdoc />
        public ITemplateResult CompileFile(string name, string path, Action<CompileContext> action = null)
        {
            return HostEnvironment.Cache[name] = HostEnvironment.CompileFile(name, path, action);
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
        public string Parse(string text, Action<TemplateContext> action)
        {
            if (text == null)
                throw new ArgumentNullException(nameof(text));
            var name = Utility.ContentToTemplateName(text);
            return Parse(name, new Resources.StringReader(text), action);
        }

        /// <inheritdoc /> 
        public string Parse(System.IO.FileInfo file, Action<TemplateContext> action)
        {
            if (file == null)
                throw new ArgumentNullException(nameof(file));
            return Parse(file.FullName, new Resources.ResourceReader(file.FullName), action);
        }
        /// <inheritdoc />
        public string Parse(string name, Resources.IResourceReader reader, Action<TemplateContext> action)
        {
            var ctx = CreateContext();
            action?.Invoke(ctx);
            ctx.Name = name;
            if (Mode == EngineMode.Compiled)
            {
                return CompileParse(name, ctx, reader);
            }
            return InterpretParse(name, ctx, reader);
        }
        /// <inheritdoc />
        public string Parse<T>(string text, T data)
        {
            if (text == null)
                throw new ArgumentNullException(nameof(text));
            var name = Utility.ContentToTemplateName(text);
            return Parse<T>(name, new Resources.StringReader(text), data);
        }
        /// <inheritdoc /> 
        public string Parse<T>(System.IO.FileInfo file, T data)
        {
            if (file == null)
                throw new ArgumentNullException(nameof(file));
            return Parse<T>(file.FullName, new Resources.ResourceReader(file.FullName), data);
        }

        /// <inheritdoc />
        private string Parse<T>(string name, Resources.IResourceReader reader, T data)
        {
            if (object.Equals(data, default(T)))
            {
                return Parse(name, reader, (Action<TemplateContext>)null);
            }
            if (data is System.Collections.IDictionary dict)
            {
                return ParseDictionary(name, reader, dict);
            }
            var t = data.GetType();
            if (t.IsNotPublic && t.Name.Contains("Anonymous"))
                return ParseAnonymous(name, reader, t, data);

            string key = HostEnvironment.Options.ModelName;
            if (string.IsNullOrEmpty(key))
            {
                if (!t.IsArray && !t.IsEnum && !t.IsGenericType)
                    key = t.Name;
                else
                    key = "Model";
            }
            return Parse(name, reader, ctx => ctx.Set(key, data, typeof(T)));
        }
        private string ParseAnonymous(string name, Resources.IResourceReader reader, Type type, object data)
        {
            var ps = type.GetProperties();
            var fs = type.GetFields();

            return Parse(name, reader, ctx =>
            {
                foreach (var f in fs)
                    ctx.Set(f.Name, f.GetValue(data), f.FieldType);
                foreach (var p in ps)
                    ctx.Set(p.Name, p.GetValue(data, null), p.PropertyType);
            });
        }
        private string ParseDictionary(string name, Resources.IResourceReader reader, System.Collections.IDictionary dict)
        {
            var type = dict.GetType();
            Type defaultType;
            if (type.IsGenericType && type.GetGenericArguments().Length == 2)
            {
                defaultType = type.GetGenericArguments()[1];
            }
            else
            {
                defaultType = typeof(object);
            }
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
                    ctx.Set(key.ToString(), value, value?.GetType() ?? defaultType);
                }
            });
        }

        private static string InterpretParse(string name, TemplateContext ctx, Resources.IResourceReader reader)
        {
            var result = ctx.InterpretTemplate(name, reader);
            using (var write = new System.IO.StringWriter())
            {
                result.Render(write, ctx);
                return write.ToString();
            }
        }
        private static string CompileParse(string name, TemplateContext ctx, Resources.IResourceReader reader)
        {
            var result = ctx.CompileTemplate(name, reader);
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
                name = Utility.ContentToTemplateName(text);
            }

            var reader = new Resources.StringReader(text);
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
            var templatePath = ctx.FindFullPath(path);
            if (string.IsNullOrEmpty(templatePath))
            {
                throw new TemplateException($"Path:\"{path}\", the file could not be found.");
            }
            if (string.IsNullOrEmpty(name))
            {
                name = templatePath;
            }
            var reader = new Resources.ResourceReader(templatePath);
            var template = new Template(ctx, reader);
            template.TemplateKey = name;
            if (template.Context != null && string.IsNullOrEmpty(template.Context.CurrentPath))
            {
                template.Context.CurrentPath = System.IO.Path.GetDirectoryName(templatePath);
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
                throw new ArgumentNullException(nameof(key));
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
                throw new ArgumentNullException(nameof(key));
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
        public IEngine EnableFileWatcher()
        {
            HostEnvironment.ResourceManager.EnableWatcher = true;
            return this;
        }

        /// <inheritdoc />
        public IEngine DisabledFileWatcher()
        {
            HostEnvironment.ResourceManager.EnableWatcher = false;
            return this;
        }

        /// <inheritdoc />
        public IEngine UseWatcherProvider(ITemplateWatcherProvider provider)
        {
            var enableWatcher = HostEnvironment.ResourceManager.EnableWatcher;
            if (enableWatcher)
                HostEnvironment.ResourceManager.EnableWatcher = false;
            HostEnvironment.TemplateWatcherProvider = provider;
            HostEnvironment.ResourceManager.EnableWatcher = enableWatcher;
            return this;
        }

        /// <inheritdoc />
        public IEngine Use(Action<IHostEnvironment> action)
        {
            action.Invoke(HostEnvironment);
            return this;
        }
        #region

        /// <inheritdoc /> 

        /// <inheritdoc />
        public IEngine UseLoader<TLoader>() where TLoader
            : IResourceLoader, new()
        {
            return UseLoader(new TLoader());
        }

        /// <inheritdoc />
        public IEngine UseLoader(IResourceLoader loader)
        {
            if (loader == null)
            {
                throw new ArgumentNullException(nameof(loader));
            }
            HostEnvironment.ResourceLoader = loader;
            return this;
        }

        /// <inheritdoc />
        public IEngine UseScopeProvider<TProvider>() where TProvider
            : IScopeProvider, new()
        {
            return UseScopeProvider(new TProvider());
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
            foreach (KeyValuePair<string, object> kv in original)
            {
                data.Set(kv.Key, kv.Value, original.GetType(kv.Key));
            }

            return this;
        }

        /// <inheritdoc />
        public IEngine UseCache<TCache>() where TCache
            : ITemplateCache, new()
        {
            return UseCache(new TCache());
        }

        /// <inheritdoc />
        public IEngine UseCache(ITemplateCache cache)
        {
            if (cache == null)
            {
                throw new ArgumentNullException(nameof(cache));
            }
            HostEnvironment.Cache = cache;
            return this;
        }
        /// <inheritdoc />
        public IEngine UseOptions<TOptions>() where TOptions
            : IOptions, new()
        {
            return UseOptions(new TOptions());
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
        #endregion 

        private void Initialize()
        {
            lock (locker)
            {
                Reset();
                var list = LoadDefaultVisitor();
                for (var i = 0; i < list.Length; i++)
                {
                    HostEnvironment.Resolver.Register(list[i]);
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
            if (HostEnvironment.Options.Mode != EngineMode.Compiled)
            {
                HostEnvironment.Options.Mode = EngineMode.Compiled;
                Initialize();
            }
            return this;
        }

        /// <inheritdoc />
        public IEngine UseInterpretationEngine()
        {
            if (HostEnvironment.Options.Mode != EngineMode.Interpreted)
            {
                HostEnvironment.Options.Mode = EngineMode.Interpreted;
                Initialize();
            }
            return this;
        }

        /// <summary>
        /// Clear compiled object or cache.
        /// </summary>
        public void Clean()
        {
            HostEnvironment.Cache?.Clear();
        }

        /// <inheritdoc />
        public void Reset()
        {
            Clean();
        }

        /// <summary>
        /// Register an new excute method.
        /// </summary>
        /// <param name="visitor"></param>
        public void Register(ITagVisitor visitor)
        {
            HostEnvironment.Resolver.Insert(0, visitor);
        }

        /// <inheritdoc/>  
        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }
        /// <inheritdoc/>  
        private void Dispose(bool disposing)
        {
            if (disposing)
                Clean();
            HostEnvironment.ResourceManager.Dispose();
        }

        /// <summary>
        /// 
        /// </summary>
        ~TemplatingEngine()
        {
            this.Dispose(false);
        }
        #endregion
    }
}