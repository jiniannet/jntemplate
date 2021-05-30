/********************************************************************************
 Copyright (c) jiniannet (http://www.jiniannet.com). All rights reserved.
 Licensed under the MIT license. See licence.txt file in the project root for full license information.
 ********************************************************************************/
using System;

namespace JinianNet.JNTemplate.Nodes
{
    /// <summary>
    /// IncludeTag
    /// </summary>
    [Serializable]
    public class IncludeTag : ComplexTag
    {
        /// <summary>
        /// The path of a tag.
        /// </summary>
        public ITag Path { get; set; }
    }
}