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
        /// <summary>
        /// 获取事务DbContext
        /// 已自动开启事务
        /// 注意:在MySql,Oracle中,DDL没有事务性，DDL自动提交,不能回滚.
        /// 执行自动建表时,需要注意事务使用方式
        /// </summary>
        /// <returns></returns>
        public TransactionDbContext GetTransactionDbContext(IsolationLevel? level = null)
        {
            TransactionDbContext dbContext = new TransactionDbContext(this.ConnectString, this.DbType, level);
            return dbContext;
        }
    }
}
