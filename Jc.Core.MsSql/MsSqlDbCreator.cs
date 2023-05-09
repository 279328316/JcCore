using Jc.Database.Provider;
using System;
using System.Data;
using System.Data.Common;
using System.Data.SqlClient;

namespace Jc.Core.MsSql
{
    public class MsSqlDbCreator:IDbCreator
    {
        /// <summary>
        /// 创建连接
        /// </summary>
        /// <returns></returns>
        public DbConnection CreateDbConnection(string connectString)
        {
            DbConnection dbConnection = null;
            dbConnection = new SqlConnection(connectString);
            dbConnection.Open();
            return dbConnection;
        }

        /// <summary>
        /// 创建DbCmd
        /// </summary>
        /// <returns></returns>
        public DbCommand CreateDbCommand(string sql = null)
        {
            DbCommand dbCommand = new SqlCommand();
            if (!string.IsNullOrEmpty(sql))
            {
                dbCommand.CommandText = sql;
            }
            dbCommand.CommandTimeout = 60;
            return dbCommand;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="tableName"></param>
        /// <param name="dt"></param>
        /// <param name="batchSize"></param>
        /// <param name="timeout"></param>
        /// <param name="useTransaction"></param>
        /// <param name="progress"></param>
        /// <exception cref="NotImplementedException"></exception>
        public void BulkCopy(string connectionString,string tableName, DataTable dt, int batchSize, int timeout = 0, bool useTransaction = true, IProgress<float> progress = null)
        {
            using (SqlConnection con = (SqlConnection)CreateDbConnection(connectionString))
            {
                SqlTransaction transaction = con.BeginTransaction();
                using (SqlBulkCopy bulkCopy = new SqlBulkCopy(con, new SqlBulkCopyOptions(), transaction))
                {
                    bulkCopy.SqlRowsCopied += new SqlRowsCopiedEventHandler((sender, e) =>
                    {
                        float p = e.RowsCopied * 1.0f / dt.Rows.Count;
                        progress?.Report(p);
                    });

                    try
                    {
                        bulkCopy.BatchSize = batchSize;
                        bulkCopy.NotifyAfter = batchSize;
                        bulkCopy.DestinationTableName = tableName;
                        bulkCopy.BulkCopyTimeout = timeout;
                        bulkCopy.WriteToServer(dt);
                        transaction.Commit();
                    }
                    catch (Exception ex)
                    {
                        transaction.Rollback();
                        throw;
                    }
                    bulkCopy.Close();
                    con.Close();
                }
            }
        }
    }
}
