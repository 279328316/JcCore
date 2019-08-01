using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Security.Cryptography;
using System.IO;

namespace Jc.Core.Security
{
    /// <summary>
    /// 获取Md5 Helper
    /// </summary>
    public class Md5Helper
    {
        /// <summary>
        /// 通过字符串获取MD5值，返回32位字符串。
        /// </summary>
        /// <param name="str"></param>
        /// <returns></returns>
        public static string GetMd5String(string str)
        {
            MD5 md5 = MD5.Create();
            byte[] data = Encoding.UTF8.GetBytes(str);
            byte[] data2 = md5.ComputeHash(data);
            return ConvertByteToString(data2);
        }

        /// <summary>
        /// 获取MD5值
        /// HashAlgorithm.Create("MD5") 或 MD5.Create() 
        /// HashAlgorithm.Create("SHA256") 或 SHA256.Create()
        /// </summary>
        /// <param name="str"></param>
        /// <param name="hash"></param>
        /// <returns></returns>
        public static string GetMd5String(string str, HashAlgorithm hash)
        {
            byte[] data = Encoding.UTF8.GetBytes(str);
            byte[] data2 = hash.ComputeHash(data);
            return ConvertByteToString(data2);
        }

        /// <summary>
        /// 从文件获取Md5值
        /// </summary>
        /// <param name="path"></param>
        /// <returns></returns>
        public static string GetMd5FromFile(string path)
        {
            MD5 md5 = MD5.Create();
            if (!File.Exists(path))
            {
                return "";
            }
            FileStream stream = File.OpenRead(path);
            byte[] data2 = md5.ComputeHash(stream);
            return ConvertByteToString(data2);
        }

        private static string ConvertByteToString(byte[] data)
        {
            StringBuilder sb = new StringBuilder();
            for (int i = 0; i < data.Length; i++)
            {
                sb.Append(data[i].ToString("x2"));
            }
            return sb.ToString();
        }

    }
}
