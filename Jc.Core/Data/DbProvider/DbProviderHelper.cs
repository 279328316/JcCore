using Jc.Core.Data.Query;
using Jc.Core.Helper;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Common;
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
                if (ConfigHelper.GetConnectString(connectString) == null)
                {
                    throw new Exception($"DbServer[{connectString}]配置无效.请检查.");
                }
                connectString = ConfigHelper.GetConnectString(connectString);
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
