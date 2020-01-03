using System;
using System.Collections.Generic;
using System.Text;
using System.Runtime.Serialization;
using System.Data;
using System.Linq.Expressions;
using System.Collections;

namespace Jc.Core.Data.Query
{
    ///  <summary>
    /// 过滤命令
    /// </summary>
    public class QueryFilter
    {
        #region Fields
        private List<Object> itemList = new List<Object>(); //所有条件Item
        private List<QueryParameter> filterParameters = new List<QueryParameter>(); //过滤条件
        private List<OrderByClause> orderByClauseList = new List<OrderByClause>();  //排序条件
        private string filterSQLString = " where ";    //过滤语句
        private string orderSQLString = ""; //排序语句

        Expression select = null; //查询属性表达式 Expression<Func<T, object>>
        Expression unSelect = null; //排除查询属性表达式 Expression<Func<T, object>>

        private List<PiMap> piMapList = null;    //查询属性mapList

        private Pager pager = null;    //分页信息
        
        /// <summary>
        /// item是否为连接Item,如果是连接Item,在添加下一个QueryItem时,忽略Conjuction
        /// </summary>
        private bool lastIsConjuction = true;
        #endregion

        #region Ctor
        /// <summary>
        /// Ctor
        /// </summary>
        public QueryFilter()
        {
        }
        #endregion

        #region Properties        
        /// <summary>
        /// Sql语句部分
        /// </summary>
        public string FilterSQLString { get { return filterSQLString; } }

        /// <summary>
        /// 排序String
        /// </summary>
        public string OrderSQLString { get { return orderSQLString; } }

        /// <summary>
        /// 所有条件List
        /// </summary>
        public List<Object> ItemList { get { return itemList; } }

        /// <summary>
        /// 执行参数
        /// </summary>
        public List<QueryParameter> FilterParameters { get { return filterParameters; } }

        /// <summary>
        /// 排序参数List
        /// </summary>
        public List<OrderByClause> OrderByClauseList { get { return orderByClauseList; } }
        
        /// <summary>
        /// 分页标识
        /// </summary>
        public bool IsPage { get { return pager!=null; } }

        /// <summary>
        /// 页序号
        /// </summary>
        public int PageIndex { get { return pager != null ? pager.PageIndex : 0; } }
        /// <summary>
        /// 页大小
        /// </summary>
        public int PageSize { get { return pager != null ? pager.PageSize : 0; } }
        /// <summary>
        /// 分页开始Index 自0开始 (pageIndex-1)*pageSize
        /// </summary>
        public int FilterStartIndex { get { return pager != null ? pager.FilterStartIndex : 0; } }
        /// <summary>
        /// 分页结束Index
        /// </summary>
        public int FilterEndIndex { get { return pager != null ? pager.FilterEndIndex : 0; } }

        /// <summary>
        /// 结果总条数用于分页
        /// </summary>
        public int TotalCount
        {
            get { return pager != null ? pager.TotalCount : 0; }
            set { if (pager != null) pager.TotalCount = value; }
        }

        /// <summary>
        /// 查询属性表达式
        /// Expression Func(T, object)
        /// </summary>
        public Expression Select
        {
            get { return select; }
            set
            {
                select = value;
                this.piMapList = null;  //查询属性改变,清理缓存
            }
        }

        /// <summary>
        /// 排除查询属性表达式
        /// Expression Func(T, object)
        /// </summary>
        public Expression UnSelect
        {
            get { return unSelect; }
            set
            {
                unSelect = value;
                this.piMapList = null;  //查询属性改变,清理缓存
            }
        }

        /// <summary>
        /// 查询属性List
        /// </summary>
        public List<PiMap> PiMapList
        {
            get
            {
                return piMapList;
            }
            set { piMapList = value; }
        }

        #endregion

        #region Methods

