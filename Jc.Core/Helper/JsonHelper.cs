using System;
using System.IO;
using System.Data;
using System.Runtime.Serialization;
using System.Runtime.Serialization.Formatters.Binary;
using System.Xml.Serialization;
using System.Xml;
using System.Text;
using System.Text.RegularExpressions;
using System.Collections.Generic;
using Newtonsoft.Json.Linq;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;

namespace Jc.Core.Helper
{
    /// <summary>
    /// Json操作类
    /// </summary>
    public class JsonHelper
    {
        /// <summary>
        /// 序列化方法
        /// </summary>
        /// <param name="obj">对象</param>
        /// <param name="setting">序列化设置 默认</param>
        /// <returns></returns>
        public static string ObjToJson(object obj, JsonSerializerSettings setting = null)
        {
            string result = "";
            try
            {
                if (setting == null)
                {
                    setting = new JsonSerializerSettings();
                    setting.DateFormatString = "yyyy-MM-dd HH:mm:ss";//日期字符串格式化
                    setting.NullValueHandling = NullValueHandling.Ignore;//忽略空值
                    setting.ReferenceLoopHandling = ReferenceLoopHandling.Ignore;//忽略空值
                    setting.ContractResolver = new CamelCasePropertyNamesContractResolver();
                }
                result = JsonConvert.SerializeObject(obj, setting);
            }
            catch(Exception ex)
            {
                throw ex;
            }
            return result;
        }

        /// <summary>
        /// Json反序列化
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="str">数据String</param>
        /// <returns></returns>
        public static T DeserializeObject<T>(string str)
        {
            T obj = default(T);
            try
            {
                if (!string.IsNullOrEmpty(str))
                {
                    obj = JsonConvert.DeserializeObject<T>(str);
                }
            }
            catch
            {
            }
            return obj;
        }
    }
}