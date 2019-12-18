using System;
using System.Collections.Generic;
using System.Text;
using System.ComponentModel;
using System.Data;
using Jc.Core.OrmBase;

namespace Jc.Core.Data.Model
{
    /// <summary>
    /// 数据表Model
    /// </summary>
    public class TableModel:DtoBase
    {
        #region Fields
        private string tableName = null;//表名称
        private TableType tbType = TableType.Table;//表类型
        private string tableTypeStr = null;//是否主键 Str
        private string tableDes = null;//表说明
        #endregion
        #region Ctor
        /// <summary>
        /// Ctor
        /// </summary>
        public TableModel()
        {
        }
        #endregion 

        #region Properties
        /// <summary>
        ///表名称
        /// </summary>
        [DisplayName("表名称")]
        public string TableName
        {
            get
            {
                return tableName;
            }
            set
            {
                tableName = value;
            }
        }
        
        /// <summary>
        /// 表类型
        /// </summary>
        [DisplayName("表类型")]
        public TableType TbType
        {
            get
            {
                return tbType;
            }
            set
            {
                tbType = value;
            }
        }
                
        /// <summary>
        /// 表类型描述
        /// </summary>
        [DisplayName("表类型描述")]
        public string TableTypeStr
        {
            get
            {
                return tableTypeStr;
            }
            set
            {
                tableTypeStr = value;
                TableType tbType = TableType.Table;
                switch (tableTypeStr.ToLower())
                {
                    case "table":
                        tbType = TableType.Table;
                        break;
                    case "view":
                        tbType = TableType.View;
                        break;
                }
                this.tbType = tbType;
            }
        }
        /// <summary>
        ///表名称
        /// </summary>
        [DisplayName("表说明")]
        public string TableDes
        {
            get
            {
                return tableDes;
            }
            set
            {
                tableDes = value;
            }
        }
        #endregion

    }
}
