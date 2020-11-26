
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Options;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Jc.Cache
{
    /// <summary>
    /// 内存缓存Helper
    /// </summary>
    public class MemoryCacheHelper : ICacheHelper
    {
        private IMemoryCache cache;

        /// <summary>
        /// 缓存名称
        /// </summary>
        public string Name { get { return "MomoryCache"; } }

        /// <summary>
        /// 默认滑动过期时间
        /// 默认时间:60min
        /// </summary>
        public TimeSpan DefaultSlidingExpireTime { get; set; }

        /// <summary>
        /// 多级缓存滑动过期时间
        /// 默认时间:10min
        /// </summary>
        public TimeSpan MCacheSlidingExpireTime { get; set; }

        /// <summary>
        /// Ctor
        /// </summary>
        public MemoryCacheHelper(TimeSpan? defaultSlidingExpireTime = null)
        {
            if (defaultSlidingExpireTime != null)
            {
                DefaultSlidingExpireTime = defaultSlidingExpireTime.Value;
            }
            else
            {
                DefaultSlidingExpireTime = TimeSpan.FromHours(1);
            }
            cache = new MemoryCache(new OptionsWrapper<MemoryCacheOptions>(new MemoryCacheOptions()));
        }

        /// <summary>
        /// 获取缓存
        /// </summary>
        /// <param name="key">缓存Key</param>
        /// <returns></returns>
        public object Get(string key)
        {
            if (string.IsNullOrEmpty(key))
            {
                throw new Exception("Key参数无效");
            }
            return cache.Get(key);
        }

        /// <summary>
        /// 获取缓存
        /// </summary>
        /// <param name="key">缓存Key</param>
        /// <returns></returns>
        public async Task<object> GetAsync(string key)
        {
            object result = await Task.Run(() => { return Get(key); });
            return result;
        }

        /// <summary>
        /// 获取缓存
        /// </summary>
        /// <param name="key">缓存Key</param>
        /// <returns></returns>
        public T Get<T>(string key) where T : class
        {
            if (string.IsNullOrEmpty(key))
            {
                throw new Exception("Key参数无效");
            }
            return (T)cache.Get(key);
        }

        /// <summary>
        /// 获取缓存
        /// </summary>
        /// <param name="key">缓存Key</param>
        /// <returns></returns>
        public async Task<T> GetAsync<T>(string key) where T : class
        {
            T result = await Task.Run(() => { return Get<T>(key); });
            return result;
        }

        /// <summary>
        /// 添加缓存
        /// </summary>
        /// <param name="key">缓存Key</param>
        /// <param name="value">缓存Value</param>
        /// <param name="slidingExpireTime">滑动过期时长（如果在过期时间内有操作，则以当前时间点延长过期时间）</param>
        /// <param name="absoluteExpireTime">绝对过期时长</param>
        /// <returns></returns>
        public void Set(string key, object value, TimeSpan? slidingExpireTime = null, TimeSpan? absoluteExpireTime = null)
        {
            if (string.IsNullOrEmpty(key))
            {
                throw new Exception("Key参数无效");
            }
            MemoryCacheEntryOptions options = new MemoryCacheEntryOptions();
            if (absoluteExpireTime.HasValue)
            {
                options.SetAbsoluteExpiration(absoluteExpireTime.Value);
            }
            else if (slidingExpireTime.HasValue)
            {
                options.SetSlidingExpiration(slidingExpireTime.Value);
            }
            else
            {
                options.SetSlidingExpiration(DefaultSlidingExpireTime);
            }
            cache.Set(key, value, options);
        }
        
        /// <summary>
        /// 添加缓存
        /// </summary>
        /// <param name="key">缓存Key</param>
        /// <param name="value">缓存Value</param>
        /// <param name="slidingExpireTime">滑动过期时长（如果在过期时间内有操作，则以当前时间点延长过期时间）</param>
        /// <param name="absoluteExpireTime">绝对过期时长</param>
        /// <returns></returns>
        public async Task SetAsync(string key, object value, TimeSpan? slidingExpireTime = null, TimeSpan? absoluteExpireTime = null)
        {
            await Task.Run(() => { Set(key,value,slidingExpireTime,absoluteExpireTime); });
        }

        /// <summary>
        /// 删除缓存
        /// </summary>
        /// <param name="key">缓存Key</param>
        /// <returns></returns>
        public void Remove(string key)
        {
            if (key == null)
            {
                throw new ArgumentNullException(nameof(key));
            }
            cache.Remove(key);
        }

        /// <summary>
        /// 删除缓存
        /// </summary>
        /// <param name="key">缓存Key</param>
        /// <returns></returns>
        public async Task RemoveAsync(string key)
        {
            await Task.Run(()=>{ Remove(key); });
        }

        /// <summary>
        /// 清空缓存
        /// </summary>
        public void Clear()
        {
            cache.Dispose();
            cache = new MemoryCache(new OptionsWrapper<MemoryCacheOptions>(new MemoryCacheOptions()));
        }

        /// <summary>
        /// 异步清空缓存
        /// </summary>
        /// <returns></returns>
        public Task ClearAsync()
        {
            Clear();
            return Task.FromResult(0);
        }

        /// <summary>
        /// 释放资源
        /// </summary>
        public void Dispose()
        {
            cache.Dispose();
        }

        #region 多级缓存

        /// <summary>
        /// 根据Key获取缓存对象
        /// 多级缓存,内存缓存时,同Get(key)方法
        /// </summary>
        /// <param name="key">Key</param>
        /// <returns>Cached item</returns>
        public object MGet(string key)
        {
            return Get(key);
        }


        /// <summary>
        /// 根据Key获取缓存对象
        /// 多级缓存,内存缓存时,同Get(key)方法
        /// </summary>
        /// <param name="key">Key</param>
        /// <returns>T</returns>
        public T MGet<T>(string key) where T : class
        {
            return Get<T>(key);
        }

        /// <summary>
        /// 设置缓存对象
        /// 如未设置滑动过期时间与相对过期时间,则使用默认滑动过期时间
        /// 多级缓存,内存缓存同Set方法
        /// </summary>
        /// <param name="key">Key</param>
        /// <param name="value">Value</param>
        /// <param name="slidingExpireTime">Sliding expire time</param>
        /// <param name="absoluteExpireTime">Absolute expire time</param>
        public void MSet(string key, object value, TimeSpan? slidingExpireTime = null, TimeSpan? absoluteExpireTime = null)
        {
            Set(key, value, slidingExpireTime, absoluteExpireTime);
        }

        /// <summary>
        /// 多级缓存 移除缓存
        /// </summary>
        /// <param name="key"></param>
        public void MRemove(string key)
        {
            Remove(key);
        }

        /// <summary>
        /// 多级缓存 清空缓存
        /// </summary>
        public void MClear()
        {
            Clear();
        }

        #endregion
    }
}
