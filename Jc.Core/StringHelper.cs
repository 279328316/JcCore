﻿using System;
using System.Collections.Generic;
using System.IO;
using System.IO.Compression;
using System.Linq;
using System.Text;

namespace Jc
{
    /// <summary>
    /// 字符串处理Helper
    /// </summary>
    public class StringHelper
    {
        /// <summary>
        /// Generate a random key
        /// </summary>
        /// <param name="n">key length，IV is 16，Key is 32</param>
        /// <returns>return random value</returns>
        public static string GetRandomStr(int length)
        {
            char[] arrChar = new char[]{
               'a','b','d','c','e','f','g','h','i','j','k','l','m','n','p','r','q','s','t','u','v','w','z','y','x',
               '0','1','2','3','4','5','6','7','8','9',
               'A','B','C','D','E','F','G','H','I','J','K','L','M','N','Q','P','R','T','S','V','U','W','X','Y','Z'
              };

            StringBuilder num = new StringBuilder();
            Random random = new Random();
            for (int i = 0; i < length; i++)
            {
                num.Append(arrChar[random.Next(0, arrChar.Length)].ToString());
            }
            return num.ToString();
        }

        /// <summary>
        /// 转全角的函数(SBC case)
        /// 任意字符串
        /// 全角空格为12288，半角空格为32 其他字符半角(33-126)与全角(65281-65374)的对应关系是：均相差65248
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        public static string ToSBC(string input)
        {
            // 半角转全角：
            char[] c = input.ToCharArray();
            for (int i = 0; i < c.Length; i++)
            {
                if (c[i] == 32)
                {
                    c[i] = (char)12288;
                    continue;
                }
                if (c[i] < 127)
                    c[i] = (char)(c[i] + 65248);
            }
            return new string(c);
        }

        /// <summary>
        /// 转半角的函数(DBC case)        
        /// 任意字符串
        /// 全角空格为12288，半角空格为32 其他字符半角(33-126)与全角(65281-65374)的对应关系是：均相差65248
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        public static string ToDBC(string input)
        {
            char[] c = input.ToCharArray();
            for (int i = 0; i < c.Length; i++)
            {
                if (c[i] == 12288)
                {
                    c[i] = (char)32;
                    continue;
                }
                if (c[i] > 65280 && c[i] < 65375)
                    c[i] = (char)(c[i] - 65248);
            }
            return new string(c);
        }


        /// <summary>
        /// 首字母小写
        /// </summary>
        /// <param name="temp"></param>
        /// <returns></returns>
        public static string FirstToLower(string temp)
        {
            if (!string.IsNullOrEmpty(temp))
            {
                temp = temp[0].ToString().ToLower() + temp.Substring(1, temp.Length - 1);
            }
            return temp;
        }
        /// <summary>
        /// 首字母大写
        /// </summary>
        /// <param name="temp"></param>
        /// <returns></returns>
        public static string FirstToUpper(string temp)
        {
            if (!string.IsNullOrEmpty(temp))
            {
                temp = temp[0].ToString().ToUpper() + temp.Substring(1, temp.Length - 1);
            }
            return temp;
        }

        /// <summary>
        /// 压缩字符串
        /// </summary>
        /// <param name="str">待压缩字符串</param>
        /// <returns>压缩后的字符串</returns>
        public static string DeflateCompress(string str)
        {
            if (string.IsNullOrEmpty(str))
            {
                return str;
            }
            byte[] strArray = Encoding.UTF8.GetBytes(str);
            using (MemoryStream output = new MemoryStream())
            {
                using (DeflateStream compressor = new DeflateStream(output, CompressionMode.Compress))
                {
                    compressor.Write(strArray, 0, str.Length);
                }
                byte[] outputArray = output.ToArray();
                return Encoding.UTF8.GetString(outputArray);
            }
        }

        /// <summary>
        /// 解压缩字符串
        /// </summary>
        /// <param name="str">待解压缩字符串</param>
        /// <returns>解压缩后的字符串</returns>
        public static string DeflateDeCompress(string str)
        {
            if (string.IsNullOrEmpty(str))
            {
                return str;
            }
            byte[] strArray = Encoding.UTF8.GetBytes(str);
            using (MemoryStream output = new MemoryStream())
            {
                using (DeflateStream compressor = new DeflateStream(output, CompressionMode.Decompress))
                {
                    compressor.Write(strArray, 0, str.Length);
                }
                byte[] outputArray = output.ToArray();
                return Encoding.UTF8.GetString(outputArray);
            }
        }

        /// <summary>
        /// GZip压缩字符串
        /// </summary>
        /// <param name="str">待压缩字符串</param>
        /// <returns>压缩后的字符串</returns>
        public static string GZipCompress(string str)
        {
            if (string.IsNullOrEmpty(str))
            {
                return str;
            }
            byte[] strArray = Encoding.UTF8.GetBytes(str);
            using (MemoryStream output = new MemoryStream())
            {
                using (GZipStream compressor = new GZipStream(output, CompressionMode.Compress))
                {
                    compressor.Write(strArray, 0, str.Length);
                }
                byte[] outputArray = output.ToArray();
                return Encoding.UTF8.GetString(outputArray);
            }
        }

        /// <summary>
        /// GZip解压缩字符串
        /// </summary>
        /// <param name="str">待解压缩字符串</param>
        /// <returns>解压缩后的字符串</returns>
        public static string GZipDeCompress(string str)
        {
            if (string.IsNullOrEmpty(str))
            {
                return str;
            }
            byte[] strArray = Encoding.UTF8.GetBytes(str);
            using (MemoryStream output = new MemoryStream())
            {
                using (GZipStream compressor = new GZipStream(output, CompressionMode.Decompress))
                {
                    compressor.Write(strArray, 0, str.Length);
                }
                byte[] outputArray = output.ToArray();
                return Encoding.UTF8.GetString(outputArray);
            }
        }

        /// <summary>
        /// 转换为小写字符串 遇到大写字母,转换为小写,并使用分隔符连接,
        /// </summary>
        /// <param name="str"></param>
        /// <param name="upperSplitChar"></param>
        /// <returns></returns>
        public static string ConvertToLowerString(string str, string upperSplitChar)
        {
            if (string.IsNullOrEmpty(str))
            {
                return str;
            }
            string result = str;
            if (!string.IsNullOrEmpty(upperSplitChar))
            {
                StringBuilder stringBuilder = new StringBuilder();
                for (int i = 0; i < str.Length; i++)
                {
                    char curChar = str[i];
                    if (i == 0)
                    {
                        if (char.IsUpper(curChar))
                        {
                            curChar = char.ToLower(curChar);
                        }
                        stringBuilder.Append(curChar);
                    }
                    else
                    {
                        if (char.IsUpper(curChar))
                        {
                            curChar = char.ToLower(curChar);
                            stringBuilder.Append(upperSplitChar);
                            stringBuilder.Append(curChar);
                        }
                        else
                        {
                            stringBuilder.Append(curChar);
                        }
                    }
                }
                result = stringBuilder.ToString();
            }
            else
            {
                result = str.ToLower();
            }
            return result;
        }
    }
}
