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
        /// <summary>
        /// 获取事务DbContext
        /// 已自动开启事务
        /// </summary>
        /// <returns></returns>
        public DbTransContext GetTransDbContext()
        {
            DbTransContext dbContext = new DbTransContext(this.connectString, this.dbType, this.subTableArgList);
            return dbContext;
        }
    }
}
