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
using System.Collections.Generic;
using System.Collections;

namespace JinianNet.JNTemplate
{
    /// <summary>
    /// 引擎入口
    /// </summary>
    public class Engine
    {
        private static Dictionary<String, String> _variable;
        private static String[] _resourceDirectories;
        private static Object _lockObject = new Object();
        private static VariableScope _scope;
        private static ITagTypeResolver _tagResolver;
        private static StringComparison _stringComparison;
        private static BindingFlags _bindingFlags;
        private static StringComparer _stringComparer;
        private static ICache _cache;

        /// <summary>
        /// 引擎配置
        /// </summary>
        /// <param name="conf">配置内容</param>
        /// <param name="scope">初始化全局数据</param>
        public static void Configure(ConfigBase conf, VariableScope scope)
        {
            if (conf == null)
            {
                throw new ArgumentNullException("\"conf\" cannot be null.");
            }
            Configure(conf.ToDictionary(),
                conf.ResourceDirectories,
                conf.TagParsers, scope);
        }

        /// <summary>
        /// 引擎配置
        /// </summary>
        /// <param name="directories">模板目录，默认为当前程序目录</param>
        /// <param name="parsers">解析器，可空</param>
        /// <param name="conf">配置参数</param>
        /// <param name="scope">全局数据，可空</param>
        public static void Configure(
            IDictionary<String, String> conf,
            String[] directories,
            String[] parsers,
            VariableScope scope)
        {
            if (conf == null)
            {
                throw new ArgumentNullException("\"conf\" cannot be null.");
            }

            _scope = scope;

            InitializationEnvironment(conf);

            if (_cache != null)
            {
                _cache.Dispose();
            }
            _cache = null;
            if (!String.IsNullOrEmpty(GetEnvironmentVariable("CachingProvider")))
            {
                Type cacheType = Type.GetType(GetEnvironmentVariable("CachingProvider"));
                _cache = (ICache)Activator.CreateInstance(cacheType);
            }


            if (Common.Utility.ToBoolean(GetEnvironmentVariable("IgnoreCase")))
            {
                _bindingFlags = BindingFlags.IgnoreCase;
                _stringComparer = StringComparer.OrdinalIgnoreCase;
                _stringComparison = StringComparison.OrdinalIgnoreCase;
            }
            else
            {
                _stringComparison = StringComparison.Ordinal;
                _bindingFlags = BindingFlags.Default;
                _stringComparer = StringComparer.Ordinal;
            }
            if (directories == null)
            {
                _resourceDirectories = new String[] {
                    Environment.CurrentDirectory
                };
            }
            else
            {
                _resourceDirectories = directories;
            }
            InitializationParser(parsers);
        }

        /// <summary>
        /// 初始化环境变量配置
        /// </summary>
        /// <param name="conf">配置内容</param>
        private static void InitializationEnvironment(IDictionary<String, String> conf)
        {
            _variable = new Dictionary<String, String>(StringComparer.OrdinalIgnoreCase);
            foreach (KeyValuePair<String, String> node in conf)
            {
                SetEnvironmentVariable(node.Key, node.Value);
            }
        }

        /// <summary>
        /// 初始化标签分析器
        /// </summary>
        /// <param name="parsers">解析器类型</param>
        private static void InitializationParser(String[] parsers)
        {
            if (parsers == null)
            {
                parsers = Field.RSEOLVER_TYPES;
            }
            ITagParser[] tps = new ITagParser[parsers.Length];

            for (Int32 i = 0; i < tps.Length; i++)
            {
                tps[i] = (ITagParser)Activator.CreateInstance(Type.GetType(parsers[i]));
            }
            _tagResolver = new Parser.TagTypeResolver(tps);
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
            if (_scope == null)
            {
                return new TemplateContext();
            }
            return new TemplateContext(_scope);
        }

        /// <summary>
        /// 从指定模板内容创建Template实例
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

            String fullPath;
            String text = Resources.Load(ResourceDirectories, path, ctx.Charset, out fullPath);
            Template template = new Template(ctx, text);
            if (fullPath != null)
            {
                template.TemplateKey = fullPath;
                ctx.CurrentPath = System.IO.Path.GetDirectoryName(fullPath);
            }
            return template;
        }

        /// <summary>
        /// 缓存
        /// </summary>
        public static ICache Cache
        {
            get { return _cache; }
        }

        /// <summary>
        /// 标签类型解析器
        /// </summary>
        public static Parser.ITagTypeResolver TagResolver
        {
            get { return _tagResolver; }
        }

        /// <summary>
        /// 字符串大小写排序配置
        /// </summary>
        internal static StringComparison ComparisonIgnoreCase
        {
            get { return _stringComparison; }
        }

        /// <summary>
        /// 绑定大小写配置
        /// </summary>
        internal static BindingFlags BindIgnoreCase
        {
            get { return _bindingFlags; }
        }

        /// <summary>
        /// 字符串大小写比较配置
        /// </summary>
        internal static StringComparer ComparerIgnoreCase
        {
            get { return _stringComparer; }
        }
        /// <summary>
        /// 资源路径
        /// </summary>
        public static String[] ResourceDirectories
        {
            get { return _resourceDirectories; }
        }

        /// <summary>
        /// 获取环境变量
        /// </summary>
        /// <param name="variable">变量名称</param>
        /// <returns></returns>
        public static String GetEnvironmentVariable(String variable)
        {
            String value;
            Dictionary<String, String> dic = (Dictionary<String, String>)GetEnvironmentVariables();

            if (dic.TryGetValue(variable, out value))
            {
                return value;
            }
            return null;
        }

        /// <summary>
        /// 获取所有环境变量
        /// </summary>
        /// <returns></returns>
        public static IDictionary GetEnvironmentVariables()
        {
            if (_variable == null)
            {
                lock (_lockObject)
                {
                    if (_variable == null)
                    {
                        Configure(EngineConfig.CreateDefault());
                    }
                }
            }
            return _variable;
        }

        /// <summary>
        /// 设置环境变量
        /// </summary>
        /// <param name="variable">变量名</param>
        /// <param name="value">值</param>
        public static void SetEnvironmentVariable(String variable, String value)
        {
            GetEnvironmentVariables()[variable] = value;
        }
    }
}