/********************************************************************************
 Copyright (c) jiniannet (http://www.jiniannet.com). All rights reserved.
 Licensed under the MIT license. See licence.txt file in the project root for full license information.
 ********************************************************************************/
using System;
using System.Threading.Tasks;

namespace JinianNet.JNTemplate.Resources
{
    /// <summary>
    /// 
    /// </summary>
    public interface IReader
    {
        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        string ReadToEnd();
#if !NF40
        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        Task<string> ReadToEndAsync();
#endif
    }
}
