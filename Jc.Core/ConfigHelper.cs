using Microsoft.Extensions.Configuration;
using System;
using System.IO;

namespace Jc
{
    /// <summary>
    /// 配置Helper
    /// </summary>
    public class ConfigHelper
    {
        /// <summary>
        /// 获取配置
        /// </summary>
        /// <param name="key">key</param>
        /// <param name="configFileName">配置文件名称</param>
        /// <returns></returns>
        public static string GetConnectString(string key, string configFileName = null)
        {
            string value = null;
            string filePath;
            if (string.IsNullOrEmpty(configFileName))
            {
                configFileName = "appsettings.json";
                filePath = Path.Combine(AppContext.BaseDirectory, configFileName);
            }
            else
            {
                if (File.Exists(configFileName))
                {
                    filePath = configFileName;
                }
                else
                {
                    filePath = Path.Combine(AppContext.BaseDirectory, configFileName);
                }
            }
            if (File.Exists(filePath))
            {
                IConfigurationBuilder builder = new ConfigurationBuilder().AddJsonFile(filePath);
                IConfiguration configuration = builder.Build();
                value = configuration.GetConnectionString(key);
            }
            return value;
        }

        /// <summary>
        /// 获取配置AppSetting T类型值 获取DateTime时不能使用此方法.
        /// </summary>
        /// <param name="key">key</param>
        /// <param name="configFileName">配置文件名称</param>
        /// <returns></returns>
        public static T GetAppSetting<T>(string key, string configFileName = null)
        {
            T result = default(T);
            Type type = typeof(T);
            string setting = GetAppSetting(key, configFileName);
            if (!string.IsNullOrEmpty(setting))
            {
                result = GetValue<T>(setting, type);
            }
            else
            {
                if (type.IsValueType && !type.IsGenericType)
                {
                    throw new Exception($"appsetting config {key} is not exists");
                }
            }
            return result;
        }

        /// <summary>
        /// 获取Setting值
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="setting"></param>
        /// <param name="typeForT"></param>
        /// <returns></returns>
        private static T GetValue<T>(string setting, Type typeForT)
        {
            T result = default(T);
            Type type = typeForT;
            if (type.IsValueType)
            {
                Type sourceType = type;
                if (type.IsGenericType)
                {
                    sourceType = type.GenericTypeArguments[0];
                }
                if (sourceType == typeof(int))
                {
                    result = (T)(object)int.Parse(setting);
                }
                else if (sourceType.IsEnum)
                {
                    result = (T)(object)Enum.Parse(sourceType, setting, true);
                }
                else if (sourceType == typeof(long))
                {
                    result = (T)(object)long.Parse(setting);
                }
                else if (sourceType == typeof(decimal))
                {
                    result = (T)(object)decimal.Parse(setting);
                }
                else if (sourceType == typeof(float))
                {
                    result = (T)(object)float.Parse(setting);
                }
                else if (sourceType == typeof(double))
                {
                    result = (T)(object)double.Parse(setting);
                }
                else if (sourceType == typeof(bool))
                {
                    result = (T)(object)bool.Parse(setting);
                }
                else if (sourceType == typeof(DateTime))
                {
                    result = (T)(object)DateTime.Parse(setting);
                }
                else if (sourceType == typeof(TimeSpan))
                {
                    result = (T)(object)TimeSpan.Parse(setting);
                }
                else
                {
                    result = JsonHelper.DeserializeObject<T>(setting);
                }
            }
            else if (type == typeof(string))
            {   // 如果为string类型时,直接返回setting
                result = (T)(object)setting;
            }
            else
            {
                result = JsonHelper.DeserializeObject<T>(setting);
            }
            return result;
        }

        /// <summary>
        /// 获取配置AppSetting
        /// </summary>
        /// <param name="key">key</param>
        /// <param name="configFileName">配置文件名称</param>
        /// <returns></returns>
        public static string GetAppSetting(string key, string configFileName = null)
        {
            string value = null;
            string filePath;
            if (string.IsNullOrEmpty(configFileName))
            {
                configFileName = "appsettings.json";
                filePath = Path.Combine(AppContext.BaseDirectory, configFileName);
            }
            else
            {
                if (File.Exists(configFileName))
                {
                    filePath = configFileName;
                }
                else
                {
                    filePath = Path.Combine(AppContext.BaseDirectory, configFileName);
                }
            }
            if (File.Exists(filePath))
            {
                IConfigurationBuilder builder = new ConfigurationBuilder().AddJsonFile(filePath);
                IConfiguration configuration = builder.Build();
                IConfigurationSection section = configuration.GetSection("appSettings");
                value = section[key];
            }
            return value;
        }

        /// <summary>
        /// 获取配置
        /// </summary>
        /// <param name="section">section</param>
        /// <param name="key">key</param>
        /// <param name="configFileName">配置文件名称</param>
        /// <returns></returns>
        public static T GetSectionSetting<T>(string section, string key, string configFileName = null)
        {
            T result = default(T);
            Type type = typeof(T);
            string setting = GetAppSetting(key, configFileName);
            if (!string.IsNullOrEmpty(setting))
            {
                result = GetValue<T>(setting, type);
            }
            else
            {
                if (type.IsValueType && !type.IsGenericType)
                {
                    throw new Exception($"config {key} is not exists");
                }
            }
            return result;
        }

        /// <summary>
        /// 获取配置
        /// </summary>
        /// <param name="section">section</param>
        /// <param name="key">key</param>
        /// <param name="configFileName">配置文件名称</param>
        /// <returns></returns>
        public static string GetSectionSetting(string section, string key, string configFileName = null)
        {
            string value = null;
            string filePath;
            if (string.IsNullOrEmpty(configFileName))
            {
                configFileName = "appsettings.json";
                filePath = Path.Combine(AppContext.BaseDirectory, configFileName);
            }
            else
            {
                if (File.Exists(configFileName))
                {
                    filePath = configFileName;
                }
                else
                {
                    filePath = Path.Combine(AppContext.BaseDirectory, configFileName);
                }
            }
            if (File.Exists(filePath))
            {
                IConfigurationBuilder builder = new ConfigurationBuilder().AddJsonFile(filePath);
                IConfiguration configuration = builder.Build();
                IConfigurationSection configSection = configuration.GetSection(section);
                if (configSection != null)
                {
                    value = configSection[key];
                }
            }
            return value;
        }
    }
}