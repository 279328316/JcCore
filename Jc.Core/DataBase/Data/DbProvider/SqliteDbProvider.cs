using System;
using System.Collections.Generic;
using System.Text;
using System.Data.Common;
using System.Linq;
using System.IO;
using Jc.Data.Query;
using System.Reflection;
using System.Data;

namespace Jc.Data
{
    /// <summary>
    /// sqlite数据库
    /// </summary>
    public class SqliteDbProvider : DbProvider
    {
        private static IDbCreator sqliteCreator;//使用静态变量缓存MsSqlDbCreator
        /// <summary>
        /// Ctor
        /// </summary>
        /// <param name="connectString"></param>
        public SqliteDbProvider(string connectString) : base(connectString)
        {
            if (sqliteCreator == null)
            {
                sqliteCreator = new SqliteDbCreator();
            }
            this.dbCreator = sqliteCreator;
        }

        /// <summary>
        /// 自连接串中获取DbName
        /// </summary>
        /// <returns></returns>
        public override string GetDbNameFromConnectString(string connectString)
        {
            string dbName = "";
            int dbInfoStartIndex = connectString.ToLower().IndexOf("data source");
            if (dbInfoStartIndex == -1)
            {
                dbInfoStartIndex = connectString.ToLower().IndexOf("database");
            }
            if (dbInfoStartIndex == -1)
            {
                throw new Exception("无效的数据库连接串");
            }
            int dbNameStartIndex = connectString.IndexOf("=", dbInfoStartIndex);
            int dbNameEndIndex = connectString.IndexOf(";", dbInfoStartIndex);
            if (dbNameEndIndex >= 0)
            {
                dbName = connectString.Substring(dbNameStartIndex + 1, dbNameEndIndex - dbNameStartIndex - 1);
            }
            else
            {
                dbName = connectString.Substring(dbNameStartIndex + 1);
            }
            return dbName;
        }

        /// <summary>
        /// 获取查询自增IdDbCommand
        /// </summary>
        /// <returns></returns>
        public override DbCommand GetAutoIdDbCommand()
        {
            DbCommand dbCommand = CreateDbCommand();
            dbCommand.CommandText = "select last_insert_rowid()";
            return dbCommand;
        }

        /// <summary>
        /// 获取分页查询DbCommand
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="filter">过滤条件</param>
        /// <param name="subTableArg">表名称,如果为空,则使用T对应表名称</param>
        /// <returns></returns>
        public override DbCommand GetQueryRecordsPageDbCommand<T>(QueryFilter filter, string subTableArg = null)
        {
            DbCommand dbCommand = CreateDbCommand();
            //表名 查询字段名 主键字段名
            string sqlStr = "Select {1} From {0} @QueryStr @OrderStr limit @PageSize offset @LowRecNum";
            string selectParams = null;
            string queryStr = null;
            string orderStr = null;

            DtoMapping dtoDbMapping = DtoMappingHelper.GetDtoMapping<T>();
            List<PiMap> piMapList = DtoMappingHelper.GetPiMapList<T>(filter);
            foreach (PiMap piMap in piMapList)
            {
                selectParams += string.IsNullOrEmpty(selectParams) ? piMap.FieldName : "," + piMap.FieldName;
            }
            if (filter != null && filter.ItemList.Count>0)
            {
                queryStr = filter.FilterSQLString;
            }
            if (filter != null && !string.IsNullOrEmpty(filter.OrderSQLString))
            {
                orderStr = filter.OrderSQLString;
            }
            else
            {   //默认By 主键 Asc
                orderStr = "order by {2} asc";
            }
            sqlStr = sqlStr.Replace("@QueryStr", queryStr);
            sqlStr = sqlStr.Replace("@LowRecNum", filter.FilterStartIndex.ToString());
            sqlStr = sqlStr.Replace("@PageSize", filter.PageSize.ToString());
            sqlStr = sqlStr.Replace("@OrderStr", orderStr);
            if (filter != null)
            {
                for (int i = 0; i < filter.FilterParameters.Count; i++)
                {
                    DbParameter dbParameter = dbCommand.CreateParameter();
                    dbParameter.ParameterName = filter.FilterParameters[i].ParameterName;
                    dbParameter.Value = filter.FilterParameters[i].ParameterValue;
                    dbParameter.DbType = filter.FilterParameters[i].ParameterDbType;
                    dbCommand.Parameters.Add(dbParameter);
                }
            }
            dbCommand.CommandText = string.Format(sqlStr, dtoDbMapping.GetTableName(subTableArg), selectParams, dtoDbMapping.PkMap.FieldName);
            return dbCommand;
        }


