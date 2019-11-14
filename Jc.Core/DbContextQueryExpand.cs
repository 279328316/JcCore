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
using Microsoft.Extensions.Primitives;

namespace Jc.Core
{
    /// <summary>
    /// DbContext
    /// DbContext数据操作访问
    /// </summary>
    public partial class DbContext
    {
        #region QueryMethods        
        
        /// <summary>
        /// 返回查询Query对象
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <returns></returns>
        public virtual IQuery<T> IQuery<T>() where T : class, new()
        {            
            IQuery<T> iquery = new IQuery<T>(this);
            return iquery;
        }


        /// <summary>
        /// 添加查询条件
        /// 参数类型只支持int guid string double float bool datetime
        /// </summary>
        /// <param name="collection"></param>
        /// <param name="operandSettings"></param>
        public IQuery<T> IQuery<T>(NameValueCollection collection, Dictionary<string, Operand> operandSettings = null) where T : class, new()
        {
            IQuery<T> query = IQuery<T>();
            List<PiMap> piMapList = DtoMappingHelper.GetPiMapList<T>();
            for (int i = 0; i < collection.AllKeys.Length; i++)
            {
                string queryItemKey = collection.AllKeys[i];
                if (!string.IsNullOrEmpty(queryItemKey))
                {
                    string queryItemVal = collection[queryItemKey];
                    Object itemValue = null;
                    if (!string.IsNullOrEmpty(queryItemVal))
                    {
                        PiMap piMap = piMapList.Where(p => queryItemKey.ToLower() == p.PiName.ToLower()
                                || queryItemKey.ToLower() == ($"min{p.PiName}").ToLower()
                                || queryItemKey.ToLower() == ($"max{p.PiName}").ToLower()
                                || queryItemKey.ToLower() == ($"{p.PiName}s").ToLower()).FirstOrDefault();
                        if(piMap==null)
                        {
                            continue;
                        }
                        #region 处理匹配到的属性
                        bool rangeMin = queryItemKey.ToLower() == ($"min{piMap.PiName}").ToLower();
                        bool rangeMax = queryItemKey.ToLower() == ($"max{piMap.PiName}").ToLower();
                        bool isList = queryItemKey.ToLower() == ($"{piMap.PiName}s").ToLower();

                        Operand operand = Operand.Equal;

                        if ((operandSettings != null) && operandSettings.ContainsKey(queryItemKey))
                        {
                            operand = operandSettings[queryItemKey];
                            itemValue = queryItemVal;
                        }
                        else if (isList)
                        {
                            operand = Operand.Contains;
                            List<string> valueList;
                            if (queryItemVal.StartsWith("[") && queryItemVal.EndsWith("]"))
                            {
                                valueList = JsonHelper.DeserializeObject<List<string>>(queryItemVal);
                            }
                            else
                            {
                                valueList = queryItemVal.Split(',')
                                    .Where(a => !string.IsNullOrEmpty(a)).Select(a => a.Trim()).ToList();
                            }
                            if (valueList?.Count > 0)
                            {
                                itemValue = valueList;
                            }
                            else
                            {
                                continue;
                            }
                        }
                        else if (rangeMin)
                        {
                            operand = Operand.GreaterThanOrEqual;
                            if (piMap.PropertyType == typeof(DateTime) || piMap.PropertyType == typeof(DateTime?))
                            {
                                DateTime dt;
                                if (DateTime.TryParse(queryItemVal, out dt))
                                {
                                    itemValue = dt.Date;
                                }
                                else
                                {
                                    throw new Exception("日期格式错误.");
                                }
                            }
                            else
                            {
                                itemValue = queryItemVal;
                            }
                        }
                        else if (rangeMax)
                        {
                            operand = Operand.LessThanOrEqual;
                            if (piMap.PropertyType == typeof(DateTime) || piMap.PropertyType == typeof(DateTime?))
                            {
                                DateTime dt;
                                if (DateTime.TryParse(queryItemVal, out dt))
                                {
                                    itemValue = dt.Date.AddDays(1);
                                }
                                else
                                {
                                    throw new Exception("日期格式错误.");
                                }
                            }
                            else
                            {
                                itemValue = queryItemVal;
                            }
                        }
                        else if (piMap.PropertyType == typeof(string))
                        {
                            operand = Operand.Like;
                            itemValue = queryItemVal;
                        }
                        else
                        {
                            itemValue = queryItemVal;
                        }
                        var queryExp = ExpressionHelper.CreateLambdaExpression<T>(operand, piMap.PiName, itemValue);
                        query.And(queryExp);
                        #endregion
                    }
                }
            }
            return query;
        }


