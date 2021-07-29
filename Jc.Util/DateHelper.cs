using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using System.Globalization;

namespace Jc
{
    /// <summary>
    /// Date Helper
    /// </summary>
    public class DateHelper
    {
        /// <summary>
        /// 计算日期时间差 d天 hh:mm:ss
        /// </summary>
        /// <param name="dt1">日期1</param>
        /// <param name="dt2">日期2</param>
        /// <returns>dt1与dt2的时间差</returns>
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
            if (dt2.Value.CompareTo(dt1.Value) < 0)
            {
                ts = dt1.Value - dt2.Value;
            }
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

        /// <summary> 
        /// 获取某一日期星期几
        /// </summary> 
        /// <param name="dateTime"> 日期 </param> 
        /// <returns> 该日期的星期 </returns> 
        public static DayOfWeek GetWeekOfYear(DateTime dateTime)
        {
            GregorianCalendar gc = new GregorianCalendar();
            return gc.GetDayOfWeek(dateTime);
        }
    }    
}
