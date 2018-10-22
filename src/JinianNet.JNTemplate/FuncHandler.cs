/********************************************************************************
 Copyright (c) jiniannet (http://www.jiniannet.com). All rights reserved.
 Licensed under the MIT license. See licence.txt file in the project root for full license information.
 ********************************************************************************/
using System;

namespace JinianNet.JNTemplate
{
    /// <summary>
    /// 方法标签委托
    /// </summary>
    /// <param name="args">方法参数</param>
    /// <returns>object</returns>
    public delegate object FuncHandler(params object[] args);
}