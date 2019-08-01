using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Jc.Core.Helper
{
    /// <summary>
    /// 字符串处理Helper
    /// </summary>
    public class StringHelper
    {
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
    }
}
