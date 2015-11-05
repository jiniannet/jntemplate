/********************************************************************************
 Copyright (c) jiniannet (http://www.jiniannet.com). All rights reserved.
 Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.
 ********************************************************************************/

using System;
namespace JinianNet.JNTemplate.Dynamic
{
    /// <summary>
    /// 动态执行方法委托
    /// </summary>
    /// <param name="container">对象</param>
    /// <param name="args">参数</param>
    /// <returns>返回结果（Void返回NULL）</returns>
    public delegate Object ExcuteMethodDelegate(Object container, Object[] args);
}
