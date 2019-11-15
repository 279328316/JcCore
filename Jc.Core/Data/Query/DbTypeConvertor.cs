using System;
using System.Collections.Generic;
using System.Data;
using System.Text;

namespace Jc.Core.Data.Query
{
    /// <summary>
    /// DbTypeConvertor
    /// </summary>
    public static class DbTypeConvertor
    {
        //缓存类型与DbType对应关系
        private static Dictionary<Type, DbType> typeDic = new Dictionary<Type, DbType>();

        // C#数据类型转换为DbType
        /// <summary>
        /// 获取数据类型对应的DbType
        /// </summary>
        /// <param name="type">类型</param>
        /// <returns></returns>
        public static DbType TypeToDbType(Type type)
        {
            DbType dbType = DbType.Object;
            try
            {
                if (typeDic.ContainsKey(type))
                {
                    dbType = typeDic[type];
                }
                else
                {
                    #region 获取DbType,并添加到dic
                    if (type.GenericTypeArguments != null && type.GenericTypeArguments.Length > 0)
                    {
                        Type realType = type.GenericTypeArguments[0];
                        string typeName = realType.Name;
                        if (realType.IsEnum)
                        {
                            typeName = typeof(int).Name;
                        }
                        dbType = (DbType)Enum.Parse(typeof(DbType), typeName);
                    }
                    else
                    {
                        string typeName = type.Name;
                        dbType = (DbType)Enum.Parse(typeof(DbType), typeName);
                    }

                    try
                    {
                        typeDic.Add(type, dbType);
                    }
                    catch
                    {
                    }
                    #endregion
                }
            }
            catch
            {
            }
            return dbType;
        }

        // C#数据类型转换为DbType
        /// <summary>
        /// 获取value对应的dbType
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        public static DbType GetDbType(object value)
        {
            DbType dbType = DbType.Object;
            if (value != null)
            {
                dbType = TypeToDbType(value.GetType());
            }
            return dbType;
        }
    }
}
