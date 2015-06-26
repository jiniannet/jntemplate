/********************************************************************************
 Copyright (c) jiniannet (http://www.jiniannet.com). All rights reserved.
 Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.
 ********************************************************************************/
using JinianNet.JNTemplate.Caching;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Text;

namespace JinianNet.JNTemplate.Configuration
{
    /// <summary>
    /// 模板配置
    /// </summary>
    public interface ITemplateConfig
    {
        /// <summary>
        /// 工作路径
        /// </summary>
        Collection<String> Paths { get; }

        /// <summary>
        /// 标签前缀
        /// </summary>
        String TagPrefix { get; }
        /// <summary>
        /// 标签后缀
        /// </summary>
        String TagSuffix { get; }

        /// <summary>
        /// 简写标签前缀
        /// </summary>
        Char TagFlag { get; }
        /// <summary>
        /// 缓存提供器
        /// </summary>
        ICache CachingProvider { get; set; }
        /// <summary>
        /// 是否抛出异常
        /// </summary>
        Boolean ThrowExceptions { get; set; }

        /// <summary>
        /// 标签分析器
        /// </summary>
        Parser.ITagTypeResolver Resolver { get; }
    }
}