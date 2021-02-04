/********************************************************************************
 Copyright (c) jiniannet (http://www.jiniannet.com). All rights reserved.
 Licensed under the MIT license. See licence.txt file in the project root for full license information.
 ********************************************************************************/
using JinianNet.JNTemplate.Caching;
using JinianNet.JNTemplate.Configuration;
using JinianNet.JNTemplate.Dynamic;
using JinianNet.JNTemplate.Nodes;
using JinianNet.JNTemplate.Parsers;
using JinianNet.JNTemplate.Resources;
using System;
using System.Collections.Generic;
using System.Reflection;
using System.Text;
#if !NET20
using System.Threading.Tasks;
#endif

namespace JinianNet.JNTemplate
{
    /// <summary>
    /// 提供运行时的通用方法与属性
    /// </summary>
    public sealed class Runtime
    {
        private static RuntimeStore store;
        private static volatile object state;

        /// <summary>
        /// config store
        /// </summary>
        internal static RuntimeStore Store
        {
            get
            {
                if (store == null)
                {
                    lock (state)
                    {
                        store = RuntimeStore.CreateStore();
                    }
                }
                return store;
            }
        }

        /// <summary>
        /// ctor
        /// </summary>
        static Runtime()
        {
            state = new object();
        }

        /// <summary>
        /// 引擎配置
        /// </summary>
        /// <param name="conf">配置内容</param>
        /// <param name="scope">初始数据</param>
        internal static void Configure(IDictionary<string, string> conf, VariableScope scope)
        {
            Store.Data = (scope == null || scope.Count == 0) ? null : scope;
            Store.Initialization(conf);
        }

        /// <summary>
        /// 模板资源搜索目录
        /// </summary>
        /// <value></value>
        public static List<string> ResourceDirectories => Store.ResourceDirectories;


        /// <summary>
        /// 全局初始数据
        /// </summary>
        public static VariableScope Data => Store.Data;

        /// <summary>
        /// 加载器
        /// </summary>
        public static IResourceLoader Loader => Store.Loader;
        /// <summary>
        /// Default encoding
        /// </summary>
        public static Encoding Encoding => Store.Encoding;

        /// <summary>
        /// Cache
        /// </summary>

        public static ICache Cache => Store.Cache;
        /// <summary>
        /// Actuator
        /// </summary>
        public static IActuator Actuator => Store.Actuator;

        /// <summary>
        /// Compile Templates
        /// </summary>
        public static TemplateCollection<Compile.ICompileTemplate> Templates => Store.Templates;

