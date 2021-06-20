/********************************************************************************
 Copyright (c) jiniannet (http://www.jiniannet.com). All rights reserved.
 Licensed under the MIT license. See licence.txt file in the project root for full license information.
 ********************************************************************************/

using JinianNet.JNTemplate.Caching;
using JinianNet.JNTemplate.Dynamic;
using JinianNet.JNTemplate.Resources;
using System;
using System.Collections.Generic;
using System.Text;

namespace JinianNet.JNTemplate.Configuration
{
    /// <summary>
    /// The config of the engine.
    /// </summary>
    public interface IConfig
    {
        /// <summary>
        /// Gets or sets the charset of the engine.
        /// </summary>
        [Obsolete("please use Encoding")]
        string Charset { get; set; }

        /// <summary>
        /// Gets or sets whether strip white-space.
        /// </summary>
        [Obsolete("please use OutMode")]
        bool StripWhiteSpace { get; set; }

        /// <summary>
        /// IgnoreCase
        /// </summary>
        [Obsolete]
        bool IgnoreCase { get; set; }

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

        /// <summary>
        /// Gets or sets the global data of the engine.
        /// </summary>
        IVariableScope Data { get; }

        /// <summary>
        /// Gets or sets the <see cref="Encoding"/> of the engine.
        /// </summary>
        Encoding Encoding { set; get; }

        /// <summary>
        /// Gets or sets the cache of the engine.
        /// </summary>
        ICache Cache { get; }

        /// <summary>
        /// Gets or sets the global resource directories of the engine.
        /// </summary>
        /// <value></value>
        List<string> ResourceDirectories { get; }

        /// <summary>
        /// Enable or disenable the compile mode.
        /// </summary>
        bool EnableCompile { get; }

        /// <summary>
        /// Enable or disenable the cache.
        /// </summary>
        bool EnableTemplateCache { get; }

        /// <summary>
        /// Gets or sets whether throw exceptions.
        /// </summary>
        bool ThrowExceptions { get; set; }

        // /// <summary>
        // /// Gets or sets whether strip white-space.
        // /// </summary>
        //bool StripWhiteSpace { get; set; }

        /// <summary>
        /// Gets or sets the detect patterns.
        /// </summary>
        TypeDetect TypeDetectPattern { get; set; }
        /// <summary>
        /// Gets or sets the output mode.
        /// </summary>
        OutMode OutMode { get; set; }
    }
}
