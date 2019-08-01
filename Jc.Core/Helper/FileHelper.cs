using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Jc.Core.Helper
{
    /// <summary>
    /// File操作Helper
    /// </summary>
    public class FileHelper
    {
        /// <summary>
        /// 获取文件扩展名 默认包含.号
        /// </summary>
        /// <param name="fileName">文件名</param>
        /// <param name="withPoint">是否包含.号 默认值true</param>
        /// <returns></returns>
        public static string GetFileExtension(string fileName,bool withPoint = true)
        {
            string exStr = "";
            int index = fileName.LastIndexOf(".");
            if (index > 0 && index < fileName.Length - 1)
            {
                int subIndex = withPoint ? index : index + 1;
                exStr = fileName.Substring(subIndex);
            }
            return exStr;
        }

        /// <summary>
        /// 返回带单位的文件大小 取部分
        /// </summary>
        /// <param name="fileSize">字节数</param>
        /// <returns></returns>
        public static string FormatFileSize(long fileSize)
        {
            if (fileSize < 0)
            {
                throw new ArgumentOutOfRangeException("fileSize");
            }
            else if (fileSize >= 1024 * 1024 * 1024)
            {
                return string.Format("{0:########0.00} GB", ((Double)fileSize) / (1024 * 1024 * 1024));
            }
            else if (fileSize >= 1024 * 1024)
            {
                return string.Format("{0:####0.00} MB", ((Double)fileSize) / (1024 * 1024));
            }
            else if (fileSize >= 1024)
            {
                return string.Format("{0:####0.00} KB", ((Double)fileSize) / 1024);
            }
            else
            {
                return string.Format("{0} bytes", fileSize);
            }

        }
    }
}
