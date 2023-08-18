using System;
using System.Collections.Generic;
using System.Reflection;
using System.Linq;
using System.Text;
using System.Linq.Expressions;
using System.Configuration;
using System.Collections.Concurrent;

namespace Jc.Database.Query
{
    /// <summary>
    /// Dto实体的解析接口
    /// </summary>
    public static class DtoMappingHelper
    {
        private static object lockForDtoMappingObj = new object();  //字典缓存写入锁

        /// <summary>
        /// 实体类缓存,静态变量是保存为了减少反射次数
        /// object 为DtoMapping
        /// </summary>
        private static ConcurrentDictionary<Type, object> dtoMappingCache = new ConcurrentDictionary<Type, object>();

        private static object lockForPiMappingCacheObj = new object();  //字典缓存写入锁

        static DtoMappingHelper()
        {
        }

        /// <summary>
        /// 获取Dto的DtoDbMapping,获取第一次后会放入一个缓存列表中
        /// </summary>
        public static TableMapping GetDtoMapping<T>()
        {
            Type type = typeof(T);
            if (!dtoMappingCache.ContainsKey(type))
            {
                lock (lockForDtoMappingObj)
                {   //获取写入锁后再次判断
                    if (!dtoMappingCache.ContainsKey(type))
                    {
                        TableMapping mapping = InitDtoMapping<T>();
                        dtoMappingCache.TryAdd(type, mapping);
                    }
                }
            }
            return (TableMapping)dtoMappingCache[type];
        }
        
        /// <summary>
        /// 通过解析获得Dto的对象的参数,Key:为类的属性名
        /// </summary>
        /// <returns>返回Dto参数</returns>
        public static TableMapping InitDtoMapping<T>()
        {
            Type type = typeof(T);
            TableMapping mapping = new TableMapping(type);
            return mapping;
        }

        /// <summary>
        /// 获取表名
        /// </summary>
        private static string GetTableName<T>()
        {
            string tableName = TableAttribute.GetTableName<T>();
            return tableName;
        }

        /// <summary>
        /// 获取表名
        /// </summary>
        public static TableAttribute GetTableAttr<T>()
        {
            Type type = typeof(T);
            TableAttribute attr = type.GetCustomAttribute<TableAttribute>();
            return attr;
        }


        /// <summary>
        /// 根据查询表达式获取查询属性,转换为表字段
        /// 忽略IsIgnore=true的属性
        /// </summary>
        /// <param name="select">查询表达式</param>
        /// <param name="unSelect">排除查询表达式</param>
        /// <returns></returns>
        public static List<FieldMapping> GetPiMapList<T>(Expression select = null, Expression unSelect = null)
        {
            List<FieldMapping> result = new List<FieldMapping>();
            List<string> inPiList = null;
            List<string> exPiList = null;
            TableMapping dtoMapping = GetDtoMapping<T>();
            if (select != null)
            {
                inPiList = ExpressionHelper.GetPiList((Expression<Func<T, object>>)select);
                for (int i = 0; i < inPiList.Count; i++)
                {
                    if (!dtoMapping.FieldMappings.Keys.Contains(inPiList[i]))
                    {
                        continue;
                        //throw new Exception("属性:" + inPiList[i] + "未包含在目标对象" + typeof(T).Name + "中.");
                    }
                    if (dtoMapping.FieldMappings[inPiList[i]].IsIgnore)
                    {   //如果是忽略字段
                        continue;
                    }
                    result.Add(dtoMapping.FieldMappings[inPiList[i]]);
                }
            }
            else if (unSelect != null)
            {
                exPiList = ExpressionHelper.GetPiList((Expression<Func<T, object>>)unSelect);
                foreach (KeyValuePair<string, FieldMapping> piMapItem in dtoMapping.FieldMappings)
                {
                    if (exPiList.Contains(piMapItem.Key))
                    {   //如果包含在排除列表,则跳过该属性
                        continue;
                    }
                    if (piMapItem.Value.IsIgnore)
                    {   //如果是忽略字段
                        continue;
                    }
                    result.Add(piMapItem.Value);
                }
            }
            else
            {   //排除忽略字段
                result = dtoMapping.FieldMappings.Where(piMap => !piMap.Value.IsIgnore).Select(piMap => piMap.Value).ToList();
            }
            return result;
        }

