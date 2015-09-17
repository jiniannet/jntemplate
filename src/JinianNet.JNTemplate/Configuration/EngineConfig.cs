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
    public class EngineConfig
    {
        private String[] _resourceDirectories;
        private Char _tagFlag;
        private String _tagPrefix;
        private String _tagSuffix;
        private String _cachingProvider;
        private Boolean _throwExceptions;
        private Boolean _stripWhiteSpace;
        private String[] _tagParsers;

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
        public String TagPrefix
        {
            get { return this._tagPrefix; }
            set { this._tagPrefix = value; }
        }
        /// <summary>
        /// 标签后缀
        /// </summary>
        public String TagSuffix
        {
            get { return this._tagSuffix; }
            set { this._tagSuffix = value; }
        }

        /// <summary>
        /// 简写标签前缀
        /// </summary>
        public Char TagFlag
        {
            get { return this._tagFlag; }
            set { this._tagFlag = value; }
        }

        /// <summary>
        /// 缓存提供器
        /// </summary>
        public String CachingProvider
        {
            get { return this._cachingProvider; }
            set { this._cachingProvider = value; }
        }

        /// <summary>
        /// 是否抛出异常
        /// </summary>
        public Boolean ThrowExceptions
        {
            get { return this._throwExceptions; }
            set { this._throwExceptions = value; }
        }

        /// <summary>
        /// 是否处理标签前后空白字符
        /// </summary>
        public Boolean StripWhiteSpace
        {
            get { return this._stripWhiteSpace; }
            set { this._stripWhiteSpace = value; }
        }

        /// <summary>
        /// 标签分析器
        /// </summary>
        public String[] TagParsers
        {
            get { return this._tagParsers; }
            set { this._tagParsers = value; }
        }

        public static EngineConfig CreateDefault()
        {
            EngineConfig conf = new EngineConfig();
            conf.CachingProvider = null;
            conf.ResourceDirectories = new String[0];
            conf.StripWhiteSpace = true;
            conf.TagFlag = '$';
            conf.TagPrefix = "${";
            conf.TagSuffix = "}";
            conf.ThrowExceptions = true;
            conf.TagParsers = new String[] {
                "JinianNet.JNTemplate.Parser.BooleanParser",
                "JinianNet.JNTemplate.Parser.NumberParser",
                "JinianNet.JNTemplate.Parser.EleseParser",
                "JinianNet.JNTemplate.Parser.EndParser",
                "JinianNet.JNTemplate.Parser.VariableParser",
                "JinianNet.JNTemplate.Parser.StringParser",
                "JinianNet.JNTemplate.Parser.ForeachParser",
                "JinianNet.JNTemplate.Parser.ForParser",
                "JinianNet.JNTemplate.Parser.SetParser",
                "JinianNet.JNTemplate.Parser.IfParser",
                "JinianNet.JNTemplate.Parser.ElseifParser",
                "JinianNet.JNTemplate.Parser.LoadParser",
                "JinianNet.JNTemplate.Parser.IncludeParser",
                "JinianNet.JNTemplate.Parser.FunctionParser",
                "JinianNet.JNTemplate.Parser.ComplexParser"
            };

            return conf;
        }
    }
}