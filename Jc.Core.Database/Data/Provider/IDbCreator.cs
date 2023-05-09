using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Common;
using System.Text;

namespace Jc.Database.Provider
{
    /// <summary>
    /// IDb Interface
    /// </summary>
    public interface IDbCreator
    {
        /// <summary>
        /// 创建连接
        /// </summary>
        /// <returns></returns>
        DbConnection CreateDbConnection(string connectString);

        /// <summary>
        /// 创建DbCmd
        /// </summary>
        /// <returns></returns>
        DbCommand CreateDbCommand(string sql = null);


        /// <summary>
        /// 使用BulkCopy插入数据
        /// </summary>
        /// <param name="tableName"></param>
        /// <param name="dt"></param>
        /// <param name="batchSize"></param>
        /// <param name="timeout"></param>
        /// <param name="useTransaction">使用事务</param>
        /// <param name="progress">0,1 进度</param>
        /// <returns></returns>
        void BulkCopy(string connectionString, string tableName, DataTable dt, int batchSize, int timeout = 0, bool useTransaction = true, IProgress<float> progress = null);
    }
}
