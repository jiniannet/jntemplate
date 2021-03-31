/********************************************************************************
 Copyright (c) jiniannet (http://www.jiniannet.com). All rights reserved.
 Licensed under the MIT license. See licence.txt file in the project root for full license information.
 ********************************************************************************/
using System;

namespace JinianNet.JNTemplate.Nodes
{
    /// <summary>
    /// IndexValueTag
    /// </summary>
    [Serializable]
    public class IndexValueTag : ChildrenTag
    {
        /// <summary>
        /// The zero-based index in the tag.
        /// </summary>
        public ITag Index { get; set; } 
    }
}
