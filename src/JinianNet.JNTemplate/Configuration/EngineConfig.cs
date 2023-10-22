/********************************************************************************
 Copyright (c) jiniannet (http://www.jiniannet.com). All rights reserved.
 Licensed under the MIT license. See licence.txt file in the project root for full license information.
 ********************************************************************************/
using JinianNet.JNTemplate.Resources;
using System;
using System.Collections.Generic;
using System.Text;

namespace JinianNet.JNTemplate.Configuration
{
    /// <summary>
    /// The default config of the engine.
    /// </summary>
    public class EngineConfig : IConfig
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="EngineConfig"/> class
        /// </summary>
        public EngineConfig() :
            base()
        {

        }

        /// <summary>
        /// Created the default config.
        /// </summary>
        /// <returns></returns>
        public static EngineConfig CreateDefault()
        {
            return new EngineConfig();
        }

        /// <inheritdoc />
        public bool DisableeLogogram { get; set; } = false;
        /// <inheritdoc />
        public string TagPrefix { get; set; } = "${";

        /// <inheritdoc />
        public string TagSuffix { get; set; } = "}";

        /// <inheritdoc />
        public char TagFlag { get; set; } = '$';

        /// <inheritdoc />
        public IDictionary<string, object> GlobalData { get; set; } = new Dictionary<string, object>();
        /// <inheritdoc />
        public Encoding Encoding { set; get; } = Encoding.UTF8;

        /// <inheritdoc />
        public List<string> ResourceDirectories { get; set; } = new List<string>();

        /// <inheritdoc />
        public bool EnableCompile { get; set; } = true;

        /// <inheritdoc />
        public bool EnableTemplateCache { get; set; } = true;

        /// <inheritdoc />
        public bool ThrowExceptions { get; set; } = true;

        // /// <inheritdoc />
        //public TypeDetect TypeDetectPattern { get; set; } = TypeDetect.Standard;
        /// <inheritdoc />
        public OutMode OutMode { get; set; } = OutMode.None;
    }
}