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
using Jc.Data.Query;
namespace Jc
{
    /// <summary>
    /// DbContext
    /// DbContext数据操作访问
    /// </summary>
    public partial class DbContext
    {
        //分表参数列表 对象类型,分表参数
        internal List<KeyValuePair<Type, string>> subTableArgList = null;

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
                KeyValuePair<Type, string> subTableKv = subTableArgList.FirstOrDefault(kv => kv.Key == typeof(T));
                if (subTableKv.Key != null)
                {
                    subTableArg = subTableKv.Value;
                }
            }
            return subTableArg;
        }
        
        /// <summary>
        /// 设置分表dbContext
        /// 需要为操作对象设置可变表名称
        /// 如TableAttr的Name为Data{0}.tablePfx参数为2000.则表名称为Data2000
        /// </summary>
        /// <typeparam name="T">操作对象类型</typeparam>
        /// <param name="subTableArg">分表参数</param>
        /// <returns>返回subTableDbContext.只能用于指定分表操作.</returns>
        public SubTableDbContext GetSubTableDbContext<T>(object subTableArg)
        {
            SubTableDbContext subTableDbContext = new SubTableDbContext(this.ConnectString, this.DbType);
            subTableDbContext.AddSubTableArg<T>(subTableArg);
            return subTableDbContext;
        }

        /// <summary>
        /// 设置分表dbContext
        /// 需要为操作对象设置可变表名称
        /// 如TableAttr的Name为Data{0}.tablePfx参数为2000.则表名称为Data2000
        /// </summary>
        /// <typeparam name="T">操作对象类型</typeparam>
        /// <param name="subTableArg">分表参数</param>
        /// <returns>返回subTableDbContext.只能用于指定分表操作.</returns>
        public virtual SubTableDbContext SubTable<T>(object subTableArg)
        {
            SubTableDbContext subTableDbContext = null;
            if (this is SubTableDbContext)
            {
                subTableDbContext = (SubTableDbContext)this;
                subTableDbContext.AddSubTableArg<T>(subTableArg);
            }
            else
            {
                subTableDbContext = new SubTableDbContext(this.ConnectString, this.DbType);
                subTableDbContext.AddSubTableArg<T>(subTableArg);
            }
            return subTableDbContext;
        }
        #endregion 
    }
}
