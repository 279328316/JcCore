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

namespace Jc.Core
{
    /// <summary>
    /// 事务DbContext
    /// 事务需要明确提交或撤回
    /// </summary>
    public sealed class DbTransContext : DbContext
    {
        internal bool isTransaction = false; //是否为事务

        internal DbConnection transDbConnection;  //dbConnection

        private DbTransaction dbTransaction;//事务使用

        internal DbTransContext(string connectString, DatabaseType dbType = DatabaseType.MsSql,
                                List<KeyValueObj<Type,string>> subTableArgList = null) :base(connectString,dbType)
        {
            if (subTableArgList != null)
            {   //使用新List,防止变量污染
                this.subTableArgList = new List<KeyValueObj<Type, string>>();
                for (int i = 0; i < subTableArgList.Count; i++)
                {
                    this.subTableArgList.Add(new KeyValueObj<Type, string>(subTableArgList[i].Key, subTableArgList[i].Value));
                }
            }
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
        /// 开启事务
        /// </summary>
        private void BeginTrans()
        {
            this.transDbConnection = this.DbProvider.CreateDbConnection();
            dbTransaction = transDbConnection.BeginTransaction();
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
                if (transDbConnection != null) { try { transDbConnection.Close(); } catch { } }
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
                if (transDbConnection != null) { try { transDbConnection.Close(); } catch { } }
                isTransaction = false;
            }
            else
            {
                throw new Exception("未开启事务,无法提交.");
            }
        }
        
        /// <summary>
        /// 关闭连接
        /// </summary>
        ~DbTransContext()
        {
            if (isTransaction)
            {
                //RollbackTrans();
                throw new Exception("未正确处理的事务,需要明确提交或撤回.");
            }
        }

    }
}
