using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace Jc.Core.Data.Query
{
    /// <summary>
    /// 属性映射对象
    /// 对PropertyInfo的封装
    /// </summary>
    public class PiMap
    {
        private PropertyInfo pi;    //pi属性对象
        private FieldAttribute fieldAttr; //fieldAttr对象

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
    }
}
