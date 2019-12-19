using System;
using System.Collections.Generic;
using System.Text;
using System.ComponentModel;
using System.Data;

namespace Jc.Core.Data.Model
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

        private string isPkStr = null;//是否主键 Str
        private string isNullAbleStr = null;//可否为空 Str
        private string fieldLengthStr = null;//字段长度 Str
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
        [Display("字段名称")]
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
        [Display("字段中文名称")]
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
        ///为兼容各种数据库,使用long类型
        /// </summary>
        [Display("是否主键")]
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
        [Display("字段类型")]
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
        [Display("字段长度")]
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
        [Display("可否为空")]
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
        [Display("备注")]
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


        /// <summary>
        ///是否主键 0非主键 1主键
        /// </summary>
        [Display("是否主键")]
        public string IsPkStr
        {
            set
            {
                isPkStr = value;

                isPk = false;
                if (!string.IsNullOrEmpty(isPkStr))
                {
                    int val = 0;
                    if (int.TryParse(isPkStr, out val))
                    {
                        if(val == 1)
                        {
                            isPk = true;
                        }
                    }
                }
            }
        }
        /// <summary>
        ///是否为空 0不能为空 1可以为空
        /// </summary>
        [Display("是否为空")]
        public string IsNullAbleStr
        {
            set
            {
                isNullAbleStr = value;

                isNullAble = false;
                if (!string.IsNullOrEmpty(isNullAbleStr))
                {
                    int val = 0;
                    if (int.TryParse(isNullAbleStr, out val))
                    {
                        if (val == 1)
                        {
                            isNullAble = true;
                        }
                    }
                }
            }
        }
        /// <summary>
        ///字段长度Str
        /// </summary>
        [Display("字段长度Str")]
        public string FieldLengthStr
        {
            set
            {
                fieldLengthStr = value;

                fieldLength = null;
                if (!string.IsNullOrEmpty(fieldLengthStr))
                {
                    int val = 0;
                    if (int.TryParse(fieldLengthStr, out val))
                    {
                        if (val >0)
                        {
                            fieldLength = val;
                        }
                    }
                }
            }
        }
        #endregion

    }
}
