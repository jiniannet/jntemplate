/********************************************************************************
 Copyright (c) jiniannet (http://www.jiniannet.com). All rights reserved.
 Licensed under the MIT license. See licence.txt file in the project root for full license information.
 ********************************************************************************/

using System;
using System.Collections;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace JinianNet.JNTemplate.Caching
{
    /// <summary>
    /// MemoryCache
    /// </summary>
    public class MemoryCache : ICache, IDistributedCache<object>
    {
        private ConcurrentDictionary<string, object> dict;

        /// <inheritdoc />
        public MemoryCache()
        {
            dict = new ConcurrentDictionary<string, object>();
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
        public void Remove(string key)
        {
            dict.TryRemove(key, out var value);
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
        /// <inheritdoc />
        public IEnumerable<KeyValuePair<string, object>> GetMany(IEnumerable<string> keys)
        {
            foreach (var key in keys)
            {
                yield return new KeyValuePair<string, object>(key, Get(key));
            }
        }


        /// <inheritdoc />
        public object GetOrAdd(string key, Func<object> factory, Func<DistributedCacheEntryOptions> optionsFactory = null)
        {
            return dict.GetOrAdd(key, (k) => factory());
        }

        /// <inheritdoc />
        public void Set(string key, object value, DistributedCacheEntryOptions options = null)
        {
            dict[key] = value;
        }


        /// <inheritdoc />
        public void SetMany(IEnumerable<KeyValuePair<string, object>> items, DistributedCacheEntryOptions options = null)
        {
            foreach (var kv in items)
            {
                dict[kv.Key] = kv.Value;
            }
        }

        /// <inheritdoc />
        public void Refresh(string key)
        {

        }

        /// <inheritdoc />
        public void RefreshMany(IEnumerable<string> keys)
        {

        }
        /// <inheritdoc />
        public void RemoveMany(IEnumerable<string> keys)
        {
            foreach (var key in keys)
            {
                Remove(key);
            }
        }
#if !NF40 && !NF45
        /// <inheritdoc />
        public Task RemoveManyAsync(IEnumerable<string> keys, CancellationToken token = default)
        {
            if (token.IsCancellationRequested)
            {
                token.ThrowIfCancellationRequested();
            }
            RemoveMany(keys);
#if NETSTANDARD2_0 || NF45
            return Task.FromResult(0);
#else
            return Task.CompletedTask;
#endif
        }
        /// <inheritdoc />
        public Task RefreshManyAsync(IEnumerable<string> keys, CancellationToken token = default)
        {
            if (token.IsCancellationRequested)
            {
                token.ThrowIfCancellationRequested();
            }
            RefreshMany(keys);
#if NETSTANDARD2_0 || NF45
            return Task.FromResult(0);
#else
            return Task.CompletedTask;
#endif
        }

        /// <inheritdoc />
        public Task RemoveAsync(string key, CancellationToken token = default)
        {
            //return token.IsCancellationRequested ?
            //    Task.FromCanceled(token) :
            if (token.IsCancellationRequested)
            {
                token.ThrowIfCancellationRequested();
            }
            Remove(key);
#if NETSTANDARD2_0 || NF45
            return Task.FromResult(0);
#else
            return Task.CompletedTask;
#endif
        }
        /// <inheritdoc />
        public Task SetManyAsync(IEnumerable<KeyValuePair<string, object>> items, DistributedCacheEntryOptions options = null, CancellationToken token = default)
        {
            if (token.IsCancellationRequested)
            {
                token.ThrowIfCancellationRequested();
            }
            SetMany(items);
#if NETSTANDARD2_0 || NF45
            return Task.FromResult(0);
#else
            return Task.CompletedTask;
#endif
        }
        /// <inheritdoc />
        public Task<IEnumerable<KeyValuePair<string, object>>> GetManyAsync(IEnumerable<string> keys, CancellationToken token = default)
        {
            if (token.IsCancellationRequested)
            {
                token.ThrowIfCancellationRequested();
            }
            return Task.FromResult(GetMany(keys));
        }

        /// <inheritdoc />
        public Task<object> GetAsync(string key, CancellationToken token = default)
        {
            if (token.IsCancellationRequested)
            {
                token.ThrowIfCancellationRequested();
            }
            return Task.FromResult(Get(key));
        }
        /// <inheritdoc />
        public Task<object> GetOrAddAsync(string key, Func<Task<object>> factory, Func<DistributedCacheEntryOptions> optionsFactory = null, CancellationToken token = default)
        {
            if (token.IsCancellationRequested)
            {
                token.ThrowIfCancellationRequested();
            }
            return Task.FromResult(GetOrAdd(key, factory, optionsFactory));
        }

        /// <inheritdoc />
        public Task SetAsync(string key, object value, DistributedCacheEntryOptions options = null, CancellationToken token = default)
        {
            if (token.IsCancellationRequested)
            {
                token.ThrowIfCancellationRequested();
            }
            Set(key, value);
#if NETSTANDARD2_0 || NF45
            return Task.FromResult(0);
#else
            return Task.CompletedTask;
#endif
        }
        /// <inheritdoc />
        public Task RefreshAsync(string key, CancellationToken token = default)
        {
#if NETSTANDARD2_0 || NF45
            return Task.FromResult(0);
#else
            return Task.CompletedTask;
#endif
        }
#endif
    }
}
