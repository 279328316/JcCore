using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace Jc.Core.Helper
{
    /// <summary>
    /// 简单日志Helper
    /// </summary>
    public class LogHelper
    {
        /// <summary>
        /// 记录应用程序日志 记录日志失败时,不会抛出任何异常
        /// </summary>
        /// <param name="logFileName"></param>
        /// <param name="msg"></param>
        public static void WriteAppLog(string msg = "", bool autoSetTime = true, string logFileName = null)
        {
            try
            {
                if (string.IsNullOrEmpty(logFileName))
                {
                    logFileName = "appLog.log";
                }
                string str = "";
                if (autoSetTime)
                {
                    str = System.DateTime.Now.ToString() + "    " + msg;
                }
                else
                {
                    str = msg;
                }

                string logDir = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "log");
                if (!Directory.Exists(logDir))
                {
                    Directory.CreateDirectory(logDir);
                }
                string logFilePath = Path.Combine(logDir, logFileName);
                StreamWriter sw;
                if (!File.Exists(logFilePath))
                {
                    sw = File.CreateText(logFilePath);
                }
                else
                {
                    sw = File.AppendText(logFilePath);
                }
                sw.WriteLine(str);
                sw.Close();
            }
            catch
            {
            }
        }

        /// <summary>
        /// 记录应用程序日志 记录日志失败时,不会抛出任何异常
        /// </summary>
        /// <param name="logFileName"></param>
        /// <param name="msg"></param>
        public static void WriteAppLogByYear(string msg = "", bool autoSetTime = true, string logFileName = null)
        {
            try
            {
                if (string.IsNullOrEmpty(logFileName))
                {
                    logFileName = "appLog" + DateTime.Now.ToString("yyyy", System.Globalization.DateTimeFormatInfo.InvariantInfo) + ".log";
                }
                string str = "";
                if (autoSetTime)
                {
                    str = System.DateTime.Now.ToString() + "    " + msg;
                }
                else
                {
                    str = msg;
                }
                string logDir = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "log");
                if (!Directory.Exists(logDir))
                {
                    Directory.CreateDirectory(logDir);
                }
                string logFilePath = Path.Combine(logDir, logFileName);
                StreamWriter sw;
                if (!File.Exists(logFilePath))
                {
                    sw = File.CreateText(logFilePath);
                }
                else
                {
                    sw = File.AppendText(logFilePath);
                }
                sw.WriteLine(str);
                sw.Close();
            }
            catch
            {
            }
        }

        /// <summary>
        /// 记录应用程序日志 记录日志失败时,不会抛出任何异常
        /// </summary>
        /// <param name="logFileName"></param>
        /// <param name="msg"></param>
        public static void WriteAppLogByMonth(string msg = "", bool autoSetTime = true, string logFileName = null)
        {
            try
            {
                if (string.IsNullOrEmpty(logFileName))
                {
                    logFileName = "appLog" + DateTime.Now.ToString("yyyyMM", System.Globalization.DateTimeFormatInfo.InvariantInfo) + ".log";
                }
                string str = "";
                if (autoSetTime)
                {
                    str = System.DateTime.Now.ToString() + "    " + msg;
                }
                else
                {
                    str = msg;
                }
                string logDir = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "log");
                if (!Directory.Exists(logDir))
                {
                    Directory.CreateDirectory(logDir);
                }
                string logFilePath = Path.Combine(logDir, logFileName);
                StreamWriter sw;
                if (!File.Exists(logFilePath))
                {
                    sw = File.CreateText(logFilePath);
                }
                else
                {
                    sw = File.AppendText(logFilePath);
                }
                sw.WriteLine(str);
                sw.Close();
            }
            catch
            {
            }
        }

        /// <summary>
        /// 记录应用程序日志 记录日志失败时,不会抛出任何异常
        /// </summary>
        /// <param name="logFileName"></param>
        /// <param name="msg"></param>
        public static void WriteAppLogByDay(string msg = "", bool autoSetTime = true, string logFileName = null)
        {
            try
            {
                if (string.IsNullOrEmpty(logFileName))
                {
                    logFileName = "appLog" + DateTime.Now.ToString("yyyyMMdd", System.Globalization.DateTimeFormatInfo.InvariantInfo) + ".log";
                }
                string str = "";
                if (autoSetTime)
                {
                    str = System.DateTime.Now.ToString() + "    " + msg;
                }
                else
                {
                    str = msg;
                }
                string logDir = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "log");
                if (!Directory.Exists(logDir))
                {
                    Directory.CreateDirectory(logDir);
                }
                string logFilePath = Path.Combine(logDir, logFileName);
                StreamWriter sw;
                if (!File.Exists(logFilePath))
                {
                    sw = File.CreateText(logFilePath);
                }
                else
                {
                    sw = File.AppendText(logFilePath);
                }
                sw.WriteLine(str);
                sw.Close();
            }
            catch
            {
            }
        }

    }
}
