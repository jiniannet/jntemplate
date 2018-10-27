/********************************************************************************
 Copyright (c) jiniannet (http://www.jiniannet.com). All rights reserved.
 Licensed under the MIT license. See licence.txt file in the project root for full license information.
 ********************************************************************************/
using System;
/********************************************************************************
 Copyright (c) jiniannet (http://www.jiniannet.com). All rights reserved.
 Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.
 ********************************************************************************/
using System.Text;

namespace JinianNet.JNTemplate.Resources
{
    /// <summary>
    /// 模板加载器
    /// </summary>
    public interface ILoadProvider
    {
        /// <summary>
        /// 加载资源
        /// </summary>
        /// <param name="filename">文件名,可以是纯文件名,也可以是完整的路径</param>
        /// <param name="encoding">编码</param>
        /// <param name="directory">追加查找目录</param>
        /// <returns></returns>
        ResourceInfo Load(string filename, Encoding encoding, params string[] directory);

        /// <summary>
        /// 获取父目录
        /// </summary>
        /// <param name="fullPath">完整路径</param>
        /// <returns></returns>
        string GetDirectoryName(string fullPath);
    }
}