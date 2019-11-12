using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Reflection;

namespace Jc.Core
{
    /// <summary>
    /// TableAttribute
    /// </summary>
    public class TableAttribute : Attribute
    {
        /// <summary>
        /// Ctor
        /// </summary>
        public TableAttribute()
        {
        }

        #region Properties
        /// <summary>
        /// 展示名称
        /// </summary>
        public string DisplayText { get; set; }

        /// <summary>
        /// 表名称
        /// 1.如果设置了表名称中存在变量{0},则表名称可变.在使用时,请传入tableNamePfx参数
        ///   如Name为Data{0}.tableNamePfx参数为2018.则表名称为Data2018
        /// 3.如果未设置Name,将使用传入tableNamePfx参数作为表名称.
        /// 4.可变表一般为分表情况下使用.设置表AutoCreate属性为true.在插入数据时会自动创建表.
        /// 5.如果表名称为空或未设置填充参数{0},则直接使用传入参数tableNamePfx作为表名
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// 是否自动创建
        /// 设置表AutoCreate属性为true.在插入数据时会,先检查表是否存在.如不存在则自动创建.
        /// 请为自动创建表,各属性添加FieldAttribute属性.
        /// </summary>
        public bool AutoCreate { get; set; }

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
            object attribute = type.GetCustomAttributes(typeof(TableAttribute), false).FirstOrDefault();
            if (attribute == null)
            {
                return null;
            }
            return ((TableAttribute)attribute).DisplayText;
        }

        /// <summary>
        /// 获取表名称
        /// </summary>
        /// <param name="obj"></param>
        /// <returns></returns>
        public static string GetTableName(object obj)
        {
            Type type = obj.GetType();
            object attribute = type.GetCustomAttributes(typeof(TableAttribute), false).FirstOrDefault();
            if (attribute == null)
            {
                return null;
            }
            return ((TableAttribute)attribute).Name;
        }

        #endregion
    }
}
