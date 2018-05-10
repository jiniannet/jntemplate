/********************************************************************************
 Copyright (c) jiniannet (http://www.jiniannet.com). All rights reserved.
 Licensed under the MIT license. See licence.txt file in the project root for full license information.
 ********************************************************************************/

using System;

namespace JinianNet.JNTemplate
{
    /// <summary>
    /// 获取索引值
    /// </summary>
    /// <param name="container">对象</param>
    /// <param name="index">索引值</param>
    /// <returns>返回结果</returns>
    public delegate object CallIndexValueDelegate(object container, object index);
}