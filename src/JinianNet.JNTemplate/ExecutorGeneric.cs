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
    /// 泛型执行器
    /// </summary>
    /// <typeparam name="T">泛型</typeparam>
    public abstract class Executor<T> : IExecutor, IExecutor<T>
    {
        /// <summary>
        /// 执行结果
        /// </summary>
        /// <returns></returns>
        public abstract T Execute();
        /// <summary>
        /// 执行结果
        /// </summary>
        /// <returns></returns>
        object IExecutor.Execute()
        {
            return this.Execute();
        }
#if NETCOREAPP || NETSTANDARD
        /// <summary>
        /// 异步执行
        /// </summary>
        public virtual async Task<T> ExecuteAsync()
        {
            return await Task<T>.Run(()=> {
                return this.Execute();
            });
        }
        /// <summary>
        /// 异步执行
        /// </summary>
        /// <returns></returns>
        async Task<object> IExecutor.ExecuteAsync()
        {
            return await this.ExecuteAsync();
        }
#endif
    }
}
