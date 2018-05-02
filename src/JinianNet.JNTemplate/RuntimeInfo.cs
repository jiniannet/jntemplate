/********************************************************************************
 Copyright (c) jiniannet (http://www.jiniannet.com). All rights reserved.
 Licensed under the MIT license. See licence.txt file in the project root for full license information.
 ********************************************************************************/
using JinianNet.JNTemplate.Parsers;
using System;
using System.Collections.Generic;
using System.Reflection;
using System.Text;

namespace JinianNet.JNTemplate
{
    /// <summary>
    /// 提供运行时的通用方法与属性
    /// </summary>
    public class RuntimeInfo : ILoader, ICallProxy
    {
        #region 字段
        private Dictionary<String, String> _environmentVariable;
        private VariableScope _data;
        private List<ITagParser> _parsers;
        private ILoader _loder;
        private static StringComparison _stringComparison;
        private Caching.ICache _cache;
        private static BindingFlags _bindingFlags;
        private static StringComparer _stringComparer;
        private ICallProxy _callProxy;

        #endregion
        /// <summary>
        /// 引擎信息
        /// </summary>
        public RuntimeInfo()
        {
            _environmentVariable = new Dictionary<String, String>(StringComparer.OrdinalIgnoreCase);
            _parsers = new List<ITagParser>();
        }

        #region 实列
        /// <summary>
        /// 环境变量
        /// </summary>
        public Dictionary<String, String> EnvironmentVariable
        {
            get { return _environmentVariable; }
        }
        /// <summary>
        /// 全局初始数据
        /// </summary>
        public VariableScope Data
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
        }

        /// <summary>
        /// 加载器
        /// </summary>
        internal ILoader Loder
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
        public Caching.ICache Cache
        {
            get { return _cache; }
            internal set { _cache = value; }
        }
        /// <summary>
        /// 动态调用代理
        /// </summary>
        internal ICallProxy DynamicCallProxy
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
        public ResourceInfo Load(String filename, Encoding encoding, params String[] directory)
        {
            return this.Loder.Load(filename, encoding, directory);
        }
        /// <summary>
        /// 获取父目录
        /// </summary>
        /// <param name="fullPath">完整路径</param>
        public string GetDirectoryName(String fullPath)
        {
            return this.Loder.GetDirectoryName(fullPath);
        }
        /// <summary>
        /// 动态执行方法
        /// </summary>
        /// <param name="container">对象</param>
        /// <param name="methodName">方法名</param>
        /// <param name="args">实参</param>
        /// <returns>执行结果（Void返回NULL）</returns> 
        public object CallMethod(Object container, String methodName, Object[] args)
        {
            return this.DynamicCallProxy.CallMethod(container, methodName, args);
        }
        /// <summary>
        /// 动态获取属性或字段
        /// </summary>
        /// <param name="value">对象</param>
        /// <param name="propertyName">属性或字段名</param>
        /// <returns>返回结果</returns> 
        public object CallPropertyOrField(Object value, String propertyName)
        {
            return this.DynamicCallProxy.CallPropertyOrField(value, propertyName);
        }
        #endregion
    }
}