using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;

namespace Jc.Database.Query
{
    /// <summary>
    /// 字段映射
    /// </summary>
    public class FieldMap
    {
        /// <summary>
        /// 字段名称
        /// </summary>
        public string FieldName { get; set; }
        
        /// <summary>
        /// 字段类型
        /// </summary>
        public Type FieldType { get; set; }
        
        /// <summary>
        /// 对应DbType类型
        /// </summary>
        public DbType FieldDbType { get; set; }
        
        /// <summary>
        /// 数据列名称
        /// </summary>
        public string DbColumnName { get; set; }
        
        /// <summary>
        /// 数据字段类型
        /// </summary>
        public string DbTypeName { get; set; }
        
        /// <summary>
        /// 数据字段长度
        /// </summary>
        public int DbFieldLength { get; set; }
        
        /// <summary>
        /// 是否主键
        /// </summary>
        public bool IsPk { get; set; }
        
        /// <summary>
        /// 是否可为空
        /// </summary>
        public bool IsNullAble { get; set; }
        
        /// <summary>
        /// 是否只读(自增主键,RowTs)
        /// </summary>
        public bool IsReadOnly { get; set; }
    }
}
