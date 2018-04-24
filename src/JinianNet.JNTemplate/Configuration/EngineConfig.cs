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
        /// <summary>
        /// 创建默认配置
        /// </summary>
        /// <returns></returns>
        public static EngineConfig CreateDefault()
        {
            EngineConfig conf = new EngineConfig();
            //conf.CachingProvider = "JinianNet.JNTemplate.Caching.MemoryCache";
            conf.StripWhiteSpace = true;
            conf.TagFlag = '$';
            conf.TagPrefix = "${";
            conf.TagSuffix = "}";
            conf.ThrowExceptions = true;
            conf.IgnoreCase = true;
            conf.TagParsers = Field.RSEOLVER_TYPES;
            conf.Charset = "utf-8";
            return conf;
        }
    }
}