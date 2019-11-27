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

namespace Jc.Core
{
    /// <summary>
    /// Json操作类
    /// </summary>
    public class JsonHelper
    {
        /// <summary>
        /// 序列化方法 同SerializeObject
        /// </summary>
        /// <param name="obj">对象</param>
        /// <param name="setting">序列化设置 默认</param>
        /// <returns></returns>
        [Obsolete("方法同SerializeObject")]
        public static string ObjToJson(object obj, JsonSerializerSettings setting = null)
        {
            return SerializeObject(obj, setting);
        }

        /// <summary>
        /// 序列化方法
        /// </summary>
        /// <param name="obj">对象</param>
        /// <param name="setting">序列化设置 默认</param>
        /// <returns></returns>
        public static string SerializeObject(object obj, JsonSerializerSettings setting = null)
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


        /// <summary>
        /// 序列化对象到文件
        /// </summary>
        /// <param name="obj">对象</param>
        /// <param name="filePath">文件路径</param>
        /// <param name="setting">序列化设置 默认</param>
        /// <returns></returns>
        public static string SerializeObjectToFile(object obj,string filePath, JsonSerializerSettings setting = null)
        {
            string result = "";
            try
            {
                result = SerializeObject(obj, setting);
                FileInfo fileInfo = new FileInfo(filePath);
                if(fileInfo.Exists)
                {
                    throw new Exception("文件已存在");
                }
                if(!fileInfo.Directory.Exists)
                {
                    Directory.CreateDirectory(fileInfo.Directory.FullName);
                }
                using (FileStream fileStream = File.Create(filePath))
                {
                    Byte[] buffer = System.Text.Encoding.UTF8.GetBytes(result);
                    fileStream.Write(buffer,0,buffer.Length);
                    fileStream.Close();
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
            return result;
        }

        /// <summary>
        /// 自文件加载对象
        /// Json反序列化
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="filePath">文件路径</param>
        /// <returns></returns>
        public static T DeserializeObjectFromFile<T>(string filePath)
        {
            T obj = default(T);
            try
            {
                if (string.IsNullOrEmpty(filePath) || !File.Exists(filePath))
                {
                    throw new Exception("文件不存在");
                }
                string content = File.ReadAllText(filePath);
                obj = DeserializeObject<T>(content);
            }
            catch
            {
            }
            return obj;
        }
    }
}