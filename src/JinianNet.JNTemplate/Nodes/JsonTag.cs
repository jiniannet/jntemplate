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
    public class JsonTag : ComplexTag
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="JsonTag"/> class
        /// </summary>
        public JsonTag()
        {
            this.Dict = new Dictionary<ITag, ITag>();
        }

        /// <summary>
        /// Gets or sets the data of the tag.
        /// </summary>
        public Dictionary<ITag, ITag> Dict { get; private set; }
    }
}
