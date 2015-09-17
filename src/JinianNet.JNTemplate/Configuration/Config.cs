/********************************************************************************
 Copyright (c) jiniannet (http://www.jiniannet.com). All rights reserved.
 Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.
 ********************************************************************************/
using JinianNet.JNTemplate.Caching;
using JinianNet.JNTemplate.Parser;
using System;
using System.Collections.Generic;
using System.Text;

namespace JinianNet.JNTemplate.Configuration
{
    /// <summary>
    /// 默认配置
    /// </summary>
    public class Config : ITemplateConfig
    {
        private System.Collections.ObjectModel.Collection<String> paths;
        private Parser.TagTypeResolver resolver;
        private ICache cachingProvider;
        private Char tagFlag;
        private String tagPrefix;
        private String tagSuffix;
        private Boolean throwExceptions;

        /// <summary>
        /// 默认配置
        /// </summary>
        public Config(Char flag, String prefix, String suffix)
        {
            this.paths = new System.Collections.ObjectModel.Collection<String>();
            this.resolver = new Parser.TagTypeResolver();
            this.resolver.Add(new BooleanParser());
            this.resolver.Add(new NumberParser());
            this.resolver.Add(new EleseParser());
            this.resolver.Add(new EndParser());
            this.resolver.Add(new VariableParser());
            this.resolver.Add(new StringParser());
            this.resolver.Add(new ForeachParser());
            this.resolver.Add(new ForParser());
            this.resolver.Add(new SetParser());
            this.resolver.Add(new IfParser());
            this.resolver.Add(new ElseifParser());
            this.resolver.Add(new LoadParser());
            this.resolver.Add(new IncludeParser());
            this.resolver.Add(new FunctionParser());
            this.resolver.Add(new ComplexParser());
            this.cachingProvider = null;
            this.tagFlag = flag;
            this.tagSuffix = suffix;
            this.tagPrefix = prefix;
            this.throwExceptions = false;
        }

        /// <summary>
        /// 默认配置
        /// </summary>
        public Config()
            : this('$', "${", "}")
        {

        }
        /// <summary>
        /// 模板文件搜寻路径
        /// </summary>
        public System.Collections.ObjectModel.Collection<String> Paths
        {
            get
            {
                return paths;
            }
        }
        /// <summary>
        /// 简写标签标记
        /// </summary>
        public Char TagFlag
        {
            get
            {
                return tagFlag;
            }
        }
        /// <summary>
        /// 缓存提供器
        /// </summary>
        public ICache CachingProvider
        {
            get
            {
                return cachingProvider;
            }
            set
            {
                cachingProvider = value;
            }
        }
        /// <summary>
        /// 解析出错是否抛出异常
        /// </summary>
        public Boolean ThrowExceptions
        {
            get
            {
                return throwExceptions;
            }
            set
            {
                throwExceptions = value;
            }
        }

        /// <summary>
        /// 标签类型解析器
        /// </summary>
        public Parser.ITagTypeResolver Resolver
        {
            get
            {
                return resolver;
            }
        }

        /// <summary>
        /// 完整标签前缀
        /// </summary>
        public string TagPrefix
        {
            get
            {
                return tagPrefix;
            }
        }

        /// <summary>
        /// 完整标签后缀
        /// </summary>
        public string TagSuffix
        {
            get
            {
                return tagSuffix;
            }
        }
    }
}