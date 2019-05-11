using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace JinianNet.JNTemplate.Caching
{
    /// <summary>
    /// 缓存操作类
    /// </summary>
    public class CacheHelpers
    {
        /// <summary>
        /// 获取缓存
        /// </summary>
        /// <param name="key"></param>
        /// <returns></returns>
        public static object Get(string key)
        {
            return Engine.Runtime.Cache.Get(key);
        }

        /// <summary>
        /// 获取int缓存
        /// </summary>
        /// <param name="key"></param>
        /// <returns></returns>
        public static long GetLong(string key)
        {
            var r = Get(key);
            if (r != null)
            {
                return long.Parse(r.ToString());
            }
            return 0;
        }
        /// <summary>
        /// 获取int缓存
        /// </summary>
        /// <param name="key"></param>
        /// <returns></returns>
        public static int GetInt(string key)
        {
            var r = Get(key);
            if (r != null)
            {
                return int.Parse(r.ToString());
            }
            return 0;
        }
        /// <summary>
        /// 获取float缓存
        /// </summary>
        /// <param name="key"></param>
        /// <returns></returns>
        public static float GetFloat(string key)
        {
            var r = Get(key);
            if (r != null)
            {
                return float.Parse(r.ToString());
            }
            return 0;
        }
        /// <summary>
        /// 获取double缓存
        /// </summary>
        /// <param name="key"></param>
        /// <returns></returns>
        public static double GetDouble(string key)
        {
            var r = Get(key);
            if (r != null)
            {
                return double.Parse(r.ToString());
            }
            return 0;
        }
        /// <summary>
        /// 获取布尔缓存
        /// </summary>
        /// <param name="key"></param>
        /// <returns></returns>
        public static bool GetBoolean(string key)
        {
            var r = Get(key);
            if (r != null)
            {
                return bool.Parse(r.ToString());
            }
            return false;
        }
        /// <summary>
        /// 获取缓存并自动转换成指定类型
        /// </summary>
        /// <typeparam name="T">返回类型</typeparam>
        /// <param name="key"></param>
        /// <returns></returns>
        public static T Get<T>(string key) where T : class
        {
            var r = Get(key);
            if (r != null)
            {
                return r as T;
            }
            return default(T);
        }
        /// <summary>
        /// 移除缓存
        /// </summary>
        /// <param name="key"></param>
        /// <returns></returns>
        public static object Remove(string key)
        {
            return Engine.Runtime.Cache.Remove(key);
        }
        /// <summary>
        /// 设置缓存
        /// </summary>
        /// <param name="key"></param>
        /// <param name="value"></param>
        /// <returns></returns>
        public static void Set(string key, object value)
        {
            Engine.Runtime.Cache.Set(key, value);
        }
    }
}
