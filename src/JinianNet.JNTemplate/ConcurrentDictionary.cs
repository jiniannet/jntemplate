#if NF35 || NF20
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace System.Collections.Concurrent
{
    /// <summary>
    /// 
    /// </summary>
    /// <typeparam name="TKey"></typeparam>
    /// <typeparam name="TValue"></typeparam>
    public class ConcurrentDictionary<TKey, TValue> : Dictionary<TKey, TValue>
    {

        private readonly object locker;
        /// <summary>
        /// 
        /// </summary>
        public ConcurrentDictionary() : base()
        {
            locker = new object();
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="comparer"></param>
        public ConcurrentDictionary(IEqualityComparer<TKey> comparer)
            : base(comparer)
        {

        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="key"></param>
        /// <param name="valueFactory"></param>
        /// <returns></returns>
        public TValue GetOrAdd(TKey key, Func<TKey, TValue> valueFactory)
        {
            lock (locker)
            {
                TValue value;
                if (this.TryGetValue(key, out value))
                {
                    return value;
                }
                return this[key] = valueFactory(key);
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="key"></param>
        /// <param name="value"></param>
        /// <returns></returns>
        public bool TryRemove(TKey key, out TValue value)
        {
            lock (locker)
            {
                if (this.TryGetValue(key, out value))
                {
                    this.Remove(key);
                    return true;
                }
                return false;
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="key"></param>
        /// <param name="addValue"></param>
        /// <param name="updateValueFactory"></param>
        /// <returns></returns>
        public TValue AddOrUpdate(TKey key, TValue addValue, Func<TKey, TValue, TValue> updateValueFactory)
        {
            lock (locker)
            {
                TValue old;
                if (this.TryGetValue(key, out old))
                {
                    return this[key] = updateValueFactory(key, old);
                }
                return this[key] = addValue;
            }
        }
    }
}
#endif