//using System;
//using System.Collections;
//using System.Runtime.Caching;

//namespace JinianNet.JNTemplate.Test
//{
//    public class UserCache : JinianNet.JNTemplate.Caching.ICache
//    {
//        MemoryCache cache = new MemoryCache("test");
//        public int Count
//        {
//            get
//            {
//                return (Int32)cache.GetCount();
//            }
//        }

//        public void Dispose()
//        {
//            cache.Dispose();
//        }

//        public object Get(string key)
//        {
//            return cache.Get(key);
//        }

//        public IEnumerator GetEnumerator()
//        {
//            return null;
//        }

//        public object Remove(string key)
//        {
//            return cache.Remove(key);
//        }

//        public void Set(string key, object value)
//        {
//            CacheItemPolicy cip = new CacheItemPolicy();
//            cip.SlidingExpiration = new TimeSpan(24, 0, 0);
//            cache.Add(key, value, cip);
//        }
//    }
//}
