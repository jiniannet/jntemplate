/********************************************************************************
 Copyright (c) jiniannet (http://www.jiniannet.com). All rights reserved.
 Licensed under the MIT license. See licence.txt file in the project root for full license information.
 ********************************************************************************/
using JinianNet.JNTemplate.Resources;
using System.Collections.Generic;
namespace JinianNet.JNTemplate.Configuration
{
    /// <summary>
    /// The default config of the engine.
    /// </summary>
    public class EngineConfig : IConfig
    {
        /// <inheritdoc />
        public List<string> ResourceDirectories { get; set; } = new List<string>();
        /// <inheritdoc />
        public string Charset { get; set; } = "utf-8";
        /// <inheritdoc />
        public string TagPrefix { get; set; }
        /// <inheritdoc />
        public string TagSuffix { get; set; }
        /// <inheritdoc />
        public char TagFlag { get; set; } = '$';
        /// <inheritdoc />
        public bool ThrowExceptions { get; set; } = true;
        /// <inheritdoc />
        public bool StripWhiteSpace { get; set; } = false;
        /// <inheritdoc />
        public bool EnableTemplateCache { get; set; } = true;
        /// <inheritdoc />
        public bool IgnoreCase { get; set; } = true;
        /// <inheritdoc />
        public bool DisableeLogogram { get; set; } = false;
        /// <inheritdoc />
        public IResourceLoader Loader { get; set; }
        /// <inheritdoc />
        public List<Parsers.ITagParser> TagParsers { get; set; } = new List<Parsers.ITagParser>();

        /// <summary>
        /// Created the default config.
        /// </summary>
        /// <returns></returns>
        public static EngineConfig CreateDefault()
        {
            return new EngineConfig();
        }
    }
}