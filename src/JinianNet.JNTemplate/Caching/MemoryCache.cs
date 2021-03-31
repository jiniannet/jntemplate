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
    /// MemoryCache
    /// </summary>
    public class MemoryCache : ICache
    {
        private Dictionary<string, object> dict;

        /// <inheritdoc />
        internal MemoryCache()
        {
            dict = new Dictionary<string, object>();
        }

        /// <inheritdoc />
        public int Count
        {
            get { return dict.Count; }
        }

        /// <inheritdoc />
        public void Dispose()
        {
            dict.Clear();
        }
        /// <inheritdoc />
        public object Get(string key)
        {
            object value;
            if (dict.TryGetValue(key, out value))
            {
                return value;
            }
            return null;
        }
        /// <inheritdoc />
        public IEnumerator GetEnumerator()
        {
            return dict.GetEnumerator();
        }
        /// <inheritdoc />
        public object Remove(string key)
        {
            return dict.Remove(key);
        }

        /// <inheritdoc />
        public void Set(string key, object value)
        {
            dict[key] = value;
        }

        /// <inheritdoc />
        public T Get<T>(string key) where T : class
        {
            var r = Get(key);
            if (r != null)
            {
                return r as T;
            }
            return default(T);
        }
        /// <inheritdoc />
        public void Set(string key, object value, TimeSpan expire)
        {
            dict[key] = value;
        }

        /// <inheritdoc />
        public void Clear()
        {
            dict.Clear();
        }
    }
}
