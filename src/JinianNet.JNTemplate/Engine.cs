/********************************************************************************
 Copyright (c) jiniannet (http://www.jiniannet.com). All rights reserved.
 Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.
 ********************************************************************************/
using System;
using JinianNet.JNTemplate.Parser;
using JinianNet.JNTemplate.Configuration;
using System.Reflection;
using System.Collections.Generic;
using JinianNet.JNTemplate.Node;

namespace JinianNet.JNTemplate
{
    /// <summary>
    /// 引擎入口
    /// </summary>
    public class Engine
    {
        #region 私有变量
        /// <summary>
        /// 实例
        /// </summary>
        private static RuntimeInfo _instance;

        /// <summary>
        /// 对象实例
        /// </summary>

        public static RuntimeInfo Runtime
        {
            get
            {
                if (_instance == null)
                {
                    Configure(Configuration.EngineConfig.CreateDefault());
                }
                return _instance;
            }
        }

        #endregion

        #region 属性 
        /// <summary>
        /// 配置加载器
        /// </summary>
        /// <param name="loder">loder实列</param>
        public static void SetLodeProvider(ILoader loder)
        {
            Runtime.Loder = loder;
        }
        /// <summary>
        /// 配置缓存提供器
        /// </summary>
        /// <param name="cache">缓存提供器实例 </param>
        public static void SetCachingProvider(Caching.ICache cache)
        {
            Runtime.Cache = cache;
        }
        /// <summary>
        /// 配置动态执行提供器
        /// </summary>
        /// <param name="proxy">ICallProxy实例 </param>
        public static void SetCallProvider(ICallProxy proxy)
        {
            Runtime.DynamicCallProxy = proxy;
        }
        /// <summary>
        /// 解析器
        /// </summary>
        public List<ITagParser> Parsers
        {
            get
            {
                return Runtime.Parsers;
            }
        }
        #endregion

        #region 引擎配置
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
            _instance = new RuntimeInfo();
            _instance.Data = scope;
            SetEnvironmentVariable("Charset",conf.Charset);
            SetEnvironmentVariable("TagPrefix",conf.TagPrefix);
            SetEnvironmentVariable("TagSuffix",conf.TagSuffix);
            SetEnvironmentVariable("TagFlag",conf.TagFlag.ToString());
            SetEnvironmentVariable("ThrowExceptions",conf.ThrowExceptions.ToString());
            SetEnvironmentVariable("StripWhiteSpace",conf.StripWhiteSpace.ToString());
            SetEnvironmentVariable("IgnoreCase",conf.IgnoreCase.ToString());
            if (conf.TagParsers == null || conf.TagParsers.Length == 0)
            {
                conf.TagParsers = LoadParsers(Field.RSEOLVER_TYPES);
            }
            for (Int32 i = 0; i < conf.TagParsers.Length; i++)
            {
                if (conf.TagParsers[i] != null)
                {
                    Runtime.Parsers.Add(conf.TagParsers[i]);
                }
            }
            if (conf.IgnoreCase)
            {
                Runtime.BindIgnoreCase = BindingFlags.IgnoreCase;
                Runtime.ComparerIgnoreCase = StringComparer.OrdinalIgnoreCase;
                Runtime.ComparisonIgnoreCase = StringComparison.OrdinalIgnoreCase;
            }
            else
            {
                Runtime.ComparisonIgnoreCase = StringComparison.Ordinal;
                Runtime.BindIgnoreCase = BindingFlags.DeclaredOnly;
                Runtime.ComparerIgnoreCase = StringComparer.Ordinal;
            }
            _instance.Cache = conf.CachingProvider;
            SetLodeProvider(conf.LoadProvider ?? new FileLoader());
            SetCallProvider(conf.CallProvider ?? new ReflectionCallProxy());
            //Configure(conf.ToDictionary(), scope, conf.TagParsers, conf.LoadProvider, conf.ExecuteProvider);
        }

