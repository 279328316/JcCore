using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using System.Text;

namespace Jc.Database.Query
{
    /// <summary>
    /// Lambda转QueryFilter
    /// </summary>
    public class QueryFilterBuilder
    {
        EntityMapping dtoDbMapping = null;

        private QueryFilter filter = new QueryFilter();

        private QueryFilterBuilder()
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
        private Operand GetOperand(ExpressionType expType, bool isFieldOp = false)
        {
            Operand operand = Operand.Equal;
            switch (expType)
            {
                case ExpressionType.Equal:
                    operand = isFieldOp ? Operand.FieldEqual : Operand.Equal;
                    break;
                case ExpressionType.GreaterThan:
                    operand = isFieldOp ? Operand.FieldGreaterThan : Operand.GreaterThan;
                    break;
                case ExpressionType.GreaterThanOrEqual:
                    operand = isFieldOp ? Operand.FieldGreaterThanOrEqual : Operand.GreaterThanOrEqual;
                    break;
                case ExpressionType.LessThan:
                    operand = isFieldOp ? Operand.FieldLessThan : Operand.LessThan;
                    break;
                case ExpressionType.LessThanOrEqual:
                    operand = isFieldOp ? Operand.FieldLessThanOrEqual : Operand.LessThanOrEqual;
                    break;
                case ExpressionType.NotEqual:
                    operand = isFieldOp ? Operand.FieldNotEqual : Operand.NotEqual;
                    break;
                default:
                    throw new Exception($"不支持的单表达式操作{expType}");
            }
            return operand;
        }

