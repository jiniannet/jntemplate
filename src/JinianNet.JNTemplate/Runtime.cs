/********************************************************************************
 Copyright (c) jiniannet (http://www.jiniannet.com). All rights reserved.
 Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.
 ********************************************************************************/
using System;
using System.Collections.Generic;
using System.Text;
using JinianNet.JNTemplate.Configuration;

namespace JinianNet.JNTemplate
{
    /// <summary>
    /// 运行时
    /// </summary>
    public class Runtiome
    {
        private static JinianNet.JNTemplate.Caching.ICache cache;
        private static JinianNet.JNTemplate.Parser.TagTypeResolver resolver;
        private static Char tagFlag;
        private static String tagPrefix;
        private static String tagSuffix;
        private static Boolean runing;

        private Runtiome()
        {

        }

        static Runtiome()
        {
            //Configure(new Config());
            runing = false;
        }

        internal static void Configure(ITemplateConfig config)
        {
            if (!runing)
            {
                cache = config.CachingProvider;
                //resolver = config.Resolver;
                tagFlag = config.TagFlag;
                tagSuffix = config.TagSuffix;
                tagPrefix = config.TagPrefix;
            }
        }

        /// <summary>
        /// 资源目录
        /// </summary>
        /// <returns></returns>
        public static String[] ResourceDirectories()
        {
            return null;
        }

        /// <summary>
        /// 简写标签标记
        /// </summary>
        public static Char TagFlag
        {
            get { return tagFlag; }
        }
        /// <summary>
        /// 缓存
        /// </summary>
        public static JinianNet.JNTemplate.Caching.ICache Cache
        {
            get { return cache; }
        }

        /// <summary>
        /// 标签类型解析器
        /// </summary>
        public static Parser.ITagTypeResolver TagResolver
        {
            get { return resolver; }
        }


        /// <summary>
        /// 完整标签前缀
        /// </summary>
        public static string TagPrefix
        {
            get { return tagPrefix; }
        }

        /// <summary>
        /// 完整标签后缀
        /// </summary>
        public static string TagSuffix
        {
            get { return tagSuffix; }
        }
    }

}