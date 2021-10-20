/********************************************************************************
 Copyright (c) jiniannet (http://www.jiniannet.com). All rights reserved.
 Licensed under the MIT license. See licence.txt file in the project root for full license information.
 ********************************************************************************/
using JinianNet.JNTemplate.Caching;
using JinianNet.JNTemplate.CodeCompilation;
using JinianNet.JNTemplate.Configuration;
using JinianNet.JNTemplate.Dynamic;
using JinianNet.JNTemplate.Parsers;
using JinianNet.JNTemplate.Resources;
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
            this.TypeDetectPattern = TypeDetect.Standard;
            this.OutMode = OutMode.None;
            this.Mode = EngineMode.Compiled;
            this.DisableeLogogram = false;
            this.EnableCache = true;
        }
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
        [Obsolete("please use the `Mode`")]
        public bool EnableCompile
        {
            get => Mode == EngineMode.Compiled;
            set => Mode = value ? EngineMode.Compiled : EngineMode.Interpreted;
        }

        /// <inheritdoc />
        [Obsolete("please use the `EnableCache`")]
        public bool EnableTemplateCache { get => EnableCache; set => EnableCache = value; }

        /// <inheritdoc />
        public bool EnableCache { get; set; }

        /// <inheritdoc />
        public bool ThrowExceptions { get; set; }
        /// <inheritdoc />
        public TypeDetect TypeDetectPattern { get; set; }
        /// <inheritdoc />
        public OutMode OutMode { get; set; }
        /// <inheritdoc />
        public IVariableScope Data { set; get; }

        /// <inheritdoc />
        public EngineMode Mode { get; set; }
    }
}