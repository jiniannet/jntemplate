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
    public class VariableElement
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="VariableElement"/> class
        /// </summary>
        /// <param name="type">type for data.</param>
        /// <param name="value">data value.</param>
        public VariableElement(Type type,object value)
        {
            this.Type = type;
            this.Value = value;
        }
        /// <summary>
        /// a type for data.
        /// </summary>
        public Type Type { get; set; }
        /// <summary>
        /// data value.
        /// </summary>
        public object Value { get; set; } 
    }
}
