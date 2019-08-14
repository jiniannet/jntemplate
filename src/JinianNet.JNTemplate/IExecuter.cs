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
    /// 执行器
    /// </summary>
    public interface IExecuter
    {
        /// <summary>
        /// 执行结果
        /// </summary>
        /// <returns></returns>
        object Execute();
#if NETCOREAPP || NETSTANDARD
        /// <summary>
        /// 异步执行
        /// </summary>
        Task<object> ExecuteAsync();
#endif
    }
}
