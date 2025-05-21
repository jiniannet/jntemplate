/********************************************************************************
 Copyright (c) jiniannet (http://www.jiniannet.com). All rights reserved.
 Licensed under the MIT license. See licence.txt file in the project root for full license information.
 ********************************************************************************/
using System;
using System.Collections.Generic;
using System.Text;

namespace JinianNet.JNTemplate.Runtime
{
    /// <summary>
    /// Represents an global options.
    /// </summary>
    public class RuntimeOptions : IOptions
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="RuntimeOptions"/> class
        /// </summary>
        internal RuntimeOptions()
        {
            this.Encoding = Encoding.UTF8;
            this.TagFlag = '$';
            this.TagPrefix = "${";
            this.TagSuffix = "}";
            this.ThrowExceptions = true;
            this.DisableCodeOutput = false;
            this.OutMode = OutMode.None;
            this.Mode = EngineMode.Compiled;
            this.DisableeLogogram = false;
            this.EnableCache = true;
            this.ResourceDirectories = new List<string>();
            this.ModelName = "";// "Model";
        }
        /// <inheritdoc />
        public string ModelName { get; set; }
        /// <inheritdoc />
        public bool DisableeLogogram { get; set; }
        /// <inheritdoc />
        public string TagPrefix { get; set; }

        /// <inheritdoc />
        public string TagSuffix { get; set; }

        /// <inheritdoc />
        public char TagFlag { get; set; }

        /// <inheritdoc />
        public Encoding Encoding { set; get; }

        /// <inheritdoc />
        public bool EnableCache { get; set; }

        /// <inheritdoc />
        public bool ThrowExceptions { get; set; }

        /// <inheritdoc />
        public bool DisableCodeOutput { get; set; }

        /// <inheritdoc />
        public OutMode OutMode { get; set; }
        /// <inheritdoc />
        public IVariableScope Data { set; get; }

        /// <inheritdoc />
        public EngineMode Mode { get; set; }

        /// <inheritdoc />
        public List<string> ResourceDirectories { get; }
    }
}