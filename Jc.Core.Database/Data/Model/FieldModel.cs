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
        private string isPkStr = "";//是否主键Str
        private string fieldType =null;//字段类型
        private int? fieldLength =null;//字段长度
        private string fieldLengthStr = "";//字段长度Str
        private bool isNullable = false;//可否为空
        private string isNullableStr = "";//是否为空Str
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
        ///可否主键Str
        /// </summary>
        public string IsPkStr
        {
            get
            {
                return IsPk.ToString();
            }
            set
            {
                isPkStr = value;
                bool tempIsPk = false;
                int intValue;
                if (int.TryParse(isPkStr, out intValue))
                {
                    tempIsPk = intValue != 0; // 如果intValue不是0，则结果为true
                }
                this.IsPk = tempIsPk;
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
        ///字段长度Str
        /// </summary>
        public string FieldLengthStr
        {
            get
            {
                return fieldLength.HasValue ? fieldLength.Value.ToString() : "0";
            }
            set
            {
                fieldLengthStr = value;
                int tempLength = 0;
                if (int.TryParse(fieldLengthStr, out tempLength))
                {
                    this.fieldLength = tempLength;
                }
            }
        }
        /// <summary>
        ///可否为空
        /// </summary>
        public bool IsNullable
        {
            get
            {
                return isNullable;
            }
            set
            {
                isNullable=value;
            }
        }

        /// <summary>
        ///可否为空Str
        /// </summary>
        public string IsNullableStr
        {
            get
            {
                return isNullable.ToString();
            }
            set
            {
                isNullableStr = value;
                bool tempIsNullable = false;
                int intValue;
                if (int.TryParse(isNullableStr, out intValue))
                {
                    tempIsNullable = intValue != 0; // 如果intValue不是0，则结果为true
                }
                this.isNullable = tempIsNullable;
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
