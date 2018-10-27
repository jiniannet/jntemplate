/********************************************************************************
 Copyright (c) jiniannet (http://www.jiniannet.com). All rights reserved.
 Licensed under the MIT license. See licence.txt file in the project root for full license information.
 ********************************************************************************/
using JinianNet.JNTemplate.Dynamic;
using JinianNet.JNTemplate.Parsers;
using JinianNet.JNTemplate.Resources;
using System;
using System.Collections.Generic;
using System.Reflection;
using System.Text;

namespace JinianNet.JNTemplate
{
    /// <summary>
    /// 提供运行时的通用方法与属性
    /// </summary>
    public class RuntimeInfo
    {
        #region 字段
        private Dictionary<string, string> _environmentVariable;
        private VariableScope _data;
        private List<ITagParser> _parsers;
        private ILoadProvider _loder;
        private static StringComparison _stringComparison;
        private Caching.ICacheProvider _cache;
        private static BindingFlags _bindingFlags;
        private static StringComparer _stringComparer;
        private IDynamicProvider _callProxy;
        private List<string> _resourceDirectories;
        private static RuntimeInfo _instance;
        private static readonly object lockHelper = new object();
        private InitializationState _state;
        #endregion

        /// <summary>
        /// 获取实例 
        /// </summary>
        /// <returns></returns>
        public static RuntimeInfo GetInstance()
        {
            if (_instance == null)
            {
                lock (lockHelper)
                {
                    // 如果类的实例不存在则创建，否则直接返回
                    if (_instance == null)
                    {
                        _instance = new RuntimeInfo();
                    }
                }
            }
            return _instance;
        }

        /// <summary>
        /// 引擎信息
        /// </summary>
        private RuntimeInfo()
        {
            _environmentVariable = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase);
            _state = InitializationState.None;
            _parsers = new List<ITagParser>();
        }



        #region 实列
        /// <summary>
        /// 状态 
        /// </summary>
        /// <value></value>
        public InitializationState State
        {
            get { return this._state; }
            internal set { this._state = value; }
        }
        /// <summary>
        /// 模板资源搜索目录
        /// </summary>
        /// <value></value>
        public List<string> ResourceDirectories
        {
            get { return this._resourceDirectories; }
            internal set { this._resourceDirectories = value; }
        }
        /// <summary>
        /// 环境变量
        /// </summary>
        public Dictionary<string, string> EnvironmentVariable
        {
            get { return _environmentVariable; }
        }
        /// <summary>
        /// 全局初始数据
        /// </summary>
        internal VariableScope Data
        {
            get { return _data; }
            set { _data = value; }
        }

        /// <summary>
        /// 标签分析器
        /// </summary>
        public List<ITagParser> Parsers
        {
            get { return _parsers; }
            internal set { _parsers = value; }
        }

        /// <summary>
        /// 加载器
        /// </summary>
        internal ILoadProvider LoadProvider
        {
            private get { return _loder; }
            set { _loder = value; }
        }

        /// <summary>
        /// 绑定大小写配置
        /// </summary>
        public BindingFlags BindIgnoreCase
        {
            get { return _bindingFlags; }
            set { _bindingFlags = value; }
        }

        /// <summary>
        /// 字符串大小写比较配置
        /// </summary>
        public StringComparer ComparerIgnoreCase
        {
            get { return _stringComparer; }
            set { _stringComparer = value; }
        }
        /// <summary>
        /// 字符串大小写比较配置
        /// </summary>
        public StringComparison ComparisonIgnoreCase
        {
            get { return _stringComparison; }
            set { _stringComparison = value; }
        }
        /// <summary>
        /// 缓存
        /// </summary>
        public Caching.ICacheProvider CacheProvider
        {
            get { return _cache; }
            internal set { _cache = value; }
        }
        /// <summary>
        /// 动态调用代理
        /// </summary>
        internal IDynamicProvider DynamicProvider
        {
            private get { return _callProxy; }
            set { _callProxy = value; }
        }
        /// <summary>
        /// 加载资源
        /// </summary>
        /// <param name="filename">文件名,可以是纯文件名,也可以是完整的路径</param>
        /// <param name="encoding">编码</param>
        /// <param name="directory">追加查找目录</param>
        /// <returns></returns>
        public ResourceInfo Load(string filename, Encoding encoding, params string[] directory)
        {
            return this.LoadProvider.Load(filename, encoding, directory);
        }
        /// <summary>
        /// 获取父目录
        /// </summary>
        /// <param name="fullPath">完整路径</param>
        public string GetDirectoryName(string fullPath)
        {
            return this.LoadProvider.GetDirectoryName(fullPath);
        }
        /// <summary>
        /// 动态执行方法
        /// </summary>
        /// <param name="container">对象</param>
        /// <param name="methodName">方法名</param>
        /// <param name="args">实参</param>
        /// <returns>执行结果（Void返回NULL）</returns> 
        public object CallMethod(object container, string methodName, object[] args)
        {
            return this.DynamicProvider.CallMethod(container, methodName, args);
        }
        /// <summary>
        /// 动态获取属性或字段
        /// </summary>
        /// <param name="value">对象</param>
        /// <param name="propertyName">属性或字段名</param>
        /// <returns>返回结果</returns> 
        public object CallPropertyOrField(object value, string propertyName)
        {
            return this.DynamicProvider.CallPropertyOrField(value, propertyName);
        }

        /// <summary>
        /// 动态获取索引值
        /// </summary>
        /// <param name="value">对象</param>
        /// <param name="index">索引</param>
        /// <returns>返回结果</returns>

        public object CallIndexValue(object value, object index)
        {
            return this.DynamicProvider.CallIndexValue(value, index);
        }

        /// <summary>
        /// 状态枚举
        /// </summary>

        public enum InitializationState
        {
            /// <summary>
            /// 未初始化
            /// </summary>
            None,
            /// <summary>
            /// 初始化中
            /// </summary>
            Initialization,
            /// <summary>
            /// 初始完成
            /// </summary>
            Complete
        }
        #endregion
    }
}