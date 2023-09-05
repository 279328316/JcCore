using System;
using System.Collections.Generic;
using System.Text;
using System.Data.Common;
using System.Configuration;
using System.Reflection;
using System.Data;
using System.Linq;
using Jc.Database.Query;
using System.Data.SqlClient;


namespace Jc.Database.Provider
{
    /// <summary>
    /// MsSql Provider
    /// </summary>
    public partial class MsSqlDbProvider : DbProvider
    {
        /// <summary>
        /// Ctor
        /// </summary>
        /// <param name="connectString"></param>
        public MsSqlDbProvider(string connectString, DatabaseType dbType = DatabaseType.MsSql) : base(connectString, dbType)
        {
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
            dbCommand.CommandText = "Select SCOPE_IDENTITY()";
            return dbCommand ; 
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
            string sqlStr = "Select {1} From {0} t1" +
                ",(Select Top (@UpRecNum) row_number() OVER (@OrderStr) Num,{2} From {0} @QueryStr) t2" +
                " Where t1.{2}=t2.{2} And t2.Num>@LowRecNum Order By t2.Num Asc";
            string selectParams = null;

            string queryStr = null;
            string orderStr = null;

            EntityMapping dtoDbMapping = EntityMappingHelper.GetMapping<T>();
            List<FieldMapping> piMapList = EntityMappingHelper.GetPiMapList<T>(filter);
            foreach (FieldMapping piMap in piMapList)
            {
                selectParams += string.IsNullOrEmpty(selectParams) ? "t2.Num,t1." + piMap.FieldName : ",t1." + piMap.FieldName;
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
            sqlStr = sqlStr.Replace("@UpRecNum", filter.FilterEndIndex.ToString());
            sqlStr = sqlStr.Replace("@OrderStr", orderStr);
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
            DbParameter outDbParameter = dbCommand.CreateParameter();
            outDbParameter.Direction = ParameterDirection.Output;
            outDbParameter.ParameterName = "@RecCount";
            outDbParameter.Size = 8;
            dbCommand.Parameters.Add(outDbParameter); //RecCount 总记录数
            dbCommand.CommandText = string.Format(sqlStr, dtoDbMapping.GetTableName(subTableArg), selectParams, dtoDbMapping.GetPkField().FieldName);
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
            dbCommand.CommandText = $"Select id from dbo.sysobjects where id = object_id(N'[dbo].[{tableName}]')";
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
            dbCommand.CommandText = $"Select id from dbo.sysobjects where id = object_id(N'[dbo].[{tableName}]')";
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
            string sqlStr = $@"Select t.name as TableName,
                               case(t.xtype) when 'U' then 'Table' when 'V' then 'View' else 'Table' end as TableTypeStr,
                               ep.value as TableDes
                               from sysobjects as t
                               left join sys.extended_properties as ep on ep.major_id = t.id and ep.minor_id = 0 and t.xtype = 'u'
                               where t.xtype = 'u' or t.xtype = 'v' order by t.xtype,t.name";
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
            string sqlStr = $@"Select c.[name] AS FieldName,cast(ep.[value] as varchar(100)) AS FieldChName,
                               cast(ep.[value] as varchar(100)) AS 'Note',st.name as FieldType,
                               cast(c.max_length as varchar(20)) as FieldLengthStr,cast(c.is_nullable as varchar(20)) AS IsNullAbleStr,
                               cast(case when k.column_name is not null then 1 else 0 end as varchar(20)) as IsPkStr
                               from sys.objects as t
                               inner join sys.columns as c on t.object_id = c.object_id and t.[name]='{tableName}'
                               left join sys.types st on c.user_type_id = st.user_type_id
                               left join sys.extended_properties as ep on ep.major_id = c.object_id and ep.minor_id = c.column_id
                               left join information_schema.key_column_usage k on t.name = k.table_name and c.name = k.column_name";
            dbCommand.CommandText = sqlStr;
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
            List<FieldMapping> piMapList = EntityMappingHelper.GetPiMapList<T>();

            string tableName = dtoDbMapping.GetTableName(subTableArg);

            StringBuilder sqlBuilder = new StringBuilder();
            sqlBuilder.Append($"Create table [{tableName}](\r\n");
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
                fieldStr = $"[{piMap.FieldName}] {attr.FieldType}";
                strBuilder.Append($"[{piMap.FieldName}] ");
                strBuilder.Append(attr.FieldType);

                string fieldType = attr.FieldType.ToLower();
                string[] charFieldTypeList = "char,nchar,varchar,nvarchar".Split(',');
                if (charFieldTypeList.Contains(fieldType))
                {
                    if (attr.Length > 0)
                    {
                        strBuilder.Append($"({attr.Length})");
                    }
                    else
                    {
                        strBuilder.Append($"(max)");
                    }
                }
                if (attr.IsPk == true)
                {
                    if (attr.FieldType.ToLower() == "int" || attr.FieldType.ToLower() == "bigint")
                    {
                        strBuilder.Append("  identity(1,1) ");
                    }
                    strBuilder.Append(" primary key ");
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
        /// 使用BulkCopy插入数据
        /// </summary>
        /// <param name="tableName"></param>
        /// <param name="dt"></param>
        /// <param name="batchSize"></param>
        /// <param name="timeout"></param>
        /// <param name="useTransaction"></param>
        /// <param name="progress">0,1 进度</param>
        /// <returns></returns>
        public override void BulkCopy(string tableName, DataTable dt,int batchSize, int timeout = 0, bool useTransaction = true, IProgress<float> progress = null)
        {
            dbCreator.BulkCopy(this.ConnectString, tableName, dt, batchSize, timeout, useTransaction, progress);
        }
    }
}
