/********************************************************************************
 Copyright (c) jiniannet (http://www.jiniannet.com). All rights reserved.
 Licensed under the MIT license. See licence.txt file in the project root for full license information.
 ********************************************************************************/
using JinianNet.JNTemplate.Caching;
using JinianNet.JNTemplate.Configuration;
using JinianNet.JNTemplate.Dynamic;
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
    public class Runtime
    {
        #region 字段
        private Dictionary<string, string> environmentVariable;
        private VariableScope data;
        private TagParser parsers;
        private IResourceLoader loader;
        private StringComparison stringComparison;
        private Caching.ICache cache;
        private BindingFlags bindingFlags;
        private StringComparer stringComparer;
        private IActuator actuator;
        private List<string> resourceDirectories;
        private Encoding encoding;
        #endregion


        #region 构造与初始化
        /// <summary>
        /// 引擎信息
        /// </summary>
        /// <param name="conf">配置信息</param>
        /// <param name="scope">全局初始数据</param>
        internal Runtime(IConfig conf, VariableScope scope)
        {
            if (conf == null)
            {
                throw new ArgumentNullException("\"conf\" cannot be null.");
            }
            this.data = scope;
            Initialization(conf);
            InitializationEnvironment(conf.ToDictionary());
        }


        /// <summary>
        /// 初始化环境变量配置
        /// </summary>
        /// <param name="conf">配置内容</param>
        private void InitializationEnvironment(IDictionary<string, string> conf)
        {
            foreach (KeyValuePair<string, string> node in conf)
            {
                environmentVariable[node.Key] = node.Value;
            }
        }

        /// <summary>
        /// 初始化基本数据
        /// </summary>
        /// <param name="conf"></param>
        private void Initialization(IConfig conf)
        {
            environmentVariable = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase);
            parsers = new TagParser();
            resourceDirectories = new List<string>();
            if (conf.TagParsers == null || conf.TagParsers.Count == 0)
            {
                parsers.AddRange(LoadParsers(Field.RSEOLVER_TYPES));
            }
            else
            {
                parsers.AddRange(LoadParsers(Field.RSEOLVER_TYPES));
            }
            loader = conf.Loader ?? new FileLoader();
            resourceDirectories = (conf.ResourceDirectories ?? new List<string>());
            cache = conf.Cache ?? MemoryCache.Instance;
            actuator = conf.Actuator ?? new ReflectionActuator();
            if (conf.IgnoreCase)
            {
                bindingFlags = BindingFlags.IgnoreCase;
                stringComparer = StringComparer.OrdinalIgnoreCase;
                stringComparison = StringComparison.OrdinalIgnoreCase;
            }
            else
            {
                stringComparison = StringComparison.Ordinal;
                bindingFlags = BindingFlags.DeclaredOnly;
                stringComparer = StringComparer.Ordinal;
            }
            if (string.IsNullOrEmpty(conf.Charset))
            {
                this.encoding = Encoding.UTF8;
            }
            else
            {
                this.encoding = Encoding.GetEncoding(conf.Charset);
            }
        }

        /// <summary>
        /// 加载标签分析器
        /// </summary>
        /// <param name="arr">解析器类型</param>
        private static ITagParser[] LoadParsers(string[] arr)
        {
            ITagParser[] list = new ITagParser[arr.Length];
            for (int i = 0; i < arr.Length; i++)
            {
                list[i] = DynamicHelpers.CreateInstance<ITagParser>(Type.GetType(arr[i]));
            }
            return list;
        }


        /// <summary>
        /// 模板资源搜索目录
        /// </summary>
        /// <value></value>
        public List<string> ResourceDirectories
        {
            get { return this.resourceDirectories; }
        }

        /// <summary>
        /// 全局初始数据
        /// </summary>
        public VariableScope Data
        {
            get { return data; }
        }

        /// <summary>
        /// 标签分析器
        /// </summary>
        public TagParser Parsers
        {
            get { return parsers; }
        }

        /// <summary>
        /// 加载器
        /// </summary>
        internal IResourceLoader Loader
        {
            get { return loader; }
        }

        /// <summary>
        /// 绑定大小写配置
        /// </summary>
        public BindingFlags BindIgnoreCase
        {
            get { return bindingFlags; }
        }

        /// <summary>
        /// 字符串大小写比较配置
        /// </summary>
        public StringComparer ComparerIgnoreCase
        {
            get { return stringComparer; }
        }

        /// <summary>
        /// 字符串大小写比较配置
        /// </summary>
        public StringComparison ComparisonIgnoreCase
        {
            get { return stringComparison; }
        }
        /// <summary>
        /// 缓存
        /// </summary>
        public Caching.ICache Cache
        {
            get { return cache; }
        }
        /// <summary>
        /// 动态调用代理
        /// </summary>
        internal IActuator Actuator
        {
            get { return actuator; }
        }

        /// <summary>
        /// Default encoding
        /// </summary>
        public Encoding Encoding
        {
            get { return encoding; }
        }

        #region 环境变量

        /// <summary>
        /// 获取环境变量
        /// </summary>
        /// <param name="variable">变量名称</param>
        /// <returns></returns>
        public string GetEnvironmentVariable(string variable)
        {
            string value;

            if (environmentVariable.TryGetValue(variable, out value))
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
        public void SetEnvironmentVariable(string variable, string value)
        {
            if (variable == null)
            {
                throw new ArgumentNullException("\"variable\" cannot be null.");
            }
            if (value == null)
            {
                environmentVariable.Remove(variable);
            }
            else
            {
                environmentVariable[variable] = value;
            }
        }

        #endregion

        #endregion
    }
}