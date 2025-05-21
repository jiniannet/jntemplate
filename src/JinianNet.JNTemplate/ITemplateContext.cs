/********************************************************************************
 Copyright (c) jiniannet (http://www.jiniannet.com). All rights reserved.
 Licensed under the MIT license. See licence.txt file in the project root for full license information.
 ********************************************************************************/
using System;
using System.Collections.Generic;
using System.Text;
using JinianNet.JNTemplate.Resources;
using JinianNet.JNTemplate.Runtime;

namespace JinianNet.JNTemplate
{
    /// <summary>
    /// 
    /// </summary>
    public interface ITemplateContext : ILexerOptions
    {
        /// <summary>
        /// Unique key of the template
        /// </summary>
        string Name { get; set; }
        /// <summary>
        /// resource loader
        /// </summary>
        ResourceManager ResourceManager { get; }
        /// <summary>
        /// 
        /// </summary>
        Resolver Resolver { get; }

        /// <summary>
        /// Gets or sets the render mode.
        /// </summary>
        OutMode OutMode { get; set; }

        /// <summary>
        /// Gets or sets the current path.
        /// </summary>
        string CurrentPath { get; set; }

        /// <summary>
        /// Gets or sets the encoding.
        /// </summary>
        Encoding Charset { get; set; }

        /// <summary>
        /// Gets or sets the exception handling.
        /// </summary>
        bool ThrowExceptions { get; set; }

        /// <summary>
        /// Enable or Disenable the cache.
        /// </summary>
        bool EnableCache { get; set; }

        /// <summary>
        /// Gets or sets the debug mode.
        /// </summary>
        bool Debug { get; set; }
        /// <summary>
        /// 
        /// </summary>
        bool IsCompileMode { get; }

        /// <summary>
        /// When an exception occurs, the output of source code is prohibited
        /// </summary>
        bool DisableCodeOutput { get; set; }


        /// <summary>
        /// Gets or sets the <see cref="IScopeProvider"/> of the engine.
        /// </summary>
        IScopeProvider ScopeProvider { set; get; }


        /// <summary>
        /// Gets or sets the cache of the engine.
        /// </summary>
        ITemplateCache Cache { set; get; }

        /// <summary>
        /// Gets or sets the global resource directories of the engine.
        /// </summary>
        /// <value></value>
        List<string> ResourceDirectories { get; }

        /// <summary>
        /// Gets or sets the <see cref="IVariableScope"/> of the context.
        /// </summary>
        IVariableScope TempData { get; set; }
    }
}
