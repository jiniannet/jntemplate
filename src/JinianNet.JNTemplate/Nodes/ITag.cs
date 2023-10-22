/********************************************************************************
 Copyright (c) jiniannet (http://www.jiniannet.com). All rights reserved.
 Licensed under the MIT license. See licence.txt file in the project root for full license information.
 ********************************************************************************/
using System.Collections.Generic;
using System.Collections.ObjectModel;

namespace JinianNet.JNTemplate.Nodes
{
    /// <summary>
    /// ITag
    /// </summary>
    public interface ITag
    {
        /// <summary>
        /// Gets the childer of the tag.
        /// </summary>
        TagCollection Children { get; }

        /// <summary>
        /// Adds an tag to the end of the children.
        /// </summary>
        /// <param name="node"></param>
        void AddChild(ITag node);

        /// <summary>
        /// Gets or sets the first token of the tag.
        /// </summary>
        Token FirstToken { get; set; }
        /// <summary>
        /// Gets or sets the last token of the tag.
        /// </summary>
        Token LastToken { get; set; }
        /// <summary>
        /// Gets or sets the output mode
        /// </summary>
        bool Out { get; }
        /// <summary>
        /// Gets or sets the previous tag output mode
        /// </summary>
        bool Previous { get; set; }

        /// <summary>
        /// 
        /// </summary>
        bool Next { get; set; }
        /// <summary>
        /// 
        /// </summary>
        bool IsPrimitive { get; }
        /// <summary>
        /// Gets a Boolean value indicating whether this Tag is simple type.
        /// </summary>
        bool IsSimple { get; }
    }
}
