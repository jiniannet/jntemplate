/********************************************************************************
 Copyright (c) jiniannet (http://www.jiniannet.com). All rights reserved.
 Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.
 ********************************************************************************/
using System;
using System.Text;
using JinianNet.JNTemplate.Parser;
using JinianNet.JNTemplate.Configuration;
using System.Reflection;
using JinianNet.JNTemplate.Caching;

namespace JinianNet.JNTemplate
{
    /// <summary>
    /// 引擎入口
    /// </summary>
    public class Engine
    {
        private static Runtime _engineRuntime;
        private static Object _lockObject = new Object();
        private static TemplateContext _context;

        /// <summary>
        /// 引擎运行时
        /// </summary>
        public static Runtime Runtime
        {
            get
            {
                if (_engineRuntime == null)
                {
                    lock (_lockObject)
                    {
                        if (_engineRuntime == null)
                        {
                            Configure(EngineConfig.CreateDefault());
                        }
                    }
                }
                return _engineRuntime;
            }
        }

        /// <summary>
        /// 引擎配置
        /// </summary>
        /// <param name="conf">配置内容</param>
        /// <param name="ctx">默认模板上下文</param>
        public static void Configure(EngineConfig conf, TemplateContext ctx)
        {
            if (conf == null)
            {
                throw new ArgumentNullException("conf");
            }

            if (conf.TagParsers == null)
            {
                conf.TagParsers = conf.TagParsers = Field.RSEOLVER_TYPES;
            }
            _context = ctx;
            ITagParser[] parsers = new ITagParser[conf.TagParsers.Length];

            for (Int32 i = 0; i < conf.TagParsers.Length; i++)
            {
                parsers[i] = (ITagParser)Activator.CreateInstance(Type.GetType(conf.TagParsers[i]));
            }

            ICache cache = null;
            if (!String.IsNullOrEmpty(conf.CachingProvider))
            {
                cache = (ICache)Activator.CreateInstance(Type.GetType(conf.CachingProvider));
            }

            Parser.TagTypeResolver resolver = new Parser.TagTypeResolver(parsers);
            if(_engineRuntime!=null&& _engineRuntime.Cache != null)
            {
                _engineRuntime.Cache.Dispose();
            }
            _engineRuntime = new Runtime(resolver,
                cache,
                conf);
        }

        /// <summary>
        /// 引擎配置
        /// </summary>
        /// <param name="conf">配置内容</param>
        public static void Configure(EngineConfig conf)
        {
            Configure(conf, null);
        }

        /// <summary>
        /// 创建模板上下文
        /// </summary>
        /// <returns></returns>
        public static TemplateContext CreateContext()
        {
            TemplateContext ctx;
            if (_context == null)
            {
                ctx = new TemplateContext();
                ctx.Charset = Encoding.Default;
                ctx.StripWhiteSpace = Runtime.StripWhiteSpace;
                ctx.ThrowExceptions = Runtime.ThrowExceptions;
            }
            else
            {
                ctx = TemplateContext.CreateContext(_context);
            }

            return ctx;
        }

        /// <summary>
        /// 根据指定路模板建Template实例
        /// </summary>
        /// <param name="text">文本</param>
        /// <returns></returns>
        public static ITemplate CreateTemplate(String text)
        {
            return new Template(CreateContext(), text);
        }

        /// <summary>
        /// 从指定路径加载模板
        /// </summary>
        /// <param name="path">模板文件</param>
        /// <returns>ITemplate</returns>
        public static ITemplate LoadTemplate(String path)
        {
            return LoadTemplate(path, CreateContext());
        }

        /// <summary>
        /// 从指定路径加载模板
        /// </summary>
        /// <param name="path">模板文件</param>
        /// <param name="ctx">模板上下文</param>
        /// <returns>ITemplate</returns>
        public static ITemplate LoadTemplate(String path, TemplateContext ctx)
        {
            Template template = new Template(ctx, null);
            String fullPath;
            template.TemplateContent = Resources.Load(Runtime.ResourceDirectories, path, template.Context.Charset, out fullPath);
            if (fullPath != null)
            {
                template.TemplateKey = fullPath;
                ctx.CurrentPath = System.IO.Path.GetDirectoryName(fullPath);
            }
            return template;
        }
    }
}