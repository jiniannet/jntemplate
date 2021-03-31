/********************************************************************************
 Copyright (c) jiniannet (http://www.jiniannet.com). All rights reserved.
 Licensed under the MIT license. See licence.txt file in the project root for full license information.
 ********************************************************************************/
using System;

namespace JinianNet.JNTemplate.Nodes
{
    /// <summary>
    /// ElseifTag
    /// </summary>
    [Serializable]
    public class ElseifTag : ComplexTag
    {

        private ITag condition;
        /// <summary>
        /// Gets or sets the condition of the tag.
        /// </summary>
        public virtual ITag Condition
        {
            get { return this.condition; }
            set { this.condition = value; }
        }
    }
}