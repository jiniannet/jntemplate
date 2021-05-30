/********************************************************************************
 Copyright (c) jiniannet (http://www.jiniannet.com). All rights reserved.
 Licensed under the MIT license. See licence.txt file in the project root for full license information.
 ********************************************************************************/
using System;
using System.Collections;
using System.ComponentModel;

namespace JinianNet.JNTemplate.Nodes
{
    /// <summary>
    /// ForeachTag
    /// </summary>
    [Serializable]
    public class ForeachTag : ComplexTag
    {

        /// <summary>
        /// Gets or sets node name of the tag.
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// Gets or sets data source of the tag.
        /// </summary>
        public ITag Source { get; set; }

        /// <inheritdoc />
        public override bool Out => false;

    }
}