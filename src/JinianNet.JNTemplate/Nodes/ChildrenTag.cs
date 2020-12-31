/********************************************************************************
 Copyright (c) jiniannet (http://www.jiniannet.com). All rights reserved.
 Licensed under the MIT license. See licence.txt file in the project root for full license information.
 ********************************************************************************/
using System;
namespace JinianNet.JNTemplate.Nodes
{
    /// <summary>
    /// 基本标签(ParseResult)
    /// </summary>
    [Serializable]
    public abstract class ChildrenTag : BasisTag
    {
        /// <summary>
        /// 父标签
        /// </summary>
        public BasisTag Parent { get; set; }
    }
}
