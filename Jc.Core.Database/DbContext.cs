using System;
using System.Collections.Generic;
using System.Text;
using System.Data;
using System.Data.Common;
using Jc.Database.Provider;
using log4net;
using System.IO;

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
        /// static Ctor
        /// 读取当前目录下applog.config,DbContextLogger配置来生成日志
        /// 也可以通过DbContext.InitLoggger来配置日志输出
        /// </summary>
        static DbContext()
        {   // 通过配置文件初始化Logger
            InitLoggger();
        }

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
                    DbCommandExecuter.ExecuteNonQuery(dbCommand);
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

        /// <summary>
        /// 初始化 DbLogger 记录日志
        /// 也可以通过配置目录下applog.config,DbContextLogger配置设置日志输出
        /// </summary>
        /// <param name="logger">info logger</param>
        /// <param name="errorLogger"> error logger </param>
        public static void InitLogger(ILog logger, ILog errorLogger = null)
        {
            DbLogHelper.InitLogger(logger, errorLogger);
        }

        /// <summary>
        /// 通过配置初始化Logger
        /// </summary>
        private static void InitLoggger()
        {
            try
            {
                //初始化DbContextLogger
                string logConfigPath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "applog.config");
                bool dbLogOpen = ConfigHelper.GetAppSetting("DbContextLog")?.ToLower() == "true";
                if (dbLogOpen && File.Exists(logConfigPath))
                {
                    DbLogHelper.InitLogger(logConfigPath, "DbRepository", "DbContextLogger");
                }
            }
            catch (Exception ex)
            {
            }
        }
        #endregion
    }
}