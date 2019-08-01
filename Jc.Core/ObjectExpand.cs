using System;
using System.Linq;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Reflection;
using System.Collections;

namespace Jc.Core
{
    /// <summary>
    /// Object 扩展
    /// </summary>
    public static class ObjectHelper
    {

        #region ICloneable 成员

        /// <summary>
        /// 复制值到目标对象(浅复制)
        /// </summary>
        /// <param name="source">目标来源</param>
        /// <param name="dest">目标对象</param>
        public static void CopyTo(this object source, object dest)
        {
            if (dest == null)
            {
                return;
            }
            List<PropertyInfo> sourcePiList = source.GetType().GetProperties(BindingFlags.Public | BindingFlags.Instance)
                .Where(p => p.CanRead).ToList(); //来源目标 可读属性
            List<PropertyInfo> destPiList = dest.GetType().GetProperties(BindingFlags.Public | BindingFlags.Instance)
                .Where(p => p.CanWrite).ToList();//写入模板 可写属性

            foreach (PropertyInfo pi in sourcePiList)
            {
                PropertyInfo destPi = destPiList.Where(p => p.Name == pi.Name).FirstOrDefault();
                if (destPi == null)
                {
                    continue;
                }
                object piValue = pi.GetValue(source, null);
                if (pi.PropertyType == destPi.PropertyType)
                {   //属性类型相同 直接赋值 int==int int?==int?
                    destPi.SetValue(dest, piValue, null);
                }
                else if (destPi.PropertyType.GenericTypeArguments?.Length > 0
                        && destPi.PropertyType.GenericTypeArguments[0] == pi.PropertyType)
                {   //int?=int
                    destPi.SetValue(dest, piValue, null);
                }
                else if (pi.PropertyType.GenericTypeArguments?.Length > 0
                        && pi.PropertyType.GenericTypeArguments[0] == destPi.PropertyType)
                {   //int = int?
                    if (piValue != null)
                    {
                        destPi.SetValue(dest, piValue, null);
                    }
                }
            }
        }

        /// <summary>
        /// 复制值到目标对象(浅复制)
        /// </summary>
        /// <param name="source">目标来源</param>
        /// <param name="dest">目标对象</param>
        /// <param name="includeExpr">包含属性</param>
        /// <param name="excludeExpr">排除属性</param>
        public static void CopyTo<T>(this object source, T dest, Expression<Func<T, object>> includeExpr, Expression<Func<T, object>> excludeExpr = null)
        {
            if (dest == null)
            {
                return;
            }
            List<PropertyInfo> sourcePiList = source.GetType().GetProperties(BindingFlags.Public | BindingFlags.Instance)
                .Where(p => p.CanRead).ToList(); //来源目标 可读属性
            List<PropertyInfo> destPiList = dest.GetType().GetProperties(BindingFlags.Public | BindingFlags.Instance)
                .Where(p => p.CanWrite).ToList();//写入模板 可写属性

            List<string> includeList = null;
            List<string> excludeList = null;
            if (excludeExpr != null)
            {   //转换为小写对比
                excludeList = ExpressionHelper.GetPiList(excludeExpr);
            }
            if (includeExpr != null)
            {
                includeList = ExpressionHelper.GetPiList(includeExpr);
            }
            foreach (PropertyInfo pi in sourcePiList)
            {
                if (excludeList != null && excludeList.Contains(pi.Name))
                {
                    continue;
                }
                if (includeList != null && !includeList.Contains(pi.Name))
                {
                    continue;
                }
                PropertyInfo destPi = destPiList.Where(p => p.Name == pi.Name).FirstOrDefault();
                if (destPi == null)
                {
                    continue;
                }
                object piValue = pi.GetValue(source, null);
                if (pi.PropertyType == destPi.PropertyType)
                {   //属性类型相同 直接赋值 int==int int?==int?
                    destPi.SetValue(dest, piValue, null);
                }
                else if (destPi.PropertyType.GenericTypeArguments?.Length>0
                        && destPi.PropertyType.GenericTypeArguments[0]== pi.PropertyType)
                {   //int?=int
                    destPi.SetValue(dest, piValue, null);
                }
                else if (pi.PropertyType.GenericTypeArguments?.Length > 0
                        && pi.PropertyType.GenericTypeArguments[0] == destPi.PropertyType)
                {   //int = int?
                    if (piValue != null)
                    {
                        destPi.SetValue(dest, piValue, null);
                    }
                }
            }
        }

        /// <summary>
        /// Map创建新对象(浅复制)
        /// </summary>
        /// <param name="source"></param>
        public static T MapTo<T>(this object source) where T :class,new()
        {
            T dest = new T();
            if(source.GetType().IsValueType==false)
            {
                source.CopyTo(dest);
            }
            return dest;
        }

        /// <summary>
        /// Map创建新对象(浅复制)
        /// </summary>
        /// <param name="source">来源</param>
        /// <param name="includeExpr">包含属性列表</param>
        /// <param name="excludeExpr">排除属性列表</param>
        public static T MapTo<T>(this object source,Expression<Func<T, object>> includeExpr, Expression<Func<T, object>> excludeExpr = null) where T : class, new()
        {
            T dest = new T();
            source.CopyTo<T>(dest, includeExpr, excludeExpr);
            return dest;
        }

        #endregion
    }
}
