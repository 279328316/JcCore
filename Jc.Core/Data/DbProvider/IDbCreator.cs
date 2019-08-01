using System;
using System.Collections.Generic;
using System.Data.Common;
using System.Text;

namespace Jc.Core.Data
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
    }
}
