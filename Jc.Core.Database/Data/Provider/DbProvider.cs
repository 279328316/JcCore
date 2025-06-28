using Jc.Database.Query;

using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Data;
using System.Data.Common;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace Jc.Database.Provider
{
    /// <summary>
    /// Db Provider
    /// </summary>
    public abstract class DbProvider
    {
        #region Fields & Properties

        private string dbName;

        private string connectString;

        private DatabaseType dbType;

        internal IDbCreator dbCreator;  //DbCreator

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
        /// 连接串
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
        }

        #endregion

        /// <summary>
        /// Ctor
        /// </summary>
        /// <param name="connectString"></param>
        internal DbProvider(string connectString, DatabaseType databaseType)
        {
            this.ConnectString = connectString;
            this.dbType = databaseType;
            this.DbName = GetDbNameFromConnectString(connectString);
            this.dbCreator = DbCreatorFactory.GetDbCreator(dbType);
        }

        #region Abstract Methods

        /// <summary>
        /// 自连接串中获取DbName
        /// </summary>
        /// <returns></returns>
        public abstract string GetDbNameFromConnectString(string connectString);

        /// <summary>
        /// 创建连接
        /// </summary>
        /// <returns></returns>
        public DbConnection CreateDbConnection()
        {
            return dbCreator.CreateDbConnection(connectString);
        }

        /// <summary>
        /// 创建DbCmd
        /// </summary>
        /// <returns></returns>
        public DbCommand CreateDbCommand(string sql = null)
        {
            return dbCreator.CreateDbCommand(sql);
        }

        /// <summary>
        /// 获取查询自增IdDbCommand
        /// </summary>
        /// <returns></returns>
        public abstract DbCommand GetAutoIdDbCommand();

        /// <summary>
        /// 获取分页查询DbCommand
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="filter">过滤条件</param>
        /// <param name="subTableArg">表名称参数.如果TableAttr设置Name.则根据Name格式化</param>
        /// <returns></returns>
        public abstract DbCommand GetQueryRecordsPageDbCommand<T>(QueryFilter filter, string subTableArg = null);

        /// <summary>
        /// 获取检查表是否存在DbCommand
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="subTableArg">表名称,如果为空,则使用T对应表名称</param>
        /// <returns></returns>
        public abstract DbCommand GetCheckTableExistsDbCommand<T>(string subTableArg = null);

        /// <summary>
        /// 获取检查表是否存在DbCommand
        /// </summary>
        /// <param name="tableName">表名称</param>
        /// <returns></returns>
        public abstract DbCommand GetCheckTableExistsDbCommand(string tableName);

        /// <summary>
        /// 获取建表DbCommand
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="tableName">新建表名称,如果为空,则使用T对应表名称</param>
        /// <returns></returns>
        public abstract DbCommand GetCreateTableDbCommand<T>(string tableName = null);

        /// <summary>
        /// 获取所有表DbCommand
        /// </summary>
        /// <returns></returns>
        public abstract DbCommand GetTableListDbCommand();

        /// <summary>
        /// 获取所有表字段DbCommand
        /// </summary>
        /// <param name="tableName">表名称</param>
        /// <returns></returns>
        public abstract DbCommand GetFieldListDbCommand(string tableName);

        /// <summary>
        /// 获取建表Sql
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="tableName">新建表名称,如果为空,则使用T对应表名称</param>
        /// <returns></returns>
        public abstract string GetCreateTableSql<T>(string tableName = null);

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
        public abstract void BulkCopy(string tableName, DataTable dt, int batchSize, int timeout = 0, bool useTransaction = true, IProgress<float> progress = null);

        #endregion

        #region Methods

        /// <summary>
        /// 获取插入DbCmd
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="dto"></param>
        /// <param name="piMapList">查询属性MapList</param>
        /// <param name="subTableArg">表名称参数.如果TableAttr设置Name.则根据Name格式化</param>
        /// <returns></returns>
        internal DbCommand GetInsertDbCmd<T>(T dto, List<FieldMapping> piMapList, string subTableArg = null) where T : class, new()
        {
            DbCommand dbCommand = CreateDbCommand();
            EntityMapping dtoMapping = EntityMappingHelper.GetMapping<T>();
            #region 设置DbCommand
            string sqlStr = "Insert into {0} ({1}) values({2})";
            string fieldParams = null;
            string valueParams = null;
            FieldMapping pkField = dtoMapping.GetPkField();
            foreach (FieldMapping piMap in piMapList)
            {
                PropertyInfo pi = piMap.Pi;
                if (piMap.FieldAttribute.ReadOnly) continue;  //跳过只读字段
                if (piMap == pkField && dtoMapping.IsAutoIncrementPk)
                {   //如果是自动设置Id 主键是int or long 自增Id 为null or 0 跳过插入
                    continue;
                }
                fieldParams = string.IsNullOrEmpty(fieldParams) ? piMap.FieldName : fieldParams + "," + piMap.FieldName;
                valueParams = string.IsNullOrEmpty(valueParams) ? "@" + piMap.FieldName : valueParams + ",@" + piMap.FieldName;

                DbParameter dbParameter = dbCommand.CreateParameter();
                dbParameter.Direction = ParameterDirection.Input;
                dbParameter.ParameterName = "@" + piMap.FieldName;
                dbParameter.Value = GetParameterValue(piMap, dto);
                dbParameter.DbType = piMap.DbType;
                dbCommand.Parameters.Add(dbParameter);
            }
            dbCommand.CommandText = string.Format(sqlStr, dtoMapping.GetTableName(subTableArg), fieldParams, valueParams);
            #endregion
            return dbCommand;
        }

        /// <summary>
        /// 获取插入DbCmd
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="list"></param>
        /// <param name="piMapList">查询属性MapList</param>
        /// <param name="subTableArg">表名称参数.如果TableAttr设置Name.则根据Name格式化</param>
        /// <returns></returns>
        internal DbCommand GetInsertListDbCmd<T>(List<T> list, List<FieldMapping> piMapList, string subTableArg = null) where T : class, new()
        {
            DbCommand dbCommand = CreateDbCommand();
            EntityMapping dtoDbMapping = EntityMappingHelper.GetMapping<T>();
            #region 设置DbCommand
            string sqlStr = "Insert into {0} ({1}) values{2}";
            string fieldParams = null;
            string valueParams = null;

            FieldMapping pkField = dtoDbMapping.GetPkField();
            foreach (FieldMapping piMap in piMapList)
            {
                PropertyInfo pi = piMap.Pi;
                if (piMap.FieldAttribute.ReadOnly) continue;  //跳过只读字段
                if (piMap == pkField && dtoDbMapping.IsAutoIncrementPk)
                {   //如果是自增主键 跳过插入
                    continue;
                }
                fieldParams = string.IsNullOrEmpty(fieldParams) ? piMap.FieldName : fieldParams + "," + piMap.FieldName;
            }
            StringBuilder strBuilder = new StringBuilder();
            for (int i = 0; i < list.Count; i++)
            {
                strBuilder.Append("(");
                StringBuilder itemBuilder = new StringBuilder();
                foreach (FieldMapping piMap in piMapList)
                {
                    PropertyInfo pi = piMap.Pi;
                    if (piMap.FieldAttribute.ReadOnly) continue;  //跳过只读字段
                    if (piMap == pkField && dtoDbMapping.IsAutoIncrementPk)
                    {   //如果是自增主键 跳过插入
                        continue;
                    }
                    itemBuilder.Append($"@{piMap.FieldName}_{i},");
                    DbParameter dbParameter = dbCommand.CreateParameter();
                    dbParameter.Direction = ParameterDirection.Input;
                    dbParameter.ParameterName = $"@{piMap.FieldName}_{i}";
                    dbParameter.Value = GetParameterValue(piMap, list[i]);
                    dbParameter.DbType = piMap.DbType;
                    dbCommand.Parameters.Add(dbParameter);
                }
                strBuilder.Append(itemBuilder.ToString().TrimEnd(','));
                strBuilder.Append("),");
            }
            valueParams = strBuilder.ToString().TrimEnd(',');
            dbCommand.CommandText = string.Format(sqlStr, dtoDbMapping.GetTableName(subTableArg), fieldParams, valueParams);
            #endregion
            return dbCommand;
        }

        /// <summary>
        /// 获取导入DbCmd 导入时,自带Id
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="dto"></param>
        /// <param name="piMapList">查询属性MapList</param>
        /// <param name="subTableArg">表名称参数.如果TableAttr设置Name.则根据Name格式化</param>
        /// <returns></returns>
        internal DbCommand GetImportDbCmd<T>(T dto, List<FieldMapping> piMapList, string subTableArg = null) where T : class, new()
        {
            DbCommand dbCommand = CreateDbCommand();
            EntityMapping dtoDbMapping = EntityMappingHelper.GetMapping<T>();
            #region 设置DbCommand
            string sqlStr = "Insert into {0} ({1}) values({2})";
            string fieldParams = null;
            string valueParams = null;
            foreach (FieldMapping piMap in piMapList)
            {
                PropertyInfo pi = piMap.Pi;
                if (piMap.FieldAttribute.ReadOnly) continue;  //跳过只读字段
                fieldParams = string.IsNullOrEmpty(fieldParams) ? piMap.FieldName : fieldParams + "," + piMap.FieldName;
                valueParams = string.IsNullOrEmpty(valueParams) ? "@" + piMap.FieldName : valueParams + ",@" + piMap.FieldName;

                DbParameter dbParameter = dbCommand.CreateParameter();
                dbParameter.Direction = ParameterDirection.Input;
                dbParameter.ParameterName = "@" + piMap.FieldName;
                dbParameter.Value = GetParameterValue(piMap, dto);
                dbParameter.DbType = piMap.DbType;
                dbCommand.Parameters.Add(dbParameter);
            }
            dbCommand.CommandText = string.Format(sqlStr, dtoDbMapping.GetTableName(subTableArg), fieldParams, valueParams);
            #endregion
            return dbCommand;
        }

        /// <summary>
        /// 获取更新DbCmd
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="dto"></param>
        /// <param name="piMapList">查询属性MapList</param>
        /// <param name="subTableArg">表名称参数.如果TableAttr设置Name.则根据Name格式化</param>
        /// <returns></returns>
        internal DbCommand GetUpdateDbCmd<T>(T dto, List<FieldMapping> piMapList, string subTableArg = null)
        {
            DbCommand dbCommand = CreateDbCommand();
            EntityMapping dtoDbMapping = EntityMappingHelper.GetMapping<T>();
            #region 设置DbCommand
            string sqlStr = "Update {0} set {1} where {2};";
            string setParams = null;
            string whereParams = null;
            FieldMapping pkField = dtoDbMapping.GetPkField();
            foreach (FieldMapping piMap in piMapList)
            {
                if (piMap == pkField && dtoDbMapping.IsAutoIncrementPk)
                {   //如果是自增主键 跳过更新
                    continue;
                }
                DbParameter dbParameter = dbCommand.CreateParameter();
                dbParameter.Direction = ParameterDirection.Input;
                dbParameter.ParameterName = $"@{piMap.FieldName}";
                dbParameter.Value = GetParameterValue(piMap, dto);
                dbParameter.DbType = piMap.DbType;
                dbCommand.Parameters.Add(dbParameter);

                if (!string.IsNullOrEmpty(setParams))
                {
                    setParams += ",";
                }
                setParams += $"{piMap.FieldName}={dbParameter.ParameterName}";
            }

            DbParameter whereParameter = dbCommand.CreateParameter();
            whereParameter.Direction = ParameterDirection.Input;
            whereParameter.ParameterName = $"@where{pkField.FieldName}";
            object pkValue = pkField.Pi.GetValue(dto);
            whereParameter.Value = pkValue != null ? pkValue : DBNull.Value;
            dbCommand.Parameters.Add(whereParameter);
            whereParams = $"{pkField.FieldName}={whereParameter.ParameterName}";

            dbCommand.CommandText = string.Format(sqlStr, dtoDbMapping.GetTableName(subTableArg), setParams, whereParams);
            #endregion

            return dbCommand;
        }

        /// <summary>
        /// 获取条件更新DbCmd
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="dto"></param>
        /// <param name="piMapList">查询属性MapList</param>
        /// <param name="subTableArg">表名称参数.如果TableAttr设置Name.则根据Name格式化</param>
        /// <returns></returns>
        internal DbCommand GetUpdateDbCmd<T>(T dto, QueryFilter filter, string subTableArg = null)
        {
            DbCommand dbCommand = CreateDbCommand();
            EntityMapping dtoDbMapping = EntityMappingHelper.GetMapping<T>();
            #region 设置DbCommand
            string sqlStr = "Update {0} set {1}";

            string setParams = null;

            if (filter != null)
            {
                if (filter.ItemList.Count > 0)
                {
                    sqlStr += filter.FilterSQLString;
                }
                for (int i = 0; i < filter.FilterParameters.Count; i++)
                {
                    DbParameter dbParameter = GetQueryParameter(dbCommand, filter.FilterParameters[i]);
                    dbCommand.Parameters.Add(dbParameter);
                }
            }
            List<FieldMapping> piMapList = EntityMappingHelper.GetPiMapList<T>(filter);
            FieldMapping pkField = dtoDbMapping.GetPkField();
            foreach (FieldMapping piMap in piMapList)
            {
                if (piMap == pkField && dtoDbMapping.IsAutoIncrementPk)
                {   //如果是自增主键 跳过更新
                    continue;
                }
                DbParameter dbParameter = dbCommand.CreateParameter();
                dbParameter.Direction = ParameterDirection.Input;
                dbParameter.ParameterName = $"@{piMap.FieldName}";
                dbParameter.Value = GetParameterValue(piMap, dto);
                dbParameter.DbType = piMap.DbType;
                dbCommand.Parameters.Add(dbParameter);

                if (!string.IsNullOrEmpty(setParams))
                {
                    setParams += ",";
                }
                setParams += $"{piMap.FieldName}={dbParameter.ParameterName}";
            }

            dbCommand.CommandText = string.Format(sqlStr, dtoDbMapping.GetTableName(subTableArg), setParams);
            #endregion

            return dbCommand;
        }

        /// <summary>
        /// 获取更新DbCmd
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="list"></param>
        /// <param name="piMapList">查询属性MapList</param>
        /// <param name="subTableArg">表名称参数.如果TableAttr设置Name.则根据Name格式化</param>
        /// <returns></returns>
        internal DbCommand GetUpdateDbCmd<T>(List<T> list, List<FieldMapping> piMapList, string subTableArg = null)
        {
            DbCommand dbCommand = CreateDbCommand();
            EntityMapping dtoDbMapping = EntityMappingHelper.GetMapping<T>();
            #region 设置DbCommand

            StringBuilder strBuilder = new StringBuilder();
            string sqlStr = "Update {0} set {1} where {2};";
            FieldMapping pkField = dtoDbMapping.GetPkField();
            for (int i = 0; i < list.Count; i++)
            {
                string setParams = null;
                string whereParams = null;
                T dto = list[i];
                foreach (FieldMapping piMap in piMapList)
                {
                    if (piMap == pkField && dtoDbMapping.IsAutoIncrementPk)
                    {   //如果是自增主键 跳过更新
                        continue;
                    }
                    DbParameter dbParameter = dbCommand.CreateParameter();
                    dbParameter.Direction = ParameterDirection.Input;
                    dbParameter.ParameterName = $"@{piMap.FieldName}_{i}";
                    dbParameter.Value = GetParameterValue(piMap, dto);
                    dbParameter.DbType = piMap.DbType;
                    dbCommand.Parameters.Add(dbParameter);

                    if (!string.IsNullOrEmpty(setParams))
                    {
                        setParams += ",";
                    }
                    setParams += $"{piMap.FieldName}={dbParameter.ParameterName}";
                }
                DbParameter whereParameter = dbCommand.CreateParameter();
                whereParameter.Direction = ParameterDirection.Input;
                whereParameter.ParameterName = $"@where{pkField.FieldName}{i}";
                object pkValue = pkField.Pi.GetValue(dto);
                whereParameter.Value = pkValue != null ? pkValue : DBNull.Value;
                dbCommand.Parameters.Add(whereParameter);
                whereParams = $"{pkField.FieldName}={whereParameter.ParameterName}";

                string updateStr = string.Format(sqlStr, dtoDbMapping.GetTableName(subTableArg), setParams, whereParams);
                strBuilder.Append(updateStr);
            }
            dbCommand.CommandText = strBuilder.ToString();
            #endregion

            return dbCommand;
        }

        /// <summary>
        /// 获取删除DbCmd
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="dto"></param>
        /// <param name="subTableArg">表名称参数.如果TableAttr设置Name.则根据Name格式化</param>
        /// <returns></returns>
        internal DbCommand GetDeleteDbCmd<T>(T dto, string subTableArg = null)
        {
            DbCommand dbCommand = CreateDbCommand();

            #region 设置DbCommand

            string sqlStr = "Delete From {0} Where {1}";
            string whereParams = null;
            EntityMapping dtoDbMapping = EntityMappingHelper.GetMapping<T>();
            FieldMapping pkField = dtoDbMapping.GetPkField();

            DbParameter dbParameter = dbCommand.CreateParameter();
            dbParameter.Direction = ParameterDirection.Input;
            dbParameter.ParameterName = "@" + pkField.FieldName;
            dbParameter.Value = pkField.Pi.GetValue(dto);
            dbParameter.DbType = pkField.DbType;
            dbCommand.Parameters.Add(dbParameter);

            whereParams = pkField.FieldName + "=@" + pkField.FieldName;
            dbCommand.CommandText = string.Format(sqlStr, dtoDbMapping.GetTableName(subTableArg), whereParams);
            #endregion

            return dbCommand;
        }

        /// <summary>
        /// 获取条件删除DbCmd
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="filter">过滤条件</param>
        /// <param name="subTableArg">表名称参数.如果TableAttr设置Name.则根据Name格式化</param>
        /// <returns></returns>
        internal DbCommand GetDeleteDbCmd<T>(QueryFilter filter, string subTableArg = null)
        {
            DbCommand dbCommand = CreateDbCommand();

            #region 设置DbCommand

            string sqlStr = "Delete From {0} ";

            EntityMapping dtoDbMapping = EntityMappingHelper.GetMapping<T>();

            if (filter != null)
            {
                if (filter.ItemList.Count > 0)
                {
                    sqlStr += filter.FilterSQLString;
                }
                for (int i = 0; i < filter.FilterParameters.Count; i++)
                {
                    DbParameter dbParameter = GetQueryParameter(dbCommand, filter.FilterParameters[i]);
                    dbCommand.Parameters.Add(dbParameter);
                }
            }
            dbCommand.CommandText = string.Format(sqlStr, dtoDbMapping.GetTableName(subTableArg));
            #endregion

            return dbCommand;
        }

        /// <summary>
        /// 获取删除DbCmd
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="subTableArg">表名称参数.如果TableAttr设置Name.则根据Name格式化</param>
        /// <returns></returns>
        internal DbCommand GetDeleteByIdDbCmd<T>(object pkValue, string subTableArg = null)
        {
            DbCommand dbCommand = CreateDbCommand();

            #region 设置DbCommand

            string sqlStr = "Delete From {0} Where {1}";
            string whereParams = null;
            EntityMapping dtoDbMapping = EntityMappingHelper.GetMapping<T>();
            FieldMapping pkField = dtoDbMapping.GetPkField();

            DbParameter dbParameter = dbCommand.CreateParameter();
            dbParameter.Direction = ParameterDirection.Input;
            dbParameter.ParameterName = "@" + pkField.FieldName;
            dbParameter.Value = pkValue;
            dbParameter.DbType = pkField.DbType;
            dbCommand.Parameters.Add(dbParameter);

            whereParams = pkField.FieldName + "=@" + pkField.FieldName;
            dbCommand.CommandText = string.Format(sqlStr, dtoDbMapping.GetTableName(subTableArg), whereParams);
            #endregion

            return dbCommand;
        }

        /// <summary>
        /// 获取查询DbCommand
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="filter"></param>
        /// <param name="subTableArg">表名称参数.如果TableAttr设置Name.则根据Name格式化</param>
        /// <returns></returns>
        internal DbCommand GetQueryDbCommand<T>(QueryFilter filter = null, string subTableArg = null)
        {
            DbCommand dbCommand = CreateDbCommand();
            string sqlStr = "Select {1} From {0}";
            string selectParams = null;
            EntityMapping dtoDbMapping = EntityMappingHelper.GetMapping<T>();
            List<FieldMapping> piMapList = EntityMappingHelper.GetPiMapList<T>(filter);
            foreach (FieldMapping piMap in piMapList)
            {
                selectParams += string.IsNullOrEmpty(selectParams) ? piMap.FieldName : $",{piMap.FieldName}";
            }
            if (filter != null)
            {
                if (filter.ItemList.Count > 0)
                {
                    sqlStr += filter.FilterSQLString;
                }
                if (!string.IsNullOrEmpty(filter.OrderSQLString))
                {
                    sqlStr += filter.OrderSQLString;
                }
                for (int i = 0; i < filter.FilterParameters.Count; i++)
                {
                    DbParameter dbParameter = GetQueryParameter(dbCommand, filter.FilterParameters[i]);
                    dbCommand.Parameters.Add(dbParameter);
                }
            }
            dbCommand.CommandText = string.Format(sqlStr, dtoDbMapping.GetTableName(subTableArg), selectParams);
            return dbCommand;
        }

        /// <summary>
        /// 获取查询DbCommand 获取所有字段 Select * From {0}
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="filter"></param>
        /// <param name="subTableArg">表名称参数.如果TableAttr设置Name.则根据Name格式化</param>
        /// <returns></returns>
        internal DbCommand GetQueryAllFieldDbCommand<T>(QueryFilter filter = null, string subTableArg = null)
        {
            DbCommand dbCommand = CreateDbCommand();
            string sqlStr = "Select * From {0}";
            EntityMapping dtoDbMapping = EntityMappingHelper.GetMapping<T>();
            List<FieldMapping> piMapList = EntityMappingHelper.GetPiMapList<T>(filter);
            if (filter != null)
            {
                if (filter.ItemList.Count > 0)
                {
                    sqlStr += filter.FilterSQLString;
                }
                if (!string.IsNullOrEmpty(filter.OrderSQLString))
                {
                    sqlStr += filter.OrderSQLString;
                }
                for (int i = 0; i < filter.FilterParameters.Count; i++)
                {
                    DbParameter dbParameter = GetQueryParameter(dbCommand, filter.FilterParameters[i]);
                    dbCommand.Parameters.Add(dbParameter);
                }
            }
            dbCommand.CommandText = string.Format(sqlStr, dtoDbMapping.GetTableName(subTableArg));
            return dbCommand;
        }

        /// <summary>
        /// 获取查询DbCommand
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="id"></param>
        /// <param name="piMapList">查询属性MapList</param>
        /// <param name="subTableArg">表名称参数.如果TableAttr设置Name.则根据Name格式化</param>
        /// <returns></returns>
        internal DbCommand GetQueryByIdDbCommand<T>(object id, List<FieldMapping> piMapList, string subTableArg = null)
        {
            DbCommand dbCommand = CreateDbCommand();
            EntityMapping dtoDbMapping = EntityMappingHelper.GetMapping<T>();
            FieldMapping pkField = dtoDbMapping.GetPkField();
            string sqlStr = "Select {1} From {0} where {2}";
            string selectParams = null;
            string whereParams = null;
            foreach (FieldMapping piMap in piMapList)
            {
                selectParams += string.IsNullOrEmpty(selectParams) ? piMap.FieldName : "," + piMap.FieldName;
            }
            DbParameter dbParameter = dbCommand.CreateParameter();
            dbParameter.Direction = ParameterDirection.Input;
            dbParameter.ParameterName = "@" + pkField.FieldName;
            dbParameter.Value = id;
            dbParameter.DbType = pkField.DbType;
            dbCommand.Parameters.Add(dbParameter);

            whereParams = pkField.FieldName + "=@" + pkField.FieldName;

            dbCommand.CommandText = string.Format(sqlStr, dtoDbMapping.GetTableName(subTableArg), selectParams, whereParams);
            return dbCommand;
        }

        /// <summary>
        /// 获取字段求和DbCommand 返回Total列
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="filter"></param>
        /// <param name="subTableArg">表名称参数.如果TableAttr设置Name.则根据Name格式化</param>
        /// <returns></returns>
        internal DbCommand GetSumDbCommand<T>(QueryFilter filter = null, string subTableArg = null)
        {
            DbCommand dbCommand = CreateDbCommand();
            string sqlStr = "Select Sum({1}) as Total From {0}";
            string selectParams = null;
            EntityMapping dtoDbMapping = EntityMappingHelper.GetMapping<T>();
            FieldMapping piMap = EntityMappingHelper.GetPiMapList<T>(filter).FirstOrDefault();
            if (piMap == null)
            {
                throw new Exception("求和字段不能为空");
            }
            selectParams = piMap.FieldName;
            if (filter != null)
            {
                if (filter.ItemList.Count > 0)
                {
                    sqlStr += filter.FilterSQLString;
                }
                for (int i = 0; i < filter.FilterParameters.Count; i++)
                {
                    DbParameter dbParameter = GetQueryParameter(dbCommand, filter.FilterParameters[i]);
                    dbCommand.Parameters.Add(dbParameter);
                }
            }
            dbCommand.CommandText = string.Format(sqlStr, dtoDbMapping.GetTableName(subTableArg), selectParams);
            return dbCommand;
        }

        /// <summary>
        /// 获取字段求和DbCommand 返回Total列
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="filter"></param>
        /// <param name="subTableArg">表名称参数.如果TableAttr设置Name.则根据Name格式化</param>
        /// <returns></returns>
        internal DbCommand GetMinDbCommand<T>(QueryFilter filter = null, string subTableArg = null)
        {
            DbCommand dbCommand = CreateDbCommand();
            string sqlStr = "Select Min({1}) as Total From {0}";
            string selectParams = null;
            EntityMapping dtoDbMapping = EntityMappingHelper.GetMapping<T>();
            FieldMapping piMap = EntityMappingHelper.GetPiMapList<T>(filter).FirstOrDefault();
            if (piMap == null)
            {
                throw new Exception("计算字段不能为空");
            }
            selectParams = piMap.FieldName;
            if (filter != null)
            {
                if (filter.ItemList.Count > 0)
                {
                    sqlStr += filter.FilterSQLString;
                }
                
                for (int i = 0; i < filter.FilterParameters.Count; i++)
                {
                    DbParameter dbParameter = GetQueryParameter(dbCommand, filter.FilterParameters[i]);
                    dbCommand.Parameters.Add(dbParameter);
                }
            }
            dbCommand.CommandText = string.Format(sqlStr, dtoDbMapping.GetTableName(subTableArg), selectParams);
            return dbCommand;
        }

        /// <summary>
        /// 获取字段求和DbCommand 返回Total列
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="filter"></param>
        /// <param name="subTableArg">表名称参数.如果TableAttr设置Name.则根据Name格式化</param>
        /// <returns></returns>
        internal DbCommand GetMaxDbCommand<T>(QueryFilter filter = null, string subTableArg = null)
        {
            DbCommand dbCommand = CreateDbCommand();
            string sqlStr = "Select Max({1}) as Total From {0}";
            string selectParams = null;
            EntityMapping dtoDbMapping = EntityMappingHelper.GetMapping<T>();
            FieldMapping piMap = EntityMappingHelper.GetPiMapList<T>(filter).FirstOrDefault();
            if (piMap == null)
            {
                throw new Exception("计算字段不能为空");
            }
            selectParams = piMap.FieldName;
            if (filter != null)
            {
                if (filter.ItemList.Count > 0)
                {
                    sqlStr += filter.FilterSQLString;
                }
                
                for (int i = 0; i < filter.FilterParameters.Count; i++)
                {
                    DbParameter dbParameter = GetQueryParameter(dbCommand, filter.FilterParameters[i]);
                    dbCommand.Parameters.Add(dbParameter);
                }
            }
            dbCommand.CommandText = string.Format(sqlStr, dtoDbMapping.GetTableName(subTableArg), selectParams);
            return dbCommand;
        }

        /// <summary>
        /// 获取查询RecCountDbCommand
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="filter"></param>
        /// <param name="subTableArg">表名称参数.如果TableAttr设置Name.则根据Name格式化</param>
        /// <returns></returns>
        internal DbCommand GetCountDbCommand<T>(QueryFilter filter, string subTableArg = null)
        {
            DbCommand dbCommand = CreateDbCommand();
            string sqlStr = "Select Count(*) as RecCount From {0} {1}";
            string queryStr = "";
            EntityMapping dtoDbMapping = EntityMappingHelper.GetMapping<T>();
            if (filter != null)
            {
                if (filter.ItemList.Count > 0)
                {
                    queryStr = filter.FilterSQLString;
                }
                for (int i = 0; i < filter.FilterParameters.Count; i++)
                {
                    DbParameter dbParameter = GetQueryParameter(dbCommand,filter.FilterParameters[i]);
                    dbCommand.Parameters.Add(dbParameter);
                }
            }
            dbCommand.CommandText = string.Format(sqlStr, dtoDbMapping.GetTableName(subTableArg), queryStr);
            return dbCommand;
        }

        /// <summary>
        /// Get Parameter Value
        /// </summary>
        /// <param name="piMap"></param>
        /// <param name="dto"></param>
        /// <returns></returns>
        protected virtual object GetParameterValue(FieldMapping piMap, object dto)
        {
            object dbValue = DBNull.Value;
            object piValue = piMap.Pi.GetValue(dto);
            if (piValue != null)
            {
                if (piMap.IsEnum)
                {   //如果为枚举类型.转换为int
                    //目前暂不支持 字段类型为枚举支持
                    dbValue = (int)piValue;
                }
                else
                {
                    dbValue = piValue;
                }
            }
            return dbValue;
        }

        /// <summary>
        /// Get Query DbParameter
        /// </summary>
        /// <param name="dbCommand"></param>
        /// <param name="queryParameter"></param>
        /// <returns></returns>
        protected virtual DbParameter GetQueryParameter(DbCommand dbCommand, QueryParameter queryParameter)
        {
            DbParameter dbParameter = dbCommand.CreateParameter();
            dbParameter.Direction = ParameterDirection.Input;
            dbParameter.ParameterName = queryParameter.ParameterName;
            dbParameter.Value = queryParameter.ParameterValue;
            dbParameter.DbType = queryParameter.ParameterDbType;
            return dbParameter;
        }

        /// <summary>
        /// 获取ConTestDbCommand
        /// </summary>
        /// <returns></returns>
        internal DbCommand GetConTestDbCommand()
        {
            DbCommand dbCommand = CreateDbCommand();
            string sqlStr = "Select 1 as a;";
            dbCommand.CommandText = sqlStr;
            return dbCommand;
        }

        #endregion
    }
}