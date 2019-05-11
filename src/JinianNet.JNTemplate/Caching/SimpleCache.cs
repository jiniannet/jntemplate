/********************************************************************************
 Copyright (c) jiniannet (http://www.jiniannet.com). All rights reserved.
 Licensed under the MIT license. See licence.txt file in the project root for full license information.
 ********************************************************************************/

using System.Collections;
using System.Collections.Generic;

namespace JinianNet.JNTemplate.Caching
{
    /// <summary>
    /// 基于静态字段的简单缓存
    /// </summary>
    public class SimpleCache : ICache
    {
        /*
         * 直接使用字典存储数据
         * 无过期设置，不适合缓存大量数据
         * 如需缓存大量数据请自行实现ICache接口
         */
        private Dictionary<string, object> dict = new Dictionary<string, object>();
        private static SimpleCache defaultCache;
        private static object initLock = new object();

        /// <summary>
        /// 默认缓存
        /// </summary>
        public static SimpleCache Default
        {
            get
            {
                if (defaultCache == null)
                {
                    lock (initLock)
                    {
                        if (defaultCache == null)
                        {
                            defaultCache = new SimpleCache();
                        }
                    }
                }
                return defaultCache;
            }
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
    }
}
