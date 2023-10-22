/********************************************************************************
 Copyright (c) jiniannet (http://www.jiniannet.com). All rights reserved.
 Licensed under the MIT license. See licence.txt file in the project root for full license information.
 ********************************************************************************/
using JinianNet.JNTemplate.CodeCompilation; 
using JinianNet.JNTemplate.Nodes;
using JinianNet.JNTemplate.Parsers;
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
        /// Register an new excute method.
        /// </summary>
        /// <param name="visitor"></param>
        void Register(ITagVisitor visitor);

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
