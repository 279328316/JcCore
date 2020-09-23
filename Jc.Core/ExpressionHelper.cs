using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Reflection;
using System.CodeDom;
using System.CodeDom.Compiler;
using Microsoft.CSharp;
using System.IO;
using System.Linq.Expressions;
using System.Collections;
using Jc.Core.Data;

namespace Jc.Core
{
    /// <summary>
    /// 表达式处理Helper
    /// </summary>
    public class ExpressionHelper
    {
        /// <summary>
        /// 通过Lambed Expression获取属性名称
        /// </summary>
        /// <param name="expr">查询表达式</param>
        /// <returns></returns>
        public static List<string> GetPiList<T>(Expression<Func<T, object>> expr)
        {
            List<string> result = new List<string>();
            if (expr.Body is NewExpression)
            {   // t=>new{t.Id,t.Name}
                NewExpression nexp = expr.Body as NewExpression;
                if (nexp.Members != null)
                {
                    result = nexp.Members.Select(member => member.Name).ToList();
                }
            }
            else if (expr.Body is NewObjectExpression)
            {   // t=>new{t.Id,t.Name}
                NewObjectExpression nexp = expr.Body as NewObjectExpression;
                if (nexp.Members != null)
                {
                    result = nexp.Members.Select(member => member.Name).ToList();
                }
            }
            else if (expr.Body is UnaryExpression)
            {   //t=>t.Id
                UnaryExpression uexp = expr.Body as UnaryExpression;
                MemberExpression mexp = uexp.Operand as MemberExpression;
                result.Add(mexp.Member.Name);
            }
            else if (expr.Body is MemberExpression)
            {   //t=>t.Id T为子类时
                MemberExpression mexp = expr.Body as MemberExpression;
                result.Add(mexp.Member.Name);
            }
            else
            {
                throw new System.Exception("不支持的Select lambda写法");
            }
            return result;
        }

