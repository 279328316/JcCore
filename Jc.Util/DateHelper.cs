using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;

namespace Jc
{
    /// <summary>
    /// Date Helper
    /// </summary>
    public class DateHelper
    {
        /// <summary>
        /// 计算日期时间差 hh:mm:ss
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
            return DateDif(dt2.Value - dt1.Value);
        }

        /// <summary>
        /// 计算日期时间 hh:mm:ss
        /// </summary>
        /// <param name="ticks">ticks</param>
        /// <returns></returns>
        public static string DateDif(long ticks)
        {
            TimeSpan ts = new TimeSpan(ticks);
            return DateDif(ts);
        }

        /// <summary>
        /// 计算日期时间 hh:mm:ss
        /// </summary>
        /// <param name="ts">TimeSpan</param>
        /// <returns></returns>
        public static string DateDif(TimeSpan ts)
        {
            int hours = (int)ts.TotalHours;//总时间分差
            int minutes = (int)(ts.TotalMinutes - hours * 60);//总时间分差
            int seconds = (int)(ts.TotalSeconds - hours * 3600 - minutes * 60);//总时间分差

            string result = string.Format("{0}:{1}:{2}",
                hours.ToString().PadLeft(2, '0'),
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
            return DateDifDay( dt2.Value - dt1.Value);
        }

        /// <summary>
        /// 计算日期时间 d天 hh:mm:ss
        /// </summary>
        /// <param name="ticks">ticks</param>
        /// <returns></returns>
        public static string DateDifDay(long ticks)
        {
            TimeSpan ts = new TimeSpan(ticks);
            return DateDifDay(ts);
        }

        /// <summary>
        /// 计算日期时间差 d天 hh:mm:ss
        /// </summary>
        /// <param name="ts">TimeSpan</param>
        /// <returns></returns>
        public static string DateDifDay(TimeSpan ts)
        {
            int days = (int)ts.TotalDays;
            int hours = (int)(ts.TotalHours - days * 24);//总时间分差
            int minutes = (int)(ts.TotalMinutes - days * 24 * 60 - hours * 60);//总时间分差
            int seconds = (int)(ts.TotalSeconds - days * 24 * 60 * 60 - hours * 3600 - minutes * 60);//总时间分差

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
