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
using Jc.Core.Helper;
using System.Data.Common;
using Jc.Core.Data.Query;

namespace Jc.Core
{
    /// <summary>
    /// DbContext
    /// DbContext数据操作访问
    /// </summary>
    public partial class DbContext
    {
        //分表参数列表 对象类型,分表参数
        internal List<KeyValueObj<Type, string>> subTableArgList = null;

        #region Methods

        /// <summary>
        /// 获取分表表名参数
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <returns></returns>
        private string GetSubTableArg<T>()
        {
            string subTableArg = null;
            if (this.subTableArgList != null)
            {
                KeyValueObj<Type, string> subTableKv = subTableArgList.FirstOrDefault(kv => kv.Key == typeof(T));
                if (subTableKv != null)
                {
                    subTableArg = subTableKv.Value;
                }
            }
            return subTableArg;
        }
        
        /// <summary>
        /// 设置分表dbContext
        /// 需要为操作对象设置可变表名称.
        /// 如TableAttr的Name为Data{0}.tablePfx参数为2018.则表名称为Data2018
        /// </summary>
        /// <returns>返回subTableDbContext.只能用于指定分表操作.</returns>
        public DbContext GetSubTableDbContext()
        {
            DbContext subTableDbContext = new DbContext(this.ConnectString, this.DbType);
            subTableDbContext.subTableArgList = new List<KeyValueObj<Type, string>>();
            return subTableDbContext;
        }

        /// <summary>
        /// 添加分表参数.返回当前SubTableDbContext
        /// </summary>
        /// <typeparam name="T">操作对象类型</typeparam>
        /// <param name="subTableArg">分表参数</param>
        /// <returns>返回当前SubTableDbContext</returns>
        public DbContext AddSubTableArg<T>(object subTableArg)
        {
            if (subTableArgList == null)
            {
                throw new Exception("不为非SubTableDbContext添加分表参数");
            }
            if (subTableArgList.Any(kv => kv.Key == typeof(T)))
            {
                throw new Exception($"不能为类型{typeof(T).Name}重复添加分表参数");
            }
            DtoMapping dtoDbMapping = DtoMappingHelper.GetDtoMapping<T>();
            string tableName = dtoDbMapping.TableAttr.Name;
            if (string.IsNullOrEmpty(tableName) || !tableName.Contains("{0}"))
            {
                throw new Exception("必须为分表对象" + typeof(T).Name + "指定包含{0}参数的TableName属性");
            }
            if (subTableArg == null)
            {
                throw new Exception("分表参数不能为空");
            }
            this.subTableArgList.Add(new KeyValueObj<Type, string>(typeof(T), subTableArg.ToString()));
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
            if (subTableArgList == null)
            {
                throw new Exception("非SubTableDbContext,不能进行此操作");
            }
            KeyValueObj<Type, string> kvObj = subTableArgList.FirstOrDefault(a => a.Key == typeof(T));
            if (kvObj == null)
            {
                throw new Exception($"不存在类型{typeof(T).Name}的分表参数");
            }
            this.subTableArgList.Remove(kvObj);
        }

        #endregion 
    }
}
