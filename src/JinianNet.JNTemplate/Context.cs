/********************************************************************************
 Copyright (c) jiniannet (http://www.jiniannet.com). All rights reserved.
 Licensed under the MIT license. See licence.txt file in the project root for full license information.
 ********************************************************************************/
using System;
using System.Text;
using JinianNet.JNTemplate.Caching;
using JinianNet.JNTemplate.Hosting;

namespace JinianNet.JNTemplate
{
    /// <summary>
    /// Base class with Context.
    /// </summary>
    [Serializable]
    public class Context
    {
        /// <summary>
        /// 
        /// </summary>
        /// <param name="hostEnvironment"></param>
        public Context(IHostEnvironment hostEnvironment)
        {
            this.OutMode = hostEnvironment.Options.OutMode;
            //this.CurrentPath = hostEnvironment.RootPath;
            this.Charset = hostEnvironment.Options.Encoding;
            this.ThrowExceptions = hostEnvironment.Options.ThrowExceptions;
            this.Environment = hostEnvironment;
            this.EnableCache = hostEnvironment.Options.EnableCache;
            this.Debug = System.Diagnostics.Debugger.IsAttached;

        }

        /// <summary>
        /// Gets or sets the render mode.
        /// </summary>
        public OutMode OutMode { get; set; }

        /// <summary>
        /// Gets or sets the current path.
        /// </summary>
        public string CurrentPath { get; set; }

        /// <summary>
        /// Gets or sets the encoding.
        /// </summary>
        public Encoding Charset { get; set; }

        /// <summary>
        /// Gets or sets the exception handling.
        /// </summary>
        public bool ThrowExceptions { get; set; }

        /// <summary>
        ///  Gets or sets the cache of the environment.
        /// </summary>
        internal IHostEnvironment Environment { get; }

        /// <summary>
        /// Gets the cache of the engine.
        /// </summary>
        public ICache Cache => Environment.Cache;

        /// <summary>
        /// Enable or Disenable the cache.
        /// </summary>
        public bool EnableCache { get; set; }

        /// <summary>
        /// Gets or sets the debug mode.
        /// </summary>
        public bool Debug { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public EngineMode Mode => Environment.Options.Mode;
    }
}
