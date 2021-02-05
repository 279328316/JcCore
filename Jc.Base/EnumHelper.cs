using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace Jc.Core
{
    /// <summary>
    /// Enum操作Helper
    /// </summary>
    public class EnumHelper
    {
        /// <summary>
        /// 将Enum类型转换为 EnumModel
        /// </summary>
        /// <param name="enumType">枚举类型 typeof(enum)</param>
        /// <param name="addNullItem">添加NullItem</param>
        /// <returns>EnumModel</returns>
        public static EnumModel GetEnumModel(Type enumType,bool addNullItem = false)
        {
            string enumTypeName = enumType.Name;
            string enumName = enumTypeName.Substring(enumTypeName.LastIndexOf(".") + 1);
            EnumModel enumModel = new EnumModel()
            {
                Name = enumName,
                DisplayName = GetDisplayName(enumType)
            };

            #region 获取值
            FieldInfo[] fields = enumType.GetFields();
            if(addNullItem)
            {
                enumModel.EnumItems.Add(new EnumItemModel()
                {
                    DisplayName = ""
                });
            }
            foreach (FieldInfo field in fields)
            {
                if (field.FieldType.IsEnum)
                {
                    int value = (int)enumType.InvokeMember(field.Name, BindingFlags.GetField, null, null, null);
                    string key = GetDisplayName(field);
                    EnumItemModel enumItem = new EnumItemModel()
                    {
                        Name = field.Name,
                        DisplayName = GetDisplayName(field),
                        Value = value
                    };
                    enumModel.EnumItems.Add(enumItem);
                }
            }
            enumModel.EnumItems.Sort((a,b)=> { return a.Value - b.Value; });
            #endregion
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
