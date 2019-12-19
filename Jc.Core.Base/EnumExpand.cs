using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Runtime.Serialization;
using System.Reflection;

namespace Jc.Core
{
    /// <summary>
    /// Enum 扩展
    /// </summary>
    public static class EnumExpand
    {
        /// <summary>
        /// Enum 枚举值 Value
        /// </summary>
        /// <param name="expr"></param>
        /// <returns></returns>
        public static int Value(this Enum enumValue)
        {
            int value = (int)Convert.ChangeType(enumValue, typeof(int));
            return value;
        }
        /// <summary>
        /// Enum 枚举值 Name
        /// </summary>
        /// <param name="expr"></param>
        /// <returns></returns>
        public static string Name(this Enum enumValue)
        {
            return enumValue.ToString();
        }

        /// <summary>
        /// Enum 枚举值 Name
        /// </summary>
        /// <param name="expr"></param>
        /// <returns></returns>
        public static string GetDisplayName(this Enum enumValue)
        {
            if (enumValue != null)
            {
                FieldInfo fieldInfo = enumValue.GetType().GetFields().FirstOrDefault(f => f.Name.Equals(enumValue.ToString()));
                if (fieldInfo == null)
                {
                    return enumValue.ToString();
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
        /// 获取Enum显示名称
        /// </summary>
        /// <param name="fieldInfo"></param>
        /// <returns></returns>
        private static string GetEnumDisplayName(FieldInfo fieldInfo)
        {
            string result = "";
            var enumDisplayAttribute = fieldInfo.GetCustomAttribute<DisplayAttribute>();
            if (enumDisplayAttribute == null)
            {
                result = fieldInfo.Name;
            }
            else
            {
                result = enumDisplayAttribute.Name;
            }
            return result;
        }
    }
}
