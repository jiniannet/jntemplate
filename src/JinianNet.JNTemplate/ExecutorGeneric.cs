/********************************************************************************
 Copyright (c) jiniannet (http://www.jiniannet.com). All rights reserved.
 Licensed under the MIT license. See licence.txt file in the project root for full license information.
 ********************************************************************************/

#if !NET20
using System.Threading.Tasks;
#endif


namespace JinianNet.JNTemplate
{
    /// <summary>
    /// Represents an executor.
    /// </summary>
    /// <typeparam name="T">The type of return object.</typeparam>
    public abstract class Executor<T> : IExecutor, IExecutor<T>
    {
        /// <inheritdoc />
        public abstract T Execute();
        /// <inheritdoc />
        object IExecutor.Execute()
        {
            return this.Execute();
        }
#if NETCOREAPP || NETSTANDARD
        /// <inheritdoc />
        public virtual async Task<T> ExecuteAsync()
        {
            return await Task<T>.Run(()=> {
                return this.Execute();
            });
        }
        /// <inheritdoc />
        async Task<object> IExecutor.ExecuteAsync()
        {
            return (object)(await this.ExecuteAsync());
        }
#endif
    }
}
