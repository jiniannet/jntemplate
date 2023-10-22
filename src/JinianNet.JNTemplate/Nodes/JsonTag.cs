/********************************************************************************
 Copyright (c) jiniannet (http://www.jiniannet.com). All rights reserved.
 Licensed under the MIT license. See licence.txt file in the project root for full license information.
 ********************************************************************************/
using System;
using System.Collections.Generic;

namespace JinianNet.JNTemplate.Nodes
{
    /// <summary>
    /// JsonTag
    /// </summary>
    [Serializable]
    public class JsonTag : BasisTag
    {
        /// <summary>
        /// Gets or sets the data of the tag.
        /// </summary>
        public Dictionary<ITag, ITag> Dict { get; } = new Dictionary<ITag, ITag>();
    }
}
