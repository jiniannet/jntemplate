/********************************************************************************
 Copyright (c) jiniannet (http://www.jiniannet.com). All rights reserved.
 Licensed under the MIT license. See licence.txt file in the project root for full license information.
 ********************************************************************************/
using System;
using System.Collections.Concurrent;

namespace JinianNet.JNTemplate.Runtime
{
    /// <summary>
    ///Provides the class for a template collection.
    /// </summary>
    public class ResultCollection<T>
    {
        private ConcurrentDictionary<string, T> dict;
        /// <summary>
        /// Initializes a new instance of the <see cref="ResultCollection{T}"/> class
        /// </summary>
        public ResultCollection()
        {
            dict = new ConcurrentDictionary<string, T>();
        }

        /// <summary>
        /// Gets the number of elements actually contained in the <see cref="ResultCollection{T}"/>.
        /// </summary>
        public int Count
        {
            get { return dict.Count; }
        }

        /// <summary>
        /// Gets or sets the element at the specified name.
        /// </summary>
        /// <param name="name">The name of the element to get or set.</param>
        /// <returns>The element at the specified name.</returns>
        public T this[string name]
        {
            get
            {
                T output;
                if (!dict.TryGetValue(name, out output))
                {
                    return default(T);
                }
                return output;
            }
            set
            {
                dict.AddOrUpdate(name, value, (k, v) => value);
            }
        }

        /// <summary>
        /// Removes the element at the specified name of the <see cref="ResultCollection{T}"/>.
        /// </summary>
        /// <param name="name">The name of the element to remove.</param>
        /// <returns>true if the element is successfully removed; otherwise, false. </returns>
        public bool Remove(string name)
        {
            return dict.TryRemove(name, out T output);
        }

        /// <summary>
        /// Gets a collection containing the keys in the <see cref="ResultCollection{T}"/>.
        /// </summary>
        public System.Collections.Generic.ICollection<string> Keys
        {
            get
            {
                return dict.Keys;
            }
        }

        /// <summary>
        /// Removes all elements from the <see cref="ResultCollection{T}"/>.
        /// </summary>
        public void Clear()
        {
            dict.Clear();
        }

        /// <summary>
        /// Adds a key/value pair to the <see cref="ResultCollection{T}"/>. by using the specified function if the key does not already exist. Returns the new value, or the existing value if the key exists.
        /// </summary>
        /// <param name="key">The key of the element to add.</param>
        /// <param name="valueFactory">he function used to generate a value for the key.</param>
        /// <returns>The value for the key. This will be either the existing value for the key if the key is already in the collection, or the new value if the key was not in the collection.</returns>
        public T GetOrAdd(string key, Func<string, T> valueFactory)
        {
            return dict.GetOrAdd(key, valueFactory);
        }
    }
}
