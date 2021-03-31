/********************************************************************************
 Copyright (c) jiniannet (http://www.jiniannet.com). All rights reserved.
 Licensed under the MIT license. See licence.txt file in the project root for full license information.
 ********************************************************************************/
using System;
namespace JinianNet.JNTemplate.Nodes
{
    /// <summary>
    /// ChildrenTag
    /// </summary>
    [Serializable]
    public abstract class ChildrenTag : BasisTag
    {
        /// <summary>
        /// Gets or sets the parent tag of the tag.
        /// </summary>
        public BasisTag Parent { get; set; }
    }
}
