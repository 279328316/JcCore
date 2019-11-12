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

        private string dbName;
        private string connectString;
        private DatabaseType dbType;
        private DbProvider dbProvider;    //DbProvider

        internal string subTablePfx = null;  //分表TableName后缀
        internal Type subTableType = null;   //分表操作类型

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
        /// 分表操作使用Ctor
        /// <param name="connectString">数据库连接串或数据库名称</param>
        /// <param name="dbType">数据库类型</param>
        /// <param name="subTablePfx">分表参数</param>
        /// <param name="type">分表操作对象类型</param>
        /// </summary>
        internal DbContext(string connectString, DatabaseType dbType = DatabaseType.MsSql,string subTablePfx = null,Type type = null)
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
            this.connectString = connectString;
            this.dbType = dbType;
            if (dbConDic.ContainsKey(connectString))
            {
                this.dbProvider = dbConDic[connectString];
                this.connectString = dbConDic[connectString].ConnectString;
            }
            else
            {
                if (!connectString.Contains(";"))
                {   //使用;判断是否为数据库名称or连接串
                    if (ConfigHelper.GetConnectString(connectString) == null)
                    {
                        throw new Exception($"DbServer[{connectString}]配置无效.请检查.");
                    }
                    this.connectString = ConfigHelper.GetConnectString(connectString);
                }
                dbProvider = DbProvider.GetDbProvider(this.connectString, dbType);
                this.dbName = dbProvider.DbName;
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
            DbContext dbContext = new DbContext(connectString, dbType);
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
                return dbName;
            }

            set
            {
                dbName = value;
            }
        }
        /// <summary>
        /// 数据库连接串
        /// </summary>
        public string ConnectString
        {
            get
            {
                return connectString;
            }

            set
            {
                connectString = value;
            }
        }
        /// <summary>
        /// 数据库类型
        /// </summary>
        public DatabaseType DbType
        {
            get
            {
                return dbType;
            }

            set
            {
                dbType = value;
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
            return dbProvider.CreateDbConnection(connectString);            
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
        /// 获取事务DbContext
        /// 已自动开启事务
        /// </summary>
        /// <returns></returns>
        public DbTransContext GetTransDbContext()
        {
            DbTransContext dbContext = new DbTransContext(this.connectString,this.dbType,this.subTablePfx,this.subTableType);
            return dbContext;
        }

        /// <summary>
        /// 设置分表dbContext
        /// 设置的DbContext只能用于本次指定分表操作.
        /// 需要为操作对象设置可变表名称.
        /// 如TableAttr的Name为Data{0}.tablePfx参数为2018.则表名称为Data2018
        /// </summary>
        /// <typeparam name="T">操作对象泛型</typeparam>
        /// <param name="subTablePfx">分表参数</param>
        /// <returns>返回subTableDbContext.只能用于指定分表操作.</returns>
        public DbContext SetSubTable<T>(string subTablePfx)
        {
            DtoMapping dtoDbMapping = DtoMappingHelper.GetDtoMapping<T>();
            string tableName = dtoDbMapping.TableAttr.Name;
            if (string.IsNullOrEmpty(tableName) || tableName.Contains("{0}"))
            {
                throw new Exception("必须为分表对象指定包含{0}参数的TableName属性");
            }
            DbContext subTableDbContext = new DbContext(this.connectString, this.dbType, subTablePfx,typeof(T));
            return subTableDbContext;
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
                throw new Exception("转换出错!", ex);
            }

        }

        #endregion
    }
}