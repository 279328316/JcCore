using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace Jc.Core.Data.Query
{
    /// <summary>
    /// Lambda转QueryFilter
    /// </summary>
    public class QueryFilterHelper
    {
        DtoMapping dtoDbMapping = null;
        private QueryFilter filter = new QueryFilter();

        private QueryFilterHelper()
        {
        }

        /// <summary>
        /// NodeType枚举
        /// </summary>
        private enum EnumNodeType
        {
            /// <summary>
            /// 二元运算符
            /// </summary>
            [Description("二元运算符")]
            BinaryOperator = 1,

            /// <summary>
            /// 一元运算符
            /// </summary>
            [Description("一元运算符")]
            UndryOperator = 2,

            /// <summary>
            /// 常量表达式
            /// </summary>
            [Description("常量表达式")]
            Constant = 3,

            /// <summary>
            /// 成员（变量）
            /// </summary>
            [Description("成员（变量）")]
            MemberAccess = 4,

            /// <summary>
            /// 函数
            /// </summary>
            [Description("函数")]
            Call = 5,

            /// <summary>
            /// 未知
            /// </summary>
            [Description("未知")]
            Unknown = -99,

            /// <summary>
            /// 不支持
            /// </summary>
            [Description("不支持")]
            NotSupported = -98
        }

        /// <summary>
        /// 判断表达式类型
        /// </summary>
        /// <param name="exp">lambda表达式</param>
        /// <returns></returns>
        private EnumNodeType CheckExpressionType(Expression exp)
        {
            switch (exp.NodeType)
            {
                case ExpressionType.AndAlso:
                case ExpressionType.OrElse:
                case ExpressionType.Equal:
                case ExpressionType.GreaterThanOrEqual:
                case ExpressionType.LessThanOrEqual:
                case ExpressionType.GreaterThan:
                case ExpressionType.LessThan:
                case ExpressionType.NotEqual:
                    return EnumNodeType.BinaryOperator;
                case ExpressionType.Constant:
                    return EnumNodeType.Constant;
                case ExpressionType.MemberAccess:
                    return EnumNodeType.MemberAccess;
                case ExpressionType.Call:
                    return EnumNodeType.Call;
                case ExpressionType.Not:
                case ExpressionType.Convert:
                    return EnumNodeType.UndryOperator;
                default:
                    return EnumNodeType.Unknown;
            }
        }

        /// <summary>
        /// 单一表达式 参数间操作获取
        /// </summary>
        /// <param name="expType"></param>
        /// <param name="isFieldOp">是否两个字段比较</param>
        /// <returns></returns>
        private Operand GetOperand(ExpressionType expType,bool isFieldOp)
        {
            Operand operand = Operand.Equal;
            if (!isFieldOp)
            {
                switch (expType)
                {
                    case ExpressionType.Equal:
                        operand = Operand.Equal;
                        break;
                    case ExpressionType.GreaterThan:
                        operand = Operand.GreaterThan;
                        break;
                    case ExpressionType.GreaterThanOrEqual:
                        operand = Operand.GreaterThanOrEqual;
                        break;
                    case ExpressionType.LessThan:
                        operand = Operand.LessThan;
                        break;
                    case ExpressionType.LessThanOrEqual:
                        operand = Operand.LessThanOrEqual;
                        break;
                    case ExpressionType.NotEqual:
                        operand = Operand.NotEqual;
                        break;
                    default:
                        throw new Exception("单表达式内部连接类型无效:" + expType.ToString());
                }
            }
            else
            {
                switch (expType)
                {
                    case ExpressionType.Equal:
                        operand = Operand.FieldEqual;
                        break;
                    case ExpressionType.GreaterThan:
                        operand = Operand.FieldGreaterThan;
                        break;
                    case ExpressionType.GreaterThanOrEqual:
                        operand = Operand.FieldGreaterThan;
                        break;
                    case ExpressionType.LessThan:
                        operand = Operand.FieldLessThan;
                        break;
                    case ExpressionType.LessThanOrEqual:
                        operand = Operand.FieldLessThanOrEqual;
                        break;
                    case ExpressionType.NotEqual:
                        operand = Operand.FieldNotEqual;
                        break;
                    default:
                        throw new Exception("单表达式内部连接类型无效:" + expType.ToString());
                }
            }
            return operand;
        }
        
        /// <summary>
        /// 表达式类型转换 连接类型
        /// </summary>
        /// <param name="expType">表达式连接类型</param>
        /// <returns></returns>
        private Conjuction GetConjuction(ExpressionType expType)
        {
            Conjuction conjuction = Conjuction.None;
            switch (expType)
            {
                case ExpressionType.And:
                case ExpressionType.AndAlso:
                    conjuction = Conjuction.And;
                    break;
                case ExpressionType.Or:
                case ExpressionType.OrElse:
                    conjuction = Conjuction.Or;
                    break;
                default:
                    throw new Exception("表达式间连接类型无效:" + expType.ToString());
            }
            return conjuction;
        }

        /// <summary>
        /// 具有二进制运算符的表达式
        /// </summary>
        /// <param name="exp"></param>
        /// <returns></returns>
        private void BinarExpressionProvider(Expression exp)
        {
            BinaryExpression be = exp as BinaryExpression;
            Expression left = be.Left;
            Expression right = be.Right;
            ExpressionType expType = be.NodeType;

            bool isExpAtom = false;     //当前Expression 是否为原子表单式 原子表达式不用括号
            isExpAtom = left is MemberExpression || left is ConstantExpression
                        || left is UnaryExpression;
            if (!(left is BinaryExpression || left is MemberExpression 
                || left is ConstantExpression || left is MethodCallExpression
                 || left is UnaryExpression))
            {
                throw new Exception("不支持的表达式类型:" + be.ToString());
            }
            if (!isExpAtom)
            {   //多个表达式情况
                filter.AddLeftParenthesis();
                if (left is BinaryExpression)
                {
                    BinarExpressionProvider(left);
                }
                else if (left is MethodCallExpression)
                {
                    QueryParameter qp = MethodCallExpressionProvider(left);
                    filter.AddQueryItem(qp);
                }
                else
                {
                    string leftStr = AtomExpressionRouter(left)?.ToString();
                    throw new Exception("不支持的表达式类型:" + left.ToString());
                }

                filter.AddLogicOperator(GetConjuction(expType));    //表达式连接 ↑左侧表达式处理  ↓右侧表达式处理

                if (right is BinaryExpression)
                {
                    BinarExpressionProvider(right);
                }
                else if (right is MethodCallExpression)
                {
                    QueryParameter qp = MethodCallExpressionProvider(right);
                    filter.AddQueryItem(qp);
                }
                else
                {
                    throw new Exception("不支持的表达式类型:" + right.ToString());
                }
                filter.AddRightParenthesis();
            }
            else
            {
                bool isFieldOp = left is MemberExpression && right is MemberExpression
                    && !((MemberExpression)left).Expression.ToString().StartsWith("value")
                    && !((MemberExpression)right).Expression.ToString().StartsWith("value");
                object leftVal = AtomExpressionRouter(left);
                Operand operand = GetOperand(expType,isFieldOp);
                object rightVal = AtomExpressionRouter(right);
                if (rightVal == null)
                {   //只有null时使用
                    if (operand == Operand.Equal)
                    {
                        operand = Operand.IsNull;
                    }
                    else
                    {
                        operand = Operand.IsNotNull;
                    }
                }
                filter.AddQueryItem(leftVal?.ToString(), Conjuction.And, operand, rightVal);
            }
        }
        /// <summary>
        /// 具有常数值的表达式
        /// </summary>
        /// <param name="exp"></param>
        /// <returns></returns>
        private object ConstantExpressionProvider(Expression exp)
        {
            ConstantExpression ce = exp as ConstantExpression;
            return ce.Value;
        }
        /// <summary>
        /// lambda 表达式
        /// </summary>
        /// <param name="exp"></param>
        /// <returns></returns>
        private object LambdaExpressionProvider(Expression exp)
        {
            LambdaExpression le = exp as LambdaExpression;
            return AtomExpressionRouter(le.Body);
        }

        /// <summary>
        /// 表示访问字段或属性
        /// </summary>
        /// <param name="exp"></param>
        /// <returns></returns>
        private object MemberExpressionProvider(Expression exp)
        {
            if (!exp.ToString().StartsWith("value"))
            {
                if (exp.ToString().ToLower().EndsWith(".value"))
                {   //NullAble属性  levels.Contains(a.ApiLevel.Value)
                    //throw new Exception("~NullAble属性,请直接使用属性名称.不要使用.Value.");
                    MemberExpression me = exp as MemberExpression;
                    me = me.Expression as MemberExpression;
                    if (me != null)
                    {
                        string memberName = me.Member.Name;
                        if (me.Expression.Type.IsValueType)
                        {
                            object value = Expression.Lambda(exp).Compile().DynamicInvoke();
                            return value;
                        }
                        else
                        {
                            string fieldName = dtoDbMapping.PiMapDic[memberName].FieldName;
                            return fieldName;
                        }
                    }
                    else
                    {
                        throw new Exception("不支持的方法:" + exp.ToString());
                    }
                }
                else if (exp is MemberExpression)
                {
                    MemberExpression me = exp as MemberExpression;
                    string memberName = me.Member.Name;
                    if (me.Expression.Type.IsValueType)
                    {
                        object value = Expression.Lambda(exp).Compile().DynamicInvoke();
                        return value;
                    }
                    else
                    {
                        string fieldName = dtoDbMapping.PiMapDic[memberName].FieldName;
                        return fieldName;
                    }
                }
                else
                {
                    throw new Exception("不支持的方法:" + exp.ToString());
                }
            }
            else
            {   //list.contais(student.Name) 类型
                object result = Expression.Lambda(exp).Compile().DynamicInvoke();
                if (result == null || result is ValueType || result is string || result is DateTime || result is char)
                {
                    return result;
                }
                //支持数组,List泛型 int,int?,float,float?,double,double?,
                //long,long?,Guid,Guid?,Datetime,Datetime?,string
                #region 处理数组类型
                StringBuilder strBuilder = new StringBuilder();
                if (result is IEnumerable<int>)
                {
                    IEnumerable<int> rl = result as IEnumerable<int>;
                    foreach (int r in rl)
                    {
                        if (strBuilder.Length == 0)
                        {
                            strBuilder.Append(r.ToString());
                        }
                        else
                        {
                            strBuilder.Append("," + r.ToString());
                        }
                    }
                }
                else if (result is IEnumerable<int?>)
                {
                    IEnumerable<int?> rl = result as IEnumerable<int?>;
                    foreach (int? r in rl)
                    {
                        if (r == null)
                        {
                            continue;
                            //throw new Exception("可为空查询列表中,不允许有null值.");
                        }
                        if (strBuilder.Length == 0)
                        {
                            strBuilder.Append(r.ToString());
                        }
                        else
                        {
                            strBuilder.Append("," + r.ToString());
                        }
                    }
                }
                else if (result is IEnumerable<float>)
                {
                    IEnumerable<float> rl = result as IEnumerable<float>;
                    foreach (float r in rl)
                    {
                        if (strBuilder.Length == 0)
                        {
                            strBuilder.Append(r.ToString());
                        }
                        else
                        {
                            strBuilder.Append("," + r.ToString());
                        }
                    }
                }
                else if (result is IEnumerable<float?>)
                {
                    IEnumerable<float?> rl = result as IEnumerable<float?>;
                    foreach (float? r in rl)
                    {
                        if (r == null)
                        {
                            continue;
                            //throw new Exception("可为空查询列表中,不允许有null值.");
                        }
                        if (strBuilder.Length == 0)
                        {
                            strBuilder.Append(r.ToString());
                        }
                        else
                        {
                            strBuilder.Append("," + r.ToString());
                        }
                    }
                }
                else if (result is IEnumerable<double>)
                {
                    IEnumerable<double> rl = result as IEnumerable<double>;
                    foreach (double r in rl)
                    {
                        if (strBuilder.Length == 0)
                        {
                            strBuilder.Append(r.ToString());
                        }
                        else
                        {
                            strBuilder.Append("," + r.ToString());
                        }
                    }
                }
                else if (result is IEnumerable<double?>)
                {
                    IEnumerable<double?> rl = result as IEnumerable<double?>;
                    foreach (double? r in rl)
                    {
                        if (r == null)
                        {
                            continue;
                            //throw new Exception("可为空查询列表中,不允许有null值.");
                        }
                        if (strBuilder.Length == 0)
                        {
                            strBuilder.Append(r.ToString());
                        }
                        else
                        {
                            strBuilder.Append("," + r.ToString());
                        }
                    }
                }
                else if (result is IEnumerable<long>)
                {
                    IEnumerable<long> rl = result as IEnumerable<long>;
                    foreach (long r in rl)
                    {
                        if (strBuilder.Length == 0)
                        {
                            strBuilder.Append(r.ToString());
                        }
                        else
                        {
                            strBuilder.Append("," + r.ToString());
                        }
                    }
                }
                else if (result is IEnumerable<long?>)
                {
                    IEnumerable<long?> rl = result as IEnumerable<long?>;
                    foreach (long? r in rl)
                    {
                        if (r == null)
                        {
                            continue;
                            //throw new Exception("可为空查询列表中,不允许有null值.");
                        }
                        if (strBuilder.Length == 0)
                        {
                            strBuilder.Append(r.ToString());
                        }
                        else
                        {
                            strBuilder.Append("," + r.ToString());
                        }
                    }
                }
                else if (result is IEnumerable<Guid>)
                {
                    IEnumerable<Guid> rl = result as IEnumerable<Guid>;
                    foreach (Guid r in rl)
                    {
                        if (strBuilder.Length == 0)
                        {
                            strBuilder.Append(r.ToString());
                        }
                        else
                        {
                            strBuilder.Append("," + r.ToString());
                        }
                    }
                }
                else if (result is IEnumerable<Guid?>)
                {
                    IEnumerable<Guid?> rl = result as IEnumerable<Guid?>;
                    foreach (Guid? r in rl)
                    {
                        if (r == null)
                        {
                            continue;
                            //throw new Exception("可为空查询列表中,不允许有null值.");
                        }
                        if (strBuilder.Length == 0)
                        {
                            strBuilder.Append(r.ToString());
                        }
                        else
                        {
                            strBuilder.Append("," + r.ToString());
                        }
                    }
                }
                else if (result is IEnumerable<DateTime>)
                {
                    IEnumerable<DateTime> rl = result as IEnumerable<DateTime>;
                    foreach (DateTime r in rl)
                    {
                        if (strBuilder.Length == 0)
                        {
                            strBuilder.Append(r.ToString());
                        }
                        else
                        {
                            strBuilder.Append("," + r.ToString());
                        }
                    }
                }
                else if (result is IEnumerable<DateTime?>)
                {
                    IEnumerable<DateTime?> rl = result as IEnumerable<DateTime?>;
                    foreach (DateTime? r in rl)
                    {
                        if (r == null)
                        {
                            continue;
                            //throw new Exception("可为空查询列表中,不允许有null值.");
                        }
                        if (strBuilder.Length == 0)
                        {
                            strBuilder.Append(r.ToString());
                        }
                        else
                        {
                            strBuilder.Append("," + r.ToString());
                        }
                    }
                }
                else if (result is IEnumerable<string>)
                {
                    IEnumerable<string> rl = result as IEnumerable<string>;
                    foreach (string r in rl)
                    {
                        if (strBuilder.Length == 0)
                        {
                            strBuilder.Append(r.ToString());
                        }
                        else
                        {
                            strBuilder.Append("," + r.ToString());
                        }
                    }
                }
                else if (result is IEnumerable<object>)
                {
                    IEnumerable<object> rl = result as IEnumerable<object>;
                    foreach (object r in rl)
                    {
                        if (r == null)
                        {
                            continue;
                            //throw new Exception("可为空查询列表中,不允许有null值.");
                        }
                        if (strBuilder.Length == 0)
                        {
                            strBuilder.Append(r.ToString());
                        }
                        else
                        {
                            strBuilder.Append("," + r.ToString());
                        }
                    }
                }
                else
                {
                    throw new Exception("不支持的方法:" + exp.ToString());
                }
                return strBuilder.ToString();
                #endregion
            }
        }
        

        /// <summary>
        /// 对静态方法或实例方法的调用
        /// </summary>
        /// <param name="exp"></param>
        /// <returns></returns>
        private QueryParameter MethodCallExpressionProvider(Expression exp)
        {
            QueryParameter qp = null;
            object name = null;
            object value = null;
            Operand op = Operand.Equal;
            MethodCallExpression mce = exp as MethodCallExpression;
            switch (mce.Method.Name)
            {
                case "Equals":
                    name = MemberExpressionProvider(mce.Object);
                    //value = ConstantExpressionProvider(mce.Arguments[0]);
                    value = Expression.Lambda(mce.Arguments[0]).Compile().DynamicInvoke().ToString();
                    op = Operand.Equal;                    
                    break;
                case "Contains":
                    if (mce.Object != null)
                    {
                        if (mce.Object is MemberExpression && ((MemberExpression)mce.Object).Member.MemberType == MemberTypes.Property)
                        {   // task.Name.Contains("T1"); task.Name.Contains(keywords);
                            name = AtomExpressionRouter(mce.Object);
                            value = AtomExpressionRouter(mce.Arguments[0]);
                            op = Operand.Like;
                        }
                        //else if (((MemberExpression)mce.Object).Member.MemberType == MemberTypes.Field)
                        //{   //permIds.Contains(a.Id)
                        //    name = AtomExpressionRouter(mce.Arguments[0]);
                        //    value = AtomExpressionRouter(mce.Object);
                        //    op = Operand.In;
                        //}
                        else
                        {   //permIds.Contains(a.Id)
                            name = AtomExpressionRouter(mce.Arguments[0]);
                            value = AtomExpressionRouter(mce.Object);
                            op = Operand.In;
                        }
                    }
                    else if (mce.Arguments.Count >= 2)
                    {   //int与int?类型 list:List<int?>
                        name = AtomExpressionRouter(mce.Arguments[1]);
                        value = AtomExpressionRouter(mce.Arguments[0]);
                        op = Operand.In;
                    }
                    else
                    {
                        throw new Exception($"不支持的方法:{mce.ToString()}");
                    }
                    break;
                case "StartsWith":
                    name = MemberExpressionProvider(mce.Object);
                    //value = ConstantExpressionProvider(mce.Arguments[0]);
                    value = Expression.Lambda(mce.Arguments[0]).Compile().DynamicInvoke().ToString();
                    op = Operand.LeftLike;
                    break;
                case "EndsWith":
                    name = MemberExpressionProvider(mce.Object);
                    //value = ConstantExpressionProvider(mce.Arguments[0]);
                    value = Expression.Lambda(mce.Arguments[0]).Compile().DynamicInvoke().ToString();
                    op = Operand.RightLike;
                    break;
                default:
                    throw new Exception("不支持的方法:" + mce.Method.Name);
            }
            qp = new QueryParameter()
            {
                FieldName = name?.ToString(),
                Op = op,
                ParameterValue = value,
                ParameterDbType = DbTypeConvertor.GetDbType(value)
            };
            return qp;
        }

        /// <summary>
        /// 表示创建一个新数组，并可能初始化该新数组的元素
        /// </summary>
        /// <param name="exp"></param>
        /// <returns></returns>
        private string NewArrayExpressionProvider(Expression exp)
        {
            NewArrayExpression ae = exp as NewArrayExpression;
            StringBuilder sbTmp = new StringBuilder();
            foreach (Expression expItem in ae.Expressions)
            {
                sbTmp.Append(AtomExpressionRouter(expItem));
                sbTmp.Append(",");
            }
            return sbTmp.ToString(0, sbTmp.Length - 1);
        }

        /// <summary>
        /// 表示一个命名的参数表达式
        /// </summary>
        /// <param name="exp"></param>
        /// <returns></returns>
        private string ParameterExpressionProvider(Expression exp)
        {
            ParameterExpression pe = exp as ParameterExpression;
            return pe.Type.Name;
        }

        /// <summary>
        /// 一元运算符的表达式
        /// </summary>
        /// <param name="exp"></param>
        /// <returns></returns>
        private object UnaryExpressionProvider(Expression exp)
        {
            UnaryExpression unaryExp = exp as UnaryExpression;
            object value = AtomExpressionRouter(unaryExp.Operand);
            return value;
        }
        
        /// <summary>
        /// 原子表达式 值计算
        /// </summary>
        /// <param name="exp"></param>
        /// <returns></returns>
        private object AtomExpressionRouter(Expression exp)
        {
            object result = null;
            ExpressionType nodeType = exp.NodeType;
            if (exp is ConstantExpression) //表示具有常数值的表达式
            {
                result = ConstantExpressionProvider(exp);
            }
            else if (exp is LambdaExpression)   //介绍 lambda 表达式。 它捕获一个类似于 .NET 方法主体的代码块
            {
                result = LambdaExpressionProvider(exp);
            }
            else if (exp is MemberExpression)   //表示访问字段或属性
            {
                result = MemberExpressionProvider(exp);
            }
            else if (exp is NewArrayExpression) //表示创建一个新数组，并可能初始化该新数组的元素
            {
                result = NewArrayExpressionProvider(exp);
            }
            else if (exp is ParameterExpression)    //表示一个命名的参数表达式。
            {
                result = ParameterExpressionProvider(exp);
            }
            else if (exp is UnaryExpression)    //表示具有一元运算符的表达式
            {
                result = UnaryExpressionProvider(exp);
            }
            else if (exp is MethodCallExpression)   //介绍 lambda 表达式。 它捕获一个类似于 .NET 方法主体的代码块
            {
                //throw new Exception("MethodCallExpressionStringProvider Is Called:" + exp.ToString());
                try
                {   //尝试直接运算Lambda表达式
                    result = Expression.Lambda(exp).Compile().DynamicInvoke();
                }
                catch (Exception ex)
                {
                }
            }
            else
            {
                try
                {   //尝试直接运算Lambda表达式
                    result = Expression.Lambda(exp).Compile().DynamicInvoke();
                }
                catch (Exception ex)
                {
                }
            }
            return result;
        }

        /// <summary>
        /// 值类型转换
        /// </summary>
        /// <param name="val"></param>
        /// <returns></returns>
        private object GetValueType(object val)
        {
            if (val == null)
            {
                return null;
            }
            else
            {
                string vtype = val.GetType().Name;
                switch (vtype)
                {
                    case "Decimal ": return (decimal)val;
                    case "Int32": return (int)val;
                    case "DateTime": return (DateTime)val;
                    case "String": return val.ToString();
                    case "Char": return val.ToString();
                    case "Boolean": return (bool)val;
                    default: return val;
                }
            }
        }
        
        /// <summary>
        /// lambda表达式转换sql
        /// 支持的方法:
        /// Contains(支持List.Contains字段与字段Contains字符串),
        /// StartsWith,EndsWith,=,!=,>,>=,小于,小于等于
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="query">查询条件</param>
        /// <returns></returns>
        public static QueryFilter GetFilter<T>(Expression<Func<T, bool>> query) where T : class, new()
        {
            QueryFilterHelper filterHelper = new QueryFilterHelper();
            filterHelper.FillFilter(null,query);
            return filterHelper.filter;
        }


        /// <summary>
        /// lambda表达式转换sql
        /// 支持的方法:
        /// Contains(支持List.Contains字段与字段Contains字符串),
        /// StartsWith,EndsWith,=,!=,>,>=,小于,小于等于
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="select">查询属性</param>
        /// <param name="query">查询条件</param>
        /// <param name="orderByClauseList">排序条件</param>
        /// <param name="unSelect">排除查询属性</param>
        /// <returns></returns>
        public static QueryFilter GetFilter<T>(Expression<Func<T, object>> select = null,
            Expression<Func<T, bool>> query = null,
            List<OrderByClause> orderByClauseList = null,
            Expression<Func<T, object>> unSelect = null) where T : class, new()
        {
            QueryFilterHelper filterHelper = new QueryFilterHelper();
            filterHelper.FillFilter(select, query, orderByClauseList, null, unSelect);
            return filterHelper.filter;
        }
        /// <summary>
        /// lambda表达式转换sql
        /// 支持的方法:
        /// Contains(支持List.Contains字段与字段Contains字符串),
        /// StartsWith,EndsWith,=,!=,>,>=,小于,小于等于
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="select">查询属性</param>
        /// <param name="query">查询条件</param>
        /// <param name="orderByClauseList">排序条件</param>
        /// <param name="pager">分页条件</param>
        /// <param name="unSelect">排除查询属性</param>
        /// <returns></returns>
        public static QueryFilter GetPageFilter<T>(Expression<Func<T, object>> select = null,
            Expression<Func<T, bool>> query = null,
            List<OrderByClause> orderByClauseList = null, 
            Pager pager = null,
            Expression<Func<T, object>> unSelect = null) where T : class, new()
        {
            if (pager == null)
            {
                throw new Exception("分页查询未指定分页信息");
            }
            if (orderByClauseList == null || orderByClauseList.Count <= 0)
            {
                throw new Exception("分页查询未指定排序信息");
            }
            QueryFilterHelper filterHelper = new QueryFilterHelper();
            filterHelper.FillFilter(select, query, orderByClauseList, pager, unSelect);
            return filterHelper.filter;
        }
        /// <summary>
        /// lambda表达式转换sql
        /// 支持的方法:
        /// Contains(支持List.Contains字段与字段Contains字符串),
        /// StartsWith,EndsWith,=,!=,>,>=,小于,小于等于
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="select"></param>
        /// <param name="query"></param>
        /// <param name="orderByClauseList"></param>
        /// <param name="pager"></param>
        /// <param name="unSelect"></param>
        /// <returns></returns>
        private void FillFilter<T>(Expression<Func<T, object>> select = null,
            Expression<Func<T, bool>> query = null,
            List<OrderByClause> orderByClauseList = null, 
            Pager pager = null,
            Expression<Func<T, object>> unSelect = null) where T : class, new()
        {
            dtoDbMapping = DtoMappingHelper.GetDtoMapping<T>();

            #region 处理查询条件            
            if (query != null)
            {
                Expression exp = query.Body as Expression;
                if (exp is BinaryExpression)
                {
                    BinarExpressionProvider(exp);
                }
                else if (exp is MethodCallExpression)
                {   //举例:list.Contains(doctor.Name) 等类型表达式
                    QueryParameter qp = MethodCallExpressionProvider(exp);
                    filter.AddQueryItem(qp);
                }
                else
                {   //暂时封闭其它类型的表达式,待以后完善
                    throw new Exception("不支持的表达式类型:" + exp.ToString());
                    //AtomExpressionRouter(exp);
                }
            }
            #endregion

            #region 处理排序
            if (orderByClauseList != null && orderByClauseList.Count > 0)
            {
                for (int i = 0; i < orderByClauseList.Count; i++)
                {
                    filter.AddOrderBy(orderByClauseList[i].FieldName, orderByClauseList[i].Order);
                }
            }
            #endregion

            #region 处理分页
            if (pager != null)
            {
                filter.InitPage(pager.PageIndex,pager.PageSize);
            }
            #endregion

            #region 处理查询属性
            filter.Select = select;
            filter.UnSelect = unSelect;
            DtoMappingHelper.GetPiMapList<T>(filter);
            #endregion
        }
    }
}
