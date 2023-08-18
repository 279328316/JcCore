using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Reflection;
using System.ComponentModel;
using System.Xml;
using System.Data;
using System.Runtime.Serialization;

using System.Linq.Expressions;
using System.Data.Common;
using Jc.Database;
using Jc.Database.Query;


namespace Jc.Database
{
    /// <summary>
    /// 分表操作DbContext
    /// </summary>
    public class SubTableDbContext : DbContext
    {
        /// <summary>
        /// Ctor
        /// </summary>
        /// <param name="connectString"></param>
        /// <param name="dbType"></param>
        public SubTableDbContext(string connectString, DatabaseType dbType = DatabaseType.MsSql) : base(connectString, dbType)
        {
            subTableArgList = new List<KeyValuePair<Type, string>>();
        }

        #region SubTable Args Methods
        /// <summary>
        /// 添加分表参数.返回当前SubTableDbContext
        /// </summary>
        /// <typeparam name="T">操作对象类型</typeparam>
        /// <param name="subTableArg">分表参数</param>
        /// <returns>返回当前SubTableDbContext</returns>
        public SubTableDbContext AddSubTableArg<T>(object subTableArg)
        {
            if (subTableArgList.Any(kv => kv.Key == typeof(T)))
            {
                throw new Exception($"不能为类型{typeof(T).Name}重复添加分表参数");
            }
            TableMapping dtoDbMapping = DtoMappingHelper.GetDtoMapping<T>();
            string primaryTableName = dtoDbMapping.PrimaryTableName;
            if (string.IsNullOrEmpty(primaryTableName) || !primaryTableName.Contains("{0}"))
            {
                throw new Exception("必须为分表对象" + typeof(T).Name + "指定包含{0}参数的TableName属性");
            }
            if (subTableArg == null)
            {
                throw new Exception("分表参数不能为空");
            }
            this.subTableArgList.Add(new KeyValuePair<Type, string>(typeof(T), subTableArg.ToString()));
            return this;
        }

        /// <summary>
        /// 查看当前分表参数List
        /// </summary>
        public List<KeyValuePair<Type, string>> GetSubTableArgs()
        {
            if (subTableArgList == null)
            {
                throw new Exception("非SubTableDbContext,不能进行此操作");
            }
            List<KeyValuePair<Type, string>> list = new List<KeyValuePair<Type, string>>();
            for (int i = 0; i < subTableArgList.Count; i++)
            {
                Type type = subTableArgList[0].Key;
                string tableName = "";
                TableAttribute attr = type.GetCustomAttribute<TableAttribute>();
                if (attr != null)
                {
                    tableName = string.Format(attr.Name, subTableArgList[0].Value);
                }
                else
                {
                    tableName = subTableArgList[0].Value;
                }
                list.Add(new KeyValuePair<Type, string>(type, tableName));
            }
            return list;
        }

        /// <summary>
        /// 移除分表参数
        /// </summary>
        /// <typeparam name="T">操作对象类型</typeparam>
        public void RemoveSubTableArg<T>()
        {
            KeyValuePair<Type, string> kvObj = subTableArgList.FirstOrDefault(a => a.Key == typeof(T));
            if (kvObj.Key == null)
            {
                throw new Exception($"不存在类型{typeof(T).Name}的分表参数");
            }
            this.subTableArgList.Remove(kvObj);
        }

        /// <summary>
        /// 移除分表参数
        /// </summary>
        public void ClearSubTableArg()
        {
            this.subTableArgList = new List<KeyValuePair<Type, string>>();
        }
        #endregion
    }
}