        /// <summary>
        /// 转换Expr
        /// 在外面调用时可以使用var以减少代码长度
        /// </summary>
        /// <param name="expr"></param>
        /// <returns></returns>
        public static Expression<Func<T, object>> CreateExpression<T>(Expression<Func<T, object>> expr)
        {
            return expr;
        }

               
        /// <summary>
        /// 创建Lambda表达式
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="operand">运算操作符</param>
        /// <param name="piName">属性名称</param>
        /// <param name="value">值</param>
        /// <returns></returns>
        public static Expression<Func<T, bool>> CreateLambdaExpression<T>(Operand operand,string piName,object value)
        {
            //构建Lambda表达式
            ParameterExpression parameter = Expression.Parameter(typeof(T), "p");
            Expression lambdaExp;
            //表达式左侧 like: p.Name
            MemberExpression left = Expression.PropertyOrField(parameter, piName);
            
            //表达式右侧，(1)值， like '张三' (2)List List.Contains(a.Status);
            Expression right;
            if (value is List<string>)
            {   //(1)构造List<int?> 类型 (2)创建list (3)(list as IList).Add(objValue);
                //(4)创建lambdaExp 使用Contains方法
                Type leftListType = Type.GetType(string.Format($"System.Collections.Generic.List`1[[{left.Type.AssemblyQualifiedName}]]"));
                List<string> valueStrList = value as List<string>;
                var list = Activator.CreateInstance(leftListType);
                for (int i = 0; i < valueStrList.Count; i++)
                {
                    if (string.IsNullOrEmpty(valueStrList[i]))
                    {
                        continue;
                    }
                    Type leftType = left.Type;
                    if (leftType.GenericTypeArguments != null && leftType.GenericTypeArguments.Length > 0)
                    {
                        leftType = leftType.GenericTypeArguments[0];
                    }
                    if (leftType == typeof(string))
                    {   // 如果为string类型.不需要转换
                        (list as IList).Add(valueStrList[i]);
                    }
                    else if(leftType.IsEnum)
                    {   //枚举类型
                        var objValue = Enum.Parse(leftType, valueStrList[i]);
                        (list as IList).Add(objValue);
                    }
                    else
                    {   //如果非string类型.调用Parse方法.
                        MethodInfo miParse = leftType.GetMethod("Parse", new Type[] { typeof(string) });
                        var objValue = miParse.Invoke(null, new object[] { valueStrList[i] });
                        (list as IList).Add(objValue);
                    }
                }
                right = Expression.Constant(list);
                List<MethodInfo> methodList = leftListType.GetMethods().ToList();
                MethodInfo method = methodList.Where(m => m.Name == "Contains").FirstOrDefault(); 
                lambdaExp = Expression.Call(right, method, left);
            }
            else
            {
                Type valueType = value.GetType();
                if (valueType == left.Type)
                {
                    right = Expression.Constant(value);
                }
                else
                {
                    object objValue;
                    if (valueType == typeof(string))
                    {   //如果传入值为字符串类型,需要进行类型转换 调用Parse
                        Type leftType = left.Type;
                        if (leftType.GenericTypeArguments != null && leftType.GenericTypeArguments.Length > 0)
                        {
                            leftType = leftType.GenericTypeArguments[0];
                        }
                        MethodInfo miParse = leftType.GetMethod("Parse", new Type[] { typeof(string) });
                        objValue = miParse.Invoke(null, new object[] { value });
                    }
                    else
                    {
                        objValue = value;
                    }

                    if (objValue.GetType() == left.Type)
                    {   //如果 转换后ObjValue类型与需要类型一致
                        right = Expression.Constant(objValue);
                    }
                    else
                    {   //如果不一致,进行类型转换 调用Convert方法
                        right = Expression.Convert(Expression.Constant(objValue), left.Type);
                    }
                }
                //比较表达式
                switch (operand)
                {
                    case Operand.GreaterThan:
                        lambdaExp = Expression.GreaterThan(left, right);
                        break;
                    case Operand.GreaterThanOrEqual:
                        lambdaExp = Expression.GreaterThanOrEqual(left, right);
                        break;
                    case Operand.LessThan:
                        lambdaExp = Expression.LessThan(left, right);
                        break;
                    case Operand.LessThanOrEqual:
                        lambdaExp = Expression.LessThanOrEqual(left, right);
                        break;
                    case Operand.LeftLike:
                        //like 查询，需要调用外部int或string的Contains方法
                        List<MethodInfo> methodList1 = typeof(string).GetMethods().ToList();
                        MethodInfo method1 = methodList1.Where(m => m.Name == "StartsWith").FirstOrDefault();
                        lambdaExp = Expression.Call(left, method1, right);
                        break;
                    case Operand.RightLike:
                        //like 查询，需要调用外部int或string的Contains方法
                        List<MethodInfo> methodList2 = typeof(string).GetMethods().ToList();
                        MethodInfo method2 = methodList2.Where(m => m.Name == "EndsWith").FirstOrDefault();
                        lambdaExp = Expression.Call(left, method2, right);
                        break;
                    case Operand.Like:
                        //like
                        List<MethodInfo> methodList3 = typeof(string).GetMethods().ToList();
                        MethodInfo method3 = methodList3.Where(m => m.Name == "Contains").FirstOrDefault();
                        lambdaExp = Expression.Call(left, method3, right);
                        break;
                    case Operand.Contains:
                        //Contains
                        List<MethodInfo> methodList4 = typeof(IEnumerable).GetMethods().ToList();
                        MethodInfo method4 = methodList4.Where(m => m.Name == "Contains").FirstOrDefault();
                        lambdaExp = Expression.Call(left, method4, right);
                        break;
                    case Operand.Equal:
                        lambdaExp = Expression.Equal(left, right);
                        break;
                    case Operand.NotEqual:
                        lambdaExp = Expression.NotEqual(left, right);
                        break;
                    default:
                        throw new System.Exception("暂不支持的Operand方式" + operand.ToString());
                        //break;
                }
            }
            Expression<Func<T, bool>> lambda = Expression.Lambda<Func<T, bool>>(lambdaExp, parameter);
            return lambda;
        }

        #region Other Methods

        /// <summary>
        /// 获取ValueSetter
        /// 用于通过表达式树方式设置对象属性值
        /// 但测试结果与普通方式差不多.暂时不用
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="pi"></param>
        /// <returns></returns>
        private static Action<T, object> GetValueSetter<T>(PropertyInfo pi)
        {
            Action<T, object> result = null;
            Type type = typeof(T);
            //创建 对实体 属性赋值的expression
            ParameterExpression parameter = Expression.Parameter(type, "t");
            ParameterExpression value = Expression.Parameter(typeof(object), "propertyValue");
            MethodInfo setter = type.GetMethod("set_" + pi.Name);
            MethodCallExpression call = Expression.Call(parameter, setter, Expression.Convert(value, pi.PropertyType));
            var lambda = Expression.Lambda<Action<T, object>>(call, parameter, value);
            result = lambda.Compile();
            return result;
        }

        /// <summary>
        /// 获取ValueReader
        /// 用于通过表达式树方式获取对象属性值
        /// 但测试结果与普通方式差不多.暂时不用
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="pi"></param>
        /// <returns></returns>
        private static LambdaExpression GetValueReader<T>(PropertyInfo pi)
        {
            //创建 对实体 属性读取的expression
            Type type = typeof(T);
            ParameterExpression parameter = Expression.Parameter(type, "t");
            MemberExpression exp = Expression.Property(parameter, pi.DeclaringType, pi.Name);
            LambdaExpression lambda = Expression.Lambda(exp, parameter);
            return lambda;
        }
        #endregion
    }
}
