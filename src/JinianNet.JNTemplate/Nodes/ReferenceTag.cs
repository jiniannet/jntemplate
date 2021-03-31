/********************************************************************************
 Copyright (c) jiniannet (http://www.jiniannet.com). All rights reserved.
 Licensed under the MIT license. See licence.txt file in the project root for full license information.
 ********************************************************************************/
using System;

namespace JinianNet.JNTemplate.Nodes
{
    /// <summary>
    /// ReferenceTag
    /// </summary>
    [Serializable]
    public class ReferenceTag : BasisTag
    {
        /// <summary>
        ///  Gets the child of the tag.
        /// </summary>
        public ITag Child
        {
            get
            {
                if (this.Children.Count > 0)
                {
                    return this.Children[0];
                }
                return null;
            }
        }
        /// <inheritdoc />
        public override void AddChild(ITag node)
        {
            if (this.Children.Count == 0)
            {
                base.AddChild(node);
            }
            else
            {
                ChildrenTag child = (ChildrenTag)node;
                if (child == null)
                {
                    throw new ArgumentException("child cannot be null.");
                }
                var parent = this.Children[0];
                child.Parent = (BasisTag)parent;
                this.Children[0] = child;
            }
        } 
    }
}