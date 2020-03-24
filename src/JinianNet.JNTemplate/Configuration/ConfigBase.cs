// /********************************************************************************
// Copyright (c) jiniannet (http://www.jiniannet.com). All rights reserved.
// Licensed under the MIT license. See licence.txt file in the project root for full license information.
// ********************************************************************************/
//using JinianNet.JNTemplate.Dynamic;
//using JinianNet.JNTemplate.Resources;
//using System;
//using System.Collections.Generic;
//using System.Reflection;

//namespace JinianNet.JNTemplate.Configuration
//{
//    /// <summary>
//    /// 基本配置信息
//    /// </summary>
//    public class ConfigBase
//    {
//        private char tagFlag = '$';
//        private string tagPrefix = "${";
//        private string tagSuffix = "}";
//        private bool throwExceptions = true;
//        private bool stripWhiteSpace = false;
//        private bool ignoreCase = true;
//        private string charset = "utf-8";
//        private Caching.ICacheProvider cachingProvider;
//        private ILoaderProvider loadProvider;
//        private IActuatorProvider actuatorProvider;
//        private List<Parsers.ITagParser> tagParsers;
//        private List<string> resourceDirectories;


//        /// <summary>
//        /// 资源路径
//        /// </summary>
//        public List<string> ResourceDirectories
//        {
//            get { return this.resourceDirectories; }
//            set { this.resourceDirectories = value; }
//        }
//        /// <summary>
//        /// 字符编码
//        /// </summary>
//        [Variable]
//        public string Charset
//        {
//            get { return this.charset; }
//            set { this.charset = value; }
//        }
//        /// <summary>
//        /// 标签前缀
//        /// </summary>
//        [Variable]
//        public string TagPrefix
//        {
//            get { return this.tagPrefix; }
//            set { this.tagPrefix = value; }
//        }

//        /// <summary>
//        /// 标签后缀
//        /// </summary>
//        [Variable]
//        public string TagSuffix
//        {
//            get { return this.tagSuffix; }
//            set { this.tagSuffix = value; }
//        }


//        /// <summary>
//        /// 简写标签前缀
//        /// </summary>
//        [Variable]
//        public char TagFlag
//        {
//            get { return this.tagFlag; }
//            set { this.tagFlag = value; }
//        }

//        /// <summary>
//        /// 是否抛出异常
//        /// </summary>
//        [Variable]
//        public bool ThrowExceptions
//        {
//            get { return this.throwExceptions; }
//            set { this.throwExceptions = value; }
//        }


//        /// <summary>
//        /// 是否处理标签前后空白字符
//        /// </summary>
//        [Variable]
//        public bool StripWhiteSpace
//        {
//            get { return this.stripWhiteSpace; }
//            set { this.stripWhiteSpace = value; }
//        }


//        /// <summary>
//        /// 是否忽略大小写
//        /// </summary>
//        [Variable]
//        public bool IgnoreCase
//        {
//            get { return this.ignoreCase; }
//            set { this.ignoreCase = value; }
//        }

//        /// <summary>
//        /// 缓存提供器
//        /// </summary>
//        public Caching.ICacheProvider CacheProvider
//        {
//            get { return this.cachingProvider; }
//            set { this.cachingProvider = value; }
//        }

//        /// <summary>
//        /// 执行提供器
//        /// </summary> 
//        public IActuatorProvider ActuatorProvider
//        {
//            get { return this.actuatorProvider; }
//            set { this.actuatorProvider = value; }
//        }

//        /// <summary>
//        /// 加载提供器
//        /// </summary>
//        public ILoaderProvider LoadProvider
//        {
//            get { return this.loadProvider; }
//            set { this.loadProvider = value; }
//        }

//        /// <summary>
//        /// 标签分析器
//        /// </summary>
//        public List<Parsers.ITagParser> TagParsers
//        {
//            get { return this.tagParsers; }
//            set { this.tagParsers = value; }
//        }

//        /// <summary>
//        /// 将符合要求的配置转换为引擎环境变量
//        /// </summary>
//        /// <returns></returns>
//        public virtual Dictionary<string, string> ToDictionary()
//        {
//            //只有标注了Variable特性且类型为Environment才会进行转换
//            var dict = new Dictionary<string, string>();
//            Type type = this.GetType();
//            Type attrType = typeof(VariableAttribute);
//            PropertyInfo[] properties = type.GetProperties();
//            VariableAttribute attr;
//            foreach (PropertyInfo p in properties)
//            {
//                if (!Attribute.IsDefined(p, attrType) || !p.CanRead)
//                {
//                    continue;
//                }
//#if NET20 || NET40
//                attr = (VariableAttribute)p.GetCustomAttributes(attrType, true)[0];
//#else
//                attr = (VariableAttribute)p.GetCustomAttribute(attrType);
//#endif
//                if (attr.Type != VariableType.Environment)
//                {
//                    continue;
//                }
//                if (string.IsNullOrEmpty(attr.Name))
//                {
//                    attr.Name = p.Name;
//                }
//#if NET20 || NET40
//                object value = p.GetValue(this, null);
//#else
//                object value = p.GetValue(this);
//#endif
//                if (value == null)
//                {
//                    continue;
//                }

//                dict[p.Name] = value.ToString();
//            }

//            return dict;
//        }
//    }
//}