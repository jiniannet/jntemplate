/********************************************************************************
 Copyright (c) jiniannet (http://www.jiniannet.com). All rights reserved.
 Licensed under the MIT license. See licence.txt file in the project root for full license information.
 ********************************************************************************/

using System;
using JinianNet.JNTemplate.Nodes;

namespace JinianNet.JNTemplate.CodeCompilation
{
    /// <summary>
    /// 
    /// </summary>
    public interface ITagCompileResult
    {
        /// <summary>
        /// 
        /// </summary>
        object Result { get; }
        /// <summary>
        /// 
        /// </summary>
        bool IsMethod { get; set; }
        /// <summary>
        /// 
        /// </summary>
        ITag Tag { get; set; }

        /// <summary>
        /// 
        /// </summary>
        Type Type { get; set; }
    }
}
