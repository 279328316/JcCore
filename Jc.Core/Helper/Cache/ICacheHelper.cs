using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Jc.Core.Helper
{
    /// <summary>
    /// Defines a cache that can be store and get items by keys.
    /// </summary>
    public interface ICacheHelper : IDisposable
    {
        /// <summary>
        /// Unique name of the cache.
        /// </summary>
        string Name { get; }

        /// <summary>
        /// 默认滑动过期时间
        /// 默认时间:60min
        /// </summary>
        TimeSpan DefaultSlidingExpireTime { get; set; }

        /// <summary>
        /// 根据Key获取缓存对象
        /// </summary>
        /// <param name="key">Key</param>
        /// <returns>Cached item</returns>
        object Get(string key);

        /// <summary>
        /// 根据Key获取缓存对象
        /// </summary>
        /// <param name="key">Key</param>
        /// <returns>T</returns>
        T Get<T>(string key);
        
        /// <summary>
        /// 设置缓存对象
        /// 如未设置滑动过期时间与相对过期时间,则使用默认滑动过期时间
        /// </summary>
        /// <param name="key">Key</param>
        /// <param name="value">Value</param>
        /// <param name="slidingExpireTime">Sliding expire time</param>
        /// <param name="absoluteExpireTime">Absolute expire time</param>
        void Set(string key, object value, TimeSpan? slidingExpireTime = null, TimeSpan? absoluteExpireTime = null);
        
        /// <summary>
        /// 移除缓存
        /// </summary>
        /// <param name="key">Key</param>
        void Remove(string key);
                
        /// <summary>
        /// 清空缓存
        /// </summary>
        void Clear();

        /// <summary>
        /// 异步清空缓存
        /// </summary>
        Task ClearAsync();
    }
}
