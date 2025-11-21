/********************************************************************************
 Copyright (c) jiniannet (http://www.jiniannet.com). All rights reserved.
 Licensed under the MIT license. See licence.txt file in the project root for full license information.
 ********************************************************************************/
using JinianNet.JNTemplate.CodeCompilation;
using JinianNet.JNTemplate.Dynamic;
using JinianNet.JNTemplate.Parsers;
using JinianNet.JNTemplate.Resources;
using JinianNet.JNTemplate.Runtime;
using System;
using System.Collections.Generic;

namespace JinianNet.JNTemplate.Hosting
{
    /// <summary>
    /// 
    /// </summary>
    public class DefaultHostEnvironment : IHostEnvironment
    {

        /// <summary>
        /// 
        /// </summary>
        /// <param name="options"></param> 
        /// <param name="scopeProvider"></param>
        /// <param name="cache"></param>
        /// <param name="resourceLoader"></param>
        /// <param name="watcherProvider"></param>
        public DefaultHostEnvironment(IOptions options = null  
            , IScopeProvider scopeProvider = null
            , ITemplateCache cache = null
            , IResourceLoader resourceLoader = null
            , ITemplateWatcherProvider watcherProvider = null)
        {
            this.EnvironmentVariable = new Dictionary<string, string>(System.StringComparer.OrdinalIgnoreCase);
            this.Options = options ?? new RuntimeOptions();
            this.RootPath = System.IO.Directory.GetCurrentDirectory();
            this.ScopeProvider = scopeProvider ?? new DefaultScopeProvider();
            this.Cache = cache ?? new TemplateCache();
            this.ApplicationName = Guid.NewGuid().ToString("N");
            this.OutputFormatters = new List<IOutputFormatter>();
            this.Resolver = new Resolver();
            this.ResourceLoader = resourceLoader ?? new FileLoader();
            this.TemplateWatcherProvider = watcherProvider ?? new FileTemplateWatcherProvider();
            this.ResourceManager = new ResourceManager(this);
#if DEBUG
            this.EnvironmentName = "DEBUG";
#else
            this.EnvironmentName = System.Diagnostics.Debugger.IsAttached ? "DEBUG" : "RELEASE";
#endif

            if (Options.Data == null
                && ScopeProvider != null)
            {
                Options.Data = ScopeProvider.CreateScope();
            }
        }

        /// <inheritdoc />
        public string ApplicationName { get; set; }

        /// <inheritdoc />
        public string RootPath { get; set; }

        /// <inheritdoc />
        public string EnvironmentName { get; set; }

        /// <inheritdoc />
        public IScopeProvider ScopeProvider { set; get; }

        /// <inheritdoc />
        public ITemplateCache Cache { set; get; }

        /// <inheritdoc />
        public IVariableScope Data { set; get; }

        /// <inheritdoc />
        public ResourceManager ResourceManager { set; get; }

        /// <inheritdoc />
        public IDictionary<string, string> EnvironmentVariable { set; get; }

        /// <inheritdoc />
        public IOptions Options { get; set; }

        /// <inheritdoc />
        public IList<IOutputFormatter> OutputFormatters { get; private set; }

        /// <inheritdoc />
        public Resolver Resolver { get; private set; }

        /// <inheritdoc />
        public ITemplateWatcherProvider TemplateWatcherProvider { set; get; }

        /// <inheritdoc />

        public IResourceLoader ResourceLoader { get; set; }
    }
}
