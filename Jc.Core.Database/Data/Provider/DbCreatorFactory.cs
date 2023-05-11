using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Data;
using System.Data.Common;
using System.Reflection;
using System.Text;

namespace Jc.Database.Provider
{
    /// <summary>
    /// DbCreator Factory
    /// </summary>
    public class DbCreatorFactory
    {
        internal static ConcurrentDictionary<DatabaseType, IDbCreator> DbCreators = new ConcurrentDictionary<DatabaseType, IDbCreator>();

        /// <summary>
        /// 获取DbCreator
        /// </summary>
        /// <param name="dbType"></param>
        /// <returns></returns>
        public static IDbCreator GetDbCreator(DatabaseType dbType)
        {
            IDbCreator dbCreator = null;
            if (DbCreators.ContainsKey(dbType))
            {
                dbCreator = DbCreators[dbType];
            }
            else
            {
                string assemblyName = null;
                string className = null;
                switch (dbType)
                {
                    case DatabaseType.MsSql:
                        assemblyName = "Jc.Core.MsSql";
                        className = "MsSqlDbCreator";
                        break;
                    case DatabaseType.Sqlite:
                        assemblyName = "Jc.Core.Sqlite";
                        className = "SqliteDbCreator";
                        break;
                    case DatabaseType.MySql:
                        assemblyName = "Jc.Core.MySql";
                        className = "MySqlDbCreator";
                        break;
                    case DatabaseType.PostgreSql:
                        assemblyName = "Jc.Core.PostgreSql";
                        className = "PostgreSqlDbCreator";
                        break;
                    default:
                        assemblyName = "Jc.Core.MsSql";
                        className = "MsSqlDbCreator";
                        break;
                }
                dbCreator = CreateDbCreator(assemblyName, className);
                DbCreators.TryAdd(dbType, dbCreator);
            }
            return dbCreator;
        }

        /// <summary>
        /// Create DbCreator From Assembly
        /// </summary>
        /// <param name="assemblyName"></param>
        /// <param name="className"></param>
        /// <returns></returns>
        /// <exception cref="Exception"></exception>
        private static IDbCreator CreateDbCreator(string assemblyName,string className)
        {
            Assembly assembly;
            try
            {
                assembly = Assembly.Load($"{assemblyName}");
            }
            catch
            {
                throw new Exception($"加载{className}访问模块失败.请检查是否已添加{assemblyName}引用.");
            }
            IDbCreator dbCreator = assembly.CreateInstance($"{assemblyName}.{className}") as IDbCreator;
            if (dbCreator == null)
            {
                throw new Exception($"加载{className}失败.");
            }
            return dbCreator;
        }
    }
}
