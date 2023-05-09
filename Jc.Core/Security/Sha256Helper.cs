using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Security.Cryptography;
using System.IO;

namespace Jc.Security
{
    /// <summary>
    /// 获取Sha256 Helper
    /// </summary>
    public class Sha256Helper
    {
        /// <summary>
        /// 对字符串SHA256加密
        /// </summary>
        /// <param name="str">待加密字符串</param>
        /// <returns>加密后的字符串</returns>
        public static string Sha256Encrypt(string str)
        {
            byte[] bytes = Encoding.UTF8.GetBytes(str);
            byte[] hash = SHA256.Create().ComputeHash(bytes);
            
            StringBuilder builder = new StringBuilder();
            for (int i = 0; i < hash.Length; i++)
            {
                builder.Append(hash[i].ToString("X2"));
            }
            return builder.ToString();
        }
    }
}
