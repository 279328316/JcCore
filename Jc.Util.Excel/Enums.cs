using System;
using System.Collections.Generic;
using System.Text;

namespace Jc.Util.Excel
{
    /// <summary>
    /// 字段类型
    /// </summary>
    public enum FieldType
    {
        /// <summary>
        /// 字符串
        /// </summary>
        String,
        /// <summary>
        /// 日期时间
        /// </summary>
        DateTime,
        /// <summary>
        /// bool 格式使用|分割,如true|false,是|否,√|×,√|,|×
        /// </summary>
        Boolean,
        /// <summary>
        /// 数字
        /// </summary>
        Number,
        /// <summary>
        /// 图像
        /// </summary>
        Image,
        /// <summary>
        /// 对象
        /// </summary>
        Object
    }
}