        /// <summary>
        /// 增加查询项
        /// </summary>
        /// <param name="qp"></param>
        /// <returns></returns>
        public virtual void AddQueryItem(QueryParameter qp)
        {
            AddQueryItem(qp.FieldName, qp.Conj, qp.Op, qp.ParameterValue, qp.ParameterDbType);
        }

        /// <summary>
        /// 增加查询项
        /// </summary>
        /// <param name="fieldName"></param>
        /// <param name="conjuction"></param>
        /// <param name="operand"></param>
        /// <param name="compareValue"></param>
        /// <returns></returns>
        public virtual void AddQueryItem(string fieldName, Conjuction conjuction, Operand operand, object compareValue)
        {
            AddQueryItem(fieldName, conjuction, operand, compareValue, DbTypeConvertor.GetDbType(compareValue));
        }

        /// <summary>
        /// 增加查询项
        /// </summary>
        /// <param name="fieldName"></param>
        /// <param name="conjuction"></param>
        /// <param name="operand"></param>
        /// <param name="value"></param>
        public virtual void AddQueryItem(string fieldName, Conjuction conjuction, Operand operand, object value, DbType dbType)
        {
            QueryParameter p = new QueryParameter();
            p.FieldName = fieldName;
            p.ParameterValue = value;
            p.ParameterDbType = dbType;
            p.Op = operand;
            p.Conj = conjuction;

            string whereClause = "";
            switch (p.Op)
            {
                case Operand.Equal:
                    p.ParameterName = GetParameterName(fieldName);
                    this.filterParameters.Add(p);
                    whereClause += " " + p.FieldName + " = " + p.ParameterName;
                    break;
                case Operand.NotEqual:
                    p.ParameterName = GetParameterName(fieldName);
                    this.filterParameters.Add(p);
                    whereClause += " " + p.FieldName + "<>" + p.ParameterName;
                    break;
                case Operand.GreaterThan:
                    p.ParameterName = GetParameterName(fieldName);
                    this.filterParameters.Add(p);
                    whereClause += " " + p.FieldName + ">" + p.ParameterName;
                    break;
                case Operand.LessThan:
                    p.ParameterName = GetParameterName(fieldName);
                    this.filterParameters.Add(p);
                    whereClause += " " + p.FieldName + "<" + p.ParameterName;
                    break;
                case Operand.LessThanOrEqual:
                    p.ParameterName = GetParameterName(fieldName);
                    this.filterParameters.Add(p);
                    whereClause += " " + p.FieldName + "<=" + p.ParameterName;
                    break;
                case Operand.GreaterThanOrEqual:
                    p.ParameterName = GetParameterName(fieldName);
                    this.filterParameters.Add(p);
                    whereClause += " " + p.FieldName + ">=" + p.ParameterName;
                    break;
                case Operand.Like:
                    p.ParameterName = GetParameterName(fieldName);
                    p.ParameterValue = string.Format("%{0}%", p.ParameterValue);
                    this.filterParameters.Add(p);
                    whereClause += " " + p.FieldName + " like " + p.ParameterName;
                    break;
                case Operand.RightLike:
                    p.ParameterName = GetParameterName(fieldName);
                    p.ParameterValue = string.Format("%{0}", p.ParameterValue);
                    this.filterParameters.Add(p);
                    whereClause += " " + p.FieldName + " like " + p.ParameterName;
                    break;
                case Operand.LeftLike:
                    p.ParameterName = GetParameterName(fieldName);
                    p.ParameterValue = string.Format("{0}%", p.ParameterValue);
                    this.filterParameters.Add(p);
                    whereClause += " " + p.FieldName + " like " + p.ParameterName;
                    break;
                case Operand.NotLike:
                    p.ParameterName = GetParameterName(fieldName);
                    p.ParameterValue = string.Format("%{0}%", p.ParameterValue);
                    this.filterParameters.Add(p);
                    whereClause += " " + p.FieldName + " not like " + p.ParameterName;
                    break;
                case Operand.Contains:
                    whereClause += " Contains ( " + p.FieldName + " , " + p.ParameterName + " )";
                    break;
                case Operand.IsNull:
                    whereClause += " " + p.FieldName + " is null";
                    break;
                case Operand.IsNotNull:
                    whereClause += " " + p.FieldName + " is not null";
                    break;
                case Operand.In:        //in 需要参数格式 @p1, @p2, @p3,...
                    p.ParameterName = AddInParam(fieldName, value, dbType);
                    if (!string.IsNullOrEmpty(p.ParameterName))
                    {   //判断参数有效 此条件有效
                        whereClause += " " + p.FieldName + " in (" + p.ParameterName + ") ";
                    }
                    else
                    {   //如果 in条件为空. 返回空列表
                        whereClause += " 1 = 0";
                    }
                    break;
                case Operand.NotIn:
                    p.ParameterName = AddInParam(fieldName,value,dbType);
                    if (!string.IsNullOrEmpty(p.ParameterName))
                    {   //判断参数有效 此条件有效
                        whereClause += " " + p.FieldName + " not in (" + p.ParameterName + ") ";
                    }
                    else
                    {   //如参数为空,忽略此参数 返回所有
                    }
                    break;
                case Operand.FieldEqual:
                    whereClause += " " + p.FieldName + " = " + p.ParameterValue.ToString();
                    break;
                case Operand.FieldGreaterThan:
                    whereClause += " " + p.FieldName + " > " + p.ParameterValue.ToString();
                    break;
                case Operand.FieldLessThan:
                    whereClause += " " + p.FieldName + " < " + p.ParameterValue.ToString();
                    break;
                case Operand.FieldGreaterThanOrEqual:
                    whereClause += " " + p.FieldName + " >= " + p.ParameterValue.ToString();
                    break;
                case Operand.FieldLessThanOrEqual:
                    whereClause += " " + p.FieldName + " <= " + p.ParameterValue.ToString();
                    break;
            }
            if (!lastIsConjuction)
            {
                string conjuctionStr = "";
                if (p.Conj == Conjuction.And)
                {
                    conjuctionStr = " and";
                }
                else if (p.Conj == Conjuction.Or)
                {
                    conjuctionStr = " or";
                }
                this.filterSQLString += conjuctionStr;
            }
            if (!string.IsNullOrEmpty(whereClause))
            {
                this.filterSQLString += whereClause;
                this.ItemList.Add(p);
                lastIsConjuction = false;
            }
        }

