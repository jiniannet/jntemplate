using System;

namespace JinianNet.JNTemplate.Common
{
    /// <summary>
    /// 缓存操作类
    /// </summary>
    public class CacheHelprs
    {
        /// <summary>
        /// 设置缓存
        /// </summary>
        /// <param name="key">键</param>
        /// <param name="value">值</param>
        public static void Set(String key,Object value)
        {
            if (Engine.Runtime.Cache != null)
            {
                Engine.Runtime.Cache.Insert(key, value);
            }
        }
        /// <summary>
        /// 获取缓存
        /// </summary>
        /// <param name="key">键</param>
        /// <returns></returns>
        public static Object Get(String key)
        {
            if (Engine.Runtime.Cache != null)
            {
                Engine.Runtime.Cache.Get(key);
            }
            return null;
        }
    }
}
