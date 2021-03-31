/********************************************************************************
 Copyright (c) jiniannet (http://www.jiniannet.com). All rights reserved.
 Licensed under the MIT license. See licence.txt file in the project root for full license information.
 ********************************************************************************/
using System;
using System.Collections;

namespace JinianNet.JNTemplate.Caching
{
    /// <summary>
    /// Cache
    /// </summary>
    public interface ICache : IEnumerable, IDisposable
    {
        /// <summary>
        /// The count of the cache item.
        /// </summary>
        int Count { get; }

        /// <summary>
        /// Adds the cache item.
        /// </summary>
        /// <param name="key">The key of the cache.</param>
        /// <param name="value">The value of the cache.</param>
        void Set(string key, object value);

        /// <summary>
        /// Adds the cache item.
        /// </summary>
        /// <param name="key">The key of the cache.</param>
        /// <param name="value">The value of the cache.</param>
        /// <param name="expire">The expiration</param>
        void Set(string key, object value, TimeSpan expire);

        /// <summary>
        /// Gets a object of the specify key.
        /// </summary>
        /// <param name="key">The key.</param>
        /// <returns></returns>
        object Get(string key);

        /// <summary>
        /// Gets a object of the specify key.
        /// </summary>
        /// <typeparam name="T">The type of the return.</typeparam>
        /// <param name="key">The key of the cache.</param>
        /// <returns></returns>
        T Get<T>(string key) where T : class;

        /// <summary>
        /// Remove a cache with specify key.
        /// </summary>
        /// <param name="key">the specify key.</param>
        /// <returns></returns>
        object Remove(string key);

        /// <summary>
        /// Clear all cached objects 
        /// </summary>
        void Clear();
    }
}