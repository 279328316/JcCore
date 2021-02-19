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
        public TransactionDbContext GetTransactionDbContext(IsolationLevel? level = null)
        {
            TransactionDbContext dbContext = new TransactionDbContext(this.ConnectString, this.DbType, level);
            return dbContext;
        }
    }
}
