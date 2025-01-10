using System;
using System.Collections.Generic;
using System.Text;
using System.Data.Common;
using System.Configuration;
using System.Reflection;
using System.Data;
using System.Linq;
using Jc.Database.Query;

namespace Jc.Database.Provider
{
    /// <summary>
    /// MySql Provider
    /// </summary>
    public class MySqlDbProvider : DbProvider
    {
        readonly DatabaseType dbType = DatabaseType.MySql;

        /// <summary>
        /// Ctor
        /// </summary>
        /// <param name="connectString"></param>
        public MySqlDbProvider(string connectString, DatabaseType dbType = DatabaseType.MySql) : base(connectString, dbType)
        {
            this.dbCreator = DbCreatorFactory.GetDbCreator(dbType);
        }

        /// <summary>
        /// 自连接串中获取DbName
        /// </summary>
        /// <returns></returns>
        public override string GetDbNameFromConnectString(string connectString)
        {
            string dbName = "";
            int dbInfoStartIndex = connectString.ToLower().IndexOf("initial catalog");
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
            dbCommand.CommandText = "select last_insert_id()";
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
            if (filter == null)
            {
                throw new Exception("Paging query must specify pagination information");
            }
            DbCommand dbCommand = CreateDbCommand();

            //表名 查询字段名 主键字段名
            string sqlStr = "Select {1} From {0} @QueryStr @OrderStr limit @LowRecNum,@PageSize;";
            string selectParams = null;

            string queryStr = null;
            string orderStr = "";
            EntityMapping dtoDbMapping = EntityMappingHelper.GetMapping<T>();
            List<FieldMapping> piMapList = EntityMappingHelper.GetPiMapList<T>(filter);
            foreach (FieldMapping piMap in piMapList)
            {
                selectParams += string.IsNullOrEmpty(selectParams) ? piMap.FieldName : "," + piMap.FieldName;
            }
            if (filter.ItemList.Count > 0)
            {
                queryStr = filter.FilterSQLString;
            }
            if (!string.IsNullOrEmpty(filter.OrderSQLString))
            {
                orderStr = filter.OrderSQLString;
            }
            sqlStr = sqlStr.Replace("@QueryStr", queryStr);
            sqlStr = sqlStr.Replace("@LowRecNum", filter.FilterStartIndex.ToString());
            sqlStr = sqlStr.Replace("@PageSize", filter.PageSize.ToString());
            sqlStr = sqlStr.Replace("@OrderStr", orderStr);

            for (int i = 0; i < filter.FilterParameters.Count; i++)
            {
                DbParameter dbParameter = dbCommand.CreateParameter();
                dbParameter.Direction = ParameterDirection.Input;
                dbParameter.ParameterName = filter.FilterParameters[i].ParameterName;
                dbParameter.Value = filter.FilterParameters[i].ParameterValue;
                dbParameter.DbType = filter.FilterParameters[i].ParameterDbType;
                dbCommand.Parameters.Add(dbParameter);
            }
            dbCommand.CommandText = string.Format(sqlStr, dtoDbMapping.GetTableName(subTableArg), selectParams, dtoDbMapping.GetPkField().FieldName);
            return dbCommand;
        }

        /// <summary>
        /// 获取建表DbCommand
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="subTableArg">新建表名称,如果为空,则使用T对应表名称</param>
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
            string sqlStr = $@"select table_name as TableName,
                                case(table_type) when 'base table' then 'Table' when 'View' then 'View' else 'Table' end as TableTypeStr,
                                table_comment as TableDes
                                from information_schema.tables
                                where table_schema='{DbName}' order by table_type,table_name";
            dbCommand.CommandText = sqlStr;
            return dbCommand;
        }