        /// <summary>
        /// 添加查询条件
        /// 参数类型只支持int guid string double float bool datetime
        /// </summary>
        /// <param name="collection"></param>
        /// <param name="operandSettings"></param>
        public IQuery<T> IQuery<T>(IEnumerable<KeyValuePair<string, StringValues>> collection, Dictionary<string, Operand> operandSettings = null) where T : class, new()
        {
            NameValueCollection nvCollection = new NameValueCollection();
            if (collection != null)
            {
                foreach(KeyValuePair<string, StringValues> kv in collection)
                {
                    nvCollection.Add(kv.Key,kv.Value);
                }
            }
            return IQuery<T>(nvCollection,operandSettings);            
        }


        /// <summary>
        /// 根据Id获取数据
        /// </summary>
        /// <param name="id">主键值</param>
        /// <param name="select">查询属性</param>
        /// <returns></returns>
        public T GetById<T>(object id,Expression<Func<T, object>> select = null) where T : class, new()
        {
            T dto = null;
            List<PiMap> piMapList = DtoMappingHelper.GetPiMapList<T>(select);
            using (DbCommand dbCommand = dbProvider.GetQueryByIdDbCommand<T>(id, piMapList, this.GetSubTableArg<T>()))
            {
                try
                {
                    dbCommand.Connection = GetDbConnection();
                    DbDataReader dr = dbCommand.ExecuteReader();
                    DataTable dt = ConvertDataReaderToDataTable(dr,1);
                    if (dt != null && dt.Rows.Count > 0)
                    {
                        dto = dt.Rows[0].ToEntity<T>();
                    }
                    CloseDbConnection(dbCommand);
                }
                catch (Exception ex)
                {
                    CloseDbConnection(dbCommand);
                    throw ex;
                }
            }
            return dto;
        }

        /// <summary>
        /// 获取单个对象
        /// </summary>
        /// <param name="where">查询条件</param>
        /// <param name="select">查询属性</param>
        /// <returns></returns>
        public T Get<T>(Expression<Func<T, bool>> where = null, Expression<Func<T, object>> select = null) where T : class, new()
        {
            IQuery<T> query = IQuery<T>().FromTable(this.GetSubTableArg<T>());
            return query.Select(select).Where(where).FirstOrDefault();
        }

        /// <summary>
        /// 获取单个对象
        /// </summary>
        /// <param name="sql">用户自定义查询</param>
        public T Get<T>(string sql) where T : class, new()
        {
            T dto = null;
            DbCommand dbCommand = dbProvider.CreateDbCommand(sql);
            using (dbCommand)
            {
                try
                {
                    dbCommand.Connection = GetDbConnection();

                    DbDataReader dr = dbCommand.ExecuteReader();
                    DataTable dt = ConvertDataReaderToDataTable(dr,1);
                    if (dt != null && dt.Rows.Count > 0)
                    {
                        dto = dt.Rows[0].ToEntity<T>();
                    }
                    CloseDbConnection(dbCommand);
                }
                catch (Exception ex)
                {
                    CloseDbConnection(dbCommand);
                    throw ex;
                }
            }
            return dto;
        }
        
        /// <summary>
        /// 获取对象列表
        /// </summary>
        /// <param name="where">查询条件</param>
        /// <param name="select">查询属性</param>
        /// <returns></returns>
        public List<T> GetList<T>(Expression<Func<T, bool>> where = null, Expression<Func<T, object>> select = null) where T : class, new()
        {
            IQuery<T> query = IQuery<T>().FromTable(this.GetSubTableArg<T>());
            return query.Where(where).Select(select).ToList();
        }

        /// <summary>
        /// 条件排序查询
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="where">查询条件</param>
        /// <param name="sortExpr">排序属性</param>
        /// <param name="order">排序方向</param>
        /// <returns></returns>
        public List<T> GetSortList<T>(Expression<Func<T, bool>> where,Expression<Func<T, object>> sortExpr, Sorting order = Sorting.Asc) where T : class, new()
        {
            IQuery<T> query = IQuery<T>().Where(where).FromTable(this.GetSubTableArg<T>());
            if (order == Sorting.Asc)
            {
                query = query.OrderBy(sortExpr);
            }
            else
            {
                query = query.OrderByDesc(sortExpr);
            }
            return query.ToList();
        }

        /// <summary>
        /// 分页查询,仅多字段单方向排序查询.
        /// 如多方向排序查询,请使用IQuery
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="where">查询条件</param>
        /// <param name="pageIndex">页序号</param>
        /// <param name="pageSize">页大小</param>
        /// <param name="sortExpr">排序属性</param>
        /// <param name="order">排序方向</param>
        /// <returns></returns>
        public PageResult<T> GetPageList<T>(Expression<Func<T, bool>> where,
            int pageIndex,int pageSize, Expression<Func<T, object>> sortExpr, Sorting order) where T : class, new()
        {
            IQuery<T> query = IQuery<T>().Where(where).FromTable(this.GetSubTableArg<T>());
            if(order== Sorting.Asc)
            {
                query = query.OrderBy(sortExpr);
            }
            else
            {
                query = query.OrderByDesc(sortExpr);
            }
            return query.ToPageList(pageIndex, pageSize);
        }
        