        /// <summary>
        /// 引擎配置
        /// </summary>
        /// <param name="conf">基本配置字典</param> 
        /// <param name="scope">全局数据，可空</param>
        /// <param name="parsers">解析器，可空</param>
        /// <param name="loader">加载器，可空</param>
        /// <param name="executor">执行器，可空</param>
        public static void Configure(
            IDictionary<String, String> conf,
            VariableScope scope,
            ITagParser[] parsers,
            ILoader loader,
            ICallProxy executor)
        {
            if (conf == null)
            {
                throw new ArgumentNullException("\"conf\" cannot be null.");
            }
           
        }

        /// <summary>
        /// 引擎配置
        /// </summary>
        /// <param name="conf">配置内容</param>
        public static void Configure(ConfigBase conf)
        {
            Configure(conf, null);
        }


        #endregion

        #region 对外方法
        /// <summary>
        /// 创建模板上下文
        /// </summary>
        /// <returns></returns>
        public static TemplateContext CreateContext()
        {
            if (Runtime.Data == null)
            {
                return new TemplateContext();
            }
            return new TemplateContext(Runtime.Data);
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
            ResourceInfo info = Runtime.Load(path, ctx.Charset);
            Template template;

            if (info != null)
            {
                template = new Template(ctx, info.Content);
                template.TemplateKey = info.FullPath;
                ctx.CurrentPath = Runtime.GetDirectoryName(info.FullPath);
            }
            else
            {
                template = new Template(ctx, String.Empty);
            }
            return template;
        }


        /// <summary>
        /// 分析标签
        /// </summary>
        /// <param name="parser">模板解析器</param>
        /// <param name="tc">TOKEN集合</param>
        /// <returns></returns>
        public static Node.Tag Resolve(TemplateParser parser, TokenCollection tc)
        {

            Tag t;
            for (Int32 i = 0; i < Runtime.Parsers.Count; i++)
            {
                if (Runtime.Parsers[i] == null)
                {
                    continue;
                }
                t = Runtime.Parsers[i].Parse(parser, tc);
                if (t != null)
                {
                    t.FirstToken = tc.First;

                    if (t.Children.Count == 0
                        || (t.LastToken = t.Children[t.Children.Count - 1].LastToken ?? t.Children[t.Children.Count - 1].FirstToken) == null
                        || tc.Last.CompareTo(t.LastToken) > 0)
                    {
                        t.LastToken = tc.Last;
                    }
                    return t;
                }
            }
            return null;
        }

        #endregion
        #region 环境变量

        /// <summary>
        /// 获取环境变量
        /// </summary>
        /// <param name="variable">变量名称</param>
        /// <returns></returns>
        public static String GetEnvironmentVariable(String variable)
        {
            String value;

            if (Runtime.EnvironmentVariable.TryGetValue(variable, out value))
            {
                return value;
            }
            return null;
        }


        /// <summary>
        /// 设置环境变量
        /// </summary>
        /// <param name="variable">变量名</param>
        /// <param name="value">值</param>
        public static void SetEnvironmentVariable(String variable, String value)
        {
            if (variable == null)
            {
                throw new ArgumentNullException("\"variable\" cannot be null.");
            }
            if (value == null)
            {
                Runtime.EnvironmentVariable.Remove(variable);
            }
            else
            {
                Runtime.EnvironmentVariable[variable] = value;
            }
        }

        #endregion

        #region 私有方法
        /// <summary>
        /// 初始化环境变量配置
        /// </summary>
        /// <param name="conf">配置内容</param>
        private static void InitializationEnvironment(IDictionary<String, String> conf)
        {
            foreach (KeyValuePair<String, String> node in conf)
            {
                SetEnvironmentVariable(node.Key, node.Value);
            }
        }

        /// <summary>
        /// 加载标签分析器
        /// </summary>
        /// <param name="arr">解析器类型</param>
        private static ITagParser[] LoadParsers(string[] arr)
        {
            ITagParser[] list = new ITagParser[arr.Length];
            for (Int32 i = 0; i < arr.Length; i++)
            {
                list[i] = CreateInstance<ITagParser>(Type.GetType(arr[i]));
            }
            return list;
        }

        private static T CreateInstance<T>(Type type)
        {
            return (T)Activator.CreateInstance(type ?? typeof(T));
        }

        #endregion
    }
}