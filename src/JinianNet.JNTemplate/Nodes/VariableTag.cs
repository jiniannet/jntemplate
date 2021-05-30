/********************************************************************************
 Copyright (c) jiniannet (http://www.jiniannet.com). All rights reserved.
 Licensed under the MIT license. See licence.txt file in the project root for full license information.
 ********************************************************************************/
using System;

namespace JinianNet.JNTemplate.Nodes
{
    /// <summary>
    /// variable
    /// </summary>
    [Serializable]
    public class VariableTag : ChildrenTag
    {
        /// <summary>
        /// The name of tag.
        /// </summary>
        public string Name { get; set; }
    }
}
