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
    public class VariableElement
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="VariableElement"/> class
        /// </summary>
        /// <param name="type">The type.see the <see cref="Type"/>.</param>
        /// <param name="value">The value.</param>
        public VariableElement(Type type, object value)
        {
            this.Type = type;
            this.Value = value;
        }
        /// <summary>
        /// Gets or sets the type of the element.
        /// </summary>
        public Type Type { get; set; }
        /// <summary>
        /// Gets or sets the value of the element.
        /// </summary>
        public object Value { get; set; }
    }
}
