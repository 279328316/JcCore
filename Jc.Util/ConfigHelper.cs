using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.IO;
using System.Text;

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
            else
            {   //兼容 .netFramework应用程序
                if (ConfigurationManager.ConnectionStrings[key] != null)
                {
                    value = ConfigurationManager.ConnectionStrings[key].ConnectionString;
                }
            }
            return value;
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
            else
            {   //兼容 .netFramework应用程序
                value = ConfigurationManager.AppSettings[key];
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
        public static string GetSectionSetting(string section,string key, string configFileName = null)
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
                value = configSection[key];
            }
            else
            {   //兼容 .netFramework应用程序
                value = ((IConfigurationSection)ConfigurationManager.GetSection(section))[key];
            }
            return value;
        }
    }
}
