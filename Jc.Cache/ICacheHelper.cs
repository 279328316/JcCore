using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Jc.Cache
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
        /// 多级缓存滑动过期时间
        /// 默认时间:10min
        /// </summary>
        TimeSpan MCacheSlidingExpireTime { get; set; }

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
        /// <returns>Cached item</returns>
        Task<object> GetAsync(string key);

        /// <summary>
        /// 根据Key获取缓存对象
        /// </summary>
        /// <param name="key">Key</param>
        /// <returns>T</returns>
        T Get<T>(string key) where T : class;

        /// <summary>
        /// 根据Key获取缓存对象
        /// </summary>
        /// <param name="key">Key</param>
        /// <returns>T</returns>
        Task<T> GetAsync<T>(string key) where T : class;

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
        /// 设置缓存对象
        /// 如未设置滑动过期时间与相对过期时间,则使用默认滑动过期时间
        /// </summary>
        /// <param name="key">Key</param>
        /// <param name="value">Value</param>
        /// <param name="slidingExpireTime">Sliding expire time</param>
        /// <param name="absoluteExpireTime">Absolute expire time</param>
        Task SetAsync(string key, object value, TimeSpan? slidingExpireTime = null, TimeSpan? absoluteExpireTime = null);

        /// <summary>
        /// 移除缓存
        /// </summary>
        /// <param name="key">Key</param>
        void Remove(string key);

        /// <summary>
        /// 移除缓存
        /// </summary>
        /// <param name="key">Key</param>
        Task RemoveAsync(string key);

        /// <summary>
        /// 清空缓存
        /// </summary>
        void Clear();

        /// <summary>
        /// 异步清空缓存
        /// </summary>
        Task ClearAsync();


        // 主要用于缓存,数据量大,使用频繁,但不经常修改的数据
        #region Multi Level Cache

        /// <summary>
        /// 根据Key获取缓存对象
        /// 多级缓存,先查询 内存缓存 => Redis缓存
        /// </summary>
        /// <param name="key">Key</param>
        /// <returns>Cached item</returns>
        object MGet(string key);

        /// <summary>
        /// 根据Key获取缓存对象
        /// 多级缓存,先查询 内存缓存 => Redis缓存
        /// </summary>
        /// <param name="key">Key</param>
        /// <returns>T</returns>
        T MGet<T>(string key) where T : class;

        /// <summary>
        /// 设置缓存对象
        /// 如未设置滑动过期时间与相对过期时间,则使用默认滑动过期时间
        /// 多级缓存,内存缓存 => Redis缓存等
        /// 主要用于缓存,数据量大,使用频繁,但不经常修改的数据
        /// </summary>
        /// <param name="key">Key</param>
        /// <param name="value">Value</param>
        /// <param name="slidingExpireTime">Sliding expire time</param>
        /// <param name="absoluteExpireTime">Absolute expire time</param>
        void MSet(string key, object value, TimeSpan? slidingExpireTime = null, TimeSpan? absoluteExpireTime = null);

        /// <summary>
        /// 多级缓存 移除缓存
        /// </summary>
        /// <param name="key">Key</param>
        void MRemove(string key);

        /// <summary>
        /// 多级缓存 清空缓存
        /// </summary>
        void MClear();
        #endregion
    }
}
