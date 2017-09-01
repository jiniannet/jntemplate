/********************************************************************************
 Copyright (c) jiniannet (http://www.jiniannet.com). All rights reserved.
 Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.
 ********************************************************************************/
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
        private String[] _resourceDirectories;
        private Char _tagFlag;
        private String _tagPrefix;
        private String _tagSuffix;
        private Boolean _throwExceptions;
        private Boolean _stripWhiteSpace;
        private Boolean _ignoreCase;
        private String _charset;
        private String _cachingProvider;
        private String[] _tagParsers;

        /// <summary>
        /// 字符编码
        /// </summary>
        [Variable]
        public String Charset
        {
            get { return this._charset; }
            set { this._charset = value; }
        }

        /// <summary>
        /// 资源路径
        /// </summary>
        public String[] ResourceDirectories
        {
            get { return this._resourceDirectories; }
            set { this._resourceDirectories = value; }
        }

        /// <summary>
        /// 标签前缀
        /// </summary>
        [Variable]
        public String TagPrefix
        {
            get { return this._tagPrefix; }
            set { this._tagPrefix = value; }
        }

        /// <summary>
        /// 标签后缀
        /// </summary>
        [Variable]
        public String TagSuffix
        {
            get { return this._tagSuffix; }
            set { this._tagSuffix = value; }
        }


        /// <summary>
        /// 简写标签前缀
        /// </summary>
        [Variable]
        public Char TagFlag
        {
            get { return this._tagFlag; }
            set { this._tagFlag = value; }
        }

        /// <summary>
        /// 是否抛出异常
        /// </summary>
        [Variable]
        public Boolean ThrowExceptions
        {
            get { return this._throwExceptions; }
            set { this._throwExceptions = value; }
        }


        /// <summary>
        /// 是否处理标签前后空白字符
        /// </summary>
        [Variable]
        public Boolean StripWhiteSpace
        {
            get { return this._stripWhiteSpace; }
            set { this._stripWhiteSpace = value; }
        }


        /// <summary>
        /// 是否忽略大小写
        /// </summary>
        [Variable]
        public Boolean IgnoreCase
        {
            get { return this._ignoreCase; }
            set { this._ignoreCase = value; }
        }

        /// <summary>
        /// 缓存提供器
        /// </summary>
        [Variable]
        public String CachingProvider
        {
            get { return this._cachingProvider; }
            set { this._cachingProvider = value; }
        }

        /// <summary>
        /// 标签分析器
        /// </summary>
        public String[] TagParsers
        {
            get { return this._tagParsers; }
            set { this._tagParsers = value; }
        }

        /// <summary>
        /// 将配置转换成字典形式
        /// </summary>
        /// <returns></returns>
        public virtual Dictionary<String, String> ToDictionary()
        {
            Dictionary<String, String> dic = new Dictionary<String, String>();
#if NETSTANDARD
            IEnumerable<PropertyInfo> pis = this.GetType().GetRuntimeProperties();
#else
            IEnumerable<PropertyInfo> pis = this.GetType().GetProperties();
#endif
#if NOTDNX
            Type type = typeof(VariableAttribute);
#endif
            foreach (PropertyInfo pi in pis)
            {
#if NOTDNX
                if( Attribute.IsDefined(pi, type))
                {
                    dic[pi.Name] = (pi.GetValue(this, null) ?? string.Empty).ToString();
                }
#else
                dic[pi.Name] = (pi.GetValue(this, null) ?? string.Empty).ToString();
#endif
            }
            return dic;
        }
    }
}
