///********************************************************************************
// Copyright (c) jiniannet (http://www.jiniannet.com). All rights reserved.
// Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.
// ********************************************************************************/

//using System;
//using System.Collections.Generic;

//namespace JinianNet.JNTemplate.Caching
//{
//    /// <summary>
//    /// 简易内存缓存
//    /// </summary>
//    public class MemoryCache : ICache
//    {
//        private Dictionary<String, Object> dictionary;
//        /// <summary>
//        /// 内存缓存
//        /// </summary>
//        public MemoryCache()
//        {
//            this.dictionary = new Dictionary<String, Object>();
//        }
//        /// <summary>
//        /// 当前缓存数量
//        /// </summary>
//        public int Count
//        {
//            get { return this.dictionary.Count; }
//        }
//        /// <summary>
//        /// 添加缓存
//        /// </summary>
//        /// <param name="key"></param>
//        /// <param name="value"></param>
//        public void Set(String key, Object value)
//        {
//            this.dictionary[key] = value;
//        }
//        /// <summary>
//        /// 获取键为key的缓存
//        /// </summary>
//        /// <param name="key"></param>
//        /// <returns></returns>
//        public object Get(String key)
//        {
//            Object value;
//            if (this.dictionary.TryGetValue(key, out value))
//            {
//                return value;
//            }
//            return null;
//        }
//        /// <summary>
//        /// 移除键为key的缓存
//        /// </summary>
//        /// <param name="key"></param>
//        /// <returns></returns>
//        public object Remove(string key)
//        {
//            Object value = Get(key);
//            this.dictionary.Remove(key);
//            return value;
//        }

//        /// <summary>
//        /// Enumerator
//        /// </summary>
//        /// <returns></returns>
//        public System.Collections.IEnumerator GetEnumerator()
//        {
//            return this.dictionary.GetEnumerator();
//        }

//        /// <summary>
//        /// 释放非托管资源
//        /// </summary>
//        public void Dispose()
//        {
//            this.dictionary.Clear();
//        }
//    }
//}
