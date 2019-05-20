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
        private ILoaderProvider _loadProvider;
        private IActuatorProvider _actuatorProvider;
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
        [Variable]
        public string Charset
        {
            get { return this._charset; }
            set { this._charset = value; }
        }
        /// <summary>
        /// 标签前缀
        /// </summary>
        [Variable]
        public string TagPrefix
        {
            get { return this._tagPrefix; }
            set { this._tagPrefix = value; }
        }

        /// <summary>
        /// 标签后缀
        /// </summary>
        [Variable]
        public string TagSuffix
        {
            get { return this._tagSuffix; }
            set { this._tagSuffix = value; }
        }


        /// <summary>
        /// 简写标签前缀
        /// </summary>
        [Variable]
        public char TagFlag
        {
            get { return this._tagFlag; }
            set { this._tagFlag = value; }
        }

        /// <summary>
        /// 是否抛出异常
        /// </summary>
        [Variable]
        public bool ThrowExceptions
        {
            get { return this._throwExceptions; }
            set { this._throwExceptions = value; }
        }


        /// <summary>
        /// 是否处理标签前后空白字符
        /// </summary>
        [Variable]
        public bool StripWhiteSpace
        {
            get { return this._stripWhiteSpace; }
            set { this._stripWhiteSpace = value; }
        }


        /// <summary>
        /// 是否忽略大小写
        /// </summary>
        [Variable]
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
        public IActuatorProvider ActuatorProvider
        {
            get { return this._actuatorProvider; }
            set { this._actuatorProvider = value; }
        }

        /// <summary>
        /// 加载提供器
        /// </summary>
        public ILoaderProvider LoadProvider
        {
            get { return this._loadProvider; }
            set { this._loadProvider = value; }
        }

        /// <summary>
        /// 标签分析器
        /// </summary>
        public List<Parsers.ITagParser> TagParsers
        {
            get { return this._tagParsers; }
            set { this._tagParsers = value; }
        }

        /// <summary>
        /// 将符合要求的配置转换为引擎环境变量
        /// </summary>
        /// <returns></returns>
        public virtual Dictionary<string, string> ToDictionary()
        {
            //只有标注了Variable特性且类型为Environment才会进行转换
            var dict = new Dictionary<string, string>();
            Type type = this.GetType();
            Type attrType = typeof(VariableAttribute);
            PropertyInfo[] properties = type.GetProperties();
            VariableAttribute attr;
            foreach (PropertyInfo p in properties)
            {
                if (!Attribute.IsDefined(p, attrType) || !p.CanRead)
                {
                    continue;
                }
#if NET20 || NET40
                attr = (VariableAttribute)p.GetCustomAttributes(attrType, true)[0];
#else
                attr = (VariableAttribute)p.GetCustomAttribute(attrType);
#endif
                if (attr.Type != VariableType.Environment)
                {
                    continue;
                }
                if (string.IsNullOrEmpty(attr.Name))
                {
                    attr.Name = p.Name;
                }
#if NET20 || NET40
                object value = p.GetValue(this, null);
#else
                object value = p.GetValue(this);
#endif
                if (value == null)
                {
                    continue;
                }

                dict[p.Name] = value.ToString();
            }

            return dict;
        }
    }
}