        /// <summary>
        /// 获取对象列表
        /// </summary>
        /// <param name="sql">用户自定义查询</param>
        public List<T> GetList<T>(string sql) where T : class, new()
        {
            List<T> list = new List<T>();
            DbCommand dbCommand = dbProvider.CreateDbCommand(sql);
            list = GetList<T>(dbCommand);
            return list;
        }

        /// <summary>
        /// 查询对象List
        /// </summary>
        /// <returns>List T</returns>
        private List<T> GetList<T>(DbCommand dbCommand) where T : class, new()
        {
            List<T> list = new List<T>();
            using (dbCommand)
            {
                try
                {
                    dbCommand.Connection = GetDbConnection();
                    DbDataReader dr = dbCommand.ExecuteReader();
                    DataTable dt = ConvertDataReaderToDataTable(dr);
                    list = dt.ToList<T>();
                    CloseDbConnection(dbCommand);
                }
                catch (Exception ex)
                {
                    CloseDbConnection(dbCommand);
                    throw ex;
                }
            }
            return list;
        }


        /// <summary>
        /// 求和
        /// </summary>
        /// <param name="where">查询条件</param>
        /// <param name="select">查询属性(必须为可求和字段)</param>
        /// <returns></returns>
        public decimal Sum<T>(Expression<Func<T, object>> select = null,
            Expression<Func<T, bool>> where = null) where T : class, new()
        {
            IQuery<T> query = IQuery<T>().FromTable(this.GetSubTableArg<T>());
            return query.Where(where).Sum(select);
        }

        /// <summary>
        /// 查询符合条件的记录数
        /// </summary>
        /// <param name="where">查询条件</param>
        /// <returns></returns>
        public int Count<T>(Expression<Func<T, bool>> where = null) where T : class, new()
        {
            IQuery<T> query = IQuery<T>().FromTable(this.GetSubTableArg<T>());
            return query.Count(where);
        }

        /// <summary>
        /// 查询符合条件的记录数
        /// </summary>
        /// <param name="field">计算属性</param>
        /// <param name="where">查询条件</param>
        /// <returns></returns>
        public string Min<T>(Expression<Func<T, object>> field, Expression<Func<T, bool>> where = null) where T : class, new()
        {
            IQuery<T> query = IQuery<T>().FromTable(this.GetSubTableArg<T>())
                                .Where(where);
            return query.Min(field);
        }

        /// <summary>
        /// 查询符合条件的记录数
        /// </summary>
        /// <param name="field">计算属性</param>
        /// <param name="where">查询条件</param>
        /// <returns></returns>
        public string Max<T>(Expression<Func<T, object>> field,Expression<Func<T, bool>> where = null) where T : class, new()
        {
            IQuery<T> query = IQuery<T>().FromTable(this.GetSubTableArg<T>())
                                .Where(where);
            return query.Max(field);
        }

        /// <summary>
        /// 查询是否存在符合条件的记录
        /// </summary>
        /// <param name="where">查询条件</param>
        /// <returns></returns>
        public bool Exists<T>(Expression<Func<T, bool>> where = null) where T : class, new()
        {
            return Count(where) >0;
        }

        /// <summary>
        /// 获取对象列表
        /// </summary>
        /// <param name="sql">查询Sql</param>
        /// <returns></returns>
        public DataTable GetDataTable(string sql)
        {
            DataTable dt = null;
            DbCommand dbCommand = dbProvider.CreateDbCommand(sql);
            using (dbCommand)
            {
                try
                {
                    dbCommand.Connection = GetDbConnection();
                    DbDataReader dr = dbCommand.ExecuteReader();
                    dt = ConvertDataReaderToDataTable(dr);
                    CloseDbConnection(dbCommand);
                }
                catch (Exception ex)
                {
                    CloseDbConnection(dbCommand);
                    throw ex;
                }
            }
            return dt;
        }

        /// <summary>
        /// 查询数据库所有表信息
        /// </summary>
        /// <returns>受影响记录数</returns>
        public List<TableModel> GetTableList()
        {
            DbCommand dbCommand = dbProvider.GetTableListDbCommand();
            return GetList<TableModel>(dbCommand);
        }

        /// <summary>
        /// 查询数据库表字段信息
        /// </summary>
        /// <param name="tableName">表名称</param>
        /// <returns>受影响记录数</returns>
        public List<FieldModel> GetTableFieldList(string tableName = null)
        {
            DbCommand dbCommand = dbProvider.GetFieldListDbCommand(tableName);
            return GetList<FieldModel>(dbCommand);
        }
        
        #endregion
    }
}