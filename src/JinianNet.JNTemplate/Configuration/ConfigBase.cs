/********************************************************************************
 Copyright (c) jiniannet (http://www.jiniannet.com). All rights reserved.
 Licensed under the MIT license. See licence.txt file in the project root for full license information.
 ********************************************************************************/
using JinianNet.JNTemplate.Dynamic;
using JinianNet.JNTemplate.Resources;
using System;
using System.Collections.Generic;
using System.Reflection;

namespace JinianNet.JNTemplate.Configuration
{
    /// <summary>
    /// 基本配置信息
    /// </summary>
    public class ConfigBase
    {
        private char _tagFlag = '$';
        private string _tagPrefix = "${";
        private string _tagSuffix = "}";
        private bool _throwExceptions = true;
        private bool _stripWhiteSpace = false;
        private bool _ignoreCase = true;
        private string _charset = "utf-8";
        private Caching.ICacheProvider _cachingProvider;
        private ILoadProvider _loadProvider;
        private IDynamicProvider _dynamicProvider;
        private List<Parsers.ITagParser> _tagParsers;
        private List<string> _resourceDirectories;


        /// <summary>
        /// 资源路径
        /// </summary>
        public List<string> ResourceDirectories
        {
            get { return this._resourceDirectories; }
            set { this._resourceDirectories = value; }
        }
        /// <summary>
        /// 字符编码
        /// </summary>
        [Property]
        public string Charset
        {
            get { return this._charset; }
            set { this._charset = value; }
        }
        /// <summary>
        /// 标签前缀
        /// </summary>
        [Property]
        public string TagPrefix
        {
            get { return this._tagPrefix; }
            set { this._tagPrefix = value; }
        }

        /// <summary>
        /// 标签后缀
        /// </summary>
        [Property]
        public string TagSuffix
        {
            get { return this._tagSuffix; }
            set { this._tagSuffix = value; }
        }


        /// <summary>
        /// 简写标签前缀
        /// </summary>
        [Property]
        public char TagFlag
        {
            get { return this._tagFlag; }
            set { this._tagFlag = value; }
        }

        /// <summary>
        /// 是否抛出异常
        /// </summary>
        [Property]
        public bool ThrowExceptions
        {
            get { return this._throwExceptions; }
            set { this._throwExceptions = value; }
        }


        /// <summary>
        /// 是否处理标签前后空白字符
        /// </summary>
        [Property]
        public bool StripWhiteSpace
        {
            get { return this._stripWhiteSpace; }
            set { this._stripWhiteSpace = value; }
        }


        /// <summary>
        /// 是否忽略大小写
        /// </summary>
        [Property]
        public bool IgnoreCase
        {
            get { return this._ignoreCase; }
            set { this._ignoreCase = value; }
        }

        /// <summary>
        /// 缓存提供器
        /// </summary>
        public Caching.ICacheProvider CacheProvider
        {
            get { return this._cachingProvider; }
            set { this._cachingProvider = value; }
        }

        /// <summary>
        /// 执行提供器
        /// </summary> 
        public IDynamicProvider DynamicProvider
        {
            get { return this._dynamicProvider; }
            set { this._dynamicProvider = value; }
        }

        /// <summary>
        /// 加载提供器
        /// </summary>
        public ILoadProvider LoadProvider
        {
            get { return this._loadProvider; }
            set { this._loadProvider = value; }
        }

        /// <summary>
        /// 标签分析器
        /// </summary>
        [Property("Parsers")]
        public List<Parsers.ITagParser> TagParsers
        {
            get { return this._tagParsers; }
            set { this._tagParsers = value; }
        }
    }
}