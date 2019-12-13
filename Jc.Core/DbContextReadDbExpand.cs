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
using Jc.Core.Data;

namespace Jc.Core
{
    /// <summary>
    /// DbContext
    /// DbContext数据操作访问
    /// </summary>
    public partial class DbContext
    {
        /// <summary>
        /// 注册只读数据库
        /// </summary>
        /// <param name="connectString">数据库连接串或数据库名称</param>
        /// <param name="dbType">数据库类型</param>
        /// <returns></returns>
        public void RegisterReadDb(string connectString, DatabaseType dbType = DatabaseType.MsSql)
        {
            try
            {
                if (readDbProviders == null)
                {
                    readDbProviders = new List<DbProvider>();
                }
                if (dbType != this.DbType)
                {
                    throw new Exception("只读库类型必须与写入数据库一致.");
                }
                DbProvider dbProvider = DbProviderHelper.GetDbProvider(connectString, dbType);
                if (readDbProviders.Any(a=>a == dbProvider))
                {
                    throw new Exception("只读库已存在.");
                }
                readDbProviders.Add(dbProvider);
            }
            catch (Exception ex)
            {
                string msg = ex.InnerException != null ? ex.InnerException.Message : ex.Message;
                throw new Exception(msg);
            }
        }
    }
}