        /// <summary>
        /// 根据查询表达式获取查询属性,转换为表字段
        /// 忽略IsIgnore=true的属性
        /// </summary>
        /// <param name="filter">filter过滤</param>
        /// <returns></returns>
        public static List<FieldMapping> GetPiMapList<T>(QueryFilter filter)
        {
            List<FieldMapping> result = new List<FieldMapping>();
            if (filter != null && filter.PiMapList != null)
            {   //如果Filter已设置过piMapList 不再重新生成.
                result = filter.PiMapList;
            }
            else
            {
                Expression<Func<T, object>> select = null;
                Expression<Func<T, object>> unSelect = null;
                if (filter != null)
                {
                    select = (Expression<Func<T, object>>)filter.Select;
                    unSelect = (Expression<Func<T, object>>)filter.UnSelect;
                }
                result = GetPiMapList<T>(select, unSelect);
                filter.PiMapList = result;  //生成成功后,缓存到filter
            }
            return result;
        }



        #region GetPiMapListWithCatch使用缓存方法
        /// <summary>
        /// PiMapList缓存
        /// 为查询时,获取PiMapList缓存
        /// Key {typeof(T).FullName}-S{select}-Un{unSelect}
        /// </summary>
        private static ConcurrentDictionary<string, List<FieldMapping>> piMappingCache = new ConcurrentDictionary<string, List<FieldMapping>>();

        /// <summary>
        /// 根据查询表达式获取查询属性,转换为表字段
        /// 忽略IsIgnore=true的属性
        /// </summary>
        /// <param name="select">查询表达式</param>
        /// <param name="unSelect">排除查询表达式</param>
        /// <returns></returns>
        public static List<FieldMapping> GetPiMapListWithCache<T>(Expression select = null, Expression unSelect = null)
        {
            string cacheKey = $"{typeof(T).FullName}-S{select}-Un{unSelect}";
            if (piMappingCache.Keys.Contains(cacheKey))
            {
                return piMappingCache[cacheKey];
            }

            List<FieldMapping> result = new List<FieldMapping>();
            List<string> inPiList = null;
            List<string> exPiList = null;
            TableMapping dtoMapping = GetDtoMapping<T>();
            if (select != null)
            {
                inPiList = ExpressionHelper.GetPiList((Expression<Func<T, object>>)select);
                for (int i = 0; i < inPiList.Count; i++)
                {
                    if (!dtoMapping.FieldMappings.Keys.Contains(inPiList[i]))
                    {
                        continue;
                        //throw new Exception("属性:" + inPiList[i] + "未包含在目标对象" + typeof(T).Name + "中.");
                    }
                    if (dtoMapping.FieldMappings[inPiList[i]].IsIgnore)
                    {   //如果是忽略字段
                        continue;
                    }
                    result.Add(dtoMapping.FieldMappings[inPiList[i]]);
                }
            }
            else if (unSelect != null)
            {
                exPiList = ExpressionHelper.GetPiList((Expression<Func<T, object>>)unSelect);
                foreach (KeyValuePair<string, FieldMapping> piMapItem in dtoMapping.FieldMappings)
                {
                    if (exPiList.Contains(piMapItem.Key))
                    {   //如果包含在排除列表,则跳过该属性
                        continue;
                    }
                    if (piMapItem.Value.IsIgnore)
                    {   //如果是忽略字段
                        continue;
                    }
                    result.Add(piMapItem.Value);
                }
            }
            else
            {   //排除忽略字段
                result = dtoMapping.FieldMappings.Where(piMap => !piMap.Value.IsIgnore).Select(piMap => piMap.Value).ToList();
            }

            try
            {
                if (!piMappingCache.Keys.Contains(cacheKey))
                {
                    piMappingCache.TryAdd(cacheKey, result);
                }
            }
            catch (Exception ex)
            {//忽略此处异常
            }
            return result;
        }
        #endregion
    }
}
