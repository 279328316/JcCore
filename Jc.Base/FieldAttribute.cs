using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Reflection;
using System.Data;
using System.Linq.Expressions;

namespace Jc
{
    /// <summary>
    /// FieldAttribute
    /// FieldName字段默认与属性值一致
    /// </summary>
    public class FieldAttribute : Attribute
    {

        /// <summary>
        /// Ctor
        /// </summary>
        public FieldAttribute()
        {
            this.DisplayText = "";
            this.Name = "";
            this.PiName = "";
            this.IsPk = false;
            this.Required = false;
            this.ReadOnly = false;
            this.IsIgnore = false;
        }

        /// <summary>
        /// Ctor
        /// </summary>
        public FieldAttribute(string name,string displayText = null)
        {
            this.Name = name;
            this.PiName = name;
            this.DisplayText = string.IsNullOrEmpty(displayText) ? name : displayText;
            this.IsPk = false;
            this.Required = false;
            this.ReadOnly = false;
            this.IsIgnore = false;
        }
        #region properties

        /// <summary>
        /// 展示名称
        /// </summary>
        public string DisplayText { get; set; }

        /// <summary>
        /// 字段名称(列名称)
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// 属性名称
        /// </summary>
        public string PiName { get; set; }
        
        /// <summary>
        /// 是否主键
        /// </summary>
        public bool IsPk { get; set; }

        /// <summary>
        /// 是否只读
        /// </summary>
        public bool ReadOnly { get; set; }

        /// <summary>
        /// 是否Required
        /// </summary>
        public bool Required { get; set; }

        /// <summary>
        /// 字段类型
        /// </summary>
        public string FieldType { get; set; }

        /// <summary>
        /// 字段长度
        /// 如果为字段类型为char,nchar,varchar,nvarchar
        /// 建表需要Length.未指定则为max
        /// </summary>
        public int Length { get; set; }

        /// <summary>
        /// 是否忽略该字段
        /// 如果没有定义该字段FieldAttribute,默认忽略该字段
        /// </summary>
        public bool IsIgnore { get; set; }
        #endregion

        #region Methods

        /// <summary>
        /// 获取展示文字
        /// </summary>
        /// <param name="obj"></param>
        /// <returns></returns>
        public static string GetDisplayText(object obj)
        {
            Type type = obj.GetType();
            object attribute = type.GetCustomAttributes(typeof(FieldAttribute), false).FirstOrDefault();
            if (attribute == null)
            {
                return null;
            }
            return ((FieldAttribute)attribute).DisplayText;
        }
        #endregion
    }

    /// <summary>
    /// FieldAttribute
    /// FieldName字段默认与属性值一致
    /// </summary>
    public class PkFieldAttribute : FieldAttribute
    {
        public PkFieldAttribute()
        {
            IsPk = true;
        }
    }

    /// <summary>
    /// NoMappingAttribute
    /// FieldName字段默认与属性值一致
    /// </summary>

    [AttributeUsage(AttributeTargets.Class | AttributeTargets.Property | AttributeTargets.Field, AllowMultiple = false)]
    public class NoMappingAttribute : Attribute
    {
        /// <summary>
        /// Ctor
        /// </summary>
        public NoMappingAttribute()
        {
        }
    }
}
