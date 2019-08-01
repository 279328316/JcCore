using System;
using System.Linq;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Text;
using System.Reflection;

namespace Jc.Core
{
    /// <summary>
    /// Expression 扩展
    /// </summary>
    public static class ExpressionExpand
    {
        /// <summary>
        /// Expression And 
        /// NewExpression 合并
        /// </summary>
        /// <param name="expr"></param>
        /// <returns></returns>
        public static Expression<Func<T, object>> Add<T>(this Expression<Func<T, object>> expr, Expression<Func<T, object>> expandExpr)
        {
            Expression<Func<T, object>> result = null;
            ParameterExpression parameter = Expression.Parameter(typeof(T), "p");
            List<MemberInfo> memberInfoList = new List<MemberInfo>();
            #region 处理原expr
            if (expr.Body is NewExpression)
            {   // t=>new{t.Id,t.Name}
                NewExpression newExp = expr.Body as NewExpression;
                if (newExp.Members != null)
                {
                    memberInfoList = newExp.Members.ToList();
                }
            }
            else if (expr.Body is NewObjectExpression)
            {
                NewObjectExpression newExp = expr.Body as NewObjectExpression;
                if (newExp.Members != null)
                {
                    memberInfoList = newExp.Members.ToList();
                }
            }
            else if (expr.Body is UnaryExpression)
            {   //t=>t.Id
                UnaryExpression unaryExpression = expr.Body as UnaryExpression;
                MemberExpression memberExp = unaryExpression.Operand as MemberExpression;
                memberInfoList.Add(memberExp.Member);
            }
            else if (expr.Body is MemberExpression)
            {   //t=>t.Id T为子类时
                MemberExpression memberExp = expr.Body as MemberExpression;
                memberInfoList.Add(memberExp.Member);
            }
            #endregion

            #region 处理扩展expr
            if (expandExpr.Body is NewExpression)
            {   // t=>new{t.Id,t.Name}
                NewExpression newExp = expandExpr.Body as NewExpression;
                for (int i = 0; i < newExp.Members.Count; i++)
                {
                    MemberExpression memberExp = Expression.Property(parameter, newExp.Members[i].Name);
                    if (!memberInfoList.Any(member => member.Name == newExp.Members[i].Name))
                    {
                        memberInfoList.Add(newExp.Members[i]);
                    }
                }
            }
            else if (expr.Body is NewObjectExpression)
            {
                NewObjectExpression newExp = expr.Body as NewObjectExpression;
                if (newExp.Members != null && newExp.Members.Count > 0)
                {
                    for (int i = 0; i < newExp.Members.Count; i++)
                    {
                        MemberExpression memberExp = Expression.Property(parameter, newExp.Members[i].Name);
                        if (!memberInfoList.Any(member => member.Name == newExp.Members[i].Name))
                        {
                            memberInfoList.Add(newExp.Members[i]);
                        }
                    }
                }
            }
            else if (expandExpr.Body is UnaryExpression)
            {   //t=>t.Id
                UnaryExpression unaryExpression = expandExpr.Body as UnaryExpression;
                MemberExpression memberExp = unaryExpression.Operand as MemberExpression;
                if (!memberInfoList.Any(exp => exp.Name == memberExp.Member.Name))
                {
                    memberInfoList.Add(memberExp.Member);
                }
            }
            #endregion
            NewObjectExpression newObjExpression = new NewObjectExpression(typeof(object).GetConstructors()[0], null, memberInfoList);
            result = Expression.Lambda<Func<T, object>>(newObjExpression, parameter);
            return result;
        }

