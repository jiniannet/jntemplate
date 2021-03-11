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
    /// 引擎入口
    /// </summary>
    public sealed class Engine
    {

        /// <summary>
        /// Version
        /// </summary>
        public static string Version => Field.Version;
        /// <summary>
        /// 是否启用编译模式
        /// </summary>
        public static bool EnableCompile
        {
            get { return Runtime.Storage.EnableCompile; }
            set { Runtime.Storage.EnableCompile = value; }
        }

        /// <summary>
        /// 引擎配置
        /// </summary>
        /// <param name="action">配置内容</param>
        public static void Configure(Action<IConfig> action)
        {
            var conf = Configuration.EngineConfig.CreateDefault();
            action?.Invoke(conf);
            Configure(conf);
        }

        /// <summary>
        /// 引擎配置
        /// </summary>
        /// <param name="action">配置内容</param>
        public static void Configure(Action<IConfig, VariableScope> action)
        {
            var conf = Configuration.EngineConfig.CreateDefault();
            var score = new VariableScope(null);
            action?.Invoke(conf, score);
            Configure(conf, score);
        }

        /// <summary>
        /// 引擎配置
        /// </summary>
        /// <param name="conf">配置内容</param>
        public static void Configure(IConfig conf)
        {
            Configure(conf, null);
        }

        /// <summary>
        /// 引擎配置
        /// </summary>
        /// <param name="conf">配置内容</param>
        /// <param name="scope">初始数据</param>
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
        /// 引擎配置
        /// </summary>
        /// <param name="conf">配置内容</param>
        /// <param name="scope">初始数据</param>
        public static void Configure(IDictionary<string, string> conf, VariableScope scope)
        {
            Runtime.Configure(conf, scope);
        }

        /// <summary>
        /// 预编译模板
        /// </summary>
        /// <param name="name">模板名称 必须唯一，建议使用模板文件绝对路径</param>
        /// <param name="fileName">模板路径</param>
        /// <param name="action">ACTION</param>
        /// <returns></returns>
        public static ICompileTemplate CompileFile(string name, string fileName, Action<CompileContext> action = null)
        {
            var res = Runtime.Loader.Load(fileName, Runtime.Encoding, Runtime.ResourceDirectories.ToArray());
            if (res == null)
            {
                throw new Exception.TemplateException($"Path:\"{fileName}\", the file could not be found.");
            }

            if (string.IsNullOrEmpty(name))
            {
                name = res.FullPath;
            }
            return Runtime.Templates[name] = Compiler.Compile(name, res.Content, action);
        }


        /// <summary>
        /// 编译并执行模板
        /// </summary>
        /// <param name="fileName">模板路径</param>
        /// <param name="ctx">TemplateContext</param>
        /// <returns></returns>
        public static string CompileFileAndExec(string fileName, TemplateContext ctx)
        {
            var full = ctx.FindFullPath(fileName);
            if (full == null)
            {
                throw new Exception.TemplateException($"\"{ fileName }\" cannot be found.");
            }
            var template = Runtime.Templates[full];
            if (template == null)
            {
                template = CompileFile(full, full, (c) => ctx.CopyTo(c));
                if (template == null)
                {
                    throw new Exception.TemplateException($"\"{ fileName }\" compile error.");
                }
            }
            using (var sw = new System.IO.StringWriter())
            {
                template.Render(sw, ctx);
                return sw.ToString();
            }
        }

        /// <summary>
        /// 预编译模板
        /// </summary>
        /// <param name="name">模板名称 必须唯一，建议使用模板文件绝对路径</param>
        /// <param name="content">模板内容 </param>
        /// <param name="action">ACTION</param>
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
        /// 预编译模板
        /// </summary> 
        /// <param name="fs">模板文件</param>
        /// <param name="action">action</param>
        /// <returns></returns>
        public static void CompileFile(System.IO.FileInfo[] fs, Action<CompileContext> action = null)
        {
            foreach (var f in fs)
            {
                CompileFile(f.FullName, f.FullName, action);
            }
        }

        /// <summary>
        /// 创建模板上下文
        /// </summary>
        /// <returns></returns>
        public static TemplateContext CreateContext()
        {
            var data = new VariableScope();
            TemplateContext ctx = new TemplateContext(data);
            if (Runtime.ResourceDirectories != null && Runtime.ResourceDirectories.Count > 0)
            {
                ctx.ResourceDirectories.AddRange(Runtime.ResourceDirectories);
            }
            return ctx;
        }


        /// <summary>
        /// 从指定模板内容创建Template实例
        /// </summary>
        /// <param name="text">文本</param>
        /// <returns></returns>
        public static ITemplate CreateTemplate(string text)
        {
            return CreateTemplate(null, text);
        }

        /// <summary>
        /// 从指定模板内容创建Template实例
        /// </summary>
        /// <param name="name">模板名称 必须唯一</param>
        /// <param name="text">文本</param>
        /// <returns></returns>
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
        /// 从指定路径加载模板
        /// </summary>
        /// <param name="fileName">模板文件</param>
        /// <returns>ITemplate</returns>
        public static ITemplate LoadTemplate(string fileName)
        {
            return LoadTemplate(null, fileName);
        }


        /// <summary>
        /// 从指定路径加载模板
        /// </summary>
        /// <param name="name">模板名称 必须唯一</param>
        /// <param name="fileName">模板文件</param>
        /// <returns>ITemplate</returns>
        public static ITemplate LoadTemplate(string name, string fileName)
        {
            if (EnableCompile)
            {
                return LoadCompileTemplate(name, fileName);
            }

            var ctx = CreateContext();
            var res = ctx.Load(fileName);
            if (res == null)
            {
                throw new Exception.TemplateException($"Path:\"{fileName}\", the file could not be found.");
            }

            var t = LoadTemplate<Template>(name, fileName, CreateContext());
            t.TemplateContent = res.Content;
            return t;
        }

        /// <summary>
        /// 从指定路径加载模板
        /// </summary>
        /// <param name="name">模板名称 必须唯一</param>
        /// <param name="fileName">模板文件</param> 
        /// <returns>ITemplate</returns>
        private static ITemplate LoadCompileTemplate(string name, string fileName)
        {
            var ctx = CreateContext();
            if (string.IsNullOrEmpty(name))
            {
                name = ctx.FindFullPath(fileName);
                if (string.IsNullOrEmpty(name))
                {
                    throw new Exception.TemplateException($"Path:\"{fileName}\", the file could not be found.");
                }
            }
            var template = LoadTemplate<CompileTemplate>(name, fileName, ctx);
            if (Runtime.Templates.Keys.Contains(name))
            {
                return template;
            }
            var res = ctx.Load(fileName);
            if (res == null)
            {
                throw new Exception.TemplateException($"Path:\"{fileName}\", the file could not be found.");
            }
            template.TemplateContent = res.Content;
            return template;
        }

        /// <summary>
        /// 从指定路径加载模板
        /// </summary>
        /// <param name="name">模板名称 必须唯一</param>
        /// <param name="fileName">模板文件</param>
        /// <param name="ctx">模板上下文</param>
        /// <returns>ITemplate</returns>
        private static T LoadTemplate<T>(string name, string fileName, TemplateContext ctx)
            where T : ITemplate, new()
        {
            T template = new T();
            template.Context = ctx;
            template.TemplateKey = name;
            if (string.IsNullOrEmpty(template.TemplateKey))
            {
                template.TemplateKey = fileName;
            }
            if (template.Context != null && string.IsNullOrEmpty(template.Context.CurrentPath))
            {
                template.Context.CurrentPath = Runtime.Loader.GetDirectoryName(fileName);
            }
            return template;
        }

        /// <summary>
        /// Register tag
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="parser"></param>
        /// <param name="compileFunc"></param>
        /// <param name="guessFunc"></param>
        public static void Register<T>(Parsers.ITagParser parser,
            Func<Nodes.ITag, CompileContext, System.Reflection.MethodInfo> compileFunc,
            Func<Nodes.ITag, CompileContext, Type> guessFunc) where T : Nodes.ITag
        {
            Runtime.RegisterTagParser(parser, 0);
            Compiler.Register<T>(compileFunc, guessFunc);
        }


        /// <summary>
        /// Register tag
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="parser"></param>
        /// <param name="func"></param> 
        public static void Register<T>(Parsers.ITagParser parser,
            Func<Nodes.ITag, TemplateContext, object> func) where T : Nodes.ITag
        {
            Runtime.RegisterTagParser(parser, 0);
            TagExecutor.Register<T>(func);
        }
    }
}