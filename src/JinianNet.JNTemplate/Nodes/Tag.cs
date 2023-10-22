/********************************************************************************
 Copyright (c) jiniannet (http://www.jiniannet.com). All rights reserved.
 Licensed under the MIT license. See licence.txt file in the project root for full license information.
 ********************************************************************************/
using System.Collections.ObjectModel;


namespace JinianNet.JNTemplate.Nodes
{
    /// <summary>
    /// Base class of the tag.
    /// </summary>
    public abstract class Tag : ITag
    {
        /// <inheritdoc />
        public TagCollection Children { get; } = new TagCollection();
        /// <inheritdoc />
        public Token FirstToken { get; set; }
        /// <inheritdoc />
        public Token LastToken { get; set; }
        /// <inheritdoc />
        public virtual bool Out => true;
        /// <inheritdoc />
        public bool Previous { get; set; }
        /// <inheritdoc />
        public bool Next { get; set; }
        /// <inheritdoc />
        public virtual bool IsPrimitive => false;
        /// <inheritdoc />
        public abstract bool IsSimple { get; }
        /// <inheritdoc />
        public virtual void AddChild(ITag node)
        {
            Children.Add(node);
        }
    }
}