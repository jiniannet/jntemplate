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
        private static EngineInfo _instance;

        /// <summary>
        /// 对象实例
        /// </summary>

        internal static EngineInfo Instance
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
        /// <param name="loder"></param>
        public static void SetLoder(ILoader loder)
        {
            Instance.Loder = loder;
        }
        /// <summary>
        /// 解析器
        /// </summary>
        public List<ITagParser> Parsers
        {
            get
            {
                return Instance.Parsers;
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
            if (conf.TagParsers == null || conf.TagParsers.Length == 0)
            {
                conf.TagParsers = Field.RSEOLVER_TYPES;
            }
            Configure(conf.ToDictionary(), scope, LoadParsers(conf.TagParsers), new FileLoader());
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
            VariableScope scope, ITagParser[] parsers, ILoader loader)
        {
            if (conf == null)
            {
                throw new ArgumentNullException("\"conf\" cannot be null.");
            }
            _instance = new EngineInfo();
            _instance.Data = scope;
            InitializationEnvironment(conf); 
            if (parsers == null || parsers.Length == 0)
            {
                parsers = LoadParsers(Field.RSEOLVER_TYPES);
            }
            for (Int32 i = 0; i < parsers.Length; i++)
            {
                if (parsers[i] != null)
                {
                    Instance.Parsers.Add(parsers[i]);
                }
            }

            SetLoder(loader ?? new FileLoader());
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
            if (Instance.Data == null)
            {
                return new TemplateContext();
            }
            return new TemplateContext(Instance.Data);
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
            ResourceInfo info = Instance.Loder.Load(path, ctx.Charset);
            Template template;

            if (info != null)
            {
                template = new Template(ctx, info.Content);
                template.TemplateKey = info.FullPath;
                ctx.CurrentPath = Instance.Loder.GetDirectoryName(info.FullPath);
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
            for (Int32 i = 0; i < Instance.Parsers.Count; i++)
            {
                if (Instance.Parsers[i] == null)
                {
                    continue;
                }
                t = Instance.Parsers[i].Parse(parser, tc);
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

            if (Instance.EnvironmentVariable.TryGetValue(variable, out value))
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
                Instance.EnvironmentVariable.Remove(variable);
            }
            else
            {
                Instance.EnvironmentVariable[variable] = value;
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
                list[i] = ((ITagParser)Activator.CreateInstance(Type.GetType(arr[i])));
            }
            return list;
        }
        #endregion
    }
}