using Jc.Core.Data.Query;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Data.Common;
using System.IO;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace Jc.Core.Data
{
    /// <summary>
    /// Db Provider
    /// </summary>
    public class DbProviderHelper
    {
        private static Dictionary<string, DbProvider> dbConDic =
            new Dictionary<string, DbProvider>();    //缓存DbCommandProvider

        /// <summary>
        /// 创建DbProvider
        /// </summary>
        /// <param name="connectString">连接串 or DbName </param>
        /// <param name="dbType">数据库类型</param>
        private static DbProvider CreateDbProvider(string connectString, DatabaseType dbType = DatabaseType.MsSql)
        {
            DbProvider dbProvider = null;

            #region Set ConnectString
            if (!connectString.Contains(";"))
            {   //使用;判断是否为数据库名称or连接串
                string dbConnectString = GetConnectString(connectString);
                if (string.IsNullOrEmpty(dbConnectString))
                {
                    throw new Exception($"DbServer[{connectString}]配置无效.请检查.");
                }
                connectString = dbConnectString;
            }
            #endregion 

            switch (dbType)
            {
                case DatabaseType.MsSql:
                    dbProvider = new MsSqlDbProvider(connectString);
                    break;
                case DatabaseType.Sqlite:
                    dbProvider = new SqliteDbProvider(connectString);
                    break;
                case DatabaseType.MySql:
                    dbProvider = new MySqlDbProvider(connectString);
                    break;
                case DatabaseType.PostgreSql:
                    dbProvider = new PostgreSqlDbProvider(connectString);
                    break;
                default:
                    dbProvider = new MsSqlDbProvider(connectString);
                    break;
            }
            dbProvider.dbType = dbType;
            return dbProvider;
        }

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
        /// 获取DbProvider
        /// </summary>
        /// <param name="connectString">dbName Or dbConnectString</param>
        /// <param name="dbType"></param>
        public static DbProvider GetDbProvider(string connectString, DatabaseType dbType = DatabaseType.MsSql)
        {
            DbProvider dbProvider = null;
            if (dbConDic.ContainsKey(connectString))
            {
                dbProvider = dbConDic[connectString];
            }
            else
            {
                dbProvider = CreateDbProvider(connectString, dbType);
                try
                {
                    dbConDic.Add(connectString, dbProvider);
                }
                catch
                {   //如果添加失败,异常不做处理
                }
            }
            return dbProvider;
        }

    }
}
