/********************************************************************************
 Copyright (c) jiniannet (http://www.jiniannet.com). All rights reserved.
 Licensed under the MIT license. See licence.txt file in the project root for full license information.
 ********************************************************************************/
using JinianNet.JNTemplate.Caching;
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
        /// <param name="parser"></param>
        /// <param name="compileBuilder"></param>
        /// <param name="typeGuesser"></param>
        /// <param name="executorBuilder"></param>
        /// <param name="scopeProvider"></param>
        /// <param name="cache"></param>
        /// <param name="resourceLoader"></param>
        public DefaultHostEnvironment(IOptions options = null
            , TagParser parser = null
            , CompileBuilder compileBuilder = null
            , TypeGuesser typeGuesser = null
            , ExecutorBuilder executorBuilder = null
            , IScopeProvider scopeProvider = null
            , ICache cache = null
            , IResourceLoader resourceLoader = null)
        {
            this.Results = new ResultCollection<IResult>();
            this.ResourceDirectories = new List<string>();
            this.EnvironmentVariable = new Dictionary<string, string>(System.StringComparer.OrdinalIgnoreCase);
            this.Options = options ?? new RuntimeOptions();
            this.RootPath = System.IO.Directory.GetCurrentDirectory();
            this.Parser = parser ?? new TagParser();
            this.Builder = compileBuilder ?? new CompileBuilder();
            this.Guesser = typeGuesser ?? new TypeGuesser();
            this.ExecutorBuilder = executorBuilder ?? new ExecutorBuilder();
            this.ScopeProvider = scopeProvider ?? new DefaultScopeProvider();
            this.Cache = cache ?? new MemoryCache();
            this.Loader = resourceLoader ?? new FileLoader();
            this.ApplicationName = Guid.NewGuid().ToString("N"); 
            this.EnvironmentName =
#if DEBUG
                "DEBUG"
#else
                "RELEASE"
#endif
                ;
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
        public ResultCollection<IResult> Results { get; }

        /// <inheritdoc />
        public TagParser Parser { set; get; }

        /// <inheritdoc />
        public CompileBuilder Builder { set; get; }

        /// <inheritdoc />
        public TypeGuesser Guesser { set; get; }

        /// <inheritdoc />
        public ExecutorBuilder ExecutorBuilder { set; get; }

        /// <inheritdoc />
        public IScopeProvider ScopeProvider { set; get; }

        /// <inheritdoc />
        public ICache Cache { set; get; }

        /// <inheritdoc />
        public IVariableScope Data { set; get; }

        /// <inheritdoc />
        public IResourceLoader Loader { set; get; }

        /// <inheritdoc />
        public List<string> ResourceDirectories { get; }

        /// <inheritdoc />
        public Dictionary<string, string> EnvironmentVariable { set; get; }

        /// <inheritdoc />
        public IOptions Options { get; set; }
    }
}
