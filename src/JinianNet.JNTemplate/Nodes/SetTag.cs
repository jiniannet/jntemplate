/********************************************************************************
 Copyright (c) jiniannet (http://www.jiniannet.com). All rights reserved.
 Licensed under the MIT license. See licence.txt file in the project root for full license information.
 ********************************************************************************/
using System;
using System.IO;

namespace JinianNet.JNTemplate.Nodes
{
    /// <summary>
    /// SetTag
    /// </summary>
    [Serializable]
    public class SetTag : ComplexTag
    {
        private string name;
        private ITag value;

        /// <summary>
        ///  Gets the name of the tag.
        /// </summary>
        public string Name
        {
            get { return this.name; }
            set { this.name = value; }
        }

        /// <summary>
        ///  Gets the value of the tag.
        /// </summary>
        public ITag Value
        {
            get { return this.value; }
            set { this.value = value; }
        } 
    }
}