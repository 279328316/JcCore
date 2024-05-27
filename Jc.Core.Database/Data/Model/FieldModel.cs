using System;
using System.Collections.Generic;
using System.Text;
using System.ComponentModel;
using System.Data;

namespace Jc.Database.Data
{
    /// <summary>
    /// 数据表字段Model
    /// </summary>
    public class FieldModel
    {
        #region Fields
        private string fieldName =null;//字段名称
        private string fieldChName =null;//字段中文名称
        private bool isPk = false;//是否主键
        private string fieldType =null;//字段类型
        private int? fieldLength =null;//字段长度
        private bool isNullAble = false;//可否为空
        private string note =null;//备注
        #endregion

        #region Ctor
        /// <summary>
        /// Ctor
        /// </summary>
        public FieldModel()
        {
        }
        #endregion 

        #region Properties
        /// <summary>
        ///字段名称
        /// </summary>
        public string FieldName
        {
            get
            {
                return fieldName;
            }
            set
            {
                fieldName=value;
            }
        }
        /// <summary>
        ///字段中文名称
        /// </summary>
        public string FieldChName
        {
            get
            {
                return fieldChName;
            }
            set
            {
                fieldChName=value;
            }
        }
        /// <summary>
        ///是否主键 1 主键 0 非主键
        /// </summary>
        public bool IsPk
        {
            get
            {
                return isPk;
            }
            set
            {
                isPk=value;
            }
        }
        /// <summary>
        ///字段类型
        /// </summary>
        public string FieldType
        {
            get
            {
                return fieldType;
            }
            set
            {
                fieldType=value;
            }
        }
        /// <summary>
        ///字段长度
        /// </summary>
        public int? FieldLength
        {
            get
            {
                return fieldLength;
            }
            set
            {
                fieldLength=value;
            }
        }
        /// <summary>
        ///可否为空
        /// </summary>
        public bool IsNullAble
        {
            get
            {
                return isNullAble;
            }
            set
            {
                isNullAble=value;
            }
        }
        /// <summary>
        ///备注
        /// </summary>
        public string Note
        {
            get
            {
                return note;
            }
            set
            {
                note=value;
            }
        }
        #endregion

    }
}
