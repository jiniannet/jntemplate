/********************************************************************************
 Copyright (c) jiniannet (http://www.jiniannet.com). All rights reserved.
 Licensed under the MIT license. See licence.txt file in the project root for full license information.
 ********************************************************************************/
#if NETCOREAPP || NETSTANDARD
using System.Threading.Tasks;
#endif


namespace JinianNet.JNTemplate
{
    /// <summary>
    /// 泛型执行器
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public interface IExecuter<T>
    {
        /// <summary>
        /// 执行结果
        /// </summary>
        /// <returns></returns>
        T Execute();
#if NETCOREAPP || NETSTANDARD
        /// <summary>
        /// 异步执行
        /// </summary>
        Task<T> ExecuteAsync();
#endif
    }
}
