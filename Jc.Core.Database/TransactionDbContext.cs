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
using System.Data.Common;


namespace Jc.Database
{
    /// <summary>
    /// 事务DbContext
    /// 事务需要明确提交或撤回
    /// </summary>
    public sealed class TransactionDbContext : SubTableDbContext
    {
        internal DbConnection transDbConnection;  //dbConnection

        private DbTransaction dbTransaction;//事务使用

        private IsolationLevel? isolationLevel = null;

        /// <summary>
        /// TransDb使用分表参数时,
        /// 先创建TransactionDbContext,使用AddSubTableArg添加分表参数
        /// </summary>
        /// <param name="connectString">connectString</param>
        /// <param name="dbType">DatabaseType</param>
        /// <param name="level">IsolationLevel</param>
        internal TransactionDbContext(string connectString, DatabaseType dbType = DatabaseType.MsSql,
                                                IsolationLevel? level = null) :base(connectString,dbType)
        {
            this.isolationLevel = level;
            BeginTrans();
        }

        internal override DbConnection GetDbConnection()
        {
            return transDbConnection;
        }

        internal override void CloseDbConnection(DbCommand dbCmd)
        {   //如果为事务,取消关闭
            if (!isTransaction)
            { 
                base.CloseDbConnection(dbCmd);
            }
        }

        internal override void CloseDbConnection(DbConnection connection)
        {   //如果为事务,取消关闭
            if (!isTransaction)
            {
                base.CloseDbConnection(connection);
            }
        }

        /// <summary>
        /// 设置DbCommand DbConnection
        /// </summary>
        /// <returns></returns>
        internal override void SetDbConnection(DbCommand dbCommand)
        {
            if (isTransaction)
            {
                dbCommand.Connection = transDbConnection;
                dbCommand.Transaction = dbTransaction;
            }
            else
            {
                //base.SetDbConnection(dbCommand);
                throw new Exception("事务连接已关闭");
            }
        }

        /// <summary>
        /// 开启事务
        /// </summary>
        private void BeginTrans()
        {
            this.transDbConnection = this.DbProvider.CreateDbConnection();
            if (isolationLevel == null)
            {
                dbTransaction = transDbConnection.BeginTransaction();
            }
            else
            {
                dbTransaction = transDbConnection.BeginTransaction(isolationLevel.Value);
            }
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
                this.CloseTransDbConnection(); 
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
                this.CloseTransDbConnection();
            }
            else
            {
                throw new Exception("未开启事务,无法提交.");
            }
        }

        /// <summary>
        /// 关闭连接
        /// </summary>
        private void CloseTransDbConnection()
        {
            dbTransaction.Dispose();
            if (transDbConnection != null)
            {
                transDbConnection.Close();
            }
            isTransaction = false;
        }
    }
}