        /// <summary>
        /// 获取建表DbCommand
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="subTableArg">表名称,如果为空,则使用T对应表名称</param>
        /// <returns></returns>
        public override DbCommand GetCreateTableDbCommand<T>(string subTableArg = null)
        {
            DbCommand dbCommand = CreateDbCommand();
            dbCommand.CommandText = GetCreateTableSql<T>(subTableArg);
            return dbCommand;
        }

        /// <summary>
        /// 获取所有表DbCommand
        /// </summary>
        /// <returns></returns>
        public override DbCommand GetTableListDbCommand()
        {
            DbCommand dbCommand = CreateDbCommand();
            string sqlStr = $@"Select name as TableName,
                                case(type) when 'table' then 'Table' when 'view' then 'View' else 'Table' end as TableTypeStr,
                                '' as TableDes
                                from sqlite_master order by type,name";
            dbCommand.CommandText = sqlStr;
            return dbCommand;
        }

        /// <summary>
        /// 获取所有表字段DbCommand
        /// </summary>
        /// <returns></returns>
        public override DbCommand GetFieldListDbCommand(string tableName)
        {
            //pragma table_info('finance_invoice')
            throw new Exception("Sqlite获取字段列表,请使用pragma.");
        }

        /// <summary>
        /// 获取检查表是否存在DbCommand
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="subTableArg">表名称,如果为空,则使用T对应表名称</param>
        /// <returns></returns>
        public override DbCommand GetCheckTableExistsDbCommand<T>(string subTableArg = null)
        {
            DbCommand dbCommand = CreateDbCommand();
            DtoMapping dtoDbMapping = DtoMappingHelper.GetDtoMapping<T>();
            string tableName = dtoDbMapping.GetTableName(subTableArg);            
            dbCommand.CommandText = $"select *  from sqlite_master where type='table' and name = '{tableName}';";
            return dbCommand;
        }
        
        /// <summary>
        /// 获取检查表是否存在DbCommand
        /// </summary>
        /// <param name="tableName">表名称</param>
        /// <returns></returns>
        public override DbCommand GetCheckTableExistsDbCommand(string tableName)
        {
            DbCommand dbCommand = CreateDbCommand();
            dbCommand.CommandText = $"select *  from sqlite_master where type='table' and name = '{tableName}';";
            return dbCommand;
        }

        /// <summary>
        /// 获取建表Sql
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="subTableArg">表名称,如果为空,则使用T对应表名称</param>
        /// <returns></returns>
        public override string GetCreateTableSql<T>(string subTableArg = null)
        {
            //表名 查询字段名 主键字段名
            DtoMapping dtoDbMapping = DtoMappingHelper.GetDtoMapping<T>();
            List<PiMap> piMapList = DtoMappingHelper.GetPiMapList<T>();
            string tableName = dtoDbMapping.GetTableName(subTableArg);            
            StringBuilder sqlBuilder = new StringBuilder();
            sqlBuilder.Append($"Create table {tableName}(\r\n");
            for (int i = 0; i < piMapList.Count; i++)
            {
                if (i < piMapList.Count - 1)
                {
                    sqlBuilder.Append(CreateField(piMapList[i]));
                }
                else
                {
                    sqlBuilder.Append(CreateField(piMapList[i], true));
                }
            }
            sqlBuilder.Append(")\r\n");
            return sqlBuilder.ToString();
        }

