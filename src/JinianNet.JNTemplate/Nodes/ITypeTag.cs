/********************************************************************************
 Copyright (c) jiniannet (http://www.jiniannet.com). All rights reserved.
 Licensed under the MIT license. See licence.txt file in the project root for full license information.
 ********************************************************************************/
using System;

namespace JinianNet.JNTemplate.Nodes
{
    /// <summary>
    /// 基本类型标签
    /// </summary>
    public interface ITypeTag
    {
        /// <summary>
        /// 值
        /// </summary>
        object Value { get; set; }
    }
}