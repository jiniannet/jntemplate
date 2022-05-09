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
    public abstract class Executor<T> : IExecutor, IExecutor<T>
    {
        /// <inheritdoc />
        public abstract T Execute();
        /// <inheritdoc />
        object IExecutor.Execute()
        {
            return this.Execute();
        }
#if !NF40 && !NF45
        /// <inheritdoc />
        public virtual Task<T> ExecuteAsync()
        {
            var r = this.Execute();
            return Task.FromResult<T>(r);
            //return Task<T>.Run(()=> {
            //    return this.Execute();
            //});
        }
        /// <inheritdoc />
        async Task<object> IExecutor.ExecuteAsync()
        {
            return (object)(await this.ExecuteAsync());
        }
#endif
    }
}