        /// <summary>
        /// 根据字段属性获取字段数据库类型
        /// </summary>
        /// <param name="piMap">字段属性Map</param>
        /// <param name="isLastField">是否为最后一个字段</param>
        /// <returns></returns>
        private string CreateField(PiMap piMap,bool isLastField = false)
        {
            string fieldStr = "";
            if (piMap.IsIgnore != true)
            {
                StringBuilder strBuilder = new StringBuilder();
                FieldAttribute attr = piMap.FieldAttr;
                if (string.IsNullOrEmpty(attr.FieldType))
                {
                    throw new Exception($"自动创建表,需指定字段[{piMap.FieldName}]的FieldType属性.");
                }
                strBuilder.Append($"{piMap.FieldName} ");

                string fieldType = GetFieldType(attr.FieldType.ToLower(), attr.Length);

                string[] charFieldTypeList = "char,nchar,varchar,nvarchar".Split(',');
                if (charFieldTypeList.Contains(fieldType))
                {
                    strBuilder.Append(" text ");
                }
                else
                {
                    strBuilder.Append($" {fieldType} ");
                }
                if (attr.IsPk == true)
                {
                    strBuilder.Append(" primary key ");

                    if (fieldType == "int" || fieldType == "integer" || fieldType== "long" || fieldType == "bigint")
                    {
                        strBuilder.Append(" autoincrement ");
                    }
                }
                strBuilder.Append(attr.Required == true ? " not null " : " null ");
                if (!isLastField)
                {
                    strBuilder.Append(",");
                }
                strBuilder.Append($"     --{attr.DisplayText}\r\n");
                fieldStr = strBuilder.ToString();
            }
            return fieldStr;
        }

        /// <summary>
        /// 获取MsSql字段类型
        /// </summary>
        /// <param name="sourceType">字段类型(来自其它数据库的字段类型)</param>
        /// <param name="length">字段长度</param>
        /// <returns></returns>
        public static string GetFieldType(string sourceType, int? length = null)
        {
            if (string.IsNullOrEmpty(sourceType))
            {
                throw new Exception("GetMsSqlFieldType失败.无效的字段类型.");
            }
            string fieldType = sourceType.ToLower();
            switch (fieldType)
            {   //MySql独有类型
                #region MySql独有类型转换
                case "blob":
                case "longblob":
                case "mediumblob":
                case "tinyblob":
                    fieldType = "blob";
                    break;
                case "double":
                    fieldType = "real";
                    break;
                case "tinyint":
                    fieldType = "integer";
                    break;
                case "mediumint":
                case "year":
                    fieldType = "integer";
                    break;
                case "char":
                    if (length == 36)
                    {
                        fieldType = "uniqueidentifier";
                    }
                    else if (!length.HasValue || length <= 0)
                    {
                        fieldType = $"{fieldType}(max)";
                    }
                    else
                    {
                        fieldType = $"{fieldType}({length})";
                    }
                    break;
                case "nchar":
                case "varchar":
                case "nvarchar":
                case "binary":
                case "varbinary":
                    if (!length.HasValue || length <= 0)
                    {
                        fieldType = $"{fieldType}(max)";
                    }
                    else
                    {
                        fieldType = $"{fieldType}({length})";
                    }
                    break;
                case "tinytext":
                case "longtext":
                case "mediumtext":
                case "geometry":
                case "geometrycollection":
                case "multilinestring":
                case "linestring":
                case "multipoint":
                case "multipolygon":
                case "point":
                case "polygon":
                    fieldType = "text";
                    break;
                #endregion

                #region Postgresql独有类型转换

                case "bool":
                    fieldType = "integer";
                    break;
                case "int2":
                    fieldType = "integer";
                    break;
                case "int4":
                    fieldType = "integer";
                    break;
                case "int8":
                    fieldType = "integer";
                    break;
                case "float4":
                    fieldType = "real";
                    break;
                case "float8":
                    fieldType = "real";
                    break;
                case "bpchar":
                    fieldType = "varchar";
                    if (!length.HasValue || length <= 0)
                    {
                        fieldType = $"{fieldType}(max)";
                    }
                    else
                    {
                        fieldType = $"{fieldType}({length})";
                    }
                    break;
                case "citext":
                case "text":
                case "json":
                    fieldType = "text";
                    break;
                case "date":
                    fieldType = "date";
                    break;
                case "timestamp":
                    fieldType = "datetime";
                    break;
                case "time":
                case "timetz":
                    fieldType = "varchar(30)";
                    break;
                case "uuid":
                    fieldType = "uniqueidentifier";
                    break;
                #endregion

                case "int":
                case "bigint":
                    fieldType = "integer";
                    break;
                case "uniqueidentifier":
                    fieldType = "uniqueidentifier";
                    break;
            }
            return fieldType;
        }

