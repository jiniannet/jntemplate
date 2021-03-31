/********************************************************************************
 Copyright (c) jiniannet (http://www.jiniannet.com). All rights reserved.
 Licensed under the MIT license. See licence.txt file in the project root for full license information.
 ********************************************************************************/
using System;

namespace JinianNet.JNTemplate.Nodes
{
    /// <summary>
    /// LoadTag
    /// </summary>
    [Serializable]
    public class LoadTag : BlockTag
    {
        private ITag path;
        /// <summary>
        /// The path of a tag.
        /// </summary>
        public ITag Path
        {
            get { return this.path; }
            set { this.path = value; }
        } 
    }
}