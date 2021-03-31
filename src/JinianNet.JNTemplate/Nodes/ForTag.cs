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
        private ITag initial;
        private ITag condition;
        private ITag dothing;

        /// <summary>
        /// Gets or sets the initial of the tag.
        /// </summary>
        public ITag Initial
        {
            get { return this.initial; }
            set { this.initial = value; }
        }

        /// <summary>
        /// Gets or sets the condition of the tag.
        /// </summary>
        public ITag Condition
        {
            get { return this.condition; }
            set { this.condition = value; }
        }

        /// <summary>
        /// do things.
        /// </summary>
        public ITag Do
        {
            get { return this.dothing; }
            set { this.dothing = value; }
        }

    }
}