        /// <summary>
        /// 添加In查询参数
        /// </summary>
        private string AddInParam(string fieldName, object paramValue, DbType dbType)
        {
            string paramName = "";
            //支持数组,List泛型 int,int?,float,float?,double,double?,
            //long,long?,Guid,Guid?,Datetime,Datetime?,string
            if (paramValue != null)
            {
                IEnumerable ienumable = paramValue as IEnumerable;
                IEnumerator enumerator = ienumable.GetEnumerator();

                while (enumerator.MoveNext())
                {
                    if (enumerator.Current != null)
                    {
                        QueryParameter qp = new QueryParameter();
                        qp.ParameterName += GetParameterName(fieldName);
                        qp.ParameterValue = enumerator.Current;
                        qp.ParameterDbType = dbType;
                        this.filterParameters.Add(qp);
                        paramName += string.IsNullOrEmpty(paramName)
                            ? qp.ParameterName : ("," + qp.ParameterName);
                    }
                }
            }
            return paramName;
        }

        /// <summary>
        /// 根据字段名获取参数名
        /// </summary>
        /// <param name="fieldName"></param>
        /// <returns></returns>
        protected string GetParameterName(string fieldName)
        {
            string parameterName = string.Format(
                                "@p{0}_{1}",
                                this.FilterParameters.Count +1,
                                fieldName.Replace('.', '_')
                                );
            return parameterName;
        }

