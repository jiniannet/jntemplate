using System;
using System.Text;

namespace JinianNet.JNTemplate.Caching
{
    /// <summary>
    /// 缓存
    /// </summary>
    public interface ICache
    {
        /// <summary>
        /// 当前缓存数量
        /// </summary>
        Int32 Count { get; }
        /// <summary>
        /// 添加缓存
        /// </summary>
        /// <param name="key"></param>
        /// <param name="value"></param>
        void Add(String key, Object value);
        /// <summary>
        /// 获取键为key的缓存
        /// </summary>
        /// <param name="key"></param>
        /// <returns></returns>
        Object Get(String key);
        /// <summary>
        /// 移除键为key的缓存
        /// </summary>
        /// <param name="key"></param>
        /// <returns></returns>
        Object Remove(String key);
    }
}
