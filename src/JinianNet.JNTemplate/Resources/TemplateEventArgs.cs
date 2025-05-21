/********************************************************************************
 Copyright (c) jiniannet (http://www.jiniannet.com). All rights reserved.
 Licensed under the MIT license. See licence.txt file in the project root for full license information.
 ********************************************************************************/
using System; 

namespace JinianNet.JNTemplate.Resources
{
    /// <summary>
    /// 
    /// </summary>
    public class TemplateEventArgs : EventArgs
    {

        /// <summary>
        /// Initializes a new instance of the TemplateEventArgs class.
        /// </summary>
        /// <param name="fullPath"></param>
        /// <param name="keys"></param>
        public TemplateEventArgs(string fullPath, string[] keys)
        {
            this.FullPath = fullPath;
            this.Keys = keys;
        }


        /// <summary>
        /// Gets the fully qualified path of the affected file or directory.
        /// </summary> 
        public string FullPath { get; }


        /// <summary>
        /// The name of the affected file or directory.
        /// </summary>
        public string[] Keys { get; }
    }
}
