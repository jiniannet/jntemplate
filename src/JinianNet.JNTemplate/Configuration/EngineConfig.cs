/********************************************************************************
 Copyright (c) jiniannet (http://www.jiniannet.com). All rights reserved.
 Licensed under the MIT license. See licence.txt file in the project root for full license information.
 ********************************************************************************/
using JinianNet.JNTemplate.Caching;
using JinianNet.JNTemplate.Dynamic;
using JinianNet.JNTemplate.Resources;
using System;
using System.Collections.Generic;
using System.Reflection;

namespace JinianNet.JNTemplate.Configuration
{
    /// <summary>
    /// 模板配置
    /// </summary>
    public class EngineConfig : IConfig
    {
        /// <summary>
        /// 资源路径
        /// </summary>
        public List<string> ResourceDirectories { get; set; }
        /// <summary>
        /// 字符编码
        /// </summary> 
        public string Charset { get; set; }
        /// <summary>
        /// 标签前缀
        /// </summary> 
        public string TagPrefix { get; set; }

        /// <summary>
        /// 标签后缀
        /// </summary> 
        public string TagSuffix { get; set; }

        /// <summary>
        /// 简写标签前缀
        /// </summary> 
        public char TagFlag { get; set; } = '$';

        /// <summary>
        /// 是否抛出异常
        /// </summary> 
        public bool ThrowExceptions { get; set; } = true;


        /// <summary>
        /// 是否处理标签前后空白字符
        /// </summary> 
        public bool StripWhiteSpace { get; set; } = false;

        /// <summary>
        /// 是否缓存模板文件
        /// </summary> 
        public bool EnableTemplateCache { get; set; } = true;

        /// <summary>
        /// 是否忽略大小写
        /// </summary> 
        public bool IgnoreCase { get; set; } = true;

        /// <summary>
        /// 是否禁用简写标签 
        /// </summary>
        public bool DisableeLogogram { get; set; } = false;

        /// <summary>
        /// 加载提供器
        /// </summary>
        public IResourceLoader Loader { get; set; }

        /// <summary>
        /// 标签分析器
        /// </summary>
        public List<Parsers.ITagParser> TagParsers { get; set; }

        /// <summary>
        /// 创建默认配置
        /// </summary>
        /// <returns></returns>
        public static EngineConfig CreateDefault()
        {
            return new EngineConfig();
        }
    }
}