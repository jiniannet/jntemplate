/********************************************************************************
 Copyright (c) jiniannet (http://www.jiniannet.com). All rights reserved.
 Licensed under the MIT license. See licence.txt file in the project root for full license information.
 ********************************************************************************/
using JinianNet.JNTemplate;
using JinianNet.JNTemplate.Caching;
using JinianNet.JNTemplate.CodeCompilation;
using JinianNet.JNTemplate.Dynamic;
using JinianNet.JNTemplate.Parsers;
using JinianNet.JNTemplate.Resources;
using JinianNet.JNTemplate.Runtime;
using System.Collections.Generic;

namespace JinianNet.JNTemplate.Hosting
{
    /// <summary>
    /// Provides information about the hosting environment an application is running in.
    /// </summary>
    public interface IHostEnvironment
    {
        /// <summary>
        /// Gets or sets the options of the host environment.
        /// </summary>
        IOptions Options { get; set; }
        /// <summary>
        /// Gets or sets the name of the application. 
        /// </summary>
        string ApplicationName { get; set; }
        /// <summary>
        /// 
        /// </summary>
        string RootPath { get; set; }

        /// <summary>
        /// Gets or sets the name of the environment. The host automatically sets this property to the value of the of the "environment" key as specified in configuration.
        /// </summary>
        string EnvironmentName { get; set; }

        /// <summary>
        /// Gets or sets the compiler result collection.
        /// </summary>
        ResultCollection<IResult> Results { get; }
        /// <summary>
        /// Gets or sets the tag parser of the engine.
        /// </summary>
        TagParser Parser { set; get; }
        /// <summary>
        /// Gets or sets the tag <see cref="CompileBuilder"/> of the engine.
        /// </summary>
        CompileBuilder Builder { set; get; }

        /// <summary>
        /// Gets or sets the tag <see cref="CompileBuilder"/> of the engine.
        /// </summary>
        TypeGuesser Guesser { set; get; }

        /// <summary>
        /// Gets or sets the tag <see cref="ExecutorBuilder"/> of the engine.
        /// </summary>
        ExecutorBuilder ExecutorBuilder { set; get; }

        /// <summary>
        /// Gets or sets the <see cref="IScopeProvider"/> of the engine.
        /// </summary>
        IScopeProvider ScopeProvider { set; get; }


        /// <summary>
        /// Gets or sets the cache of the engine.
        /// </summary>
        ICache Cache { set; get; }

        /// <summary>
        /// Gets or sets the <see cref="IResourceLoader"/> of the engine.
        /// </summary>
        IResourceLoader Loader { set; get; }

        /// <summary>
        /// Gets or sets the environment variable of the engine.
        /// </summary>
        Dictionary<string, string> EnvironmentVariable { get; }
    }
}
