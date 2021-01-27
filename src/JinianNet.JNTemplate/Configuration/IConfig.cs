/********************************************************************************
 Copyright (c) jiniannet (http://www.jiniannet.com). All rights reserved.
 Licensed under the MIT license. See licence.txt file in the project root for full license information.
 ********************************************************************************/

using JinianNet.JNTemplate.Dynamic;
using JinianNet.JNTemplate.Resources;
using System.Collections.Generic;

namespace JinianNet.JNTemplate.Configuration
{
    /// <summary>
    /// 配置
    /// </summary>
    public interface IConfig
    {
        /// <summary>
        /// 资源路径
        /// </summary>
        List<string> ResourceDirectories { get; set; }
        /// <summary>
        /// 字符编码
        /// </summary>
        string Charset { get; set; }
        /// <summary>
        /// 标签前缀
        /// </summary>
        string TagPrefix { get; set; }
        /// <summary>
        /// 标签后缀
        /// </summary>
        string TagSuffix { get; set; }

        /// <summary>
        /// 简写标签前缀
        /// </summary>
        char TagFlag { get; set; }

        /// <summary>
        /// 是否抛出异常
        /// </summary>
        bool ThrowExceptions { get; set; }

        /// <summary>
        /// 是否处理标签前后空白字符
        /// </summary>
        bool StripWhiteSpace { get; set; }

        /// <summary>
        /// 是否忽略大小写
        /// </summary>
        bool IgnoreCase { get; set; }

        /// <summary>
        /// 加载提供器
        /// </summary>
        IResourceLoader Loader { get; set; }

        /// <summary>
        /// 标签分析器
        /// </summary>
        List<Parsers.ITagParser> TagParsers { get; set; }


        /// <summary>
        /// 将符合要求的配置转换为引擎环境变量
        /// </summary>
        /// <returns></returns>
        Dictionary<string, string> ToDictionary();
    }
}
