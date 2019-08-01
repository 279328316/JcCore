using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Reflection;
using System.ComponentModel;
using System.Xml;
using System.Data;
using System.Runtime.Serialization;

using System.Linq.Expressions;
using Jc.Core.Helper;
using System.Data.Common;

namespace Jc.Core
{
    /// <summary>
    /// DtoBase INotifyPropertyChanged
    /// </summary>
    public sealed class DbTransContext : DbContext
    {
        private DbTransaction dbTransaction;//事务使用

        internal DbTransContext(string connectString, DatabaseType dbType = DatabaseType.MsSql):base(connectString,dbType)
        {
            BeginTrans();
        }

        /// <summary>
        /// 开启事务
        /// </summary>
        internal void BeginTrans()
        {
            this.dbConnection = GetDbConnection();
            dbTransaction = dbConnection.BeginTransaction();
            isTransaction = true;
        }

        /// <summary>
        /// 回滚事务
        /// </summary>
        public void RollbackTrans()
        {
            if (isTransaction)
            {
                dbTransaction.Rollback();
                dbTransaction.Dispose();
                if (dbConnection != null) { try { dbConnection.Close(); } catch { } }
                isTransaction = false;
            }
            else
            {
                throw new Exception("未开启事务,无法回滚.");
            }
        }

        /// <summary>
        /// 提交事务
        /// </summary>
        public void CommitTrans()
        {
            if (isTransaction)
            {
                dbTransaction.Commit();
                dbTransaction.Dispose();
                if (dbConnection != null) { try { dbConnection.Close(); } catch { } }
                isTransaction = false;
            }
            else
            {
                throw new Exception("未开启事务,无法提交事务.");
            }
        }

    }
}
