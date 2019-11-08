using Jc.Core.Data.Query;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Common;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace Jc.Core.Data
{
    /// <summary>
    /// Db Provider
    /// </summary>
    public abstract class DbProvider
    {
        #region Fields

        private string dbName;
        private string connectString;

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
        #endregion

        /// <summary>
        /// Ctor
        /// </summary>
        /// <param name="connectString"></param>
        internal DbProvider(string connectString)
        {
            this.ConnectString = connectString;
            this.DbName = GetDbNameFromConnectString(connectString);
        }

        /// <summary>
        /// 获取DbProvider
        /// </summary>
        /// <param name="connectString">连接串</param>
        /// <param name="dbType">数据库类型</param>
        public static DbProvider GetDbProvider(string connectString, DatabaseType dbType = DatabaseType.MsSql)
        {
            DbProvider dbProvider = null;
            switch (dbType)
            {
                case DatabaseType.MsSql:
                    dbProvider = new MsSqlDbProvider(connectString);
                    break;
                case DatabaseType.Sqlite:
                    dbProvider = new SqliteDbProvider(connectString);
                    break;
                case DatabaseType.MySql:
                    dbProvider = new MySqlDbProvider(connectString);
                    break;
                case DatabaseType.PostgreSql:
                    dbProvider = new PostgreSqlDbProvider(connectString);
                    break;
                default:
                    dbProvider = new MsSqlDbProvider(connectString);
                    break;
            }
            return dbProvider;
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
        public DbConnection CreateDbConnection(string connectString)
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
        /// <param name="tableNamePfx">表名称参数.如果TableAttr设置Name.则根据Name格式化</param>
        /// <returns></returns>
        public abstract DbCommand GetQueryRecordsPageDbCommand<T>(QueryFilter filter, string tableNamePfx = null);

        /// <summary>
        /// 获取检查表是否存在DbCommand
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="tableName">表名称,如果为空,则使用T对应表名称</param>
        /// <returns></returns>
        public abstract DbCommand GetCheckTableExistsDbCommand<T>(string tableName = null);

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
        #endregion

        #region Methods
        
        /// <summary>
        /// 获取插入DbCmd
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="dto"></param>
        /// <param name="piMapList">查询属性MapList</param>
        /// <param name="tableNamePfx">表名称参数.如果TableAttr设置Name.则根据Name格式化</param>
        /// <returns></returns>
        internal DbCommand GetInsertDbCmd<T>(T dto, List<PiMap> piMapList, string tableNamePfx = null) where T : class, new()
        {
            DbCommand dbCommand = CreateDbCommand();
            DtoMapping dtoDbMapping = DtoMappingHelper.GetDtoMapping<T>();
            #region 设置DbCommand
            string sqlStr = "Insert into {0} ({1}) values({2})";
            string fieldParams = null;
            string valueParams = null;
            foreach (PiMap piMap in piMapList)
            {
                PropertyInfo pi = piMap.Pi;
                if (piMap.FieldAttr.ReadOnly) continue;  //跳过只读字段
                if (piMap.FieldAttr.IsPk)
                {   //如果是自动设置Id 主键是int or long 自增Id 为null or 0 跳过插入
                    object pkValue = pi.GetValue(dto);
                    if (pi.PropertyType == typeof(int) || pi.PropertyType == typeof(int?))
                    {
                        if (pkValue ==null || (int)pi.GetValue(dto, null) == 0)
                        {
                            continue;
                        }
                    }
                    else if (pi.PropertyType == typeof(long) || pi.PropertyType == typeof(long?))
                    {
                        if (pkValue == null || (long)pi.GetValue(dto, null) == 0)
                        {
                            continue;
                        }
                    }
                }
                fieldParams = string.IsNullOrEmpty(fieldParams) ? piMap.FieldName : fieldParams + "," + piMap.FieldName;
                valueParams = string.IsNullOrEmpty(valueParams) ? "@" + piMap.FieldName : valueParams + ",@" + piMap.FieldName;

                DbParameter dbParameter = dbCommand.CreateParameter();
                dbParameter.Direction = ParameterDirection.Input;
                dbParameter.ParameterName = "@" + piMap.FieldName;
                dbParameter.Value = pi.GetValue(dto) ?? DBNull.Value;
                dbParameter.DbType = DbTypeConvertor.TypeToDbType(piMap.PropertyType);
                dbCommand.Parameters.Add(dbParameter);
            }
            dbCommand.CommandText = string.Format(sqlStr, dtoDbMapping.GetTableName<T>(tableNamePfx), fieldParams, valueParams);
            #endregion
            return dbCommand;
        }


        /// <summary>
        /// 获取插入DbCmd
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="list"></param>
        /// <param name="piMapList">查询属性MapList</param>
        /// <param name="tableNamePfx">表名称参数.如果TableAttr设置Name.则根据Name格式化</param>
        /// <returns></returns>
        internal DbCommand GetInsertDbCmd<T>(List<T> list, List<PiMap> piMapList, string tableNamePfx = null) where T : class, new()
        {
            DbCommand dbCommand = CreateDbCommand();
            DtoMapping dtoDbMapping = DtoMappingHelper.GetDtoMapping<T>();
            #region 设置DbCommand
            string sqlStr = "Insert into {0} ({1}) values{2}";
            string fieldParams = null;
            string valueParams = null;

            foreach (PiMap piMap in piMapList)
            {
                PropertyInfo pi = piMap.Pi;
                if (piMap.FieldAttr.ReadOnly) continue;  //跳过只读字段
                if (piMap.FieldAttr.IsPk)
                {   //如果是自动设置Id 主键是int or long 自增Id 为null or 0 跳过插入
                    if (pi.PropertyType == typeof(int) || pi.PropertyType == typeof(int?))
                    {
                        continue;
                    }
                    else if (pi.PropertyType == typeof(long) || pi.PropertyType == typeof(long?))
                    {
                        continue;
                    }
                }
                fieldParams = string.IsNullOrEmpty(fieldParams) ? piMap.FieldName : fieldParams + "," + piMap.FieldName;
            }
            StringBuilder strBuilder = new StringBuilder();
            for (int i = 0; i < list.Count; i++)
            {
                strBuilder.Append("(");
                StringBuilder itemBuilder = new StringBuilder();
                foreach (PiMap piMap in piMapList)
                {
                    PropertyInfo pi = piMap.Pi;
                    if (piMap.FieldAttr.ReadOnly) continue;  //跳过只读字段
                    if (piMap.FieldAttr.IsPk)
                    {   //如果是自动设置Id 主键是int or long 自增Id 为null or 0 跳过插入
                        object pkValue = pi.GetValue(list[i]);
                        if (pi.PropertyType == typeof(int) || pi.PropertyType == typeof(int?))
                        {
                            continue;                            
                        }
                        else if (pi.PropertyType == typeof(long) || pi.PropertyType == typeof(long?))
                        {
                            continue;                            
                        }
                    }
                    itemBuilder.Append($"@{piMap.FieldName}{i},");
                    DbParameter dbParameter = dbCommand.CreateParameter();
                    dbParameter.Direction = ParameterDirection.Input;
                    dbParameter.ParameterName = $"@{piMap.FieldName}{i}";
                    dbParameter.Value = pi.GetValue(list[i]) ?? DBNull.Value;
                    dbParameter.DbType = DbTypeConvertor.TypeToDbType(piMap.PropertyType);
                    dbCommand.Parameters.Add(dbParameter);
                }
                strBuilder.Append(itemBuilder.ToString().TrimEnd(','));
                strBuilder.Append("),");
            }
            valueParams = strBuilder.ToString().TrimEnd(',');
            dbCommand.CommandText = string.Format(sqlStr, dtoDbMapping.GetTableName<T>(tableNamePfx), fieldParams, valueParams);
            #endregion
            return dbCommand;
        }

        /// <summary>
        /// 获取导入DbCmd 导入时,自带Id
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="dto"></param>
        /// <param name="piMapList">查询属性MapList</param>
        /// <param name="tableNamePfx">表名称参数.如果TableAttr设置Name.则根据Name格式化</param>
        /// <returns></returns>
        internal DbCommand GetImportDbCmd<T>(T dto, List<PiMap> piMapList, string tableNamePfx = null) where T : class, new()
        {
            DbCommand dbCommand = CreateDbCommand();
            DtoMapping dtoDbMapping = DtoMappingHelper.GetDtoMapping<T>();
            #region 设置DbCommand
            string sqlStr = "Insert into {0} ({1}) values({2})";
            string fieldParams = null;
            string valueParams = null;
            foreach (PiMap piMap in piMapList)
            {
                PropertyInfo pi = piMap.Pi;
                if (piMap.FieldAttr.ReadOnly) continue;  //跳过只读字段
                fieldParams = string.IsNullOrEmpty(fieldParams) ? piMap.FieldName : fieldParams + "," + piMap.FieldName;
                valueParams = string.IsNullOrEmpty(valueParams) ? "@" + piMap.FieldName : valueParams + ",@" + piMap.FieldName;

                DbParameter dbParameter = dbCommand.CreateParameter();
                dbParameter.Direction = ParameterDirection.Input;
                dbParameter.ParameterName = "@" + piMap.FieldName;
                dbParameter.Value = pi.GetValue(dto) ?? DBNull.Value;
                dbParameter.DbType = DbTypeConvertor.TypeToDbType(piMap.PropertyType);
                dbCommand.Parameters.Add(dbParameter);
            }
            dbCommand.CommandText = string.Format(sqlStr, dtoDbMapping.GetTableName<T>(tableNamePfx), fieldParams, valueParams);
            #endregion
            return dbCommand;
        }

        /// <summary>
        /// 获取更新DbCmd
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="dto"></param>
        /// <param name="piMapList">查询属性MapList</param>
        /// <param name="tableNamePfx">表名称参数.如果TableAttr设置Name.则根据Name格式化</param>
        /// <returns></returns>
        internal DbCommand GetUpdateDbCmd<T>(T dto, List<PiMap> piMapList, string tableNamePfx = null)
        {
            DbCommand dbCommand = CreateDbCommand();
            DtoMapping dtoDbMapping = DtoMappingHelper.GetDtoMapping<T>();
            #region 设置DbCommand

            string sqlStr = "Update {0} set {1} where {2}";
            string setParams = null;
            string whereParams = null;
            foreach (PiMap piMap in piMapList)
            {
                DbParameter dbParameter = dbCommand.CreateParameter();
                dbParameter.Direction = ParameterDirection.Input;
                dbParameter.ParameterName = "@" + piMap.FieldName;
                object value = piMap.Pi.GetValue(dto);
                dbParameter.Value = value != null ? value : DBNull.Value;
                dbParameter.DbType = DbTypeConvertor.TypeToDbType(piMap.PropertyType);
                dbCommand.Parameters.Add(dbParameter);
                setParams = string.IsNullOrEmpty(setParams) ? piMap.FieldName + "=@" + piMap.FieldName : setParams + "," + piMap.FieldName + "=@" + piMap.FieldName;
            }
            if (!piMapList.Contains(dtoDbMapping.PkMap))
            {   //如果更新列表没有包含主键参数,需要添加根据主键更新
                DbParameter dbPkParameter = dbCommand.CreateParameter();
                dbPkParameter.Direction = ParameterDirection.Input;
                dbPkParameter.ParameterName = "@" + dtoDbMapping.PkMap.FieldName;
                object pkValue = dtoDbMapping.PkMap.Pi.GetValue(dto);
                dbPkParameter.Value = pkValue != null ? pkValue : DBNull.Value;
                dbCommand.Parameters.Add(dbPkParameter);
            }

            whereParams = dtoDbMapping.PkMap.FieldName + "=@" + dtoDbMapping.PkMap.FieldName;
            
            dbCommand.CommandText = string.Format(sqlStr, dtoDbMapping.GetTableName<T>(tableNamePfx), setParams, whereParams);
            #endregion

            return dbCommand;
        }

        /// <summary>
        /// 获取删除DbCmd
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="dto"></param>
        /// <param name="tableNamePfx">表名称参数.如果TableAttr设置Name.则根据Name格式化</param>
        /// <returns></returns>
        internal DbCommand GetDeleteDbCmd<T>(T dto, string tableNamePfx = null)
        {
            DbCommand dbCommand = CreateDbCommand();

            #region 设置DbCommand

            string sqlStr = "Delete From {0} Where {1}";
            string whereParams = null;
            DtoMapping dtoDbMapping = DtoMappingHelper.GetDtoMapping<T>();

            DbParameter dbParameter = dbCommand.CreateParameter();
            dbParameter.Direction = ParameterDirection.Input;
            dbParameter.ParameterName = "@" + dtoDbMapping.PkMap.FieldName;
            dbParameter.Value = dtoDbMapping.PkMap.Pi.GetValue(dto);
            dbParameter.DbType = DbTypeConvertor.TypeToDbType(dtoDbMapping.PkMap.PropertyType);
            dbCommand.Parameters.Add(dbParameter);

            whereParams = dtoDbMapping.PkMap.FieldName + "=@" + dtoDbMapping.PkMap.FieldName;
            dbCommand.CommandText = string.Format(sqlStr, dtoDbMapping.GetTableName<T>(tableNamePfx), whereParams);
            #endregion

            return dbCommand;
        }

        /// <summary>
        /// 获取删除DbCmd
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="filter">过滤条件</param>
        /// <param name="tableNamePfx">表名称参数.如果TableAttr设置Name.则根据Name格式化</param>
        /// <returns></returns>
        internal DbCommand GetDeleteDbCmd<T>(QueryFilter filter, string tableNamePfx = null)
        {
            DbCommand dbCommand = CreateDbCommand();

            #region 设置DbCommand

            string sqlStr = "Delete From {0}";

            DtoMapping dtoDbMapping = DtoMappingHelper.GetDtoMapping<T>();

            if (filter == null || filter.FilterParameters.Count <= 0)
            {
                throw new Exception("条件删除时,删除条件不能为空.");
            }
            sqlStr += filter.FilterSQLString;

            for (int i = 0; i < filter.FilterParameters.Count; i++)
            {
                DbParameter dbParameter = dbCommand.CreateParameter();
                dbParameter.Direction = ParameterDirection.Input;
                dbParameter.ParameterName = filter.FilterParameters[i].ParameterName;
                dbParameter.Value = filter.FilterParameters[i].ParameterValue;
                dbParameter.DbType = filter.FilterParameters[i].ParameterDbType;
                dbCommand.Parameters.Add(dbParameter);
            }
            dbCommand.CommandText = string.Format(sqlStr, dtoDbMapping.GetTableName<T>(tableNamePfx));
            #endregion

            return dbCommand;
        }

        /// <summary>
        /// 获取删除DbCmd
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="tableNamePfx">表名称参数.如果TableAttr设置Name.则根据Name格式化</param>
        /// <returns></returns>
        internal DbCommand GetDeleteByIdDbCmd<T>(object pkValue, string tableNamePfx = null)
        {
            DbCommand dbCommand = CreateDbCommand();

            #region 设置DbCommand

            string sqlStr = "Delete From {0} Where {1}";
            string whereParams = null;
            DtoMapping dtoDbMapping = DtoMappingHelper.GetDtoMapping<T>();

            DbParameter dbParameter = dbCommand.CreateParameter();
            dbParameter.Direction = ParameterDirection.Input;
            dbParameter.ParameterName = "@" + dtoDbMapping.PkMap.FieldName;
            dbParameter.Value = pkValue;
            dbParameter.DbType = DbTypeConvertor.TypeToDbType(dtoDbMapping.PkMap.PropertyType);
            dbCommand.Parameters.Add(dbParameter);

            whereParams = dtoDbMapping.PkMap.FieldName + "=@" + dtoDbMapping.PkMap.FieldName;
            dbCommand.CommandText = string.Format(sqlStr, dtoDbMapping.GetTableName<T>(tableNamePfx), whereParams);
            #endregion

            return dbCommand;
        }

        /// <summary>
        /// 获取查询DbCommand
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="filter"></param>
        /// <param name="tableNamePfx">表名称参数.如果TableAttr设置Name.则根据Name格式化</param>
        /// <returns></returns>
        internal DbCommand GetQueryDbCommand<T>(QueryFilter filter = null, string tableNamePfx = null)
        {
            DbCommand dbCommand = CreateDbCommand();
            string sqlStr = "Select {1} From {0}";
            string selectParams = null;
            DtoMapping dtoDbMapping = DtoMappingHelper.GetDtoMapping<T>();
            List<PiMap> piMapList = DtoMappingHelper.GetPiMapList<T>(filter);
            foreach (PiMap piMap in piMapList)
            {
                selectParams += string.IsNullOrEmpty(selectParams) ? piMap.FieldName : "," + piMap.FieldName;
            }
            if (filter != null && filter.ItemList.Count>0)
            {
                sqlStr += filter.FilterSQLString;
            }
            if (filter != null && !string.IsNullOrEmpty(filter.OrderSQLString))
            {
                sqlStr += filter.OrderSQLString;
            }
            if (filter != null)
            {
                for (int i = 0; i < filter.FilterParameters.Count; i++)
                {
                    DbParameter dbParameter = dbCommand.CreateParameter();
                    dbParameter.Direction = ParameterDirection.Input;
                    dbParameter.ParameterName = filter.FilterParameters[i].ParameterName;
                    dbParameter.Value = filter.FilterParameters[i].ParameterValue;
                    dbParameter.DbType = filter.FilterParameters[i].ParameterDbType;
                    dbCommand.Parameters.Add(dbParameter);
                }
            }
            dbCommand.CommandText = string.Format(sqlStr, dtoDbMapping.GetTableName<T>(tableNamePfx), selectParams);
            return dbCommand;
        }
        
        /// <summary>
        /// 获取查询DbCommand
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="id"></param>
        /// <param name="piMapList">查询属性MapList</param>
        /// <param name="tableNamePfx">表名称参数.如果TableAttr设置Name.则根据Name格式化</param>
        /// <returns></returns>
        internal DbCommand GetQueryByIdDbCommand<T>(object id, List<PiMap> piMapList, string tableNamePfx = null)
        {
            DbCommand dbCommand = CreateDbCommand();
            DtoMapping dtoDbMapping = DtoMappingHelper.GetDtoMapping<T>();
            string sqlStr = "Select {1} From {0} where {2}";
            string selectParams = null;
            string whereParams = null;
            foreach (PiMap piMap in piMapList)
            {
                selectParams += string.IsNullOrEmpty(selectParams) ? piMap.FieldName : "," + piMap.FieldName;
            }
            DbParameter dbParameter = dbCommand.CreateParameter();
            dbParameter.Direction = ParameterDirection.Input;
            dbParameter.ParameterName = "@" + dtoDbMapping.PkMap.FieldName;
            dbParameter.Value = id;
            dbParameter.DbType = DbTypeConvertor.TypeToDbType(dtoDbMapping.PkMap.PropertyType);
            dbCommand.Parameters.Add(dbParameter);

            whereParams = dtoDbMapping.PkMap.FieldName + "=@" + dtoDbMapping.PkMap.FieldName;

            dbCommand.CommandText = string.Format(sqlStr, dtoDbMapping.GetTableName<T>(tableNamePfx), selectParams, whereParams);
            return dbCommand;
        }

        /// <summary>
        /// 获取字段求和DbCommand 返回Total列
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="filter"></param>
        /// <param name="tableNamePfx">表名称参数.如果TableAttr设置Name.则根据Name格式化</param>
        /// <returns></returns>
        internal DbCommand GetSumDbCommand<T>(QueryFilter filter = null, string tableNamePfx = null)
        {
            DbCommand dbCommand = CreateDbCommand();
            string sqlStr = "Select Sum{1} as Total From {0}";
            string selectParams = null;
            DtoMapping dtoDbMapping = DtoMappingHelper.GetDtoMapping<T>();
            PiMap piMap = DtoMappingHelper.GetPiMapList<T>(filter).FirstOrDefault();
            if (piMap == null)
            {
                throw new Exception("求和字段不能为空");
            }
            selectParams = piMap.FieldName;            
            if (filter != null && filter.ItemList.Count > 0)
            {
                sqlStr += filter.FilterSQLString;
            }
            if (filter != null && filter.FilterParameters.Count>0)
            {
                for (int i = 0; i < filter.FilterParameters.Count; i++)
                {
                    DbParameter dbParameter = dbCommand.CreateParameter();
                    dbParameter.Direction = ParameterDirection.Input;
                    dbParameter.ParameterName = filter.FilterParameters[i].ParameterName;
                    dbParameter.Value = filter.FilterParameters[i].ParameterValue;
                    dbParameter.DbType = filter.FilterParameters[i].ParameterDbType;
                    dbCommand.Parameters.Add(dbParameter);
                }
            }
            dbCommand.CommandText = string.Format(sqlStr, dtoDbMapping.GetTableName<T>(tableNamePfx), selectParams);
            return dbCommand;
        }


        /// <summary>
        /// 获取CountDbCommand 返回RecCount列
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="filter"></param>
        /// <param name="tableNamePfx">表名称参数.如果TableAttr设置Name.则根据Name格式化</param>
        /// <returns></returns>
        internal DbCommand GetCountDbCommand<T>(QueryFilter filter = null, string tableNamePfx = null)
        {
            DbCommand dbCommand = CreateDbCommand();
            string sqlStr = "Select Count(*) as RecCount From {0}";
            string selectParams = null;
            DtoMapping dtoDbMapping = DtoMappingHelper.GetDtoMapping<T>();
            if (filter != null && filter.ItemList.Count > 0)
            {
                sqlStr += filter.FilterSQLString;
            }
            if (filter != null && filter.FilterParameters.Count > 0)
            {
                for (int i = 0; i < filter.FilterParameters.Count; i++)
                {
                    DbParameter dbParameter = dbCommand.CreateParameter();
                    dbParameter.Direction = ParameterDirection.Input;
                    dbParameter.ParameterName = filter.FilterParameters[i].ParameterName;
                    dbParameter.Value = filter.FilterParameters[i].ParameterValue;
                    dbParameter.DbType = filter.FilterParameters[i].ParameterDbType;
                    dbCommand.Parameters.Add(dbParameter);
                }
            }
            dbCommand.CommandText = string.Format(sqlStr, dtoDbMapping.GetTableName<T>(tableNamePfx), selectParams);
            return dbCommand;
        }

        /// <summary>
        /// 获取查询RecCountDbCommand
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="filter"></param>
        /// <param name="tableNamePfx">表名称参数.如果TableAttr设置Name.则根据Name格式化</param>
        /// <returns></returns>
        internal DbCommand GetRecCountDbCommand<T>(QueryFilter filter, string tableNamePfx = null)
        {
            DbCommand dbCommand = CreateDbCommand();
            string sqlStr = "Select Count(*) as RecCount From {0} {1}";
            string queryStr = "";
            DtoMapping dtoDbMapping = DtoMappingHelper.GetDtoMapping<T>();
            if (filter != null && filter.ItemList.Count > 0)
            {
                queryStr = filter.FilterSQLString;
            }
            if (filter != null && filter.FilterParameters.Count > 0)
            {
                for (int i = 0; i < filter.FilterParameters.Count; i++)
                {
                    DbParameter dbParameter = dbCommand.CreateParameter();
                    dbParameter.Direction = ParameterDirection.Input;
                    dbParameter.ParameterName = filter.FilterParameters[i].ParameterName;
                    dbParameter.Value = filter.FilterParameters[i].ParameterValue;
                    dbParameter.DbType = filter.FilterParameters[i].ParameterDbType;
                    dbCommand.Parameters.Add(dbParameter);
                }
            }
            dbCommand.CommandText = string.Format(sqlStr, dtoDbMapping.GetTableName<T>(tableNamePfx), queryStr);
            return dbCommand;
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
