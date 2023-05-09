
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace Jc
{
    /// <summary>
    /// Enum操作Helper
    /// </summary>
    public class EnumHelper
    {
        /// <summary>
        /// 获取EnumModel对象,包含Enumitems
        /// </summary>
        /// <param name="enumType">枚举类型</param>
        /// <param name="sortExpr">排序字段</param>
        /// <param name="order">排序方向</param>
        /// <returns></returns>
        public static EnumModel GetEnumModel<T>(Expression<Func<EnumItem, object>> sortExpr = null, Sorting order = Sorting.Asc)
        {
            return GetEnumModel(typeof(T), null, sortExpr, order);
        }

        /// <summary>
        /// 获取EnumModel对象,包含Enumitems
        /// </summary>
        /// <param name="defaultItemName">添加默认ItemName 设置为null时,不添加默认项</param>
        /// <param name="sortExpr">排序字段</param>
        /// <param name="order">排序方向</param>
        /// <returns></returns>
        public static EnumModel GetEnumModel<T>(string defaultItemName, Expression<Func<EnumItem, object>> sortExpr = null, Sorting order = Sorting.Asc)
        {
            return GetEnumModel(typeof(T), defaultItemName, sortExpr, order);
        }

        /// <summary>
        /// 获取EnumModel对象,包含Enumitems
        /// </summary>
        /// <param name="enumType">枚举类型 typeof(enum)</param>
        /// <param name="addNullItem">添加NullItem</param>
        /// <returns>EnumModel</returns>
        public static EnumModel GetEnumModel(Type enumType,Expression<Func<EnumItem, object>> sortExpr = null, Sorting order = Sorting.Asc)
        {
            return GetEnumModel(enumType, null, sortExpr, order);
        }

        /// <summary>
        /// 获取EnumModel对象,包含Enumitems
        /// </summary>
        /// <param name="enumType">枚举类型 typeof(enum)</param>
        /// <param name="addNullItem">添加NullItem</param>
        /// <returns>EnumModel</returns>
        public static EnumModel GetEnumModel(Type enumType, string defaultItemName, Expression<Func<EnumItem, object>> sortExpr = null, Sorting order = Sorting.Asc)
        {
            string enumTypeName = enumType.Name;
            string enumName = enumTypeName.Substring(enumTypeName.LastIndexOf(".") + 1);
            EnumModel enumModel = new EnumModel()
            {
                Name = enumName,
                DisplayName = GetDisplayName(enumType),
                EnumItems = GetEnumItems(enumType, defaultItemName, sortExpr, order)
            };
            return enumModel;
        }

        /// <summary>
        /// 获取显示名称
        /// </summary>
        /// <param name="obj"></param>
        /// <returns></returns>
        public static string GetDisplayName(Enum obj)
        {
            if (obj != null)
            {
                FieldInfo fieldInfo = obj.GetType().GetFields().FirstOrDefault(f => f.Name.Equals(obj.ToString()));
                if (fieldInfo == null)
                {
                    return obj.ToString();
                }
                else
                {
                    return GetDisplayName(fieldInfo);
                }
            }
            else
            {
                return null;
            }
        }

        /// <summary>
        /// 获取EnumItems
        /// 默认排序字段为DisplayName,排序方向为升序
        /// </summary>
        /// <param name="enumType">枚举类型</param>
        /// <param name="sortExpr">排序字段</param>
        /// <param name="order">排序方向</param>
        /// <returns></returns>
        public static List<EnumItem> GetEnumItems<T>(Expression<Func<EnumItem, object>> sortExpr = null, Sorting order = Sorting.Asc)
        {
            return GetEnumItems(typeof(T), null, sortExpr, order);
        }

        /// <summary>
        /// 获取EnumItems
        /// 默认项默认不添加 默认项 Value为null,为列表第0项,不参与排序
        /// 默认排序字段为DisplayName,排序方向为升序
        /// </summary>
        /// <param name="defaultItemName">添加默认ItemName 设置为null时,不添加默认项</param>
        /// <param name="sortExpr">排序字段</param>
        /// <param name="order">排序方向</param>
        /// <returns></returns>
        public static List<EnumItem> GetEnumItems<T>(string defaultItemName, Expression<Func<EnumItem, object>> sortExpr = null, Sorting order = Sorting.Asc)
        {
            return GetEnumItems(typeof(T), defaultItemName, sortExpr, order);
        }

        /// <summary>
        /// 获取EnumItems
        /// 默认排序字段为DisplayName,排序方向为升序
        /// </summary>
        /// <param name="enumType">枚举类型</param>
        /// <param name="sortExpr">排序字段</param>
        /// <param name="order">排序方向</param>
        /// <returns></returns>
        public static List<EnumItem> GetEnumItems(Type enumType, Expression<Func<EnumItem, object>> sortExpr = null, Sorting order = Sorting.Asc)
        {
            return GetEnumItems(enumType, null, sortExpr, order);
        }

        /// <summary>
        /// 获取EnumItems
        /// 默认项默认不添加 默认项 Value为null,为列表第0项,不参与排序
        /// 默认排序字段为DisplayName,排序方向为升序
        /// </summary>
        /// <param name="enumType">枚举类型</param>
        /// <param name="defaultItemName">添加默认ItemName 设置为null时,不添加默认项</param>
        /// <param name="sortExpr">排序字段</param>
        /// <param name="order">排序方向</param>
        /// <returns></returns>
        public static List<EnumItem> GetEnumItems(Type enumType,string defaultItemName, Expression<Func<EnumItem, object>> sortExpr = null, Sorting order = Sorting.Asc)
        {
            List<EnumItem> enumItems = new List<EnumItem>();
            FieldInfo[] fields = enumType.GetFields();
            foreach (FieldInfo field in fields)
            {
                if (field.FieldType.IsEnum)
                {
                    int value = (int)enumType.InvokeMember(field.Name, BindingFlags.GetField, null, null, null);
                    string key = GetDisplayName(field);
                    EnumItem enumItem = new EnumItem()
                    {
                        Name = field.Name,
                        DisplayName = GetDisplayName(field),
                        Value = value
                    };
                    enumItems.Add(enumItem);
                }
            }
            enumItems = enumItems.OrderBy(a=>a.DisplayName).ToList();
            if(sortExpr != null)
            {
                string sortField = null;
                if (sortExpr.Body is UnaryExpression)
                {   //t=>t.Id
                    UnaryExpression uexp = sortExpr.Body as UnaryExpression;
                    MemberExpression mexp = uexp.Operand as MemberExpression;
                    sortField = mexp.Member.Name;
                }
                if (order == Sorting.Asc)
                {
                    if (sortField == "Name")
                    {
                        enumItems = enumItems.OrderBy(a => a.Name).ToList();
                    }
                    else if (sortField == "DisplayName")
                    {
                        enumItems = enumItems.OrderBy(a => a.DisplayName).ToList();
                    }
                    else if (sortField == "Value")
                    {
                        enumItems = enumItems.OrderBy(a => a.Value).ToList();
                    }
                }
                else
                {
                    if (sortField == "Name")
                    {
                        enumItems = enumItems.OrderByDescending(a => a.Name).ToList();
                    }
                    else if (sortField == "DisplayName")
                    {
                        enumItems = enumItems.OrderByDescending(a => a.DisplayName).ToList();
                    }
                    else if (sortField == "Value")
                    {
                        enumItems = enumItems.OrderByDescending(a => a.Value).ToList();
                    }
                }
            }
            if (defaultItemName != null)
            {   //在0位置添加默认Item "--请选择--"
                enumItems.Insert(0, new EnumItem()
                {
                    Name = defaultItemName,
                    DisplayName = defaultItemName,
                    Value = null
                });
            }
            return enumItems;
        }

        /// <summary>
        /// 获取枚举显示名称
        /// 如果obj为null,返回null
        /// 如果obj为枚举类型时,Type参数可忽略
        /// 如果obj为非枚举类型时,需传入Type参数.转换失败时,返回obj.toString()字符串
        /// </summary>
        /// <param name="obj"></param>
        /// <param name="type"></param>
        /// <returns></returns>
        public static string TryGetDisplayName(object obj,Type type = null)
        {
            if (obj == null)
            {
                return null;
            }
            string result = obj.ToString();
            string enumName = obj.ToString();
            if (type != null)
            {   // obj为int,short,double等其它类型时 尝试类型转换
                #region 处理传入string与其它类型
                if (obj.GetType() != type && obj.GetType() != typeof(string))
                {
                    int? objValue = null;
                    try
                    {   //转换失败时,忽略异常
                        objValue = Convert.ToInt32(obj);
                    }
                    catch (Exception ex)
                    {
                    }
                    if (objValue != null && Enum.IsDefined(type, objValue))
                    {
                        enumName = Enum.GetName(type, objValue);
                    }
                    else
                    {
                        enumName = null;
                    }
                }
                #endregion
            }
            else 
            {
                type = obj.GetType();
            }
            if (type.IsEnum && !string.IsNullOrEmpty(enumName))
            {
                FieldInfo fieldInfo = type.GetFields().FirstOrDefault(f => f.Name.Equals(enumName));
                if (fieldInfo != null)
                {
                    result = GetDisplayName(fieldInfo);
                }
            }
            return result;
        }

        /// <summary>
        /// 获取 Jc DisplayName
        /// </summary>
        /// <param name="type"></param>
        /// <returns></returns>
        public static string GetDisplayName(Type type)
        {
            string result = "";
            DisplayNameAttribute attr = type.GetCustomAttribute<DisplayNameAttribute>();
            if (attr != null)
            {
                result = attr.DisplayName;
            }
            if(string.IsNullOrEmpty(result))
            {
                result = type.Name;
            }
            return result;
        }        

        /// <summary>
        /// 获取Enum显示名称
        /// </summary>
        /// <param name="fieldInfo"></param>
        /// <returns></returns>
        public static string GetDisplayName(FieldInfo fieldInfo)
        {
            string result = "";
            DisplayNameAttribute attr = fieldInfo.GetCustomAttribute<DisplayNameAttribute>();
            if (attr != null)
            {
                result = attr.DisplayName;
            }
            if(string.IsNullOrEmpty(result))
            {
                result = fieldInfo.Name;
            }
            return result;
        }        
    }
}
