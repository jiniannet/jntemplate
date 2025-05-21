/********************************************************************************
 Copyright (c) jiniannet (http://www.jiniannet.com). All rights reserved.
 Licensed under the MIT license. See licence.txt file in the project root for full license information.
 ********************************************************************************/
using System;

namespace JinianNet.JNTemplate.Runtime
{
    /// <summary>
    /// 
    /// </summary>
    public interface ILexerOptions
    {
        /// <summary>
        /// Gets or sets whether disablee logogram .
        /// </summary>
        bool DisableeLogogram { get; set; }
        /// <summary>
        /// Gets or sets the tag prefix .
        /// </summary> 
        string TagPrefix { get; set; }

        /// <summary>
        /// Gets or sets the tag suffix.
        /// </summary> 
        string TagSuffix { get; set; }

        /// <summary>
        /// Gets or sets the tag flag.
        /// </summary> 
        char TagFlag { get; set; }
    }
}
