/********************************************************************************
 Copyright (c) jiniannet (http://www.jiniannet.com). All rights reserved.
 Licensed under the MIT license. See licence.txt file in the project root for full license information.
 ********************************************************************************/
using System;
using System.Collections.Concurrent;

namespace JinianNet.JNTemplate
{
    /// <summary>
    ///TemplatesCollection
    /// </summary>
    public class TemplateCollection<T> where T : Compile.ICompileTemplate
    {
        private ConcurrentDictionary<string, T> dict;
        /// <summary>
        /// ctor
        /// </summary>
        public TemplateCollection()
        {
            dict = new ConcurrentDictionary<string, T>();
        }

        /// <summary>
        /// template count
        /// </summary>
        public int Count
        {
            get { return dict.Count; }
        }

        /// <summary>
        /// get or set template
        /// </summary>
        /// <param name="name">template name</param>
        /// <returns></returns>
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
        /// remove template
        /// </summary>
        /// <param name="name">template name</param>
        /// <returns></returns>
        public bool Remove(string name)
        {
            T output;
            return dict.TryRemove(name, out output);
        }

        /// <summary>
        /// keys
        /// </summary>
        public System.Collections.Generic.ICollection<string> Keys
        {
            get
            {
                return dict.Keys;
            }
        }

        /// <summary>
        /// clear data
        /// </summary>
        public void Clear()
        {
            dict.Clear();
        }
    }
}
