﻿using System;
using System.Collections.Generic;
using System.Text;
using System.Data.Common;
using System.Configuration;
using System.Reflection;
using System.Data;
using System.Linq;
using Jc.Database.Query;
using Newtonsoft.Json.Linq;

namespace Jc.Database.Provider
{
    /// <summary>
    /// PostgreSql Provider
    /// </summary>
    public class PostgreSqlDbProvider : DbProvider
    {
        readonly DatabaseType dbType = DatabaseType.PostgreSql;

        /// <summary>
        /// Ctor
        /// </summary>
        /// <param name="connectString"></param>
        public PostgreSqlDbProvider(string connectString, DatabaseType dbType = DatabaseType.PostgreSql) : base(connectString, dbType)
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
            int dbInfoStartIndex = connectString.ToLower().IndexOf("-d");
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
            dbCommand.CommandText = " returning id;";   // 需要保留空格
            return dbCommand;
        }

        /// <summary>
        /// override GetParameterValue Method
        /// </summary>
        /// <param name="piMap"></param>
        /// <param name="dto"></param>
        /// <returns></returns>
        protected override object GetParameterValue(FieldMapping piMap, object dto)
        {
            object dbValue = base.GetParameterValue(piMap, dto);
            if(dbValue != DBNull.Value)
            {
                // PostgreSql 日期处理为UTC格式
                if (piMap.DbType == System.Data.DbType.DateTime)
                {
                    DateTime? dateTime = dbValue as DateTime?;
                    dbValue = DateHelper.ToUniversalTime(dateTime);
                }
            }
            return dbValue;
        }

        /// <summary>
        /// 获取查询参数
        /// </summary>
        /// <param name="dbCommand"></param>
        /// <param name="queryParameter"></param>
        /// <returns></returns>
        protected override DbParameter GetQueryParameter(DbCommand dbCommand, QueryParameter queryParameter)
        {
            DbParameter dbParameter = base.GetQueryParameter(dbCommand, queryParameter);

            if (queryParameter.ParameterDbType == System.Data.DbType.DateTime)
            {
                DateTime? dateTime = dbParameter.Value as DateTime?;
                dbParameter.Value = DateHelper.ToUniversalTime(dateTime);
            }
            return dbParameter;
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
            string sqlStr = "Select {1} From {0} @QueryStr @OrderStr limit @PageSize offset @LowRecNum;";
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
                DbParameter dbParameter = GetQueryParameter(dbCommand, filter.FilterParameters[i]);
                dbCommand.Parameters.Add(dbParameter);
            }
            dbCommand.CommandText = string.Format(sqlStr, dtoDbMapping.GetTableName(subTableArg), selectParams, dtoDbMapping.GetPkField().FieldName);
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
            string sqlStr =
                @"select tableName,tableType from
                (select tableName,'table' as tableType from pg_tables where schemaname='public'
                union select viewName as tableName,'view' as tableType from pg_views where schemaname ='public'
                )a order by tableType,tableName";
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
            string sqlStr = $@"select fieldname,fieldchname,note,fieldtype,FieldLengthStr,
                                case(t.fieldname=pkinfo.colname) when true then '1' else '0' end as IsPkStr,
                                IsNullAbleStr
                                from
                                (
                                select a.attname as FieldName,col_description(a.attrelid,a.attnum) as FieldChName,
                                col_description(a.attrelid,a.attnum) as Note,
                                pg_type.typname as FieldType,
                                cast(cols.character_maximum_length as varchar(50)) as FieldLengthStr,
                                case(a.attnotnull) when false then '1' else '0' end as IsNullAbleStr
                                from pg_class c,pg_attribute a
                                left join pg_type on pg_type.oid = a.atttypid
                                left join information_schema.columns cols
                                on cols.table_schema='public' and cols.table_name='{tableName}' and cols.column_name=a.attname
                                where c.relname = '{tableName}' and a.attrelid = c.oid and a.attnum>0
                                ) t
                                left join
                                (
                                select pg_constraint.conname AS pk_name, pg_attribute.attname AS colname,pg_type.typname AS typename
                                from pg_constraint
                                inner join pg_class ON pg_constraint.conrelid = pg_class.oid
                                inner join pg_attribute ON pg_attribute.attrelid = pg_class.oid
                                and pg_attribute.attnum = pg_constraint.conkey[1]
                                inner join pg_type ON pg_type.oid = pg_attribute.atttypid
                                where pg_class.relname = '{tableName}' and pg_constraint.contype = 'p'
                                ) pkInfo
                                on t.fieldname=pkinfo.colname";
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
            dbCommand.CommandText = $"Select * from information_schema.tables where table_schema='public' and table_type='BASE TABLE' and table_name='{tableName}';";
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
            dbCommand.CommandText = $"Select * from information_schema.tables where table_schema='public' and table_type='BASE TABLE' and table_name='{tableName}';";
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
                sqlBuilder.Append($"Create table public.{tableName}(\r\n");
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
                for (int i = 0; i < piMapList.Count; i++)
                {
                    if (piMapList[i].IsIgnore != true)
                    {
                        FieldAttribute attr = piMapList[i].FieldAttribute;
                        if (!string.IsNullOrEmpty(attr.DisplayText))
                        {
                            sqlBuilder.Append($" comment on column public.{tableName}.{piMapList[i].FieldName} is '{attr.DisplayText}';\r\n");
                        }
                    }
                }
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
                string[] charFieldTypeList = "char,nchar,varchar,nvarchar".Split(',');

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
                    if (attr.FieldType.ToLower() == "int" || attr.FieldType.ToLower() == "integer"
                        || attr.FieldType.ToLower() == "bigint")
                    {
                        strBuilder.Append(" serial ");
                    }
                    strBuilder.Append(" primary key ");
                }
                strBuilder.Append(attr.Required == true ? " not null " : " null ");
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