/********************************************************************************
 Copyright (c) jiniannet (http://www.jiniannet.com). All rights reserved.
 Licensed under the MIT license. See licence.txt file in the project root for full license information.
 ********************************************************************************/

using System;
using System.Collections;
using System.Collections.Generic;

namespace JinianNet.JNTemplate.Caching
{
    /// <summary>
    /// 内存缓存
    /// </summary>
    public class MemoryCache : ICache
    {
        /*
         * 直接使用字典存储数据
         * 无过期设置，不适合缓存大量数据
         * 如需缓存大量数据请自行实现ICache接口
         */
        private Dictionary<string, object> dict;
        private static MemoryCache defaultCache;
        private static object initLock = new object();

        /// <summary>
        /// 默认缓存
        /// </summary>
        public static MemoryCache Instance
        {
            get
            {
                if (defaultCache == null)
                {
                    lock (initLock)
                    {
                        if (defaultCache == null)
                        {
                            defaultCache = new MemoryCache();
                        }
                    }
                }
                return defaultCache;
            }
        }

        /// <summary>
        /// 内存缓存
        /// </summary>
        private MemoryCache()
        {
            dict = new Dictionary<string, object>();
        }

        /// <summary>
        /// 当前缓存数量
        /// </summary>
        public int Count
        {
            get { return dict.Count; }
        }

        /// <summary>
        /// 释放资源
        /// </summary>
        public void Dispose()
        {
            dict.Clear();
        }
        /// <summary>
        /// 获取缓存
        /// </summary>
        /// <param name="key">键</param>
        /// <returns></returns>
        public object Get(string key)
        {
            object value;
            if (dict.TryGetValue(key, out value))
            {
                return value;
            }
            return null;
        }
        /// <summary>
        /// 获取Enumerator
        /// </summary>
        /// <returns></returns>
        public IEnumerator GetEnumerator()
        {
            return dict.GetEnumerator();
        }
        /// <summary>
        /// 移除缓存
        /// </summary>
        /// <param name="key"></param>
        /// <returns></returns>
        public object Remove(string key)
        {
            return dict.Remove(key);
        }

        /// <summary>
        /// 设置缓存
        /// </summary>
        /// <param name="key">健</param>
        /// <param name="value">值</param>
        public void Set(string key, object value)
        {
            dict[key] = value;
        }

        /// <summary>
        /// 获取缓存并自动转换成指定类型
        /// </summary>
        /// <typeparam name="T">返回类型</typeparam>
        /// <param name="key"></param>
        /// <returns></returns>
        public T Get<T>(string key) where T : class
        {
            var r = Get(key);
            if (r != null)
            {
                return r as T;
            }
            return default(T);
        }
        /// <summary>
        /// 添加缓存
        /// </summary>
        /// <param name="key">键</param>
        /// <param name="value">值</param>
        /// <param name="expire">过期时间</param>
        public void Set(string key, object value, TimeSpan expire)
        {
            dict[key] = value;
        }

        /// <summary>
        /// 清空所有缓存
        /// </summary>
        public void Clear()
        {
            dict.Clear();
        }
    }
}
