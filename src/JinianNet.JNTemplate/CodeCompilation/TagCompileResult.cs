/********************************************************************************
 Copyright (c) jiniannet (http://www.jiniannet.com). All rights reserved.
 Licensed under the MIT license. See licence.txt file in the project root for full license information.
 ********************************************************************************/

using System;
using System.Reflection;
using JinianNet.JNTemplate.Nodes;

namespace JinianNet.JNTemplate.CodeCompilation
{
    /// <summary>
    /// 
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public class TagCompileResult<T> : ITagCompileResult
    {
        /// <summary>
        /// 
        /// </summary>
        /// <param name="r"></param>
        public TagCompileResult(T r) :
            this(r, r is MethodInfo)
        {
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="r"></param>
        /// <param name="isMethod"></param>
        public TagCompileResult(T r, bool isMethod)
        {
            this.Result = r;
            this.IsMethod = isMethod;

        }

        /// <inheritdoc />
        public T Result { get; set; }
        /// <inheritdoc />
        public bool IsMethod { get; set; }
        /// <inheritdoc />
        object ITagCompileResult.Result => Result;
        /// <inheritdoc />
        public ITag Tag { get; set; }
        /// <inheritdoc />
        public Type Type { get; set; }
    }
}