        /// <summary>
        /// 单一表达式 参数间操作获取
        /// </summary>
        /// <param name="expType"></param>
        /// <returns></returns>
        private bool IsExpressionConjectionOperand(ExpressionType expType)
        {
            bool result = false;
            switch (expType)
            {
                case ExpressionType.And:
                case ExpressionType.AndAlso:
                case ExpressionType.Or:
                case ExpressionType.OrElse:
                    result = true;
                    break;
            }
            return result;
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
        /// <param name="expression"></param>
        /// <returns></returns>
        private void BinaryExpressionProvider(Expression expression)
        {
            BinaryExpression binaryExpression = expression as BinaryExpression;
            Expression left = binaryExpression.Left;
            Expression right = binaryExpression.Right;
            ExpressionType expType = binaryExpression.NodeType;

            if (!(left is BinaryExpression || left is MemberExpression
                || left is ConstantExpression || left is MethodCallExpression
                 || left is UnaryExpression))
            {
                throw new Exception("不支持的表达式类型:" + binaryExpression.ToString());
            }

            if (!IsExpressionConjectionOperand(expType))
            {   //如果为非表达式连接操作.作为单表达式处理
                object param = null;
                object paramValue = null;

                bool leftIsField = IsFieldExpression(left);

                bool rightIsField = IsFieldExpression(right);

                if (leftIsField)
                {
                    param = AtomExpressionRouter(left);
                    paramValue = AtomExpressionRouter(right);
                }
                else if (!leftIsField && rightIsField)
                {
                    paramValue = AtomExpressionRouter(left);
                    param = AtomExpressionRouter(right);

                    //取消对表达式在右侧,不规范写法支持
                    //throw new Exception("Unsupported non-standard expressions");
                }
                else if (!leftIsField && !rightIsField)
                {
                    throw new Exception("Unsupported non-standard expressions");
                }
                bool isFieldOp = false;   // 取消对与字段比较支持
                if (leftIsField && rightIsField)
                {
                    isFieldOp = true;
                }
                Operand operand = GetOperand(expType, isFieldOp);
                if (paramValue == null)
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
                filter.AddQueryItem(param?.ToString(), Conjuction.And, operand, paramValue);
            }
            else
            {   //多个表达式情况
                filter.AddLeftParenthesis();
                HandleFilterExpression(left);
                filter.AddLogicOperator(GetConjuction(expType));    //表达式连接 ↑左侧表达式处理  ↓右侧表达式处理
                HandleFilterExpression(right);
                filter.AddRightParenthesis();
            }
        }

        /// <summary>
        /// 判断是否为Field Expression
        /// </summary>
        /// <param name="exp"></param>
        /// <returns></returns>
        private bool IsFieldExpressionOrgin(Expression exp)
        {
            if (exp is null)
            {
                return false;
            }
            bool result = false;
            result = (exp is MemberExpression
                    && ((MemberExpression)exp).Expression != null
                    && !((MemberExpression)exp).Expression.ToString().StartsWith("value"))
                    || (exp is MethodCallExpression
                    && ((MethodCallExpression)exp).Object is MemberExpression
                    && ((MemberExpression)((MethodCallExpression)exp).Object).Expression != null
                    && !((MemberExpression)((MethodCallExpression)exp).Object).Expression.ToString().StartsWith("value"))
                    || (exp is UnaryExpression
                    && ((UnaryExpression)exp).Operand is MemberExpression
                    && ((MemberExpression)((UnaryExpression)exp).Operand).Expression != null
                    && !((MemberExpression)((UnaryExpression)exp).Operand).Expression.ToString().StartsWith("value"));
            return result;
        }

        /// <summary>
        /// 判断是否为Field Expression
        /// </summary>
        /// <param name="exp"></param>
        /// <returns></returns>
        private bool IsFieldExpression(Expression exp)
        {
            if (exp is null)
            {
                return false;
            }
            bool result = false;
            if (exp is MemberExpression)   //表示访问字段或属性
            {
                MemberExpression me = exp as MemberExpression;
                result = me.Expression != null && !me.Expression.ToString().StartsWith("value");
            }
            else if (exp is UnaryExpression) //表示创建一个新数组，并可能初始化该新数组的元素
            {   // a.Id = int? exp="Convert(a.Id, Nullable`1)"  Convert,Not
                UnaryExpression unaryExp = exp as UnaryExpression;
                if (unaryExp.Operand is MemberExpression)
                {
                    MemberExpression me = unaryExp.Operand as MemberExpression;
                    result = me.Expression != null && !me.Expression.ToString().StartsWith("value");
                }
            }
            else if (exp is MethodCallExpression)   //介绍 lambda 表达式。 它捕获一个类似于 .NET 方法主体的代码块
            {   // a.UserName.ToLower()
                MethodCallExpression callExpression = exp as MethodCallExpression;
                if (callExpression.Object is MemberExpression)
                {
                    MemberExpression me = callExpression.Object as MemberExpression;
                    result = me.Expression != null && !me.Expression.ToString().StartsWith("value");
                }
            }
            return result;
        }

        /// <summary>
        /// 处理FilterExpression 添加查询条件
        /// </summary>
        /// <param name="exp"></param>
        private void HandleFilterExpression(Expression exp)
        {
            if (exp is BinaryExpression)
            {
                BinaryExpressionProvider(exp);
            }
            else if (exp is UnaryExpression)
            {   //举例:!list.Contains(doctor.Name) !a.IsDeleted !a.IsDeleted.Value 等类型表达式
                #region UnaryExpression NodeType = Not
                bool isNotOperand = exp.NodeType == ExpressionType.Not;
                if (exp.NodeType == ExpressionType.Not)
                {
                    UnaryExpression unary = exp as UnaryExpression;
                    if (unary.Operand is MethodCallExpression)
                    {   // 举例: !list.Contains(doctor.Name)
                        QueryParameter qp = MethodCallExpressionProvider(unary.Operand, isNotOperand);
                        filter.AddQueryItem(qp);
                    }
                    else if (unary.Operand is MemberExpression)
                    {   //!a.IsDeleted !a.IsDeleted.Value
                        object fieldName = MemberExpressionProvider(unary.Operand);
                        QueryParameter qp = new QueryParameter()
                        {
                            FieldName = fieldName?.ToString(),
                            Op = isNotOperand ? Operand.NotEqual : Operand.Equal,
                            ParameterValue = true,
                            ParameterDbType = DbType.Boolean
                        };
                        filter.AddQueryItem(qp);
                    }
                }
                else
                {
                    throw new Exception("不支持的表达式类型:" + exp.ToString());
                }
                #endregion
            }
            else if (exp is MethodCallExpression)
            {
                QueryParameter qp = MethodCallExpressionProvider(exp);
                filter.AddQueryItem(qp);
            }
            else if (exp is MemberExpression)
            {
                object fieldName = MemberExpressionProvider(exp);
                QueryParameter qp = new QueryParameter()
                {
                    FieldName = fieldName?.ToString(),
                    Op = Operand.Equal,
                    ParameterValue = true,
                    ParameterDbType = DbType.Boolean
                };
                filter.AddQueryItem(qp);
            }
            else
            {   //string leftStr = AtomExpressionRouter(left)?.ToString();
                throw new Exception("不支持的表达式类型:" + exp.ToString());
            }
        }

        #region Expression Value Calc Provider 计算表达式值

        /// <summary>
        /// 具有常数值的表达式
        /// </summary>
        /// <param name="exp"></param>
        /// <returns></returns>
        private object ConstantExpressionProvider(Expression exp)
        {
            ConstantExpression ce = exp as ConstantExpression;
            object result = ce.Value;
            return result;
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
            if (!(exp is MemberExpression))
            {
                throw new Exception($"表达式{exp}不是MemberExpression类型.");
            }
            object result = null;
            MemberExpression me = exp as MemberExpression;
            string memberName = me.Member.Name;
            string expStr = exp.ToString();
            if (expStr.StartsWith("value"))
            {   //引用其它对象属性类型 如 list.contais(student.Name) , a.HospitalId == hospital.Id, a.TaskState == task.TaskState
                // 处理student.Name,hospital.Id,task.TaskState
                object expResult = Expression.Lambda(exp).Compile().DynamicInvoke();
                if (me.Type.IsEnum)
                {   // 查询条件中的枚举类型特殊处理,转换为int
                    result = (int)expResult;
                }
                else
                {
                    result = expResult;
                }
            }
            else
            {
                if (memberName == "Value")
                {   //NullAble属性  levels.Contains(a.ApiLevel.Value)
                    if (me.Expression != null)
                    {
                        result = MemberExpressionProvider(me.Expression);
                    }
                }
                else
                {
                    if (me.Expression != null && !me.Expression.Type.IsValueType)
                    {
                        if (me.Expression.Type == dtoDbMapping.EntityType && dtoDbMapping.FieldMappings.ContainsKey(memberName))
                        {
                            result = dtoDbMapping.FieldMappings[memberName].FieldName;
                        }
                        else
                        {
                            result = Expression.Lambda(exp).Compile().DynamicInvoke();
                        }
                    }
                    else
                    {   // me.Expression == null DateTime.Now Guid.Empty
                        // me.Expression.Type.IsValueType 值类型
                        result = Expression.Lambda(exp).Compile().DynamicInvoke();
                    }
                }
            }
            return result;
        }

        /// <summary>
        /// 对静态方法或实例方法的调用
        /// </summary>
        /// <param name="exp"></param>
        /// <param name="isNotOprand"></param>
        /// <returns></returns>
        private QueryParameter MethodCallExpressionProvider(Expression exp, bool isNotOprand = false)
        {
            QueryParameter qp = null;
            object name = null;
            object value = null;
            DbType parameterDbType = DbType.Object;
            Operand op = Operand.Equal;
            MethodCallExpression mce = exp as MethodCallExpression;

            switch (mce.Method.Name)
            {
                case "Equals":
                    if (IsFieldExpression(mce.Object))
                    {
                        name = AtomExpressionRouter(mce.Object);
                        value = AtomExpressionRouter(mce.Arguments[0]);
                    }
                    else
                    {
                        name = AtomExpressionRouter(mce.Arguments[0]);
                        value = AtomExpressionRouter(mce.Object);
                    }
                    op = isNotOprand ? Operand.NotEqual : Operand.Equal;
                    parameterDbType = DbTypeConvertor.GetDbType(value);
                    break;
                case "Contains":
                    if (mce.Object != null && mce.Arguments?.Count == 1)
                    {   // 来决定采用mce.Object或mce.Arguments[0]作为字段
                        if (IsFieldExpression(mce.Object))
                        {   // a.UserName.ToLower().Contains("ABC".ToLower()) or a.UserName.ToLower().Contains("ABC")
                            // or a.UserName.ToLower().Contains(queryObj.UserName)
                            name = AtomExpressionRouter(mce.Object);
                            value = AtomExpressionRouter(mce.Arguments[0]);
                            op = isNotOprand ? Operand.NotLike : Operand.Like;
                            parameterDbType = DbTypeConvertor.GetDbType(value);
                        }
                        else
                        {   //permIds.Contains(a.Id) or userNames.Contains(a.UserName.Tolower()) or queryObj.Ids.Contains(a.Id)
                            name = AtomExpressionRouter(mce.Arguments[0]);
                            value = AtomExpressionRouter(mce.Object);
                            parameterDbType = DbTypeConvertor.TypeToDbType(mce.Arguments[0].Type);
                            op = isNotOprand ? Operand.NotIn : Operand.In;
                        }
                    }
                    else if (mce.Arguments.Count >= 2)
                    {   //int与int?类型 list:List<int?>
                        name = AtomExpressionRouter(mce.Arguments[1]);
                        value = AtomExpressionRouter(mce.Arguments[0]);
                        parameterDbType = DbTypeConvertor.TypeToDbType(mce.Arguments[1].Type);
                        op = isNotOprand ? Operand.NotIn : Operand.In;
                    }
                    else
                    {
                        throw new Exception($"不支持的方法:{mce.ToString()}");
                    }
                    break;
                case "StartsWith":
                    name = AtomExpressionRouter(mce.Object);
                    value = AtomExpressionRouter(mce.Arguments[0]);
                    op = Operand.LeftLike;
                    parameterDbType = DbTypeConvertor.GetDbType(value);
                    break;
                case "EndsWith":
                    name = AtomExpressionRouter(mce.Object);
                    value = AtomExpressionRouter(mce.Arguments[0]);
                    op = Operand.RightLike;
                    parameterDbType = DbTypeConvertor.GetDbType(value);
                    break;
                default:
                    throw new Exception("不支持的方法:" + mce.Method.Name);
            }
            qp = new QueryParameter()
            {
                FieldName = name?.ToString(),
                Op = op,
                ParameterValue = value,
                ParameterDbType = parameterDbType
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
                MethodCallExpression callExpression = exp as MethodCallExpression;
                if (callExpression.Object is MemberExpression)
                {
                    string expStr = exp.ToString();
                    if (expStr.StartsWith("value"))
                    {   //引用其它对象属性类型 如 list.contais(student.Name) , a.HospitalId == hospital.Id
                        result = Expression.Lambda(exp).Compile().DynamicInvoke();
                    }
                    else
                    {   // 字段情况处理
                        MemberExpression me = callExpression.Object as MemberExpression;
                        object meResult = AtomExpressionRouter(me);
                        switch (callExpression.Method.Name)
                        {
                            case "ToLower":
                                result = $"lower({meResult})";
                                break;
                            case "ToUpper":
                                result = $"upper({meResult})";
                                break;
                            default:
                                if (me.Expression != null && !me.Expression.Type.IsValueType)
                                {
                                    throw new Exception($"Unsuport Method {callExpression.Method.Name} for expression {callExpression}");
                                }
                                else
                                {   // me.Expression == null DateTime.Now Guid.Empty
                                    // me.Expression.Type.IsValueType 值类型
                                    result = Expression.Lambda(exp).Compile().DynamicInvoke();
                                }
                                break;
                        }
                    }
                }
                else if (callExpression.Object is ConstantExpression)
                {
                    result = Expression.Lambda(exp).Compile().DynamicInvoke();
                }
                else
                {
                    try
                    {   //尝试直接运算Lambda表达式
                        result = Expression.Lambda(exp).Compile().DynamicInvoke();
                    }
                    catch (Exception ex)
                    {
                        throw;
                    }
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
                    throw;
                }
            }
            return result;
        }

        #endregion

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
        /// <param name="select">查询属性</param>
        /// <param name="orderByClauseList">排序条件</param>
        /// <param name="unSelect">排除查询属性</param>
        /// <returns></returns>
        public static QueryFilter GetFilter<T>(
            Expression<Func<T, bool>> query = null,
            Expression<Func<T, object>> select = null,
            List<OrderByClause> orderByClauseList = null,
            Expression<Func<T, object>> unSelect = null) where T : class, new()
        {
            QueryFilterBuilder filterHelper = new QueryFilterBuilder();
            filterHelper.FillFilter(query, select, orderByClauseList, null, unSelect);
            return filterHelper.filter;
        }

        /// <summary>
        /// lambda表达式转换sql
        /// 支持的方法:
        /// Contains(支持List.Contains字段与字段Contains字符串),
        /// StartsWith,EndsWith,=,!=,>,>=,小于,小于等于
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="query">查询条件</param>
        /// <param name="select">查询属性</param>
        /// <param name="orderByClauseList">排序条件</param>
        /// <param name="pager">分页条件</param>
        /// <param name="unSelect">排除查询属性</param>
        /// <returns></returns>
        public static QueryFilter GetPageFilter<T>(
            Expression<Func<T, bool>> query = null,
            Expression<Func<T, object>> select = null,
            List<OrderByClause> orderByClauseList = null,
            Pager pager = null,
            Expression<Func<T, object>> unSelect = null) where T : class, new()
        {
            if (pager == null)
            {
                //pager = new Pager(1, 30);
                throw new Exception("Paging query must specify pagination information");
            }
            QueryFilterBuilder filterHelper = new QueryFilterBuilder();
            filterHelper.FillFilter(query, select, orderByClauseList, pager, unSelect);
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
        private void FillFilter<T>(
            Expression<Func<T, bool>> query = null,
            Expression<Func<T, object>> select = null,
            List<OrderByClause> orderByClauseList = null,
            Pager pager = null,
            Expression<Func<T, object>> unSelect = null) where T : class, new()
        {
            dtoDbMapping = EntityMappingHelper.GetMapping<T>();

            #region 处理查询条件
            if (query != null)
            {
                Expression exp = query.Body as Expression;
                HandleFilterExpression(exp);
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
                filter.InitPage(pager.PageIndex, pager.PageSize);
            }
            #endregion

            #region 处理查询属性
            filter.Select = select;
            filter.UnSelect = unSelect;
            #endregion
        }
    }
}