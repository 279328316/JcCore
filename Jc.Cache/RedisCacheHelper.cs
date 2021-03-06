﻿
using Microsoft.Extensions.Options;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;
using StackExchange.Redis;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Jc.Cache
{
    /// <summary>
    /// Configuration options for RedisCache
    /// </summary>
    public class RedisCacheOptions : IOptions<RedisCacheOptions>
    {
        /// <summary>
        /// Ctor
        /// </summary>
        public RedisCacheOptions()
        {
        }

        /// <summary>
        /// The configuration used to connect to Redis
        /// </summary>
        public string Configuration { get; set; }
        
        /// <summary>
        /// The configuration used to connect to Redis. This is preferred over Configuration.
        /// </summary>
        public ConfigurationOptions ConfigurationOptions { get; set; }

        /// <summary>
        /// The Redis instance name
        /// </summary>
        public string InstanceName { get; set; }

        /// <summary>
        /// Value
        /// </summary>
        public RedisCacheOptions Value
        {
            get
            {
                return this;
            }
        }
    }

    /// <summary>
    /// RedisCache Helper
    /// </summary>
    public class RedisCacheHelper : ICacheHelper
    {
        private IDatabase cache;

        private ConnectionMultiplexer connection;

        private readonly string instance;

        /// <summary>
        /// 缓存名称
        /// </summary>
        public string Name { get { return "Redis"; } }

        /// <summary>
        /// 默认滑动过期时间
        /// 默认时间:60min
        /// </summary>
        public TimeSpan DefaultSlidingExpireTime { get; set; }

        /// <summary>
        /// 多级缓存滑动过期时间
        /// 默认时间:1min
        /// </summary>
        public TimeSpan MCacheSlidingExpireTime { get; set; }

        /// <summary>
        /// Ctor
        /// </summary>
        /// <param name="options"></param>
        /// <param name="database"></param>
        /// <param name="defaultSlidingExpireTime">默认滑动过期时间 默认60分钟</param>
        /// <param name="mCacheSlidingExpireTime">多级缓存滑动过期时间 默认10分钟</param>
        public RedisCacheHelper(RedisCacheOptions options, int database = 0, TimeSpan? defaultSlidingExpireTime = null, TimeSpan? mCacheSlidingExpireTime = null)
        {
            string configuration = $"{options.Configuration},allowAdmin=true,syncTimeout=600000,connectRetry=3,connectTimeout=600000,keepAlive=180";
            connection = ConnectionMultiplexer.Connect(configuration);
            cache = connection.GetDatabase(database);
            instance = options.InstanceName;

            if (defaultSlidingExpireTime != null)
            {
                DefaultSlidingExpireTime = defaultSlidingExpireTime.Value;
            }
            else
            {
                DefaultSlidingExpireTime = TimeSpan.FromHours(1);
            }

            if (mCacheSlidingExpireTime != null)
            {
                MCacheSlidingExpireTime = mCacheSlidingExpireTime.Value;
            }
            else
            {
                MCacheSlidingExpireTime = TimeSpan.FromMinutes(1);
            }
            if (MCacheSlidingExpireTime.CompareTo(DefaultSlidingExpireTime)>0)
            {
                MCacheSlidingExpireTime = DefaultSlidingExpireTime;
            }

            //多级缓存使用
            MemoryCacheHelper = new MemoryCacheHelper(MCacheSlidingExpireTime);
        }

        /// <summary>
        /// Get Key
        /// </summary>
        /// <param name="key"></param>
        /// <returns></returns>
        public string GetKeyForRedis(string key)
        {
            return instance + key;
        }

        /// <summary>
        /// 验证缓存项是否存在
        /// </summary>
        /// <param name="key">缓存Key</param>
        /// <returns></returns>
        public bool Exists(string key)
        {
            if (string.IsNullOrEmpty(key))
            {
                throw new Exception("Key参数无效");
            }
            return cache.KeyExists(GetKeyForRedis(key));
        }

        /// <summary>
        /// 验证缓存项是否存在
        /// </summary>
        /// <param name="key">缓存Key</param>
        /// <returns></returns>
        public async Task<bool> ExistsAsync(string key)
        {
            if (string.IsNullOrEmpty(key))
            {
                throw new Exception("Key参数无效");
            }
            bool result = await cache.KeyExistsAsync(GetKeyForRedis(key));
            return result;
        }

        /// <summary>
        /// 获取缓存
        /// </summary>
        /// <param name="key">缓存Key</param>
        /// <returns></returns>
        public object Get(string key)
        {
            if (key == null)
            {
                throw new ArgumentNullException(nameof(key));
            }
            var value = cache.StringGet(GetKeyForRedis(key));
            if (!value.HasValue)
            {
                return null;
            }
            return JsonConvert.DeserializeObject(value);
        }


        /// <summary>
        /// 获取缓存
        /// </summary>
        /// <param name="key">缓存Key</param>
        /// <returns></returns>
        public async Task<object> GetAsync(string key)
        {
            if (key == null)
            {
                throw new ArgumentNullException(nameof(key));
            }
            RedisValue value = await cache.StringGetAsync(GetKeyForRedis(key));
            if (!value.HasValue)
            {
                return null;
            }
            return JsonConvert.DeserializeObject(value);
        }

        /// <summary>
        /// 获取缓存
        /// </summary>
        /// <param name="key">缓存Key</param>
        /// <returns></returns>
        public T Get<T>(string key) where T :class
        {
            if (string.IsNullOrEmpty(key))
            {
                throw new Exception("Key参数无效");
            }
            string value = cache.StringGet(GetKeyForRedis(key));

            if (string.IsNullOrEmpty(value))
            {
                return null;
            }
            return JsonConvert.DeserializeObject<T>(value);
        }
        
        /// <summary>
        /// 获取缓存
        /// </summary>
        /// <param name="key">缓存Key</param>
        /// <returns></returns>
        public async Task<T> GetAsync<T>(string key) where T :class
        {
            if (string.IsNullOrEmpty(key))
            {
                throw new Exception("Key参数无效");
            }
            string value = await cache.StringGetAsync(GetKeyForRedis(key));

            if (string.IsNullOrEmpty(value))
            {
                return null;
            }
            return JsonConvert.DeserializeObject<T>(value);
        }

        /// <summary>
        /// 添加缓存
        /// </summary>
        /// <param name="key">缓存Key</param>
        /// <param name="value">缓存Value</param>
        /// <param name="slidingExpireTime">滑动过期时长（如果在过期时间内有操作，则以当前时间点延长过期时间,Redis中无效）</param>
        /// <param name="absoluteExpireTime">绝对过期时长</param>
        /// <returns></returns>
        public void Set(string key, object value, TimeSpan? slidingExpireTime = null, TimeSpan? absoluteExpireTime = null)
        {
            if (string.IsNullOrEmpty(key))
            {
                throw new Exception("Key参数无效");
            }
            if (!absoluteExpireTime.HasValue)
            {
                if (slidingExpireTime.HasValue)
                {
                    absoluteExpireTime = slidingExpireTime;
                }
                else
                {
                    absoluteExpireTime = DefaultSlidingExpireTime;
                }
            }
            JsonSerializerSettings setting = new JsonSerializerSettings();
            setting.NullValueHandling = NullValueHandling.Ignore;//忽略空值
            setting.ReferenceLoopHandling = ReferenceLoopHandling.Ignore;//忽略循环
            setting.ContractResolver = new CamelCasePropertyNamesContractResolver();
            cache.StringSet(GetKeyForRedis(key), Encoding.UTF8.GetBytes(JsonConvert.SerializeObject(value, setting)), absoluteExpireTime.Value);
        }


        /// <summary>
        /// 添加缓存
        /// </summary>
        /// <param name="key">缓存Key</param>
        /// <param name="value">缓存Value</param>
        /// <param name="slidingExpireTime">滑动过期时长（如果在过期时间内有操作，则以当前时间点延长过期时间,Redis中无效）</param>
        /// <param name="absoluteExpireTime">绝对过期时长</param>
        /// <returns></returns>
        public async Task SetAsync(string key, object value, TimeSpan? slidingExpireTime = null, TimeSpan? absoluteExpireTime = null)
        {
            if (string.IsNullOrEmpty(key))
            {
                throw new Exception("Key参数无效");
            }
            if (!absoluteExpireTime.HasValue)
            {
                if (slidingExpireTime.HasValue)
                {
                    absoluteExpireTime = slidingExpireTime;
                }
                else
                {
                    absoluteExpireTime = DefaultSlidingExpireTime;
                }
            }

            JsonSerializerSettings setting = new JsonSerializerSettings();
            setting.NullValueHandling = NullValueHandling.Ignore;//忽略空值
            setting.ReferenceLoopHandling = ReferenceLoopHandling.Ignore;//忽略循环
            setting.ContractResolver = new CamelCasePropertyNamesContractResolver();

            await cache.StringSetAsync(GetKeyForRedis(key), Encoding.UTF8.GetBytes(JsonConvert.SerializeObject(value, setting)), absoluteExpireTime.Value);
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
            cache.KeyDelete(GetKeyForRedis(key));
        }

        /// <summary>
        /// 删除缓存
        /// </summary>
        /// <param name="key">缓存Key</param>
        /// <returns></returns>
        public async Task RemoveAsync(string key)
        {
            if (key == null)
            {
                throw new ArgumentNullException(nameof(key));
            }
            await cache.KeyDeleteAsync(GetKeyForRedis(key));
        }

        /// <summary>
        /// Dispose
        /// </summary>
        public void Dispose()
        {
            if (connection != null)
                connection.Dispose();
            GC.SuppressFinalize(this);
        }

        /// <summary>
        /// 清空缓存
        /// </summary>
        public void Clear()
        {
            KeyDeleteWithPrefix(GetKeyForRedis("*"));
        }

        /// <summary>
        /// 异步清空缓存
        /// </summary>
        /// <returns></returns>
        public async Task ClearAsync()
        {
            await KeyDeleteWithPrefixAsync(GetKeyForRedis("*"));
        }

        /// <summary>
        /// 根据prefix删除
        /// </summary>
        /// <param name="prefix"></param>
        private void KeyDeleteWithPrefix(string prefix)
        {
            if (string.IsNullOrWhiteSpace(prefix))
            {
                throw new ArgumentException("Prefix cannot be empty", nameof(cache));
            }
            cache.ScriptEvaluate(@"
                local keys = redis.call('keys', ARGV[1]) 
                for i=1,#keys,5000 do 
                redis.call('del', unpack(keys, i, math.min(i+4999, #keys)))
                end", values: new RedisValue[] { prefix });
        }

        /// <summary>
        /// 根据prefix删除
        /// </summary>
        /// <param name="prefix"></param>
        private async Task KeyDeleteWithPrefixAsync(string prefix)
        {
            if (string.IsNullOrWhiteSpace(prefix))
            {
                throw new ArgumentException("Prefix cannot be empty", nameof(cache));
            }
            await cache.ScriptEvaluateAsync(@"
                local keys = redis.call('keys', ARGV[1]) 
                for i=1,#keys,5000 do 
                redis.call('del', unpack(keys, i, math.min(i+4999, #keys)))
                end", values: new RedisValue[] { prefix });
        }

        #region 多级缓存

        private MemoryCacheHelper MemoryCacheHelper { get; set; }
        
        /// <summary>
        /// 根据Key获取缓存对象
        /// 多级缓存,内存缓存 => Redis缓存
        /// </summary>
        /// <param name="key">Key</param>
        /// <returns>Cached item</returns>
        public object MGet(string key)
        {
            object result = null;
            if (Exists(key))
            {   //远程缓存检查key是否存在 自二级缓存获取数据
                result = MemoryCacheHelper.Get(key);
                if (result == null)
                {
                    result = Get(key);
                    if (result != null)
                    {   //写入二级缓存
                        MemoryCacheHelper.MSet(key, result);
                    }
                }
            }
            return result;
        }


        /// <summary>
        /// 根据Key获取缓存对象
        /// 多级缓存,内存缓存 => Redis缓存
        /// </summary>
        /// <param name="key">Key</param>
        /// <returns>T</returns>
        public T MGet<T>(string key) where T : class
        {
            T result = null;
            if (Exists(key))
            {   //远程缓存检查key是否存在 自二级缓存获取数据
                result = MemoryCacheHelper.Get<T>(key);
                if (result == null)
                {
                    result = Get<T>(key);
                    if (result != null)
                    {   //写入二级缓存
                        TimeSpan expireTime = new TimeSpan(DateTime.Now.Add(MCacheSlidingExpireTime).Ticks);
                        MemoryCacheHelper.Set(key, result, null, expireTime);
                    }
                }
            }
            return result;
        }

        /// <summary>
        /// 设置缓存对象
        /// 如未设置滑动过期时间与相对过期时间,则使用默认滑动过期时间
        /// 多级缓存,内存缓存 => Redis缓存
        /// </summary>
        /// <param name="key">Key</param>
        /// <param name="value">Value</param>
        /// <param name="slidingExpireTime">Sliding expire time</param>
        /// <param name="absoluteExpireTime">Absolute expire time</param>
        public void MSet(string key, object value, TimeSpan? slidingExpireTime = null, TimeSpan? absoluteExpireTime = null)
        {
            TimeSpan expireTime = new TimeSpan(DateTime.Now.Add(MCacheSlidingExpireTime).Ticks);
            if (absoluteExpireTime.HasValue)
            {
                if (expireTime.CompareTo(absoluteExpireTime.Value)>0)
                {
                    expireTime = absoluteExpireTime.Value;
                }
            }
            MemoryCacheHelper.Set(key, value, null, expireTime);
            Set(key, value, slidingExpireTime, absoluteExpireTime);
        }

        /// <summary>
        /// 多级缓存 移除缓存
        /// </summary>
        /// <param name="key"></param>
        public void MRemove(string key)
        {
            MemoryCacheHelper.Remove(key);
            Remove(key);
        }

        /// <summary>
        /// 多级缓存 清空缓存
        /// </summary>
        public void MClear()
        {
            MemoryCacheHelper.Clear();
            Clear();
        }
        #endregion
    }
}
