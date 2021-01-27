/********************************************************************************
 Copyright (c) jiniannet (http://www.jiniannet.com). All rights reserved.
 Licensed under the MIT license. See licence.txt file in the project root for full license information.
 ********************************************************************************/
using System;
using JinianNet.JNTemplate.Configuration;
using JinianNet.JNTemplate.Resources;
using JinianNet.JNTemplate.Compile;
using JinianNet.JNTemplate.Dynamic;
#if !NET20
using System.Threading.Tasks;
#endif

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
            get { return Runtime.Store.EnableCompile; }
            set { Runtime.Store.EnableCompile = value; }
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
            Runtime.Configure(conf, scope);
        } 

        /// <summary>
        /// 预编译模板
        /// </summary>
        /// <param name="name">模板名称 必须唯一，建议使用模板文件绝对路径</param>
        /// <param name="fileName">模板路径</param>
        /// <param name="action">ACTION</param>
        /// <returns></returns>
        public static ICompileTemplate Precompiled(string name, string fileName, Action<CompileContext> action = null)
        {
            if (string.IsNullOrEmpty(name))
            {
                throw new System.ArgumentNullException("name cannot be null.");
            }
            return Runtime.Templates[name] = Compiler.CompileFile(name, fileName, action);
        }

        /// <summary>
        /// 预编译模板
        /// </summary> 
        /// <param name="fs">模板文件</param>
        /// <param name="action">action</param>
        /// <returns></returns>
        public static void Precompiled(System.IO.FileInfo[] fs, Action<CompileContext> action = null)
        {
            foreach (var f in fs)
            {
                Runtime.Templates[f.FullName] = Compiler.CompileFile(f.FullName, f.FullName, action);
            }
        }

        /// <summary>
        /// 创建模板上下文
        /// </summary>
        /// <returns></returns>
        public static TemplateContext CreateContext()
        {
            var data = new VariableScope(Runtime.Data);
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
            ITemplate template;
            if (EnableCompile)
            {
                template = new CompileTemplate(CreateContext(), text);
            }
            else
            {
                template = new Template(CreateContext(), text);
            }
            if (string.IsNullOrEmpty(name))
            {
                name = text.GetHashCode().ToString();
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
                return LoadTemplate<CompileTemplate>(name, fileName, CreateContext());
            }
            return LoadTemplate<Template>(name, fileName, CreateContext());
        }

        /// <summary>
        /// 从指定路径加载模板
        /// </summary>
        /// <param name="name">模板名称 必须唯一</param>
        /// <param name="fileName">模板文件</param>
        /// <param name="ctx">模板上下文</param>
        /// <returns>ITemplate</returns>
        public static T LoadTemplate<T>(string name, string fileName, TemplateContext ctx)
            where T : ITemplate, new()
        {
            T template = new T();
            template.Context = ctx;
            template.Path = ctx.FindFullPath(fileName);
            template.TemplateKey = name;
            if (string.IsNullOrWhiteSpace(template.Path))
            {
                throw new Exception.TemplateException($"Path:\"{fileName}\", the file could not be found.");
            } 
            if (string.IsNullOrWhiteSpace(template.TemplateKey))
            {
                template.TemplateKey = template.Path;
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
            Executor.Register<T>(func);
        } 
    }
}