/********************************************************************************
 Copyright (c) jiniannet (http://www.jiniannet.com). All rights reserved.
 Licensed under the MIT license. See licence.txt file in the project root for full license information.
 ********************************************************************************/

using System;

namespace JinianNet.JNTemplate.Nodes
{
    /// <summary>
    /// The tag of base type .
    /// </summary>
    /// <typeparam name="T"></typeparam>
    [Serializable]
    public abstract class TypeTag<T> : SpecialTag, ITypeTag
    {
        /// <summary>
        /// Gets or sets the value of the tag.
        /// </summary>
        public T Value { get; set; }

        /// <summary>
        /// Gets or sets the value of the tag.
        /// </summary>
        object ITypeTag.Value
        {
            get { return this.Value; }
            set { this.Value = (T)value; }
        }
    }
}