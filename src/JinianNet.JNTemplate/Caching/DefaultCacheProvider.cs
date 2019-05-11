using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace JinianNet.JNTemplate.Caching
{
    /// <summary>
    /// 默认缓存提供器
    /// </summary>
    public class DefaultCacheProvider : ICacheProvider
    {
        private ICache cache;

        /// <summary>
        /// 构造函数
        /// </summary>
        public DefaultCacheProvider(ICache cache)
        {
            this.cache = cache;
        }

        /// <summary>
        /// 无参构造函数
        /// </summary>
        public DefaultCacheProvider()
            : this(SimpleCache.Default)
        {

        }

        /// <summary>
        /// 获取缓存
        /// </summary>
        /// <returns></returns>
        public ICache CreateCache()
        {
            return cache;
        }
    }
}
