using Jc.Core.Data;
using System;
using System.Data.Common;
using System.Data.SQLite;

namespace Jc.Core.Sqlite
{
    public class SqliteDbCreator:IDbCreator
    {
        /// <summary>
        /// 创建连接
        /// </summary>
        /// <returns></returns>
        public DbConnection CreateDbConnection(string connectString)
        {
            SQLiteConnection dbConnection = null;
            dbConnection = new SQLiteConnection(connectString);
            dbConnection.DefaultTimeout = 30000;    //尝试30s等待
            dbConnection.Open();
            return dbConnection;
        }

        /// <summary>
        /// 创建DbCmd
        /// </summary>
        /// <returns></returns>
        public DbCommand CreateDbCommand(string sql = null)
        {
            DbCommand dbCommand = new SQLiteCommand();
            if (!string.IsNullOrEmpty(sql))
            {
                dbCommand.CommandText = sql;
            }
            dbCommand.CommandTimeout = 60;
            return dbCommand;
        }
    }
}
