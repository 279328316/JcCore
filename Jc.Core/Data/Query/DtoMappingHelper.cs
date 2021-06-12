using System;
using System.Collections.Generic;
using System.Reflection;
using System.Linq;
using System.Text;
using System.Linq.Expressions;
using System.Configuration;
using System.Collections.Concurrent;

namespace Jc.Data.Query
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
        public static DtoMapping GetDtoMapping<T>()
        {
            Type t = typeof(T);
            if (!dtoMappingCache.ContainsKey(t))
            {
                lock (lockForDtoMappingObj)
                {   //获取写入锁后再次判断
                    if (!dtoMappingCache.ContainsKey(t))
                    {
                        DtoMapping mapping = new DtoMapping();
                        SetDtoMappingDic<T>(mapping);
                        mapping.EntityType = t;
                        mapping.TableAttr = GetDtoTableAttr<T>();
                        dtoMappingCache.TryAdd(t, mapping);
                    }
                }
            }
            return (DtoMapping)dtoMappingCache[t];
        }
        
        /// <summary>
        /// 通过解析获得Dto的对象的参数,Key:为类的属性名
        /// </summary>
        /// <returns>返回Dto参数</returns>
        public static void SetDtoMappingDic<T>(DtoMapping mapping)
        {
            Dictionary<string, FieldAttribute> fieldDic = new Dictionary<string, FieldAttribute>();
            Dictionary<string, PropertyInfo> piDic = new Dictionary<string, PropertyInfo>();
            Type type = typeof(T);
            PropertyInfo[] piList = type.GetProperties();
            foreach (PropertyInfo pi in piList)
            {
                FieldAttribute attr = GetCustomAttribute<FieldAttribute>(pi);
                NoMappingAttribute noMappingAttr = GetCustomAttribute<NoMappingAttribute>(pi);
                if (attr != null)
                {   //如果列名没有赋值,则将列名定义和属性名一样的值
                    if (string.IsNullOrEmpty(attr.Name))
                    {
                        attr.Name = pi.Name;
                        attr.PiName = pi.Name;
                    }
                }
                else
                {   //如果实体没定义Field信息,则自动添加
                    attr = new FieldAttribute()
                    {
                        Name = pi.Name,
                        PiName = pi.Name,
                    };
                }
                if(!pi.CanRead || !pi.CanWrite)
                {   //如果只读或只写,则该字段IsIgnore属性设置为True
                    attr.IsIgnore = true;
                }
                if (noMappingAttr != null)
                {   //如果发现NoMapping,则该字段IsIgnore属性设置为True
                    attr.IsIgnore = true;
                }
                PiMap piMap = new PiMap(pi, attr);
                mapping.PiMapDic.Add(piMap.PiName, piMap);
            }
        }

        /// <summary>
        /// 获取表名
        /// </summary>
        private static string GetDtoTableName<T>()
        {
            string tableName = "";
            Type type = typeof(T);
            TableAttribute attr = type.GetCustomAttribute<TableAttribute>();
            if (attr == null)
            {   //如果未设置,默认使用类名称
                attr = new TableAttribute();
                attr.Name = type.ToString();
                tableName = attr.Name;
            }
            else
            {   //如果表名没有赋值,则将列名定义和属性名一样的值
                if (string.IsNullOrEmpty(attr.Name))
                {
                    attr.Name = type.Name;
                }
                tableName = attr.Name;
            }
            return tableName;
        }

        /// <summary>
        /// 获取表名
        /// </summary>
        public static TableAttribute GetDtoTableAttr<T>()
        {
            Type type = typeof(T);
            TableAttribute attr = type.GetCustomAttribute<TableAttribute>();
            if (attr == null)
            {   //如果未设置,默认使用类名称
                attr = new TableAttribute();
                attr.Name = type.Name;
                attr.DisplayText = type.Name;
            }
            return attr;
        }



        /// <summary>
        /// 获得指定成员的特性对象
        /// </summary>
        /// <typeparam name="T">要获取属性的类型</typeparam>
        /// <param name="pInfo">属性原型</param>
        /// <returns>返回T对象</returns>
        private static T GetCustomAttribute<T>(PropertyInfo pInfo) where T : Attribute, new()
        {
            Type attributeType = typeof(T);
            Attribute attrObj = Attribute.GetCustomAttribute(pInfo, attributeType);
            T rAttrObj = attrObj as T;
            return rAttrObj;
        }

        /// <summary>
        /// 根据查询表达式获取查询属性,转换为表字段
        /// 忽略IsIgnore=true的属性
        /// </summary>
        /// <param name="select">查询表达式</param>
        /// <param name="unSelect">排除查询表达式</param>
        /// <returns></returns>
        public static List<PiMap> GetPiMapList<T>(Expression select = null, Expression unSelect = null)
        {
            List<PiMap> result = new List<PiMap>();
            List<string> inPiList = null;
            List<string> exPiList = null;
            DtoMapping dtoMapping = GetDtoMapping<T>();
            if (select != null)
            {
                inPiList = ExpressionHelper.GetPiList((Expression<Func<T, object>>)select);
                for (int i = 0; i < inPiList.Count; i++)
                {
                    if (!dtoMapping.PiMapDic.Keys.Contains(inPiList[i]))
                    {
                        continue;
                        //throw new Exception("属性:" + inPiList[i] + "未包含在目标对象" + typeof(T).Name + "中.");
                    }
                    if (dtoMapping.PiMapDic[inPiList[i]].IsIgnore)
                    {   //如果是忽略字段
                        continue;
                    }
                    result.Add(dtoMapping.PiMapDic[inPiList[i]]);
                }
            }
            else if (unSelect != null)
            {
                exPiList = ExpressionHelper.GetPiList((Expression<Func<T, object>>)unSelect);
                foreach (KeyValuePair<string, PiMap> piMapItem in dtoMapping.PiMapDic)
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
                result = dtoMapping.PiMapDic.Where(piMap => !piMap.Value.IsIgnore).Select(piMap => piMap.Value).ToList();
            }
            return result;
        }

        /// <summary>
        /// 根据查询表达式获取查询属性,转换为表字段
        /// 忽略IsIgnore=true的属性
        /// </summary>
        /// <param name="filter">filter过滤</param>
        /// <returns></returns>
        public static List<PiMap> GetPiMapList<T>(QueryFilter filter)
        {
            List<PiMap> result = new List<PiMap>();
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
        private static ConcurrentDictionary<string, List<PiMap>> piMappingCache = new ConcurrentDictionary<string, List<PiMap>>();

        /// <summary>
        /// 根据查询表达式获取查询属性,转换为表字段
        /// 忽略IsIgnore=true的属性
        /// </summary>
        /// <param name="select">查询表达式</param>
        /// <param name="unSelect">排除查询表达式</param>
        /// <returns></returns>
        public static List<PiMap> GetPiMapListWithCatch<T>(Expression select = null, Expression unSelect = null)
        {
            string cacheKey = $"{typeof(T).FullName}-S{select}-Un{unSelect}";
            if (piMappingCache.Keys.Contains(cacheKey))
            {
                return piMappingCache[cacheKey];
            }

            List<PiMap> result = new List<PiMap>();
            List<string> inPiList = null;
            List<string> exPiList = null;
            DtoMapping dtoMapping = GetDtoMapping<T>();
            if (select != null)
            {
                inPiList = ExpressionHelper.GetPiList((Expression<Func<T, object>>)select);
                for (int i = 0; i < inPiList.Count; i++)
                {
                    if (!dtoMapping.PiMapDic.Keys.Contains(inPiList[i]))
                    {
                        continue;
                        //throw new Exception("属性:" + inPiList[i] + "未包含在目标对象" + typeof(T).Name + "中.");
                    }
                    if (dtoMapping.PiMapDic[inPiList[i]].IsIgnore)
                    {   //如果是忽略字段
                        continue;
                    }
                    result.Add(dtoMapping.PiMapDic[inPiList[i]]);
                }
            }
            else if (unSelect != null)
            {
                exPiList = ExpressionHelper.GetPiList((Expression<Func<T, object>>)unSelect);
                foreach (KeyValuePair<string, PiMap> piMapItem in dtoMapping.PiMapDic)
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
                result = dtoMapping.PiMapDic.Where(piMap => !piMap.Value.IsIgnore).Select(piMap => piMap.Value).ToList();
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
