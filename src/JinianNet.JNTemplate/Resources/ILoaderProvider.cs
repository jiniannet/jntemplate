/********************************************************************************
 Copyright (c) jiniannet (http://www.jiniannet.com). All rights reserved.
 Licensed under the MIT license. See licence.txt file in the project root for full license information.
 ********************************************************************************/
using System;
using System.Text;

namespace JinianNet.JNTemplate.Resources
{
    /// <summary>
    /// 加载提供器
    /// </summary>
    public interface ILoaderProvider
    {
        /// <summary>
        /// 创建加载器
        /// </summary>
        /// <returns></returns>
        IResourceLoader CreateLoader();
    }
}