/********************************************************************************
 Copyright (c) jiniannet (http://www.jiniannet.com). All rights reserved.
 Licensed under the MIT license. See licence.txt file in the project root for full license information.
 ********************************************************************************/
using System;
using JinianNet.JNTemplate.Configuration;
using JinianNet.JNTemplate.CodeCompilation;
using JinianNet.JNTemplate.Runtime;
using JinianNet.JNTemplate.Resources;
using JinianNet.JNTemplate.Caching;
using System.Reflection;
using JinianNet.JNTemplate.Nodes;
using System.Collections.Generic;

namespace JinianNet.JNTemplate
{
    /// <summary>
    /// The template engine
    /// </summary>
    public interface IEngine
    {
        /// <summary>
        /// Gets the <see cref="RuntimeOptions"/>.
        /// </summary>
        RuntimeOptions Options { get; }

        /// <summary>
        /// Configuration engine which <see cref="Action{IConfig}"/>.
        /// </summary>
        /// <param name="action">The <see cref="Action{action}"/>.</param>
        /// <returns>The <see cref="TemplatingEngine"/>.</returns>
        IEngine Configure(Action<IOptions> action);

        /// <summary>
        /// Configuration engine which <see cref="IOptions"/>.
        /// </summary>
        /// <param name="option">The <see cref="IOptions"/>.</param>
        /// <returns>The <see cref="TemplatingEngine"/>.</returns>
        IEngine Configure(IConfig option);

        /// <summary>
        /// Compile a template with a given file
        /// </summary>
        /// <param name="name">Unique key of the template</param>
        /// <param name="path">The fully qualified path of the file to load.</param>
        /// <param name="action">The <see cref="Action{CompileContext}"/>.</param>
        /// <returns></returns>
        ICompilerResult CompileFile(string name, string path, Action<CompileContext> action = null);

        /// <summary>
        /// Compile a template with a given contents
        /// </summary>
        /// <param name="name">Unique key of the template</param>
        /// <param name="content">The template contents.</param>
        /// <param name="action">The <see cref="Action{CompileContext}"/>.</param>
        /// <returns></returns>
        ICompilerResult Compile(string name, string content, Action<CompileContext> action = null);

        /// <summary>
        /// Creates template context.
        /// </summary>
        /// <returns>An instance of a <see cref="TemplateContext"/>.</returns>
        TemplateContext CreateContext();


        /// <summary>
        /// Creates template with specified text.
        /// </summary>
        /// <param name="text">The template contents.</param>
        /// <returns>An instance of a template.</returns>
        ITemplate CreateTemplate(string text);

        /// <summary>
        /// Creates template with specified text.
        /// </summary>
        /// <param name="name">Unique key of the template</param>
        /// <param name="text">The template contents.</param>
        /// <returns>An instance of a template.</returns>
        ITemplate CreateTemplate(string name, string text);

        /// <summary>
        /// Loads the template on the specified path.
        /// </summary>
        /// <param name="path">The fully qualified path of the file to load.</param>
        /// <returns>An instance of a template.</returns>
        ITemplate LoadTemplate(string path);

        /// <summary>
        /// Loads the template on the specified path.
        /// </summary>
        /// <param name="name">Unique key of the template</param>
        /// <param name="path">The fully qualified path of the file to load.</param>
        /// <returns>An instance of a template.</returns>
        ITemplate LoadTemplate(string name, string path);


        /// <summary>
        /// Appends the specified directory name to the resource path list.
        /// </summary>
        /// <param name="path">The name of the directory to be appended to the resource path.</param>
        IEngine AppendResourcePath(string path);

        /// <summary>
        /// Gets an value from environment variables.
        /// </summary>
        /// <param name="key">The key of the value to get.</param>
        /// <returns>The value associated with the specified key.</returns>
        string GetEnvironmentVariable(string key);

        /// <summary>
        /// Sets an value from environment variables.
        /// </summary>
        /// <param name="key">The key of the value to set.</param>
        /// <param name="value">The variable to add to.</param>
        void SetEnvironmentVariable(string key, string value);

        /// <summary>
        /// Sets an <see cref="IResourceLoader"/> values from engine.
        /// </summary>
        /// <param name="loader">The <see cref="IResourceLoader"/> to add set.</param> 
        IEngine UseLoader(IResourceLoader loader);

        /// <summary>
        /// Sets an <see cref="IScopeProvider"/> values from engine.
        /// </summary>
        /// <param name="provider">The <see cref="IScopeProvider"/> to add set.</param> 
        IEngine UseScopeProvider(IScopeProvider provider);

        /// <summary>
        /// Sets an <see cref="ICache"/> values from engine.
        /// </summary>
        /// <param name="cache">The <see cref="ICache"/> to add set.</param> 
        IEngine UseCache(ICache cache);


        /// <summary>
        /// Initialize the engine with default <see cref="RuntimeOptions"/>.
        /// </summary>
        /// <param name="options">The <see cref="RuntimeOptions"/> to add set.</param> 
        IEngine UseOptions(RuntimeOptions options);

        /// <summary>
        /// Initialize the engine with default options.
        /// </summary>
        IEngine UseDefaultOptions();

        /// <summary>
        /// Enable compilation mode.
        /// </summary>
        /// <returns></returns>
        IEngine EnableCompile();

        /// <summary>
        /// Disable compilation mode.
        /// </summary>
        /// <returns></returns>
        IEngine DisableCompile();

        /// <summary>
        /// Clear compiled object and cache.
        /// </summary>
        void Clean();

        /// <summary>
        /// Register an new tag.
        /// </summary>
        /// <typeparam name="T">Type of the new tag. </typeparam>
        /// <param name="parseMethod">parser of the new tag.</param>
        /// <param name="compileMethod">compile method of the new tag.</param>
        /// <param name="guessMethod">guess method of the new tag.</param>
        /// <param name="index">The zero-based index.</param>
        void Register<T>(Func<TemplateParser, TokenCollection, ITag> parseMethod,
           Func<ITag, CompileContext, MethodInfo> compileMethod,
           Func<ITag, CompileContext, Type> guessMethod,
           int index = 0) where T : ITag;

        /// <summary>
        /// Register an new parsing method.
        /// </summary>
        /// <param name="func">parser of the new tag.</param>
        /// <param name="index">The zero-based index.</param>
        void RegisterParseFunc(Func<TemplateParser, TokenCollection, ITag> func,
           int index = 0);

        /// <summary>
        /// Register an new compile method.
        /// </summary>
        /// <typeparam name="T">Type of the new tag. </typeparam> 
        /// <param name="func">compile method of the new tag.</param> 
        void RegisterCompileFunc<T>(Func<ITag, CompileContext, MethodInfo> func)
           where T : ITag;

        /// <summary>
        /// Register an new guess method.
        /// </summary>
        /// <typeparam name="T">Type of the new tag. </typeparam> 
        /// <param name="func">guess method of the new tag.</param>
        void RegisterGuessFunc<T>(Func<ITag, CompileContext, Type> func)
           where T : ITag;

        /// <summary>
        /// Register an new excute method.
        /// </summary>
        /// <typeparam name="T">Type of the new tag. </typeparam> 
        /// <param name="func">excute method of the new tag.</param>
        void RegisterExecuteFunc<T>(Func<ITag, TemplateContext, object> func)
           where T : ITag;

    }
}