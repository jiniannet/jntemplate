/********************************************************************************
 Copyright (c) jiniannet (http://www.jiniannet.com). All rights reserved.
 Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.
 ********************************************************************************/

using System;
using System.Text;
using System.Collections.Generic;
using JinianNet.JNTemplate.Parser;
using JinianNet.JNTemplate.Configuration;
using System.Collections.ObjectModel;

namespace JinianNet.JNTemplate
{
    /// <summary>
    /// 引擎入口
    /// </summary>
    public class Engine
    {

        private static TemplateContext _context;
        //private static Collection<String> _resourceDirectories;
        private static Parser.ITagTypeResolver _tagResolver;
        private static EngineConfig _config;

        static Engine()
        {
            Configure(EngineConfig.CreateDefault());
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
            if (String.IsNullOrEmpty(conf.TagSuffix))
            {
                throw new ArgumentNullException("conf.TagSuffix");
            }
            if (String.IsNullOrEmpty(conf.TagPrefix))
            {
                throw new ArgumentNullException("conf.TagPrefix");
            }
            if (conf.TagParsers == null)
            {
                conf.TagParsers = new String[0];
            }
            if (conf.ResourceDirectories == null)
            {
                conf.ResourceDirectories = new String[0];
            }
            _context = ctx;
            _config = conf;

            ITagParser[] parsers = new ITagParser[conf.TagParsers.Length];

            for (Int32 i = 0; i < conf.TagParsers.Length; i++)
            {
                parsers[i] = (ITagParser)System.Activator.CreateInstance(Type.GetType(conf.TagParsers[i])); ;
            }

            _tagResolver = new Parser.TagTypeResolver(parsers);

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
        /// 资源目录
        /// </summary>
        /// <returns></returns>
        public static String[] ResourceDirectories
        {
            get { return _config.ResourceDirectories;}
        }

        /// <summary>
        /// 简写标签标记
        /// </summary>
        public static Char TagFlag
        {
            get { return _config.TagFlag; }
        }
        ///// <summary>
        ///// 缓存
        ///// </summary>
        //public static JinianNet.JNTemplate.Caching.ICache Cache
        //{
        //    get { return _config.CachingProvider; }
        //}

        /// <summary>
        /// 标签类型解析器
        /// </summary>
        public static Parser.ITagTypeResolver TagResolver
        {
            get { return _tagResolver; }
        }


        /// <summary>
        /// 完整标签前缀
        /// </summary>
        internal static string TagPrefix
        {
            get { return _config.TagPrefix; }
        }

        /// <summary>
        /// 完整标签后缀
        /// </summary>
        internal static string TagSuffix
        {
            get { return _config.TagSuffix; }
        }

        public static TemplateContext CreateContext()
        {
            TemplateContext ctx;
            if (_context == null)
            {
                ctx = new TemplateContext();
                ctx.Charset = Encoding.Default;
                ctx.StripWhiteSpace = _config.StripWhiteSpace;
                ctx.ThrowExceptions = true;
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
            return new Template(CreateContext(),text);
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

            if (!String.IsNullOrEmpty(path))
            {
                String fullPath = path;
                //判断是否本地路径的绝对形式
                if (fullPath.IndexOf(System.IO.Path.VolumeSeparatorChar) != -1 //win下判断是否包含卷分隔符（即：） c:\user\Administrator\default.html
                    || fullPath[0] == '/') //liunx判断是否为/开头，/usr/share/doc/default.html  win下请不要使用/开头的路径
                {
                    ctx.CurrentPath = System.IO.Path.GetDirectoryName(fullPath);
                    template.TemplateContent = Resources.Load(fullPath, template.Context.Charset);
                }
                else
                {
                    Int32 index = Resources.FindPath(path, out fullPath); //如果是相对路径，则进行路径搜索
                    if (index != -1)
                    {
                        //设定当前工作目录 如果模板中存在Inclub或load标签，它们的处理路径会以CurrentPath 设定的路径为基准
                        ctx.CurrentPath = ResourceDirectories[index];
                        template.TemplateContent = Resources.Load(fullPath, template.Context.Charset);
                    }
                }
            }

            return template;
        }
    }
}