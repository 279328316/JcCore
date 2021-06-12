using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace Jc.Data.Query
{
    /// <summary>
    /// 属性映射对象
    /// 对PropertyInfo的封装
    /// </summary>
    public class PiMap
    {
        private PropertyInfo pi;    //pi属性对象
        private FieldAttribute fieldAttr; //fieldAttr对象
        private bool? isEnum = null;    //属性是否为枚举类型
        private DbType dbType;  //数据字段DbType

        /// <summary>
        /// Ctor
        /// </summary>
        public PiMap()
        {
        }

        /// <summary>
        /// Ctor
        /// </summary>
        public PiMap(PropertyInfo pi, FieldAttribute fieldAttr)
        {
            this.pi = pi;
            this.fieldAttr = fieldAttr;
            this.dbType = DbTypeConvertor.TypeToDbType(pi.PropertyType);
        }

        /// <summary>
        /// 属性名称
        /// </summary>
        public string PiName
        {
            get
            {
                return pi.Name;
            }
        }

        /// <summary>
        /// 是否为枚举类型
        /// 目前暂不支持 字段类型为枚举支持
        /// </summary>
        public bool IsEnum
        {
            get
            {
                if(isEnum == null)
                {
                    isEnum = Pi.PropertyType.GenericTypeArguments.Length > 0 ?
                        Pi.PropertyType.GenericTypeArguments[0].IsEnum
                        : Pi.PropertyType.IsEnum;
                }
                return isEnum.Value;
            }
        }

        /// <summary>
        /// 字段名称
        /// 如果字段名为数据库关键词,需转义
        /// </summary>
        public string FieldName
        {
            get
            {
                return fieldAttr.Name;
            }
        }

        /// <summary>
        /// 是否忽略
        /// </summary>
        public bool IsIgnore
        {
            get
            {
                return fieldAttr.IsIgnore;
            }
        }

        /// <summary>
        /// 属性对象
        /// </summary>
        public PropertyInfo Pi
        {
            get
            {
                return pi;
            }

            set
            {
                pi = value;
            }
        }

        /// <summary>
        /// 属性类型
        /// </summary>
        public Type PropertyType
        {
            get { return pi.PropertyType; }
        }

        /// <summary>
        /// FieldAttr对象
        /// </summary>
        public FieldAttribute FieldAttr
        {
            get
            {
                return fieldAttr;
            }

            set
            {
                fieldAttr = value;
            }
        }                

        /// <summary>
        /// 字段DbType
        /// </summary>
        public DbType DbType
        {
            get { return dbType; }
        }
    }
}
