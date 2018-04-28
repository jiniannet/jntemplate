
using System;
using System.Collections;
using System.Collections.Generic;
#if NET20 || NET40
using System.Runtime.Caching;
#endif

namespace JinianNet.JNTemplate.Test
{
    public class UserCache : JinianNet.JNTemplate.Caching.ICache
    {
#if NET20 || NET40
        MemoryCache cache = new MemoryCache("test");
        public int Count
        {
            get
            {
                return (Int32)cache.GetCount();
            }
        }

        public void Dispose()
        {
            cache.Dispose();
        }

        public object Get(string key)
        {
            return cache.Get(key);
        }

        public IEnumerator GetEnumerator()
        {
            return null;
        }

        public object Remove(string key)
        {
            return cache.Remove(key);
        }

        public void Set(string key, object value)
        {
            CacheItemPolicy cip = new CacheItemPolicy();
            cip.SlidingExpiration = new TimeSpan(24, 0, 0);
            cache.Add(key, value, cip); 
        }
#else
        Dictionary<string, object> cache = new Dictionary<string, object>();
        public int Count
        {
            get
            {
                return (Int32)cache.Count;
            }
        }

        public void Dispose()
        {
            cache.Clear();
        }

        public object Get(string key)
        {
            object value;
            if (cache.TryGetValue(key, out value))
                return value;
            return null;
        }

        public IEnumerator GetEnumerator()
        {
            return null;
        }

        public object Remove(string key)
        {
            return cache.Remove(key);
        }

        public void Set(string key, object value)
        {
            cache[key] = value;
        }
#endif
    }
}
