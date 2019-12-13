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
using Jc.Core.Helper;
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
        private static Dictionary<string, DbProvider> dbConDic =
            new Dictionary<string, DbProvider>();    //缓存DbCommandProvider

        private DbProvider dbProvider;    //DbProvider
                
        /// <summary>
        /// Ctor
        /// <param name="connectString">数据库连接串或数据库名称</param>
        /// <param name="dbType">数据库类型</param>
        /// </summary>
        internal DbContext(string connectString, DatabaseType dbType = DatabaseType.MsSql)
        {
            InitDbContext(connectString, dbType);
        }

        /// <summary>
        /// 初始化DbContext
        /// </summary>
        /// <param name="connectString"></param>
        /// <param name="dbType"></param>
        private void InitDbContext(string connectString, DatabaseType dbType = DatabaseType.MsSql)
        {
            if (dbConDic.ContainsKey(connectString))
            {
                this.dbProvider = dbConDic[connectString];
                connectString = dbConDic[connectString].ConnectString;
            }
            else
            {
                if (!connectString.Contains(";"))
                {   //使用;判断是否为数据库名称or连接串
                    if (ConfigHelper.GetConnectString(connectString) == null)
                    {
                        throw new Exception($"DbServer[{connectString}]配置无效.请检查.");
                    }
                    connectString = ConfigHelper.GetConnectString(connectString);
                }
                dbProvider = DbProvider.GetDbProvider(connectString, dbType);
                try
                {
                    dbConDic.Add(connectString, dbProvider);
                }
                catch
                {   //如果添加失败,异常不做处理
                }
            }
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
        /// 执行客户命令
        /// </summary>
        /// <returns>受影响记录数</returns>
        public int ExCustomerCommand(string sql)
        {
            int rowCount = 0;
            using (DbCommand dbCommand = dbProvider.CreateDbCommand(sql))
            {
                try
                {
                    dbCommand.Connection = GetDbConnection();
                    rowCount = dbCommand.ExecuteNonQuery();
                    CloseDbConnection(dbCommand);
                }
                catch (Exception ex)
                {
                    CloseDbConnection(dbCommand);
                    throw ex;
                }
            }
            return rowCount;
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
        /// 返回第一行第一列数据
        /// </summary>
        /// <param name="sql">查询Sql</param>
        /// <returns></returns>
        public object ExScalar(string sql)
        {
            object result = null;
            if (!string.IsNullOrEmpty(sql))
            {
                DbCommand dbCommand = dbProvider.CreateDbCommand(sql);
                using (dbCommand)
                {
                    try
                    {
                        dbCommand.Connection = GetDbConnection();
                        result = dbCommand.ExecuteScalar();
                        CloseDbConnection(dbCommand);
                    }
                    catch (Exception ex)
                    {
                        CloseDbConnection(dbCommand);
                        throw ex;
                    }
                }
            }
            return result;
        }
                
        /// <summary>
        /// 关闭非事务DbConnection
        /// </summary>
        internal void CloseDbConnection(DbConnection connection)
        {
            if (connection != null) { try { connection.Close(); connection.Dispose(); } catch { } }
        }

        /// <summary>
        /// 关闭非事务DbConnection
        /// </summary>
        internal void CloseDbConnection(DbCommand dbCmd)
        {
            if (dbCmd != null) { CloseDbConnection(dbCmd.Connection); }
        }

        /// <summary>
        /// 将DataReader转换为DataTable
        /// </summary>
        /// <param name="dr">DataReader</param>
        /// <param name="loadAmount">加载数量</param>
        /// <returns></returns>
        internal DataTable ConvertDataReaderToDataTable(DbDataReader dr,int? loadAmount = null)
        {
            try
            {
                DataTable dt = new DataTable();
                int fieldCount = dr.FieldCount;
                for (int intCounter = 0; intCounter < fieldCount; ++intCounter)
                {
                    dt.Columns.Add(dr.GetName(intCounter), dr.GetFieldType(intCounter));
                }
                dt.BeginLoadData();

                object[] objValues = new object[fieldCount];
                int rowsCount = 0;
                while (dr.Read())
                {
                    rowsCount++;
                    dr.GetValues(objValues);
                    dt.LoadDataRow(objValues, true);

                    if (rowsCount>=loadAmount)
                    {
                        break;
                    }
                }
                dr.Close();
                dt.EndLoadData();
                return dt;
            }
            catch (Exception ex)
            {
                throw new Exception($"读取数据出错:{ex.Message}", ex);
            }

        }

        #endregion
    }
}