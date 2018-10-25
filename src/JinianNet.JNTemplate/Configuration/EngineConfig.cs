/********************************************************************************
 Copyright (c) jiniannet (http://www.jiniannet.com). All rights reserved.
 Licensed under the MIT license. See licence.txt file in the project root for full license information.
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
            return new EngineConfig();
        }
    }
}