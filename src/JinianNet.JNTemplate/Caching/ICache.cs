/********************************************************************************
 Copyright (c) jiniannet (http://www.jiniannet.com). All rights reserved.
 Licensed under the MIT license. See licence.txt file in the project root for full license information.
 ********************************************************************************/
using System;
using System.Collections;

namespace JinianNet.JNTemplate.Caching
{
    /// <summary>
    /// 缓存提供者
    /// </summary>
    public interface ICache : IEnumerable, IDisposable
    {
        ///// <summary>
        ///// 创建缓存
        ///// </summary>
        ///// <returns></returns>
        //ICache CreateCache();

        /// <summary>
        /// 当前缓存数量
        /// </summary>
        int Count { get; }
        /// <summary>
        /// 添加缓存（自行实现时注意设置默认过期时间）
        /// </summary>
        /// <param name="key">键</param>
        /// <param name="value">值</param>
        void Set(string key, object value);

        /// <summary>
        /// 添加缓存
        /// </summary>
        /// <param name="key">键</param>
        /// <param name="value">值</param>
        /// <param name="expire">过期时间</param>
        void Set(string key, object value, TimeSpan expire);
        /// <summary>
        /// 获取键为key的缓存
        /// </summary>
        /// <param name="key">键</param>
        /// <returns></returns>
        object Get(string key);

        /// <summary>
        /// 获取缓存并自动转换成指定类型
        /// </summary>
        /// <typeparam name="T">返回类型</typeparam>
        /// <param name="key">键</param>
        /// <returns></returns>
        T Get<T>(string key) where T : class;

        /// <summary>
        /// 移除键为key的缓存
        /// </summary>
        /// <param name="key">键</param>
        /// <returns></returns>
        object Remove(string key);

        /// <summary>
        /// 清空所有缓存
        /// </summary>
        void Clear();
    }
}