        /// <summary>
        /// 获取所有表字段DbCommand
        /// </summary>
        /// <returns></returns>
        public override DbCommand GetFieldListDbCommand(string tableName)
        {
            DbCommand dbCommand = CreateDbCommand();
            string sqlStr = $@"Select Column_name as FieldName,Column_Comment as FieldChName,Column_Comment as Note,
                                Data_type as FieldType,
                                case (lower(Data_type))
                                when 'char' then replace(replace(Column_Type,'char(',''),')','')
                                when 'nchar' then replace(replace(Column_Type,'nchar(',''),')','')
                                when 'varchar' then replace(replace(Column_Type,'varchar(',''),')','')
                                when 'nvarchar' then replace(replace(Column_Type,'nvarchar(',''),')','')
                                when 'tinyint' then replace(replace(Column_Type,'tinyint(',''),')','')
                                else null end as FieldLengthStr,
                                case(Is_NullAble) when 'YES' then '1' else '0' end as IsNullAbleStr,
                                case(Column_Key) when 'PRI' then '1' else '0' end as IsPkStr
                                from information_schema.columns
                                where table_schema='{DbName}' and table_name='{tableName}'";
            dbCommand.CommandText = sqlStr;
            return dbCommand;
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
            EntityMapping dtoDbMapping = EntityMappingHelper.GetMapping<T>();
            string tableName = dtoDbMapping.GetTableName(subTableArg);
            dbCommand.CommandText = $"Select * from information_schema.tables where table_schema = '{DbName}' and table_name ='{tableName}';";
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
            dbCommand.CommandText = $"Select * from information_schema.tables where table_schema = '{DbName}' and table_name ='{tableName}';";
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
            EntityMapping dtoDbMapping = EntityMappingHelper.GetMapping<T>();
            string createTableSql = "";
            if (!string.IsNullOrEmpty(dtoDbMapping.TableAttr?.CreateTableSql))
            {
                createTableSql = dtoDbMapping.TableAttr.CreateTableSql;

                if (createTableSql.Contains("{0}"))
                {   // 处理分表情况
                    if (subTableArg == null)
                    {
                        throw new Exception($"对象{dtoDbMapping.EntityType.Name},分表参数不能为空,请使用分表DbContext.调用GetSubTableDbContext获取分表DbContext");
                    }
                    createTableSql = string.Format(createTableSql, subTableArg.ToString().ToLower());
                }
            }
            else
            {
                string tableName = dtoDbMapping.GetTableName(subTableArg);
                List<FieldMapping> piMapList = EntityMappingHelper.GetPiMapList<T>();
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
                createTableSql = sqlBuilder.ToString();
            }
            return createTableSql;
        }

        /// <summary>
        /// 根据字段属性获取字段数据库类型
        /// </summary>
        /// <param name="piMap">字段属性Map</param>
        /// <param name="isLastField">是否为最后一个字段</param>
        /// <returns></returns>
        private string CreateField(FieldMapping piMap, bool isLastField = false)
        {
            string fieldStr = "";
            if (piMap.IsIgnore != true)
            {
                StringBuilder strBuilder = new StringBuilder();
                FieldAttribute attr = piMap.FieldAttribute;
                if (string.IsNullOrEmpty(attr.FieldType))
                {
                    throw new Exception($"自动创建表,需指定字段[{piMap.FieldName}]的FieldDbType属性.");
                }
                strBuilder.Append($"{piMap.FieldName} ");

                string fieldType = attr.FieldType.ToLower();
                string[] charFieldTypeList = "char,nchar,varchar,nvarchar,tinyint".Split(',');

                strBuilder.Append($" {attr.FieldType} ");
                if (charFieldTypeList.Contains(fieldType))
                {
                    if (attr.Length > 0)
                    {
                        strBuilder.Append($"({attr.Length.ToString()})");
                    }
                }
                if (attr.IsPk == true)
                {
                    if (attr.FieldType.ToLower() == "int" || attr.FieldType.ToLower() == "integer" || attr.FieldType.ToLower() == "bigint")
                    {
                        strBuilder.Append(" auto_increment ");
                    }
                    strBuilder.Append(" primary key ");
                }
                strBuilder.Append(attr.Required == true ? " not null " : " null ");
                if (!string.IsNullOrEmpty(attr.DisplayText))
                {
                    strBuilder.Append($" comment '{attr.DisplayText}'\r\n");
                }
                if (!isLastField)
                {
                    strBuilder.Append(",");
                }
                fieldStr = strBuilder.ToString();
            }
            return fieldStr;
        }

        public override void BulkCopy(string tableName, DataTable dt, int batchSize, int timeout = 0, bool useTransaction = true, IProgress<float> progress = null)
        {
            throw new NotImplementedException();
        }
    }
}