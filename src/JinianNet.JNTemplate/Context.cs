/********************************************************************************
 Copyright (c) jiniannet (http://www.jiniannet.com). All rights reserved.
 Licensed under the MIT license. See licence.txt file in the project root for full license information.
 ********************************************************************************/
using System;
using System.Text;
using JinianNet.JNTemplate.Caching;
using JinianNet.JNTemplate.Runtime;

namespace JinianNet.JNTemplate
{
    /// <summary>
    /// Base class with Context.
    /// </summary>
    [Serializable]
    public class Context
    {
        /// <summary>
        /// Strip white-space characters from the template
        /// </summary>
        public bool StripWhiteSpace { get; set; } = false;

        /// <summary>
        /// Gets or sets the current path.
        /// </summary>
        public string CurrentPath { get; set; }

        /// <summary>
        /// Gets or sets the encoding.
        /// </summary>
        public Encoding Charset { get; set; } = Encoding.UTF8;

        /// <summary>
        /// Gets or sets the exception handling.
        /// </summary>
        public bool ThrowExceptions { get; set; } = true;
        /// <summary>
        /// Gets or sets the cache of the context.
        /// </summary>
        internal RuntimeOptions Options { get; set; }
        /// <summary>
        /// Gets the cache of the engine.
        /// </summary>
        public ICache Cache => Options.Cache;
    }
}
