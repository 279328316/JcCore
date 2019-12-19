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
        /// Key为关键字,Value为值
        /// </summary>
        /// <param name="enumType">枚举类型 typeof(enum)</param>
        /// <returns></returns>
        public static Dictionary<string, int> GetDictionary(Type enumType)
        {
            Dictionary<string, int> dic = new Dictionary<string, int>();

            #region 获取值
            Type typeDescription = typeof(DisplayNameAttribute);
            FieldInfo[] fields = enumType.GetFields();
            string key = "";
            int value = 0;
            foreach (FieldInfo field in fields)
            {
                if (field.FieldType.IsEnum)
                {
                    value = (int)enumType.InvokeMember(field.Name, BindingFlags.GetField, null, null, null);
                    key = GetDisplayName(field);
                    dic.Add(key, value);
                }
            }
            #endregion
            //dic = dic.OrderBy(kv => kv.Value).ToDictionary(kv => kv.Key, kv => kv.Value);
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
                    return GetDisplayName(fieldInfo);
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
                    result = GetDisplayName(fieldInfo);
                }
            }
            return result;
        }

        /// <summary>
        /// 获取Enum显示名称
        /// </summary>
        /// <param name="fieldInfo"></param>
        /// <returns></returns>
        private static string GetDisplayName(FieldInfo fieldInfo)
        {
            string result = "";
            var enumDisplayAttribute = fieldInfo.GetCustomAttribute<DisplayNameAttribute>();
            if (enumDisplayAttribute != null)
            {
                result = enumDisplayAttribute.DisplayName;
            }
            if(string.IsNullOrEmpty(result))
            {
                result = fieldInfo.Name;
            }
            return result;
        }        
    }
}
