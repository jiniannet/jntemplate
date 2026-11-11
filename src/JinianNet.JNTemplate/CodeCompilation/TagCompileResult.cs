/********************************************************************************
 Copyright (c) jiniannet (http://www.jiniannet.com). All rights reserved.
 Licensed under the MIT license. See licence.txt file in the project root for full license information.
 ********************************************************************************/

using System;
using System.Reflection;
using JinianNet.JNTemplate.Nodes;

namespace JinianNet.JNTemplate.CodeCompilation;

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
    /// <param name="tag"></param>
    /// <param name="type"></param>
    public TagCompileResult(T r, ITag tag, Type type) :
        this(r, r is MethodInfo, tag, type)
    {
    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="r"></param>
    /// <param name="isMethod"></param>
    /// <param name="tag"></param>
    /// <param name="type"></param>
    public TagCompileResult(T r, bool isMethod, ITag tag, Type type)
    {
        this.Result = r;
        this.IsMethod = isMethod;
        this.Tag = tag;
        this.Type = type;

    }

    /// <inheritdoc />
    public T Result { get; set; }
    /// <inheritdoc />
    public bool IsMethod { get; set; }
    /// <inheritdoc />
    object ITagCompileResult.Result => Result ?? throw new ArgumentNullException(nameof(Result));
    /// <inheritdoc />
    public ITag Tag { get; set; }
    /// <inheritdoc />
    public Type Type { get; set; }
}
