using System;
using System.Collections.Generic;
using System.Text;
using System.Data;
using System.Data.Common;
using Jc.Core.Data;
using System.Linq;
using System.Linq.Expressions;
using System.Collections;
using System.Reflection;
using System.Collections.Specialized;
using Jc.Core.Data.Model;
using Jc.Core.Data.Query;

namespace Jc.Core
{
    /// <summary>
    /// DbContext
    /// DbContext数据操作访问
    /// </summary>
    public partial class DbContext
    {
        internal DbProvider dbProvider;    //DbProvider

        /// <summary>
        /// Ctor
        /// <param name="connectString">数据库连接串或数据库名称</param>
        /// <param name="dbType">数据库类型</param>
        /// </summary>
        internal DbContext(string connectString, DatabaseType dbType = DatabaseType.MsSql)
        {
            this.dbProvider = DbProviderHelper.GetDbProvider(connectString, dbType);
        }

        /// <summary>
        /// 创建DbContext
        /// </summary>
        /// <param name="connectString">数据库连接串或数据库名称</param>
        /// <param name="dbType">数据库类型</param>
        /// <returns></returns>
        public static DbContext CreateDbContext(string connectString, DatabaseType dbType = DatabaseType.MsSql)
        {
            DbContext dbContext;
            try
            {
                dbContext = new DbContext(connectString, dbType);
            }
            catch (Exception ex)
            {
                string msg = ex.InnerException != null ? ex.InnerException.Message : ex.Message;
                throw new Exception(msg);
            }
            return dbContext;
        }
        
        #region Properties
        /// <summary>
        /// 数据库名称
        /// </summary>
        public string DbName
        {
            get
            {
                return dbProvider.DbName;
            }
        }
        /// <summary>
        /// 数据库连接串
        /// </summary>
        public string ConnectString
        {
            get
            {
                return dbProvider.ConnectString;
            }
        }
        /// <summary>
        /// 数据库类型
        /// </summary>
        public DatabaseType DbType
        {
            get
            {
                return dbProvider.DbType;
            }
        }

        /// <summary>
        /// 数据库类型
        /// </summary>
        internal DbProvider DbProvider
        {
            get
            {
                return dbProvider;
            }
        }
        #endregion

        #region 对象方法

        /// <summary>
        /// 获取DbConnection
        /// </summary>
        /// <returns></returns>
        internal virtual DbConnection GetDbConnection()
        {
            return dbProvider.CreateDbConnection();
        }

        /// <summary>
        /// 连接测试
        /// </summary>
        /// <returns>robj</returns>
        public void ConTest()
        {
            using (DbCommand dbCommand = dbProvider.GetConTestDbCommand())
            {
                try
                {
                    dbCommand.Connection = GetDbConnection();
                    dbCommand.ExecuteNonQuery();
                    CloseDbConnection(dbCommand);
                }
                catch (Exception ex)
                {
                    CloseDbConnection(dbCommand);
                    throw ex;
                }
            }
        }

        
        /// <summary>
        /// 关闭非事务DbConnection
        /// </summary>
        internal void CloseDbConnection(DbConnection connection)
        {
            if (connection != null) { try { connection.Close(); connection.Dispose(); } catch { } }
        }

        #endregion
    }
}