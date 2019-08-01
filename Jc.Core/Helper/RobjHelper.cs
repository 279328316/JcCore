using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using System.Reflection;

namespace Jc.Core.Helper
{
    /// <summary>
    /// 返回对象处理Helper RobjHelper
    /// </summary>
    public class RobjHelper
    {
        /// <summary>
        /// Robj Json序列化属性过滤
        /// 将非输出属性设置为null
        /// </summary>
        public static void PiFilter<T>(Robj<T> robj,string excludePiStr = null,string includePiStr = null)
        {
            if(robj==null || robj.Result==null)
            {
                return;
            }
            PiFilter(robj.Result, excludePiStr, includePiStr);
        }

        /// <summary>
        /// Robj Json序列化属性过滤
        /// 将非输出属性设置为null
        /// </summary>
        public static void PiFilter<T>(Robj<List<T>> robj, string excludePiStr = null, string includePiStr = null)
        {
            if (robj == null || robj.Result == null)
            {
                return;
            }
            for (int i = 0; i < robj.Result.Count; i++)
            {
                PiFilter(robj.Result[i], excludePiStr, includePiStr);
            }
        }

        /// <summary>
        /// Robj Json序列化属性过滤
        /// 将非输出属性设置为null
        /// </summary>
        public static void PiFilter<T>(Robj<PageResult<T>> robj, string excludePiStr = null, string includePiStr = null)
        {
            if (robj == null || robj.Result == null || robj.Result.Rows==null)
            {
                return;
            }
            for(int i=0;i<robj.Result.Rows.Count;i++)
            {
                PiFilter(robj.Result.Rows[i], excludePiStr, includePiStr);
            }
        }

        /// <summary>
        /// 将Obj相应的属性设置为null
        /// </summary>
        /// <param name="obj"></param>
        /// <param name="excludePiStr">排除属性列表以,号分割</param>
        /// <param name="includePiStr">包含属性列表以,号分割</param>
        private static void PiFilter(object obj, string excludePiStr = null, string includePiStr = null)
        {
            if(obj == null || (string.IsNullOrEmpty(excludePiStr) && string.IsNullOrEmpty(includePiStr)))
            {
                return;
            }
            List<PropertyInfo> piList = obj.GetType().GetProperties(BindingFlags.Public | BindingFlags.Instance)
                .Where(p => p.CanRead && p.CanWrite).ToList();

            List<string> excludeList = null;    //排除属性列表
            List<string> includeList = null;    //包含属性列表
            if (!string.IsNullOrEmpty(excludePiStr))
            {   //转换为小写对比
                excludeList = excludePiStr.ToLower().Split(',').Select(item => item.Trim()).ToList();
            }
            if (!string.IsNullOrEmpty(includePiStr))
            {
                includeList = includePiStr.ToLower().Split(',').Select(item=>item.Trim()).ToList();
            }

            for (int i = 0; i < piList.Count; i++)
            {
                if (includeList != null && includeList.Contains(piList[i].Name.ToLower()))
                {   //如果在包含列表中,继续
                    continue;
                }
                if (excludeList != null && !excludeList.Contains(piList[i].Name.ToLower()))
                {   //如果排除列表不为空,且不在排除列表中,继续
                    continue;
                }
                if (IsNullable(piList[i].PropertyType))
                {
                    piList[i].SetValue(obj, null);
                }
            }
        }

        /// <summary>
        /// 判断类型是否可以为空
        /// </summary>
        /// <param name="theType"></param>
        /// <returns></returns>
        private static bool IsNullable(Type theType)
        {
            bool result = true;
            if (theType.IsValueType)
            {
                result = (theType.IsGenericType &&
                    theType.GetGenericTypeDefinition().Equals(typeof(Nullable<>)));
            }
            return result;
        }
    }    
}
