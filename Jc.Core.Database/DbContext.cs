using System;
using System.Collections.Generic;
using System.Text;
using System.Data;
using System.Data.Common;
using Jc.Database.Provider;


namespace Jc.Database
{
    /// <summary>
    /// DbContext
    /// DbContext数据操作访问
    /// </summary>
    public partial class DbContext
    {
        internal DbProvider dbProvider;    //DbProvider

        internal bool isTransaction = false; //是否为事务
        
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
        /// 设置DbCommand DbConnection
        /// </summary>
        /// <returns></returns>
        internal virtual void SetDbConnection(DbCommand dbCommand)
        {
            DbConnection dbConnection = GetDbConnection();
            dbCommand.Connection = dbConnection;
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
                    SetDbConnection(dbCommand);
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
        internal virtual void CloseDbConnection(DbConnection connection)
        {
            if (connection != null)
            { 
                try 
                { 
                    connection.Close();
                    connection.Dispose();
                } 
                catch(Exception ex)
                {
                    DbLogHelper.Error($"CloseDbConnection Error:{ ex.Message }");
                }
            }
        }

        #endregion
    }
}