        /// <summary>
        /// Expression Remove 
        /// NewExpression 合并
        /// </summary>
        /// <param name="expr"></param>
        /// <returns></returns>
        public static Expression<Func<T, object>> Remove<T>(this Expression<Func<T, object>> expr, Expression<Func<T, object>> expandExpr)
        {
            Expression<Func<T, object>> result = null;
            ParameterExpression parameter = Expression.Parameter(typeof(T), "p");
            List<MemberInfo> memberInfoList = new List<MemberInfo>();
            List<MemberInfo> removeMemberInfoList = new List<MemberInfo>();
            #region 处理原expr
            if (expr.Body is NewExpression)
            {   // t=>new{t.Id,t.Name}
                NewExpression newExp = expr.Body as NewExpression;
                if (newExp.Members != null)
                {
                    memberInfoList = newExp.Members.ToList();
                }
            }
            else if (expr.Body is NewObjectExpression)
            {
                NewObjectExpression newExp = expr.Body as NewObjectExpression;
                if (newExp.Members != null)
                {
                    memberInfoList = newExp.Members.ToList();
                }
            }
            else if (expr.Body is UnaryExpression)
            {   //t=>t.Id
                UnaryExpression unaryExpression = expr.Body as UnaryExpression;
                MemberExpression memberExp = unaryExpression.Operand as MemberExpression;
                memberInfoList.Add(memberExp.Member);
            }
            #endregion

            #region 处理扩展expr
            if (expandExpr.Body is NewExpression)
            {   // t=>new{t.Id,t.Name}
                NewExpression newExp = expandExpr.Body as NewExpression;
                for (int i = 0; i < newExp.Members.Count; i++)
                {
                    MemberExpression memberExp = Expression.Property(parameter, newExp.Members[i].Name);
                    if (!removeMemberInfoList.Any(member => member.Name == newExp.Members[i].Name))
                    {
                        removeMemberInfoList.Add(newExp.Members[i]);
                    }
                }
            }
            else if (expr.Body is NewObjectExpression)
            {
                NewObjectExpression newExp = expr.Body as NewObjectExpression;
                if (newExp.Members != null && newExp.Members.Count > 0)
                {
                    for (int i = 0; i < newExp.Members.Count; i++)
                    {
                        MemberExpression memberExp = Expression.Property(parameter, newExp.Members[i].Name);
                        if (!removeMemberInfoList.Any(member => member.Name == newExp.Members[i].Name))
                        {
                            removeMemberInfoList.Add(newExp.Members[i]);
                        }
                    }
                }
            }
            else if (expandExpr.Body is UnaryExpression)
            {   //t=>t.Id
                UnaryExpression unaryExpression = expandExpr.Body as UnaryExpression;
                MemberExpression memberExp = unaryExpression.Operand as MemberExpression;
                if (!memberInfoList.Any(exp => exp.Name == memberExp.Member.Name))
                {
                    removeMemberInfoList.Add(memberExp.Member);
                }
            }
            #endregion

            for (int i = memberInfoList.Count - 1; i >= 0; i--)
            {
                if (removeMemberInfoList.Any(member => member.Name == memberInfoList[i].Name))
                {
                    memberInfoList.Remove(memberInfoList[i]);
                }
            }
            if (memberInfoList.Count <= 0)
            {
                throw new System.Exception("Expression Remove Error.All Properties are removed.");
            }
            NewObjectExpression newObjExpression = new NewObjectExpression(typeof(object).GetConstructors()[0], null, memberInfoList);
            result = Expression.Lambda<Func<T, object>>(newObjExpression, parameter);
            return result;
        }

        /// <summary>
        /// Expression And
        /// </summary>
        /// <param name="expr"></param>
        /// <returns></returns>
        public static Expression<Func<T, bool>> And<T>(this Expression<Func<T, bool>> expr, Expression<Func<T, bool>> expandExpr)
        {
            Expression<Func<T, bool>> result = Expression.Lambda<Func<T, bool>>(Expression.And(expandExpr.Body, expr.Body), expr.Parameters);
            return result;
        }

        /// <summary>
        /// Expression And
        /// </summary>
        /// <param name="expr"></param>
        /// <returns></returns>
        public static Expression<Func<T, bool>> Or<T>(this Expression<Func<T, bool>> expr, Expression<Func<T, bool>> expandExpr)
        {
            Expression<Func<T, bool>> result = Expression.Lambda<Func<T, bool>>(Expression.Or(expandExpr.Body, expr.Body), expr.Parameters);
            return result;
        }
    }
}
