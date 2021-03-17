/********************************************************************************
 Copyright (c) jiniannet (http://www.jiniannet.com). All rights reserved.
 Licensed under the MIT license. See licence.txt file in the project root for full license information.
 ********************************************************************************/
using System;
using System.Collections.Concurrent;

namespace JinianNet.JNTemplate
{
    /// <summary>
    ///Provides the class for a template collection.
    /// </summary>
    public class TemplateCollection<T> where T : Compile.ICompileTemplate
    {
        private ConcurrentDictionary<string, T> dict;
        /// <summary>
        /// Initializes a new instance of the <see cref="TemplateCollection{T}"/> class
        /// </summary>
        public TemplateCollection()
        {
            dict = new ConcurrentDictionary<string, T>();
        }

        /// <summary>
        /// Gets the number of elements actually contained in the <see cref="TemplateCollection{T}"/>.
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
        /// Removes the element at the specified name of the <see cref="TemplateCollection{T}"/>.
        /// </summary>
        /// <param name="name">The name of the element to remove.</param>
        /// <returns>true if the element is successfully removed; otherwise, false. </returns>
        public bool Remove(string name)
        {
            return dict.TryRemove(name, out T output);
        }

        /// <summary>
        /// Gets a collection containing the keys in the <see cref="TemplateCollection{T}"/>.
        /// </summary>
        public System.Collections.Generic.ICollection<string> Keys
        {
            get
            {
                return dict.Keys;
            }
        }

        /// <summary>
        /// Removes all elements from the <see cref="TemplateCollection{T}"/>.
        /// </summary>
        public void Clear()
        {
            dict.Clear();
        }
    }
}
