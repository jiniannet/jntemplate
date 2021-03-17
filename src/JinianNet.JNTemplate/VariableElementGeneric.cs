/********************************************************************************
 Copyright (c) jiniannet (http://www.jiniannet.com). All rights reserved.
 Licensed under the MIT license. See licence.txt file in the project root for full license information.
 ********************************************************************************/
using System;

namespace JinianNet.JNTemplate
{
    /// <summary>
    /// Represents an variable element.
    /// </summary>
    public class VariableElement<T>
    {
        /// <summary>
        /// Gets or sets the type of the element.
        /// </summary>
        public Type Type { get; set; }
        /// <summary>
        /// Gets or sets the value of the element.
        /// </summary>
        public T Value { get; set; }
    }

}
