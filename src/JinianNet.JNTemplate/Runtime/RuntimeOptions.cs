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
        private bool enabled;
        /// <summary>
        /// Initializes a new instance of the <see cref="RuntimeOptions"/> class
        /// </summary>
        internal RuntimeOptions()
            : this(true)
        {

        }


        /// <summary>
        /// Initializes a new instance of the <see cref="RuntimeOptions"/> class
        /// </summary>
        /// <param name="enableCompile"></param>
        internal RuntimeOptions(bool enableCompile)
        {
            CompilerResults = new ResultCollection<ICompilerResult>();
            this.Data = null;
            this.Variable = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase);
            //IgnoreCase
            this.Cache = new MemoryCache();
            this.ResourceDirectories = new List<string>();
            this.Loader = new FileLoader();
            this.Encoding = Encoding.UTF8;
            this.TagFlag = '$';
            this.TagPrefix = "${";
            this.TagSuffix = "}";
            this.StripWhiteSpace = false;
            this.ThrowExceptions = true;
            this.Parser = new TagParser();
            this.EnableCompile = enableCompile;
            this.TypeDetectPattern = TypeDetect.Standard;
            this.OutMode = OutMode.None;
        }


        private void Initialize()
        {
            if (this.enabled)
            {
                this.Builder = new CompileBuilder();
                this.Guesser = new TypeGuesser();
            }
            else
            {
                this.ExecutorBuilder = new ExecutorBuilder();
            }
        }

        ///// <summary>
        ///// Initializes instance.
        ///// </summary>
        ///// <param name="dict">The <see cref="IDictionary{TKey, TValue}"/>.</param>
        //public void Initialization(IDictionary<string, string> dict)
        //{
        //    foreach (KeyValuePair<string, string> node in dict)
        //    {
        //        if (string.IsNullOrEmpty(node.Value))
        //        {
        //            continue;
        //        }
        //        this.Variable[node.Key] = node.Value;
        //    }
        //}

        /// <summary>
        /// Gets or sets whether disablee logogram .
        /// </summary>
        public bool DisableeLogogram { get; set; }
        /// <summary>
        /// Gets or sets the tag prefix .
        /// </summary> 
        public string TagPrefix { get; set; }

        /// <summary>
        /// Gets or sets the tag suffix.
        /// </summary> 
        public string TagSuffix { get; set; }

        /// <summary>
        /// Gets or sets the tag flag.
        /// </summary> 
        public char TagFlag { get; set; }

        /// <summary>
        /// Gets or sets the global data of the engine.
        /// </summary>
        public VariableScope Data { internal set; get; }

        /// <summary>
        /// Gets or sets the <see cref="IResourceLoader"/> of the engine.
        /// </summary>
        public IResourceLoader Loader { internal set; get; }

        /// <summary>
        /// Gets or sets the <see cref="Encoding"/> of the engine.
        /// </summary>
        public Encoding Encoding { set; get; }

        /// <summary>
        /// Gets or sets the cache of the engine.
        /// </summary>
        public ICache Cache { internal set; get; }

        /// <summary>
        /// Gets or sets the global resource directories of the engine.
        /// </summary>
        /// <value></value>
        public List<string> ResourceDirectories { internal set; get; }
        /// <summary>
        /// Gets or sets the environment variable of the engine.
        /// </summary>
        public Dictionary<string, string> Variable { internal set; get; }
        /// <summary>
        /// Gets or sets the tag parser of the engine.
        /// </summary>
        public TagParser Parser { internal set; get; }
        /// <summary>
        /// Gets or sets the tag <see cref="CompileBuilder"/> of the engine.
        /// </summary>
        public CompileBuilder Builder { internal set; get; }

        /// <summary>
        /// Gets or sets the tag <see cref="CompileBuilder"/> of the engine.
        /// </summary>
        public TypeGuesser Guesser { internal set; get; }

        /// <summary>
        /// Gets or sets the tag <see cref="ExecutorBuilder"/> of the engine.
        /// </summary>
        public ExecutorBuilder ExecutorBuilder { internal set; get; }

        /// <summary>
        /// Enable or disenable the compile mode.
        /// </summary>
        public bool EnableCompile
        {
            internal set { 
                enabled = value;
                Initialize();
            }
            get { return enabled; }
        }

        /// <summary>
        /// Enable or disenable the cache.
        /// </summary>
        public bool EnableTemplateCache { internal set; get; } = true;

        /// <summary>
        /// Gets or sets the compiler result collection.
        /// </summary>
        public ResultCollection<ICompilerResult> CompilerResults { get; }

        /// <summary>
        /// Gets or sets whether throw exceptions.
        /// </summary>
        public bool ThrowExceptions { get; set; }

        /// <summary>
        /// Gets or sets whether strip white-space.
        /// </summary>
        public bool StripWhiteSpace { get; set; }

        /// <summary>
        /// Gets or sets the detect patterns.
        /// </summary>
        public TypeDetect TypeDetectPattern { get; set; }
        /// <summary>
        /// Gets or sets the output mode.
        /// </summary>
        public OutMode OutMode { get; set; }

    }
}