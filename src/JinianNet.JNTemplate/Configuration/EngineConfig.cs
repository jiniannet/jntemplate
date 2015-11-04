/********************************************************************************
 Copyright (c) jiniannet (http://www.jiniannet.com). All rights reserved.
 Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.
 ********************************************************************************/
using System;

namespace JinianNet.JNTemplate.Configuration
{
    /// <summary>
    /// 模板配置
    /// </summary>
    public class EngineConfig : ConfigBase
    {
        private String _cachingProvider;
        private String[] _tagParsers;

        /// <summary>
        /// 缓存提供器
        /// </summary>
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
        /// 创建默认配置
        /// </summary>
        /// <returns></returns>
        public static EngineConfig CreateDefault()
        {
            EngineConfig conf = new EngineConfig();
            //conf.CachingProvider = "JinianNet.JNTemplate.Caching.MemoryCache";
            conf.ResourceDirectories = new String[0];
            conf.StripWhiteSpace = true;
            conf.TagFlag = '$';
            conf.TagPrefix = "${";
            conf.TagSuffix = "}";
            conf.ThrowExceptions = true;
            conf.IgnoreCase = true;
            conf.TagParsers = Field.RSEOLVER_TYPES;
            return conf;
        }
    }
}