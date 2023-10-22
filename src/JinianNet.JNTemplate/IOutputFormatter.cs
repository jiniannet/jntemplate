/********************************************************************************
 Copyright (c) jiniannet (http://www.jiniannet.com). All rights reserved.
 Licensed under the MIT license. See licence.txt file in the project root for full license information.
 ********************************************************************************/
using JinianNet.JNTemplate.Hosting;
using System;
using System.IO;

namespace JinianNet.JNTemplate
{
    /// <summary>
    ///  
    /// </summary>
    public interface IOutputFormatter
    {
        /// <summary>
        /// 
        /// </summary>
        /// <param name="type"></param>
        /// <returns></returns>
        bool CanWriteType(Type type);
        /// <summary>
        /// 
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        string Format(object input);
    }
}
