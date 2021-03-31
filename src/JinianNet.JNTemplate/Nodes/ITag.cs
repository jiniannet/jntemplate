/********************************************************************************
 Copyright (c) jiniannet (http://www.jiniannet.com). All rights reserved.
 Licensed under the MIT license. See licence.txt file in the project root for full license information.
 ********************************************************************************/
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
        Collection<ITag> Children { get; }

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

    }
}
