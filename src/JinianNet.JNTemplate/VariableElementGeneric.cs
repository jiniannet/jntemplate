/********************************************************************************
 Copyright (c) jiniannet (http://www.jiniannet.com). All rights reserved.
 Licensed under the MIT license. See licence.txt file in the project root for full license information.
 ********************************************************************************/
using System;

namespace JinianNet.JNTemplate
{
    /// <summary>
    /// Defines a variable element.
    /// </summary>
    public class VariableElement<T>
    {
        /// <summary>
        /// a type for data.
        /// </summary>
        public Type Type { get; set; }
        /// <summary>
        /// data value.
        /// </summary>
        public T Value { get; set; }
    }

}
