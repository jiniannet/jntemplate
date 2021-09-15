/********************************************************************************
 Copyright (c) jiniannet (http://www.jiniannet.com). All rights reserved.
 Licensed under the MIT license. See licence.txt file in the project root for full license information.
 ********************************************************************************/
using System.Threading.Tasks;

namespace JinianNet.JNTemplate
{
    /// <summary>
    /// Represents an executor.
    /// </summary>
    /// <typeparam name="T">The type of return object.</typeparam>
    public interface IExecutor<T>
    {
        /// <summary>
        /// Execute the object.
        /// </summary>
        /// <returns></returns>
        T Execute();
#if !NF40 && !NF45
        /// <summary>
        /// Asynchronously execute the object.
        /// </summary>
        Task<T> ExecuteAsync();
#endif
    }
}
