/********************************************************************************
 Copyright (c) jiniannet (http://www.jiniannet.com). All rights reserved.
 Licensed under the MIT license. See licence.txt file in the project root for full license information.
 ********************************************************************************/
using System;
using System.Collections.Generic;
using System.Text;
using JinianNet.JNTemplate.Hosting;
using JinianNet.JNTemplate.Resources;
using JinianNet.JNTemplate.Runtime;

namespace JinianNet.JNTemplate
{
    /// <summary>
    /// Base class with Context.
    /// </summary>
    [Serializable]
    public class Context : ITemplateContext
    {

        /// <summary>
        /// Initializes a new instance of the <see cref="Context"/> class.
        /// </summary>
        /// <param name="options"></param>
        /// <param name="resourceManager"></param>
        /// <param name="resolver"></param>
        /// <param name="cache"></param>
        /// <param name="provider"></param>
        public Context(ResourceManager resourceManager, Resolver resolver, ITemplateCache cache, IScopeProvider provider, IOptions options)
        {
            this.ResourceManager = resourceManager;
            this.Resolver = resolver;
            this.Cache = cache;
            this.ScopeProvider = provider;

            this.OutMode  = options.OutMode;
            this.Charset = options.Encoding;
            this.ThrowExceptions = options.ThrowExceptions;
            this.EnableCache = options.EnableCache;
            this.IsCompileMode = options.Mode == EngineMode.Compiled;
            this.DisableCodeOutput = options.DisableCodeOutput;
            this.ResourceDirectories = options.ResourceDirectories;
            this.TagFlag = options.TagFlag;
            this.DisableeLogogram = options.DisableeLogogram;
            this.TagPrefix = options.TagPrefix;
            this.TagSuffix = options.TagSuffix;

            this.Name = null;
            this.TempData = null;
            this.Debug = System.Diagnostics.Debugger.IsAttached;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="Context"/> class.
        /// </summary>
        /// <param name="env"> </param>
        public Context(IHostEnvironment env) :
            this(env.ResourceManager, env.Resolver, env.Cache, env.ScopeProvider, env.Options)
        {

        }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="resourceManagerder"></param>
        /// <param name="resolver"></param>
        /// <param name="cache"></param>
        /// <param name="provider"></param>
        public Context(ResourceManager resourceManagerder, Resolver resolver, ITemplateCache cache, IScopeProvider provider) :
            this(resourceManagerder, resolver, cache, provider, new RuntimeOptions())
        {

        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="ctx"></param>
        public Context(ITemplateContext ctx)
            : this(ctx.ResourceManager, ctx.Resolver, ctx.Cache, ctx.ScopeProvider)

        {
            this.OutMode  = ctx.OutMode;
            this.Charset = ctx.Charset;
            this.ThrowExceptions = ctx.ThrowExceptions;
            this.EnableCache = ctx.EnableCache;
            this.IsCompileMode = ctx.IsCompileMode;
            this.DisableCodeOutput = ctx.DisableCodeOutput;
            this.ResourceDirectories = ctx.ResourceDirectories;
            this.Name = ctx.Name;
            this.Debug = ctx.Debug;
            this.CurrentPath = ctx.CurrentPath;
            this.DisableeLogogram = ctx.DisableeLogogram;
            this.TagPrefix = ctx.TagPrefix;
            this.TagSuffix = ctx.TagSuffix;
            this.TagFlag = ctx.TagFlag;
            this.TempData = ctx.TempData;
        }
        /// <inheritdoc />
        public string Name { get; set; }

        /// <inheritdoc />
        public ResourceManager ResourceManager { get; }

        /// <inheritdoc />
        public OutMode OutMode { get; set; }

        /// <inheritdoc />
        public string CurrentPath { get; set; }

        /// <inheritdoc />
        public Encoding Charset { get; set; }

        /// <inheritdoc />
        public bool ThrowExceptions { get; set; }
        /// <inheritdoc />
        public bool EnableCache { get; set; }

        /// <inheritdoc />
        public bool Debug { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public bool IsCompileMode { get; }

        /// <inheritdoc />
        public bool DisableCodeOutput { get; set; }


        /// <inheritdoc />
        public IScopeProvider ScopeProvider { set; get; }


        /// <inheritdoc />
        public ITemplateCache Cache { set; get; }

        /// <inheritdoc />
        public List<string> ResourceDirectories { get; }

        /// <inheritdoc />
        public bool DisableeLogogram { get; set; }
        /// <inheritdoc />
        public string TagPrefix { get; set; }

        /// <inheritdoc />
        public string TagSuffix { get; set; }

        /// <inheritdoc />
        public char TagFlag { get; set; }
        /// <inheritdoc />
        public Resolver Resolver { get; }
        /// <inheritdoc />
        public IVariableScope TempData { get; set; }
    }
}
