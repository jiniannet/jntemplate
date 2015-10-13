/********************************************************************************
 Copyright (c) jiniannet (http://www.jiniannet.com). All rights reserved.
 Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.
 ********************************************************************************/
using System;
using System.Reflection;
using JinianNet.JNTemplate.Caching;
using JinianNet.JNTemplate.Parser;

namespace JinianNet.JNTemplate
{
    /// <summary>
    /// 引擎运行时
    /// </summary>
    public class Runtime : Configuration.ConfigBase
    {

        private ITagTypeResolver _tagResolver;
        private StringComparison _stringComparison;
        private BindingFlags _bindingFlags;
        private StringComparer _stringComparer;
        private ICache _cache;

        /// <summary>
        /// 运行时构造函数
        /// </summary>
        /// <param name="tagResolver">标签解析器</param>
        /// <param name="cache">缓存</param>
        /// <param name="conf">配置信息</param>
        internal Runtime(ITagTypeResolver tagResolver,
            ICache cache,
            Configuration.ConfigBase conf)
        {
            if (tagResolver == null)
            {
                throw new ArgumentException("tagResolver");
            }
            if (conf == null)
            {
                throw new ArgumentException("conf");
            }
            if (String.IsNullOrEmpty(conf.TagPrefix))
            {
                throw new ArgumentException("conf.TagPrefix");
            }
            if (String.IsNullOrEmpty(conf.TagSuffix))
            {
                throw new ArgumentException("conf.TagSuffix");
            }
            if (conf.ResourceDirectories == null)
            {
                ResourceDirectories = new String[0];
            }
            else
            {
                ResourceDirectories = conf.ResourceDirectories;
            }
            this._tagResolver = tagResolver;
            this._cache = cache;
            TagFlag = conf.TagFlag;
            TagPrefix = conf.TagPrefix; ;
            TagSuffix = conf.TagSuffix;
            ThrowExceptions = conf.ThrowExceptions;
            StripWhiteSpace = conf.StripWhiteSpace;
            IgnoreCase = conf.IgnoreCase;
            if (IgnoreCase)
            {
                this._bindingFlags = BindingFlags.IgnoreCase;
                this._stringComparer = StringComparer.OrdinalIgnoreCase;
                this._stringComparison = StringComparison.OrdinalIgnoreCase;
            }
            else
            {
                this._stringComparison = StringComparison.Ordinal;
                this._bindingFlags = BindingFlags.Default;
                this._stringComparer = StringComparer.Ordinal;
            }
        }

        /// <summary>
        /// 缓存
        /// </summary>
        public ICache Cache
        {
            get { return _cache; }
        }

        /// <summary>
        /// 标签类型解析器
        /// </summary>
        public Parser.ITagTypeResolver TagResolver
        {
            get { return this._tagResolver; }
        }

        /// <summary>
        /// 字符串大小写排序配置
        /// </summary>
        public StringComparison ComparisonIgnoreCase
        {
            get { return this._stringComparison; }
        }

        /// <summary>
        /// 绑定大小写配置
        /// </summary>
        public BindingFlags BindIgnoreCase
        {
            get { return this._bindingFlags; }
        }

        /// <summary>
        /// 字符串大小写比较配置
        /// </summary>
        public StringComparer ComparerIgnoreCase
        {
            get { return this._stringComparer; }
        }
    }
}
