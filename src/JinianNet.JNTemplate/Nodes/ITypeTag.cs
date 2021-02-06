
using System;

namespace JinianNet.JNTemplate.Nodes
{
    /// <summary>
    /// 基本类型标签
    /// </summary>
    /// <typeparam name="T">类型</typeparam>
    public interface ITypeTag<T>
    {
        /// <summary>
        /// 值
        /// </summary>
        T Value { get; set; }
    }
}