        /// <summary>
        /// Sqlite数据库中与c#中的数据类型对照
        /// </summary>
        /// <param name="fieldType"></param>
        /// <returns></returns>
        public static string GetSystemFieldType(string fieldType, int? fieldLength, bool isNullAble = true, bool isPk = false)
        {
            string sysFieldType = "object";
            if (fieldType != null)
            {
                switch (fieldType.ToLower())
                {
                    case "int":
                        sysFieldType = "int" + ((isNullAble || isPk) ? "?" : "");
                        break;
                    case "bigint":
                        sysFieldType = "long" + ((isNullAble || isPk) ? "?" : "");
                        break;
                    case "binary":
                        sysFieldType = "byte[]";
                        break;
                    case "datetime":
                        sysFieldType = "DateTime" + (isNullAble ? "?" : "");
                        break;
                    case "decimal":
                        sysFieldType = "decimal" + (isNullAble ? "?" : "");
                        break;
                    case "float":
                        sysFieldType = "double" + (isNullAble ? "?" : "");
                        break;
                    case "char":
                        sysFieldType = "string";
                        break;
                    case "text":
                        sysFieldType = "string";
                        break;
                    case "nchar":
                        sysFieldType = "string";
                        break;
                    case "nvarchar":
                        sysFieldType = "string";
                        break;
                    case "real":
                        sysFieldType = "Single";
                        break;
                    case "smalldatetime":
                        sysFieldType = "DateTime" + (isNullAble ? "?" : "");
                        break;
                    case "smallint":
                        sysFieldType = "Int16" + (isNullAble ? "?" : "");
                        break;
                    case "smallmoney":
                        sysFieldType = "Decimal" + (isNullAble ? "?" : "");
                        break;
                    case "timestamp":
                        sysFieldType = "byte[]";   //不建议使用的类型
                        break;
                    case "tinyint":
                        sysFieldType = "byte" + (isNullAble ? "?" : "");
                        break;
                    case "varbinary":
                        sysFieldType = "byte[]";
                        break;
                    case "varchar":
                        sysFieldType = "string";
                        break;
                    case "variant":
                        sysFieldType = "object";
                        break;
                    case "uniqueidentifier":
                        sysFieldType = "Guid" + ((isNullAble || isPk) ? "?" : "");
                        break;
                    case "bit":
                        sysFieldType = "bool" + (isNullAble ? "?" : "");
                        break;
                    case "image":
                        sysFieldType = "byte[]";
                        break;
                    case "money":
                        sysFieldType = "decimal" + (isNullAble ? "?" : "");
                        break;
                    case "numeric":
                        sysFieldType = "decimal" + (isNullAble ? "?" : "");
                        break;
                    case "ntext":
                        sysFieldType = "string";
                        break;
                    default:
                        sysFieldType = "string";
                        break;
                }
            }
            return sysFieldType;
        }

        public override void BulkCopy(string tableName, DataTable dt, int batchSize, int timeout = 0, bool useTransaction = true, IProgress<float> progress = null)
        {
            throw new NotImplementedException();
        }
    }    
}
