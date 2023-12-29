using System;
using System.Linq;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Reflection;
using System.Collections;

namespace Jc
{
    /// <summary>
    /// Object 扩展
    /// </summary>
    public static class DictionaryHelper
    {
        public static Dictionary<string, string> ConvertToDictionary(object obj,bool ignoreNull = true)
        {
            Dictionary<string, string> dic = new Dictionary<string, string>();
            if (obj != null)
            {
                List<PropertyInfo> piList = obj.GetType().GetProperties()
                            .Where(pi => pi.CanRead).ToList();
                if (piList != null && piList.Count > 0)
                {
                    for (int i = 0; i < piList.Count; i++)
                    {
                        string valueStr;
                        object value = piList[i].GetValue(obj);
                        if(value == null)
                        {
                            if (!ignoreNull)
                            {
                                dic.Add(piList[i].Name.ToLower(), string.Empty);
                            }
                            continue;
                        }
                        if (value is ValueType || value is string)
                        {
                            valueStr = value.ToString();
                        }
                        else
                        {
                            valueStr = JsonHelper.SerializeObject(value);
                        }
                        dic.Add(piList[i].Name.ToLower(), valueStr);
                    }
                }
            }
            return dic;
        }
    }
}
