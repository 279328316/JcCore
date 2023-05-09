using Jc.Database.Provider;
using MySqlConnector;
using System;
using System.Data;
using System.Data.Common;

namespace Jc.Core.MySql
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

        public void BulkCopy(string connectionString, string tableName, DataTable dt, int batchSize, int timeout = 0, bool useTransaction = true, IProgress<float> progress = null)
        {
            throw new Exception("BulkCopy暂不支持");
        }
    }
}
