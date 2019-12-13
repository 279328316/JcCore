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
        private List<DbContext> readDbContexts = null;  //读库DbContext

        /// <summary>
        /// 注册只读数据库
        /// </summary>
        /// <param name="connectString">数据库连接串或数据库名称</param>
        /// <param name="dbType">数据库类型</param>
        /// <returns></returns>
        public DbContext RegisterReadDbContext(string connectString, DatabaseType dbType = DatabaseType.MsSql)
        {
            DbContext dbContext;
            try
            {
                dbContext = new DbContext(connectString, dbType);
            }
            catch (Exception ex)
            {
                string msg = ex.InnerException != null ? ex.InnerException.Message : ex.Message;
                throw new Exception(msg);
            }
            RegisterReadDbContext(dbContext);
            return dbContext;
        }

        /// <summary>
        /// 注册只读数据库
        /// </summary>
        /// <param name="dbContext">只读数据库DbContext</param>
        /// <param name="weight">使用权重</param>
        /// <returns></returns>
        public DbContext RegisterReadDbContext(DbContext dbContext,int weight = 1)
        {
            if(this.ConnectString == dbContext.ConnectString)
            {
                throw new Exception("不能注册自己为当前数据库的只读库.");
            }

            return dbContext;
        }

    }
}
