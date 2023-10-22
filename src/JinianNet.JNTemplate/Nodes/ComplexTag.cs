/********************************************************************************
 Copyright (c) jiniannet (http://www.jiniannet.com). All rights reserved.
 Licensed under the MIT license. See licence.txt file in the project root for full license information.
 ********************************************************************************/
using System;

namespace JinianNet.JNTemplate.Nodes
{
    /// <summary>
    /// ComplexTag
    /// </summary>
    [Serializable]
    public abstract class ComplexTag : Tag, ITag
    {
        /// <inheritdoc />
        public override bool IsSimple => false;
    }
}
