using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace Jc.Core.Helper
{
    /// <summary>
    /// Enum操作Helper
    /// </summary>
    public class EnumHelper
    {
        /// <summary>
        /// 将Enum类型转换为 Dictionary
        /// Key为关键字,Value为显示文本.
        /// </summary>
        /// <param name="enumType">枚举类型 typeof(enum)</param>
        /// <param name="sortingType">排序方式 默认不排序</param>
        /// <param name="order">排序方向</param>
        /// <returns></returns>
        public static Dictionary<string, string> GetEnumDictionary(Type enumType,KeyValueSortingType sortingType = KeyValueSortingType.Default,Sorting order = Sorting.Asc)
        {
            Dictionary<string, string> dic = new Dictionary<string, string>();
            Dictionary<string, string> tempDic = new Dictionary<string, string>();

            #region 获取值
            Type typeDescription = typeof(EnumDisplayAttribute);
            FieldInfo[] fields = enumType.GetFields();
            string strText = "";
            string strValue = "";
            foreach (FieldInfo field in fields)
            {
                if (field.FieldType.IsEnum)
                {
                    strValue = ((int)enumType.InvokeMember(field.Name, BindingFlags.GetField, null, null, null)).ToString();
                    object[] arr = field.GetCustomAttributes(typeDescription, true);
                    if (arr.Length > 0)
                    {
                        EnumDisplayAttribute aa = (EnumDisplayAttribute)arr[0];
                        strText = aa.DisplayName;
                    }
                    else
                    {
                        strText = field.Name;
                    }
                    tempDic.Add(strValue, strText);
                }
            }
            #endregion

            //处理排序
            #region 排序处理
            switch (sortingType)
            {
                case KeyValueSortingType.Key:
                    if (order == Sorting.Asc)
                    {
                        dic = dic.Union(tempDic.OrderBy(kv => kv.Key));                        
                    }
                    else
                    {
                        dic = dic.Union(tempDic.OrderByDescending(kv => kv.Key));
                    }
                    break;
                case KeyValueSortingType.Value:
                    if (order == Sorting.Asc)
                    {
                        dic = dic.Union(tempDic.OrderBy(kv => kv.Value));
                    }
                    else
                    {
                        dic = dic.Union(tempDic.OrderByDescending(kv => kv.Value));
                    }
                    break;
                default:
                    dic = dic.Union(tempDic);
                    break;
            }
            #endregion

            return dic;
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
                    return GetEnumDisplayName(fieldInfo);
                }
            }
            else
            {
                return null;
            }
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
                    result = GetEnumDisplayName(fieldInfo);
                }
            }
            return result;
        }

        /// <summary>
        /// 获取Enum显示名称
        /// </summary>
        /// <param name="fieldInfo"></param>
        /// <returns></returns>
        private static string GetEnumDisplayName(FieldInfo fieldInfo)
        {
            string result = "";
            var enumDisplayAttribute = fieldInfo.GetCustomAttribute<EnumDisplayAttribute>();
            if (enumDisplayAttribute == null)
            {
                result = fieldInfo.Name;
            }
            else
            {
                result = enumDisplayAttribute.DisplayName;
            }
            return result;
        }

    }

    /// <summary>
    /// KeyValuePair排序方式
    /// </summary>
    public enum KeyValueSortingType
    {
        /// <summary>
        /// 默认不做排序
        /// </summary>
        Default = 0,
        /// <summary>
        /// 按照Key排序
        /// </summary>
        Key = 1,
        /// <summary>
        /// 按照Value排序
        /// </summary>
        Value = 2
    }
}
