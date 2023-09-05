
using Jc.Database;
using Jc.Database.Query;

using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.Data.Common;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace Jc.Database
{
    /// <summary>
    /// 查询
    /// </summary>
    public class IQuery<T> where T : class, new()
    {
        #region Fields
        private DbContext dbContext = null;
        string subTableArg;    //表名称参数
        Pager pager;
        List<OrderByClause> orderByClauseList = new List<OrderByClause>();
        Expression<Func<T, bool>> query = null;
        Expression<Func<T, object>> select = null;
        Expression<Func<T, object>> unSelect = null;
        #endregion

        #region Properties
        /// <summary>
        /// QueryExpression
        /// </summary>
        internal Expression<Func<T, bool>> QueryExpression
        {
            get { return this.query; }
        }

        /// <summary>
        /// OrderBy
        /// </summary>
        internal List<OrderByClause> OrderByClauseList { get { return orderByClauseList; } set { orderByClauseList = value; } }
        #endregion

        #region Ctor
        /// <summary>
        /// Ctor
        /// </summary>
        /// <param name="dbContext">dbContext</param>
        /// <param name="subTableArg">分表参数</param>
        public IQuery(DbContext dbContext, string subTableArg = null)
        {
            if (dbContext == null)
            {
                throw new Exception("IQuery初始化失败,dbContext对象不能为空.");
            }
            this.dbContext = dbContext;
            this.subTableArg = subTableArg;
        }
        #endregion

        #region Methods

        /// <summary>
        /// 分表参数
        /// </summary>
        /// <param name="subTableArg">分表参数</param>
        /// <returns></returns>
        public IQuery<T> FromTable(string subTableArg)
        {
            this.subTableArg = subTableArg;
            return this;
        }

        /// <summary>
        /// 选择的字段
        /// </summary>
        /// <param name="select">查询属性</param>
        /// <param name="unSelect">排除查询属性</param>
        /// <returns></returns>
        public IQuery<T> Select(Expression<Func<T, object>> select = null,
            Expression<Func<T, object>> unSelect = null)
        {
            this.select = select;
            this.unSelect = unSelect;
            return this;
        }

        /// <summary>
        /// 排除查询的属性
        /// </summary>
        /// <param name="unSelect">排除查询属性</param>
        /// <returns></returns>
        public IQuery<T> UnSelect(Expression<Func<T, object>> unSelect = null)
        {
            this.unSelect = unSelect;
            return this;
        }

        /// <summary>
        /// whereclip
        /// </summary>
        /// <param name="query">查询条件</param>
        /// <returns></returns>
        public IQuery<T> Where(Expression<Func<T, bool>> query = null)
        {
            And(query);
            return this;
        }

        /// <summary>
        /// whereclip
        /// </summary>
        /// <param name="query">查询条件</param>
        /// <param name="select">查询属性</param>
        /// <param name="unSelect">排除查询属性</param>
        /// <returns></returns>
        public IQuery<T> Where(Expression<Func<T, bool>> query = null,
            Expression<Func<T, object>> select = null,
            Expression<Func<T, object>> unSelect = null)
        {
            this.query = query;
            this.select = select;
            this.unSelect = unSelect;
            return this;
        }
        /// <summary>
        /// 追加And查询条件
        /// </summary>
        /// <param name="query">查询条件</param>
        /// <returns></returns>
        public IQuery<T> And(Expression<Func<T, bool>> query = null)
        {
            if (this.query == null)
            {
                this.query = query;
            }
            else
            {
                this.query = this.query.And(query);
            }
            return this;
        }

        /// <summary>
        /// 追加Or查询条件
        /// </summary>
        /// <param name="query">查询条件</param>
        /// <returns></returns>
        public IQuery<T> Or(Expression<Func<T, bool>> query = null)
        {
            if (this.query == null)
            {
                this.query = query;
            }
            else
            {
                this.query = this.query.Or(query);
            }
            return this;
        }

        /// <summary>
        /// 执行查询记录
        /// </summary>
        /// <returns></returns>
        public T FirstOrDefault()
        {
            Pager pager = new Pager(1,1);
            if (orderByClauseList == null || orderByClauseList.Count <= 0)
            {
                EntityMapping dtoDbMapping = EntityMappingHelper.GetMapping<T>();
                if (dtoDbMapping?.HasPkField == true)
                {
                    orderByClauseList.Add(new OrderByClause(dtoDbMapping.GetPkField().FieldName));
                }
            }
            QueryFilter filter = QueryFilterBuilder.GetPageFilter(query, select, orderByClauseList,pager, unSelect);
            
            T dto = null;
            using (DbCommand dbCommand = dbContext.DbProvider.GetQueryRecordsPageDbCommand<T>(filter, subTableArg))
            {
                try
                {
                    dbContext.SetDbConnection(dbCommand);
                    using (DataTable dt = DbCommandExecuter.ExecuteDataTable(dbCommand,1, dbContext.LogHelper))
                    {
                        if (dt?.Rows.Count > 0)
                        {
                            dto = dt.Rows[0].ToEntity<T>();
                        }
                    }
                    dbContext.CloseDbConnection(dbCommand);
                }
                catch (Exception ex)
                {
                    dbContext.CloseDbConnection(dbCommand);
                    throw ex;
                }
            }
            return dto;
        }

        /// <summary>
        /// 返回字段Sum
        /// </summary>
        /// <returns></returns>
        public decimal Sum(Expression<Func<T, object>> select = null)
        {
            this.select = select;
            QueryFilter filter = QueryFilterBuilder.GetFilter(query, select);

            decimal result = 0;
            using (DbCommand dbCommand = dbContext.DbProvider.GetSumDbCommand<T>(filter, subTableArg))
            {
                try
                {
                    dbContext.SetDbConnection(dbCommand);
                    object objVal = DbCommandExecuter.ExecuteScalar(dbCommand, dbContext.LogHelper);
                    if (objVal != DBNull.Value)
                    {   //使用属性字典
                        result = (decimal)objVal;
                    }
                    dbContext.CloseDbConnection(dbCommand);
                }
                catch (Exception ex)
                {
                    dbContext.CloseDbConnection(dbCommand);
                    throw ex;
                }
            }
            return result;
        }
        
        /// <summary>
        /// 返回RecCount
        /// </summary>
        /// <returns></returns>
        public int Count(Expression<Func<T, bool>> query = null)
        {
            if (query != null)
            {
                this.query = query;
            }
            QueryFilter filter = QueryFilterBuilder.GetFilter(this.query);

            int result = 0;
            using (DbCommand dbCommand = dbContext.DbProvider.GetCountDbCommand<T>(filter, subTableArg))
            {
                try
                {
                    dbContext.SetDbConnection(dbCommand);
                    object objVal = DbCommandExecuter.ExecuteScalar(dbCommand, dbContext.LogHelper);
                    if (objVal != DBNull.Value)
                    {   //使用属性字典
                        result = Convert.ToInt32(objVal);
                    }
                    dbContext.CloseDbConnection(dbCommand);
                }
                catch (Exception ex)
                {
                    dbContext.CloseDbConnection(dbCommand);
                    throw ex;
                }
            }
            return result;
        }

        /// <summary>
        /// 返回字段最小值
        /// </summary>
        /// <param name="select">计算属性</param>
        /// <returns></returns>
        public string Min(Expression<Func<T, object>> select)
        {
            this.select = select;
            QueryFilter filter = QueryFilterBuilder.GetFilter(query, select);

            string result = null;
            using (DbCommand dbCommand = dbContext.DbProvider.GetMinDbCommand<T>(filter, subTableArg))
            {
                try
                {
                    dbContext.SetDbConnection(dbCommand);
                    object objVal = DbCommandExecuter.ExecuteScalar(dbCommand, dbContext.LogHelper);
                    if (objVal != DBNull.Value)
                    {   //使用属性字典
                        result = objVal.ToString();
                    }
                    dbContext.CloseDbConnection(dbCommand);
                }
                catch (Exception ex)
                {
                    dbContext.CloseDbConnection(dbCommand);
                    throw ex;
                }
            }
            return result;
        }


        /// <summary>
        /// 返回字段最大值
        /// </summary>
        /// <param name="select">计算属性</param>
        /// <returns></returns>
        public string Max(Expression<Func<T, object>> select)
        {
            this.select = select;
            QueryFilter filter = QueryFilterBuilder.GetFilter(query, select);

            string result = null;
            using (DbCommand dbCommand = dbContext.DbProvider.GetMaxDbCommand<T>(filter, subTableArg))
            {
                try
                {
                    dbContext.SetDbConnection(dbCommand);
                    object objVal = DbCommandExecuter.ExecuteScalar(dbCommand, dbContext.LogHelper);
                    if (objVal != DBNull.Value)
                    {   //使用属性字典
                        result = objVal.ToString();
                    }
                    dbContext.CloseDbConnection(dbCommand);
                }
                catch (Exception ex)
                {
                    dbContext.CloseDbConnection(dbCommand);
                    throw ex;
                }
            }
            return result;
        }

        /// <summary>
        /// 执行查询记录
        /// </summary>
        /// <returns></returns>
        public List<T> ToList()
        {
            QueryFilter filter = QueryFilterBuilder.GetFilter(query, select, orderByClauseList, unSelect);
            if (pager != null)
            {
                filter.InitPage(pager.PageIndex, pager.PageSize);
            }
            List<T> list = new List<T>();
            using (DbCommand dbCommand = this.dbContext.DbProvider.GetQueryDbCommand<T>(filter, subTableArg))
            {
                try
                {
                    this.dbContext.SetDbConnection(dbCommand);
                    using (DataTable dt = DbCommandExecuter.ExecuteDataTable(dbCommand, null, dbContext.LogHelper))
                    {
                        list = dt?.ToList<T>();
                    }
                    dbContext.CloseDbConnection(dbCommand);
                }
                catch (Exception ex)
                {
                    this.dbContext.CloseDbConnection(dbCommand);
                    throw ex;
                };
            }
            return list;
        }

        /// <summary>
        /// 执行查询记录,返回DataTable
        /// </summary>
        /// <returns></returns>
        public DataTable ToDataTable()
        {
            DataTable dt = null;
            QueryFilter filter = QueryFilterBuilder.GetFilter(query, select, orderByClauseList, unSelect);
            if (pager != null)
            {
                filter.InitPage(pager.PageIndex, pager.PageSize);
            }
            DbCommand dbCommand = null;
            if (filter.IsPage)
            {
                dbCommand = this.dbContext.DbProvider.GetQueryRecordsPageDbCommand<T>(filter, subTableArg);
            }
            else
            {
                dbCommand = this.dbContext.DbProvider.GetQueryDbCommand<T>(filter, subTableArg);
            }
            try
            {
                this.dbContext.SetDbConnection(dbCommand);
                dt = DbCommandExecuter.ExecuteDataTable(dbCommand, null, dbContext.LogHelper);
                dbContext.CloseDbConnection(dbCommand);
            }
            catch (Exception ex)
            {
                this.dbContext.CloseDbConnection(dbCommand);
                throw ex;
            }
            finally
            {
                dbCommand.Dispose();
            }
            return dt;
        }

        /// <summary>
        /// 执行查询记录
        /// </summary>
        /// <returns></returns>
        public PageResult<T> ToPageList()
        {
            PageResult<T> result = new PageResult<T>();
            QueryFilter filter = QueryFilterBuilder.GetPageFilter(query, select, orderByClauseList, pager,unSelect);

            List<T> list = new List<T>();
            DbCommand dbCommand = null;
            try
            {
                if (!filter.IsPage)
                {
                    throw new Exception("分页查询未指定分页信息");
                }
                else
                {
                    dbCommand = dbContext.DbProvider.GetQueryRecordsPageDbCommand<T>(filter, subTableArg);
                }
                dbContext.SetDbConnection(dbCommand);
                using (DataTable dt = DbCommandExecuter.ExecuteDataTable(dbCommand, null, dbContext.LogHelper))
                {
                    list = dt?.ToList<T>();
                }
                int totalCount = 0;
                DbCommand getRecCountDbCommand = dbContext.DbProvider.GetCountDbCommand<T>(filter, subTableArg);
                getRecCountDbCommand.Connection = dbCommand.Connection;
                object valueObj = DbCommandExecuter.ExecuteScalar(getRecCountDbCommand, dbContext.LogHelper);
                if (valueObj != null && valueObj != DBNull.Value)
                {
                    totalCount = Convert.ToInt32(valueObj);
                }
                //分页查询 获取记录总数
                result.Rows = list;
                result.Total = totalCount;
                filter.TotalCount = totalCount;
                dbContext.CloseDbConnection(dbCommand);
            }
            catch (Exception ex)
            {
                dbContext.CloseDbConnection(dbCommand);
                throw ex;
            }
            return result;
        }
        
        /// <summary>
        /// 执行查询记录
        /// </summary>
        /// <returns></returns>
        public PageResult<T> ToPageList(int pageIndex, int pageSize)
        {
            this.Page(pageIndex, pageSize);
            return ToPageList();
        }

        /// <summary>
        /// whereclip
        /// </summary>
        /// <param name="pageIndex"></param>
        /// <param name="pageSize"></param>
        /// <returns></returns>
        public IQuery<T> Page(int pageIndex, int pageSize)
        {
            this.pager = new Pager(pageIndex, pageSize);
            return this;
        }

        /// <summary>
        /// 添加Orderby
        /// </summary>
        /// <param name="sort">排序字段</param>
        /// <param name="order">排序方向 asc desc</param>
        /// <param name="expr">默认排序属性</param>
        /// <param name="defaultOrder">默认排序方向</param>
        /// <returns></returns>
        public IQuery<T> AutoOrderBy(string sort, string order, Expression<Func<T, object>> expr = null,Sorting defaultOrder = Sorting.Asc)
        {
            if (!string.IsNullOrEmpty(sort))
            {
                List<FieldMapping> piMapList = EntityMappingHelper.GetPiMapList<T>();
                if (piMapList != null && piMapList.Count > 0)
                {
                    FieldMapping piMap = piMapList.FirstOrDefault(a => a.PiName.ToLower() == sort.ToLower() && a.IsIgnore == false);
                    if (piMap != null)
                    {
                        this.orderByClauseList.Add(new OrderByClause()
                        {
                            FieldName = piMap.FieldName,
                            Order = order.ToLower() == "desc" ? Sorting.Desc : Sorting.Asc
                        });
                    }
                    else
                    {
                        throw new Exception($"无效的排序属性{sort}");
                    }
                }
            }
            else if (expr != null)
            {
                if(defaultOrder == Sorting.Asc)
                {
                    OrderBy(expr);
                }
                else
                {
                    OrderByDesc(expr);
                }
            }
            return this;
        }
        
        /// <summary>
        /// OrderBy
        /// </summary>
        /// <returns></returns>
        public IQuery<T> OrderBy(Expression<Func<T, object>> expr)
        {
            List<FieldMapping> piMapList = EntityMappingHelper.GetPiMapList<T>(expr);
            if (piMapList != null && piMapList.Count > 0)
            {
                for (int i = 0; i < piMapList.Count; i++)
                {
                    this.orderByClauseList.Add(new OrderByClause() {
                        FieldName = piMapList[i].FieldName,
                        Order = Sorting.Asc });
                }
            }
            else
            {
                throw new Exception("无效的查询排序表达式.");
            }
            return this;
        }

        /// <summary>
        /// OrderByDesc
        /// </summary>
        /// <returns></returns>
        public IQuery<T> OrderByDesc(Expression<Func<T, object>> expr)
        {
            List<FieldMapping> piMapList = EntityMappingHelper.GetPiMapList<T>(expr);
            if (piMapList != null && piMapList.Count > 0)
            {
                for (int i = 0; i < piMapList.Count; i++)
                {
                    this.orderByClauseList.Add(new OrderByClause()
                    {
                        FieldName = piMapList[i].FieldName,
                        Order = Sorting.Desc
                    });
                }
            }
            else
            {
                throw new Exception("无效的查询排序表达式.");
            }
            return this;
        }        
        #endregion
    }
}