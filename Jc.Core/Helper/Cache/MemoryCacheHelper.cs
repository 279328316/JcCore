using Jc.Core;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Options;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Jc.Core.Helper
{
    public class MemoryCacheHelper : ICacheHelper
    {
        protected IMemoryCache cache;

        public string Name { get { return "MomoryCache"; } }

        public TimeSpan DefaultSlidingExpireTime { get; set; }

        /// <summary>
        /// Ctor
        /// </summary>
        /// <param name="name"></param>
        public MemoryCacheHelper()
        {
            DefaultSlidingExpireTime = TimeSpan.FromHours(1);
            cache = new MemoryCache(new OptionsWrapper<MemoryCacheOptions>(new MemoryCacheOptions()));
        }

        /// <summary>
        /// 获取缓存
        /// </summary>
        /// <param name="key">缓存Key</param>
        /// <returns></returns>
        public object Get(string key)
        {
            ExHelper.ThrowIfNull(key, "Key参数无效");
            return cache.Get(key);
        }

        /// <summary>
        /// 获取缓存
        /// </summary>
        /// <param name="key">缓存Key</param>
        /// <returns></returns>
        public T Get<T>(string key) where T : class
        {
            ExHelper.ThrowIfNull(key, "Key参数无效");
            return (T)cache.Get(key);
        }

        /// <summary>
        /// 添加缓存
        /// </summary>
        /// <param name="key">缓存Key</param>
        /// <param name="value">缓存Value</param>
        /// <param name="expiresSliding">滑动过期时长（如果在过期时间内有操作，则以当前时间点延长过期时间）</param>
        /// <param name="expiressAbsoulte">绝对过期时长</param>
        /// <returns></returns>
        public void Set(string key, object value, TimeSpan? slidingExpireTime = null, TimeSpan? absoluteExpireTime = null)
        {
            ExHelper.ThrowIfNull(key, "Key参数无效");
            ExHelper.ThrowIfNull(key, "Value参数无效");

            MemoryCacheEntryOptions options = new MemoryCacheEntryOptions();
            if (slidingExpireTime.HasValue)
            {
                options.SetSlidingExpiration(slidingExpireTime.Value);
            }
            else if (absoluteExpireTime.HasValue)
            {
                options.SetSlidingExpiration(absoluteExpireTime.Value);
            }
            else
            {
                options.SetSlidingExpiration(DefaultSlidingExpireTime);
            }
            cache.Set(key, value, options);
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
    }
}