        /// <summary>
        /// 
        /// </summary>
        /// <param name="loader"></param>
        public static void SetLoader(IResourceLoader loader)
        {
            if (loader == null)
            {
                throw new ArgumentNullException(nameof(loader));
            }
            Runtime.Store.Loader = loader;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="path"></param>
        public static void AppendResourcePath(string path)
        {
            if (!Store.ResourceDirectories.Contains(path))
            {
                Store.ResourceDirectories.Add(path);
            }
        }

        /// <summary>
        /// 获取环境变量
        /// </summary>
        /// <param name="variable">变量名称</param>
        /// <returns></returns>
        public static string GetEnvironmentVariable(string variable)
        {
            string value;

            if (Store.Variable.TryGetValue(variable, out value))
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
        public static void SetEnvironmentVariable(string variable, string value)
        {
            if (variable == null)
            {
                throw new ArgumentNullException("\"variable\" cannot be null.");
            }
            if (value == null)
            {
                Store.Variable.Remove(variable);
            }
            else
            {
                Store.Variable[variable] = value;
            }
        }

        /// <summary>
        /// 装载标签解析器
        /// </summary>
        /// <param name="parser">ITagParser</param>
        /// <param name="index">索引</param>
        public static void RegisterTagParser(ITagParser parser, int index = -1)
        {
            if (Store.Parsers.Contains(parser))
            {
                return;
            }
            if (index < 0)
            {
                Store.Parsers.Add(parser);
            }
            else
            {
                Store.Parsers.IndexOf(parser, index);
            }
        }

        /// <summary>
        /// 分析标签
        /// </summary>
        /// <param name="parser"></param>
        /// <param name="tc"></param>
        /// <returns></returns>
        public static ITag Parsing(TemplateParser parser, TokenCollection tc)
        {
            if (tc == null || tc.Count == 0 || parser == null)
            {
                return null;
            }

            var parsers = Store.Parsers;
            ITag t;
            for (int i = 0; i < parsers.Count; i++)
            {
                if (parsers[i] == null)
                {
                    continue;
                }
                t = parsers[i].Parse(parser, tc);
                if (t != null)
                {
                    t.FirstToken = tc.First;

                    if (t.Children.Count == 0 ||
                        (t.LastToken = t.Children[t.Children.Count - 1].LastToken ?? t.Children[t.Children.Count - 1].FirstToken) == null ||
                        tc.Last.CompareTo(t.LastToken) > 0)
                    {
                        t.LastToken = tc.Last;
                    }
                    return t;
                }
            }
            return null;
        }


        /// <summary>
        /// core
        /// </summary>
        internal class RuntimeStore
        {
            /// <summary>
            /// 
            /// </summary>
            /// <returns></returns>
            internal static RuntimeStore CreateStore()
            {
                RuntimeStore store = new RuntimeStore();
                store.Data = null;
                store.Variable = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase) {
                    { nameof(IConfig.Charset),"utf-8" },
                    { nameof(IConfig.TagPrefix),"${" },
                    { nameof(IConfig.TagSuffix),"}" },
                    { nameof(IConfig.TagFlag),"$" },
                    { nameof(IConfig.ThrowExceptions),"True" },
                    { nameof(IConfig.StripWhiteSpace),"False" },
                    { nameof(IConfig.IgnoreCase),"True" }
                };
                store.Actuator = new ReflectionActuator();
                store.Cache = new MemoryCache();
                store.Parsers = new List<ITagParser>();
                store.ResourceDirectories = new List<string>();
                store.Loader = new FileLoader();
                store.Encoding = Encoding.UTF8;
                store.Templates = new TemplateCollection<Compile.ICompileTemplate>();
                store.BindIgnoreCase = BindingFlags.IgnoreCase;
                store.ComparerIgnoreCase = StringComparer.OrdinalIgnoreCase;
                store.ComparisonIgnoreCase = StringComparison.OrdinalIgnoreCase;
                foreach (var type in Field.RSEOLVER_TYPES)
                {
                    var parser = DynamicHelpers.CreateInstance<ITagParser>(type);
                    store.Parsers.Add(parser);
                }
                return store;
            }

            /// <summary>
            /// 初始化基本数据
            /// </summary>
            /// <param name="dict"></param>
            public void Initialization(IDictionary<string, string> dict)
            {
                foreach (KeyValuePair<string, string> node in dict)
                {
                    if (string.IsNullOrEmpty(node.Value))
                    {
                        continue;
                    }
                    switch (node.Key)
                    {
                        case nameof(IConfig.Charset):
                            this.Encoding = Encoding.GetEncoding(node.Value);
                            break;
                        case nameof(IConfig.IgnoreCase):
                            if (Utility.StringToBoolean(node.Value))
                            {
                                this.BindIgnoreCase = BindingFlags.IgnoreCase;
                                this.ComparerIgnoreCase = StringComparer.OrdinalIgnoreCase;
                                this.ComparisonIgnoreCase = StringComparison.OrdinalIgnoreCase;
                            }
                            else
                            {
                                this.ComparisonIgnoreCase = StringComparison.Ordinal;
                                this.BindIgnoreCase = BindingFlags.DeclaredOnly;
                                this.ComparerIgnoreCase = StringComparer.Ordinal;
                            }
                            break;
                        default:
                            this.Variable[node.Key] = node.Value;
                            break;
                    }
                }
            }

            /// <summary>
            /// 全局初始数据
            /// </summary>
            public VariableScope Data { set; get; }

            /// <summary>
            /// 加载器
            /// </summary>
            public IResourceLoader Loader { set; get; }

            /// <summary>
            /// 绑定大小写配置
            /// </summary>
            public BindingFlags BindIgnoreCase { set; get; }

            /// <summary>
            /// 字符串大小写比较配置
            /// </summary>
            public StringComparer ComparerIgnoreCase { set; get; }

            /// <summary>
            /// 字符串大小写比较配置
            /// </summary>
            public StringComparison ComparisonIgnoreCase { set; get; }

            /// <summary>
            /// Default encoding
            /// </summary>
            public Encoding Encoding { set; get; }

            /// <summary>
            /// Cache
            /// </summary>

            public ICache Cache { set; get; }
            /// <summary>
            /// Actuator
            /// </summary>
            public IActuator Actuator { set; get; }

            /// <summary>
            /// 模板资源搜索目录
            /// </summary>
            /// <value></value>
            public List<string> ResourceDirectories { set; get; }
            /// <summary>
            /// 
            /// </summary>
            public Dictionary<string, string> Variable { set; get; }
            /// <summary>
            /// 
            /// </summary>
            public List<ITagParser> Parsers { set; get; }

            /// <summary>
            /// 是否启用编译模式
            /// </summary>
            public bool EnableCompile { get; set; } = true;

            /// <summary>
            /// 是否启用模板缓存
            /// </summary>
            public bool EnableTemplateCache { get; set; } = true;

            /// <summary>
            /// 
            /// </summary>
            public TemplateCollection<Compile.ICompileTemplate> Templates { get; set; }

        }
    }
}