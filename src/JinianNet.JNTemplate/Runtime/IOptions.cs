/********************************************************************************
 Copyright (c) jiniannet (http://www.jiniannet.com). All rights reserved.
 Licensed under the MIT license. See licence.txt file in the project root for full license information.
 ********************************************************************************/

using JinianNet.JNTemplate.Configuration;
using JinianNet.JNTemplate.Resources;
using System;
using System.Collections.Generic;
using System.Text;

namespace JinianNet.JNTemplate.Runtime
{
    /// <summary>
    /// Represents an global options.
    /// </summary>
    public interface IOptions : ILexerOptions
    {
        /// <summary>
        /// 
        /// </summary>
        string ModelName { get; set; } 

        /// <summary>
        /// Gets or sets the <see cref="Encoding"/> of the engine.
        /// </summary>
        Encoding Encoding { set; get; }

        /// <summary>
        /// Enable or disenable the compile mode.
        /// </summary>
        EngineMode Mode { get; set; }

        /// <summary>
        /// Enable or disenable the cache.
        /// </summary>
        bool EnableCache { get; set; }

        /// <summary>
        /// Gets or sets whether throw exceptions.
        /// </summary>
        bool ThrowExceptions { get; set; } 

        /// <summary>
        /// Gets or sets the output mode.
        /// </summary>
        OutMode OutMode { get; set; }

        /// <summary>
        /// Gets or sets the global data of the engine.
        /// </summary>
        IVariableScope Data { set; get; }
        /// <summary>
        /// Gets or sets the global resource directories of the engine.
        /// </summary>
        /// <value></value>
        List<string> ResourceDirectories { get; }

        /// <summary>
        /// 
        /// </summary>
        bool DisableCodeOutput { get; set; }
    }
}
