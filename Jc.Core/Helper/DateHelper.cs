using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;

namespace Jc.Core.Helper
{
    public class DateHelper
    {
        /// <summary>
        /// 计算日期时间差
        /// hh:mm:ss
        /// </summary>
        /// <param name="dt1"></param>
        /// <param name="dt2"></param>
        /// <returns></returns>
        public static string DateDif(DateTime? dt1 = null,DateTime? dt2 = null)
        {
            if (dt1 == null)
            {
                dt1 = DateTime.Now;
            }
            if (dt2 == null)
            {
                dt2 = DateTime.Now;
            }
            TimeSpan ts = dt2.Value - dt1.Value;
            int hours = (int)ts.TotalHours;//总时间分差
            int minutes = (int)(ts.TotalMinutes- hours*60);//总时间分差
            int seconds = (int)(ts.TotalSeconds - hours* 3600 - minutes * 60);//总时间分差

            string result = string.Format("{0}:{1}:{2}",
                hours.ToString().PadLeft(2,'0'),
                minutes.ToString().PadLeft(2, '0'),
                seconds.ToString().PadLeft(2, '0'));
            return result;
        }

        /// <summary>
        /// 计算日期时间差
        /// d天 hh:mm:ss
        /// </summary>
        /// <param name="dt1"></param>
        /// <param name="dt2"></param>
        /// <returns></returns>
        public static string DateDifDay(DateTime? dt1 = null, DateTime? dt2 = null)
        {
            if (dt1 == null)
            {
                dt1 = DateTime.Now;
            }
            if (dt2 == null)
            {
                dt2 = DateTime.Now;
            }
            TimeSpan ts = dt2.Value - dt1.Value;
            int days = (int)ts.TotalDays;
            int hours = (int)(ts.TotalHours - days * 24);//总时间分差
            int minutes = (int)(ts.TotalMinutes - hours * 60);//总时间分差
            int seconds = (int)(ts.TotalSeconds - hours * 3600 - minutes * 60);//总时间分差

            string result = string.Format("{0}:{1}:{2}",
                hours.ToString().PadLeft(2, '0'),
                minutes.ToString().PadLeft(2, '0'),
                seconds.ToString().PadLeft(2, '0'));
            if (days > 0)
            {
                result = $"{days}天 {result}";
            }
            return result;
        }
    }    
}
