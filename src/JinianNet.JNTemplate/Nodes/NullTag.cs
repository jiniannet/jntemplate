/********************************************************************************
 Copyright (c) jiniannet (http://www.jiniannet.com). All rights reserved.
 Licensed under the MIT license. See licence.txt file in the project root for full license information.
 ********************************************************************************/
using System;

namespace JinianNet.JNTemplate.Nodes
{
    /// <summary>
    /// NullTag
    /// </summary>
    [Serializable]
    public class NullTag : SpecialTag
    {
        /// <inheritdoc />
        public override bool IsPrimitive => true;
        /// <inheritdoc />
        public override string ToString()
        {
            return string.Empty;
        }
    }
}