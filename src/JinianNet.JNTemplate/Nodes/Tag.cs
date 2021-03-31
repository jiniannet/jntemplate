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
        private Token first, last;
        private Collection<ITag> children = new Collection<ITag>();
        /// <summary>
        /// Gets the childer of the tag.
        /// </summary>
        public Collection<ITag> Children
        {
            get { return children; }
        }
        /// <summary>
        /// Adds an tag to the end of the children.
        /// </summary>
        /// <param name="node"></param>
        public virtual void AddChild(ITag node)
        {
            if (node != null)
            {
                children.Add(node);
            }
        }

        /// <summary>
        /// Returns the first token of the tag.
        /// </summary>
        public Token FirstToken
        {
            get { return this.first; }
            set { this.first = value; }
        }
        /// <summary>
        /// Returns the last token of the tag.
        /// </summary>
        public Token LastToken
        {
            set { this.last = value; }
            get { return this.last; }
        }
    }
}