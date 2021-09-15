/********************************************************************************
 Copyright (c) jiniannet (http://www.jiniannet.com). All rights reserved.
 Licensed under the MIT license. See licence.txt file in the project root for full license information.
 ********************************************************************************/
using JinianNet.JNTemplate.CodeCompilation; 
using JinianNet.JNTemplate.Nodes; 
using System; 
using System.Reflection;

namespace JinianNet.JNTemplate.Hosting
{
    /// <summary>
    /// 
    /// </summary>
    public interface IHost
    {
        /// <summary>
        /// 
        /// </summary>
        IHostEnvironment HostEnvironment { get; }

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

        /// <summary>
        /// /
        /// </summary>
        void Reset();


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
    }
}
