
using Jc.Core;
using Microsoft.Extensions.Caching.Redis;
using Newtonsoft.Json;
using StackExchange.Redis;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Jc.Core.Helper
{
    public class RedisCacheHelper : ICacheHelper
    {
        protected IDatabase cache;

        private ConnectionMultiplexer connection;

        private readonly string instance;

        public string Name { get { return "Redis"; } }

        public TimeSpan DefaultSlidingExpireTime { get; set; }

        /// <summary>
        /// Ctor
        /// </summary>
        /// <param name="options"></param>
        /// <param name="database"></param>
        public RedisCacheHelper(RedisCacheOptions options, int database = 0)
        {
            connection = ConnectionMultiplexer.Connect(options.Configuration);
            cache = connection.GetDatabase(database);
            instance = options.InstanceName;

            DefaultSlidingExpireTime = TimeSpan.FromHours(1);
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
            ExHelper.ThrowIfNull(key, "Key参数无效");
            return cache.KeyExists(GetKeyForRedis(key));
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
        public T Get<T>(string key)
        {
            ExHelper.ThrowIfNull(key, "Key参数无效");
            string value = cache.StringGet(GetKeyForRedis(key));

            if (!string.IsNullOrEmpty(value))
            {
                return default(T);
            }
            return JsonConvert.DeserializeObject<T>(value);
        }

        /// <summary>
        /// 添加缓存
        /// </summary>
        /// <param name="key">缓存Key</param>
        /// <param name="value">缓存Value</param>
        /// <param name="expiresSliding">滑动过期时长（如果在过期时间内有操作，则以当前时间点延长过期时间,Redis中无效）</param>
        /// <param name="expiressAbsoulte">绝对过期时长</param>
        /// <returns></returns>
        public void Set(string key, object value, TimeSpan? slidingExpireTime = null, TimeSpan? absoluteExpireTime = null)
        {
            ExHelper.ThrowIfNull(key, "Key参数无效");
            if (!absoluteExpireTime.HasValue)
            {
                if (slidingExpireTime.HasValue)
                {
                    absoluteExpireTime = new TimeSpan(DateTime.Now.Add(slidingExpireTime.Value).Ticks);
                }
                else
                {
                    absoluteExpireTime = new TimeSpan(DateTime.Now.Add(DefaultSlidingExpireTime).Ticks);
                }
            }
            cache.StringSet(GetKeyForRedis(key), Encoding.UTF8.GetBytes(JsonConvert.SerializeObject(value)), absoluteExpireTime.Value);
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
        public Task ClearAsync()
        {
            Clear();
            return Task.FromResult(0);
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
    }
}
