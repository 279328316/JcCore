using System;
using System.Collections.Generic;
using System.Text;
using System.ComponentModel;
using System.Data;

namespace Jc.Core.Data.Model
{
    /// <summary>
    /// 数据Model
    /// </summary>
    public class DbModel
    {
        #region Fields
        private string dbName = null;//表名称
        private string dbDes = null;//表说明
        private DatabaseType dbType = DatabaseType.MsSql;   //数据库
        #endregion

        #region Ctor
        /// <summary>
        /// Ctor
        /// </summary>
        public DbModel()
        {
        }
        /// <summary>
        /// Ctor
        ///<param name="dbName">数据库名称</param>
        ///<param name="dbType">数据库类型 默认为MsSql</param>
        ///<param name="dbDes">数据库描述</param>
        /// </summary>
        public DbModel(string dbName, DatabaseType dbType = DatabaseType.MsSql, string dbDes = null)
        {
            this.dbName = dbName;
            this.dbType = dbType;
            this.dbDes = dbDes;
        }
        #endregion 

        #region Properties
        /// <summary>
        ///数据库名称
        /// </summary>
        [Display("数据库名称")]
        public string DbName
        {
            get
            {
                return dbName;
            }
            set
            {
                dbName = value;
            }
        }
        /// <summary>
        ///数据库类型
        /// </summary>
        [Display("数据库类型")]
        public DatabaseType DbType
        {
            get
            {
                return dbType;
            }
            set
            {
                dbType = value;
            }
        }

        /// <summary>
        ///数据库名称
        /// </summary>
        [Display("数据库说明")]
        public string DbDes
        {
            get
            {
                return dbDes;
            }
            set
            {
                dbDes = value;
            }
        }
        #endregion

    }
}
