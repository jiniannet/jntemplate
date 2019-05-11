/********************************************************************************
 Copyright (c) jiniannet (http://www.jiniannet.com). All rights reserved.
 Licensed under the MIT license. See licence.txt file in the project root for full license information.
 ********************************************************************************/
using System;
using System.Collections;

namespace JinianNet.JNTemplate.Caching
{
    /// <summary>
    /// 缓存提供者
    /// </summary>
    public interface ICacheProvider
    {
        /// <summary>
        /// 创建缓存
        /// </summary>
        /// <returns></returns>
        ICache CreateCache();
    }
}