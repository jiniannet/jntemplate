/********************************************************************************
 Copyright (c) jiniannet (http://www.jiniannet.com). All rights reserved.
 Licensed under the MIT license. See licence.txt file in the project root for full license information.
 ********************************************************************************/
using System;

namespace JinianNet.JNTemplate
{
    /// <summary>
    /// 变量元素
    /// </summary>
    public class VariableElement<T>
    {
        /// <summary>
        /// 类型
        /// </summary>
        public Type Type { get; set; }
        /// <summary>
        /// 值
        /// </summary>
        public T Value { get; set; }
    }

}
