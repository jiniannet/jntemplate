/********************************************************************************
 Copyright (c) jiniannet (http://www.jiniannet.com). All rights reserved.
 Licensed under the MIT license. See licence.txt file in the project root for full license information.
 ********************************************************************************/
using System;

namespace JinianNet.JNTemplate.Nodes
{
    /// <summary>
    /// ForTag
    /// </summary>
    [Serializable]
    public class ForTag : ComplexTag
    {
        /// <summary>
        /// Gets or sets the initial of the tag.
        /// </summary>
        public ITag Initial { get; set; }

        /// <summary>
        /// Gets or sets the condition of the tag.
        /// </summary>
        public ITag Condition { get; set; }

        /// <summary>
        /// do things.
        /// </summary>
        public ITag Do { get; set; }

        /// <inheritdoc />
        public override bool Out => false;

    }
}