/********************************************************************************
 Copyright (c) jiniannet (http://www.jiniannet.com). All rights reserved.
 Licensed under the MIT license. See licence.txt file in the project root for full license information.
 ********************************************************************************/

using System;

namespace JinianNet.JNTemplate.Nodes
{
    /// <summary>
    /// ArrayTag
    /// </summary>
    [Serializable]
    public class ArrayTag : SpecialTag
    {
        /// <summary>
        /// Gets or sets the value of the tag.
        /// </summary>
        public Object[] Value { get; set; }
    }
}