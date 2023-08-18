using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Reflection;

namespace Jc
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

        /// <summary>
        /// Ctor
        /// </summary>
        public TableAttribute(string name)
        {
            this.Name = name;
        }

        #region Properties
        /// <summary>
        /// 展示名称
        /// </summary>
        public string DisplayText { get; set; }

        /// <summary>
        /// 表名称
        /// 1.如果设置了表名称中存在变量{0},则表名称可变.在使用时,请传入subTableArg参数
        ///   如Name为Data{0}.subTableArg参数为2018.则表名称为Data2018
        /// 3.如果未设置Name,将使用传入subTableArg参数作为表名称.
        /// 4.可变表一般为分表情况下使用.设置表AutoCreate属性为true.在插入数据时会自动创建表.
        /// 5.如果表名称为空或未设置填充参数{0},则直接使用传入参数subTableArg作为表名
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// 是否自动创建
        /// 设置表AutoCreate属性为true.在插入数据时会,先检查表是否存在.如不存在则自动创建.
        /// 请为自动创建表,各属性添加FieldAttribute属性.
        /// </summary>
        public bool AutoCreate { get; set; }

        /// <summary>
        /// 主键字段 属性名称
        /// </summary>
        public string PkField { get; set; }

        /// <summary>
        /// 表名和字段名 大写字母分割字符 默认为空
        /// </summary>
        public string UpperSplitChar { get; set; }

        /// <summary>
        /// 表名 大写字母分割字符 默认为空
        /// </summary>
        public string TableNameUpperSplitChar { get; set; }

        /// <summary>
        /// 字段名 大写字母分割字符 默认为空
        /// </summary>
        public string FieldNameUpperSplitChar { get; set; }
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
            if (obj == null)
            {
                return null;
            }
            Type type = obj.GetType();
            return GetTableName(type);
        }

        /// <summary>
        /// 获取表名称
        /// </summary>
        /// <param name="obj"></param>
        /// <returns></returns>
        public static string GetTableName<T>()
        {
            Type type = typeof(T);
            return GetTableName(type);
        }

        /// <summary>
        /// 获取表名称
        /// </summary>
        /// <param name="obj"></param>
        /// <returns></returns>
        public static string GetTableName(Type type)
        {
            TableAttribute tableAttribute = type.GetCustomAttributes(typeof(TableAttribute), false).FirstOrDefault() as TableAttribute;
            if (tableAttribute == null)
            {
                return type.Name.ToLower();
            }
            string tempName = null;
            if (string.IsNullOrEmpty(tableAttribute.Name))
            {
                tempName = type.Name;
                if (!string.IsNullOrEmpty(tableAttribute.TableNameUpperSplitChar))
                {
                    tempName = ConvertToLowerString(tempName, tableAttribute.TableNameUpperSplitChar);
                }
                else if (!string.IsNullOrEmpty(tableAttribute.UpperSplitChar))
                {
                    tempName = ConvertToLowerString(tempName, tableAttribute.UpperSplitChar);
                }
                else
                {
                    tempName = tempName.ToLower();
                }
            }
            else
            {
                tempName = tableAttribute.Name;
            }
            return tempName;
        }

        /// <summary>
        /// 转换为小写字符串 遇到大写字母,转换为小写,并使用分隔符连接,
        /// </summary>
        /// <param name="str"></param>
        /// <param name="upperSplitChar"></param>
        /// <returns></returns>
        private static string ConvertToLowerString(string str, string upperSplitChar)
        {
            if (string.IsNullOrEmpty(str))
            {
                return str;
            }
            string result = str;
            if (!string.IsNullOrEmpty(upperSplitChar))
            {
                StringBuilder stringBuilder = new StringBuilder();
                for (int i = 0; i < str.Length; i++)
                {
                    char curChar = str[i];
                    if (i == 0)
                    {
                        if (char.IsUpper(curChar))
                        {
                            curChar = char.ToLower(curChar);
                        }
                        stringBuilder.Append(curChar);
                    }
                    else
                    {
                        if (char.IsUpper(curChar))
                        {
                            curChar = char.ToLower(curChar);
                        }
                        stringBuilder.Append(upperSplitChar);
                        stringBuilder.Append(curChar);
                    }
                }
                result = stringBuilder.ToString();
            }
            else
            {
                result = str.ToLower();
            }
            return str;
        }
        #endregion
    }
}
