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
    /// ��������
    /// </summary>
    public class QueryFilter
    {
        #region Fields
        private List<Object> itemList = new List<Object>(); //��������Item
        private List<QueryParameter> filterParameters = new List<QueryParameter>(); //��������
        private List<OrderByClause> orderByClauseList = new List<OrderByClause>();  //��������
        private string filterSQLString = " where ";    //�������
        private string orderSQLString = ""; //�������

        Expression select = null; //��ѯ���Ա��ʽ Expression<Func<T, object>>
        Expression unSelect = null; //�ų���ѯ���Ա��ʽ Expression<Func<T, object>>

        private List<PiMap> piMapList = null;    //��ѯ����mapList

        private Pager pager = null;    //��ҳ��Ϣ
        
        /// <summary>
        /// item�Ƿ�Ϊ����Item,���������Item,�������һ��QueryItemʱ,����Conjuction
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
        /// Sql��䲿��
        /// </summary>
        public string FilterSQLString { get { return filterSQLString; } }

        /// <summary>
        /// ����String
        /// </summary>
        public string OrderSQLString { get { return orderSQLString; } }

        /// <summary>
        /// ��������List
        /// </summary>
        public List<Object> ItemList { get { return itemList; } }

        /// <summary>
        /// ִ�в���
        /// </summary>
        public List<QueryParameter> FilterParameters { get { return filterParameters; } }

        /// <summary>
        /// �������List
        /// </summary>
        public List<OrderByClause> OrderByClauseList { get { return orderByClauseList; } }
        
        /// <summary>
        /// ��ҳ��ʶ
        /// </summary>
        public bool IsPage { get { return pager!=null; } }

        /// <summary>
        /// ҳ���
        /// </summary>
        public int PageIndex { get { return pager != null ? pager.PageIndex : 0; } }
        /// <summary>
        /// ҳ��С
        /// </summary>
        public int PageSize { get { return pager != null ? pager.PageSize : 0; } }
        /// <summary>
        /// ��ҳ��ʼIndex ��0��ʼ (pageIndex-1)*pageSize
        /// </summary>
        public int FilterStartIndex { get { return pager != null ? pager.FilterStartIndex : 0; } }
        /// <summary>
        /// ��ҳ����Index
        /// </summary>
        public int FilterEndIndex { get { return pager != null ? pager.FilterEndIndex : 0; } }

        /// <summary>
        /// ������������ڷ�ҳ
        /// </summary>
        public int TotalCount
        {
            get { return pager != null ? pager.TotalCount : 0; }
            set { if (pager != null) pager.TotalCount = value; }
        }

        /// <summary>
        /// ��ѯ���Ա��ʽ
        /// Expression Func(T, object)
        /// </summary>
        public Expression Select
        {
            get { return select; }
            set
            {
                select = value;
                this.piMapList = null;  //��ѯ���Ըı�,������
            }
        }

        /// <summary>
        /// �ų���ѯ���Ա��ʽ
        /// Expression Func(T, object)
        /// </summary>
        public Expression UnSelect
        {
            get { return unSelect; }
            set
            {
                unSelect = value;
                this.piMapList = null;  //��ѯ���Ըı�,������
            }
        }

        /// <summary>
        /// ��ѯ����List
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
        /// ���Ӳ�ѯ��
        /// </summary>
        /// <param name="qp"></param>
        /// <returns></returns>
        public virtual void AddQueryItem(QueryParameter qp)
        {
            AddQueryItem(qp.FieldName, qp.Conj, qp.Op, qp.ParameterValue, qp.ParameterDbType);
        }

        /// <summary>
        /// ���Ӳ�ѯ��
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
        /// ���Ӳ�ѯ��
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
                case Operand.In:        //in ��Ҫ������ʽ @p1, @p2, @p3,...
                    p.ParameterName = AddInParam(fieldName, value, dbType);
                    if (!string.IsNullOrEmpty(p.ParameterName))
                    {   //�жϲ�����Ч ��������Ч
                        whereClause += " " + p.FieldName + " in (" + p.ParameterName + ") ";
                    }
                    else
                    {   //��� in����Ϊ��. ���ؿ��б�
                        whereClause += " 1 = 0";
                    }
                    break;
                case Operand.NotIn:
                    p.ParameterName = AddInParam(fieldName,value,dbType);
                    if (!string.IsNullOrEmpty(p.ParameterName))
                    {   //�жϲ�����Ч ��������Ч
                        whereClause += " " + p.FieldName + " not in (" + p.ParameterName + ") ";
                    }
                    else
                    {   //�����Ϊ��,���Դ˲��� ��������
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
        /// ���In��ѯ����
        /// </summary>
        private string AddInParam(string fieldName, object paramValue, DbType dbType)
        {
            string paramName = "";
            //֧������,List���� int,int?,float,float?,double,double?,
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
        /// �����ֶ�����ȡ������
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
        /// �������������
        /// </summary>
        private void Clear()
        {
            this.filterParameters.Clear();
            this.ItemList.Clear();
            this.filterSQLString = "Where ";
            lastIsConjuction = true;
        }

        /// <summary>
        /// ����Զ���Sql����
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
        /// �����߼����ӷ�
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
        /// ���������� 
        /// </summary>
        public virtual void AddLeftParenthesis()
        {
            string paramStr = " (";
            this.ItemList.Add(paramStr);
            this.filterSQLString += paramStr;
            lastIsConjuction = true;
        }

        /// <summary>
        /// ����������
        /// </summary>
        public virtual void AddRightParenthesis()
        {
            string paramStr = " )";
            this.ItemList.Add(paramStr);
            this.filterSQLString += paramStr;
        }

        /// <summary>
        /// �����������
        /// </summary>
        /// <param name="fieldName"></param>
        /// <param name="order"></param>
        public virtual void AddOrderBy(string fieldName, Sorting order)
        {    //��Sqlע��
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
        /// ��ʼ����ҳ��Ϣ
        /// <param name="pageIndex">�ڼ�ҳ</param>
        /// <param name="pageSize">ҳ��С</param>
        /// </summary>
        public void InitPage(int pageIndex = 1, int pageSize = 10)
        {
            this.pager = new Pager(pageIndex,pageSize);
        }

        /// <summary>
        /// ���Sql�Ƿ�Ϸ�
        /// �Ϸ�����true
        /// </summary>
        /// <param name="str"></param>
        /// <returns></returns>
        public static bool CheckSql(string str)
        {
            str = str.ToLower();    //ת��ΪСд�Ƚ�
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
        /// ���˵�sql�ؼ���,����sql
        /// </summary>
        /// <param name="str"></param>
        /// <returns></returns>
        public static string FormatSafeSql(string str)
        {
            str = str.ToLower();    //ת��ΪСд�Ƚ�
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
