using Jc.Core.Data;
using MySql.Data.MySqlClient;
using System;
using System.Data.Common;

namespace Jc.Core.Data
{
    public class MySqlDbCreator: IDbCreator
    {
        /// <summary>
        /// 创建连接
        /// </summary>
        /// <returns></returns>
        public DbConnection CreateDbConnection(string connectString)
        {
            DbConnection dbConnection = null;
            dbConnection = new MySqlConnection(connectString);
            dbConnection.Open();
            return dbConnection;
        }

        /// <summary>
        /// 创建DbCmd
        /// </summary>
        /// <returns></returns>
        public DbCommand CreateDbCommand(string sql = null)
        {
            DbCommand dbCommand = new MySqlCommand();
            if (!string.IsNullOrEmpty(sql))
            {
                dbCommand.CommandText = sql;
            }
            dbCommand.CommandTimeout = 60;
            return dbCommand;
        }
    }
}