        /// <summary>
        /// 清空所有条件项
        /// </summary>
        private void Clear()
        {
            this.filterParameters.Clear();
            this.ItemList.Clear();
            this.filterSQLString = "Where ";
            lastIsConjuction = true;
        }

        /// <summary>
        /// 添加自定义Sql命令
        /// </summary>
        /// <param name="cmd"></param>
        public virtual void AddCustomCmd(string cmd)
        {
            string paramStr = cmd;
            this.ItemList.Add(paramStr);
            this.filterSQLString += paramStr;
            lastIsConjuction = false;
        }

        /// <summary>
        /// 增加逻辑连接符
        /// </summary>
        /// <param name="logicOperator"></param>
        public virtual void AddLogicOperator(Conjuction logicOperator)
        {
            if (lastIsConjuction) return;
            string paramStr = "";
            if (logicOperator == Conjuction.And)
            {
                paramStr = " and";
            }
            else if (logicOperator == Conjuction.Or)
            {
                paramStr = " or";
            }
            this.ItemList.Add(logicOperator);
            this.filterSQLString += paramStr;
            lastIsConjuction = true;
        }

        /// <summary>
        /// 增加左括符 
        /// </summary>
        public virtual void AddLeftParenthesis()
        {
            string paramStr = " (";
            this.ItemList.Add(paramStr);
            this.filterSQLString += paramStr;
            lastIsConjuction = true;
        }

        /// <summary>
        /// 增加右括符
        /// </summary>
        public virtual void AddRightParenthesis()
        {
            string paramStr = " )";
            this.ItemList.Add(paramStr);
            this.filterSQLString += paramStr;
        }

        /// <summary>
        /// 添加排序条件
        /// </summary>
        /// <param name="fieldName"></param>
        /// <param name="order"></param>
        public virtual void AddOrderBy(string fieldName, Sorting order)
        {    //防Sql注入
            if (CheckSql(fieldName))
            {
                OrderByClauseList.Add(new OrderByClause(fieldName, order));
                if (string.IsNullOrEmpty(this.orderSQLString))
                {
                    this.orderSQLString += " order by";
                }
                else
                {
                    this.orderSQLString += ",";
                }
                if (order == Sorting.Asc)
                {
                    this.orderSQLString += " " + fieldName + " asc";
                }
                else
                {
                    this.orderSQLString += " " + fieldName + " desc";
                }
            }
        }

        /// <summary>
        /// 初始化分页信息
        /// <param name="pageIndex">第几页</param>
        /// <param name="pageSize">页大小</param>
        /// </summary>
        public void InitPage(int pageIndex = 1, int pageSize = 10)
        {
            this.pager = new Pager(pageIndex,pageSize);
        }

        /// <summary>
        /// 检查Sql是否合法
        /// 合法返回true
        /// </summary>
        /// <param name="str"></param>
        /// <returns></returns>
        public static bool CheckSql(string str)
        {
            str = str.ToLower();    //转换为小写比较
            string injStr = "'|and|exec|insert|select|delete|update|count|*|%|chr|mid|master|truncate|char|declare|;|or|-|+|,";
            string[] injStrList = injStr.Split('|');
            for (int i = 0; i < injStrList.Length; i++)
            {
                if (str == injStrList[i])
                {
                    return false;
                }
            }
            return true;
        }

        /// <summary>
        /// 过滤掉sql关键字,返回sql
        /// </summary>
        /// <param name="str"></param>
        /// <returns></returns>
        public static string FormatSafeSql(string str)
        {
            str = str.ToLower();    //转换为小写比较
            string injStr = "'|and|exec|insert|select|delete|update|count|*|%|chr|mid|master|truncate|char|declare|;|or|-|+|,";
            string[] injStrList = injStr.Split('|');
            for (int i = 0; i < injStrList.Length; i++)
            {
                if (str.Contains(injStrList[i]))
                {
                    str = str.Replace(injStrList[i],"");
                }
            }
            return str;
        }
        #endregion
    }
}
