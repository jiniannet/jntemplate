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
    public interface ITemplateConfiguration
    {
        /// <summary>
        /// 工作路径
        /// </summary>
        Collection<String> Paths { get; }
        //String TagPrefix;
        //String TagSuffix;
        /// <summary>
        /// 标签标志
        /// </summary>
        Char TagFlag {get;}
        /// <summary>
        /// 缓存提供器
        /// </summary>
        ICache CachingProvider { get; }
        /// <summary>
        /// 是否抛出异常
        /// </summary>
        Boolean ThrowExceptions { get; }

        /// <summary>
        /// 标签分析器
        /// </summary>
        Parser.ITagTypeResolver Resolver { get; }
    }
}
