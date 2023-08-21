using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace Jc.Database.Query
{
    /// <summary>
    /// 属性映射对象 Pi=>Field
    /// </summary>
    public class FieldMapping
    {
        private PropertyInfo pi;    //pi属性对象
        private TableAttribute tableAttribute; //fieldAttr对象
        private FieldAttribute fieldAttribute; //fieldAttr对象
        private bool isEnum = false;    //属性是否为枚举类型
        private DbType dbType;  //数据字段DbType

        #region Properties

        /// <summary>
        /// 属性名称
        /// </summary>
        public string PiName { get => pi.Name; }

        /// <summary>
        /// 是否为枚举类型
        /// </summary>
        public bool IsEnum { get => isEnum; }

        /// <summary>
        /// 字段名称
        /// 如果字段名为数据库关键词,需转义
        /// </summary>
        public string FieldName { get => fieldAttribute.Name; }

        /// <summary>
        /// 是否忽略
        /// </summary>
        public bool IsIgnore { get => fieldAttribute.IsIgnore; }

        /// <summary>
        /// 属性对象
        /// </summary>
        public PropertyInfo Pi { get => pi; }

        /// <summary>
        /// 属性类型
        /// </summary>
        public Type PropertyType { get => pi.PropertyType; }

        /// <summary>
        /// FieldAttr对象
        /// </summary>
        public FieldAttribute FieldAttribute { get => fieldAttribute; }

        /// <summary>
        /// 字段DbType
        /// </summary>
        public DbType DbType { get => dbType; }
        #endregion

        /// <summary>
        /// Ctor
        /// </summary>
        public FieldMapping(PropertyInfo pi, TableAttribute tableAttribute)
        {
            this.pi = pi;
            this.tableAttribute= tableAttribute;
            this.fieldAttribute = GetFieldAttribute(pi, tableAttribute);
            Type piType = pi.PropertyType;
            this.dbType = DbTypeConvertor.TypeToDbType(piType);
            this.isEnum = piType.GenericTypeArguments.Length > 0 ? piType.GenericTypeArguments[0].IsEnum : piType.IsEnum;
        }

        /// <summary>
        /// 获取FieldAttribute
        /// </summary>
        /// <param name="pi"></param>
        /// <returns></returns>
        private FieldAttribute GetFieldAttribute(PropertyInfo pi, TableAttribute tableAttribute)
        {
            FieldAttribute fieldAttribute = Attribute.GetCustomAttribute(pi, typeof(FieldAttribute)) as FieldAttribute;
            NoMappingAttribute noMappingAttr = Attribute.GetCustomAttribute(pi, typeof(NoMappingAttribute)) as NoMappingAttribute;

            if (fieldAttribute != null)
            {   // 如果列名没有赋值,则将列名定义和属性名一样的值
                if (string.IsNullOrEmpty(fieldAttribute.Name))
                {
                    string fieldName = GetFieldName(pi.Name, fieldAttribute, tableAttribute);
                    fieldAttribute.Name = fieldName;
                }
                fieldAttribute.PiName = pi.Name;
            }
            else
            {   //如果实体没定义Field信息,则自动添加
                string fieldName = GetFieldName(pi.Name, null, tableAttribute);

                fieldAttribute = new FieldAttribute()
                {
                    Name = fieldName,
                    PiName = pi.Name,
                };
            }
            if (!pi.CanRead || !pi.CanWrite)
            {   //如果只读或只写,则该字段IsIgnore属性设置为True
                fieldAttribute.IsIgnore = true;
            }
            if (noMappingAttr != null)
            {   //如果发现NoMapping,则该字段IsIgnore属性设置为True
                fieldAttribute.IsIgnore = true;
            }
            return fieldAttribute;
        }

        /// <summary>
        /// 获取FieldName
        /// </summary>
        /// <param name="obj"></param>
        /// <returns></returns>
        public static string GetFieldName(string piName, FieldAttribute fieldAttribute, TableAttribute tableAttribute)
        {
            string field = piName;

            if (fieldAttribute != null && !string.IsNullOrEmpty(fieldAttribute.UpperSplitChar))
            {
                field = StringHelper.ConvertToLowerString(field, fieldAttribute.UpperSplitChar);
            }
            else if (tableAttribute != null && !string.IsNullOrEmpty(tableAttribute.FieldNameUpperSplitChar))
            {
                field = StringHelper.ConvertToLowerString(field, tableAttribute.FieldNameUpperSplitChar);
            }
            else if (tableAttribute != null && !string.IsNullOrEmpty(tableAttribute.UpperSplitChar))
            {
                field = StringHelper.ConvertToLowerString(field, tableAttribute.UpperSplitChar);
            }
            field = field.ToLower();
            return field;
        }

    }
}
