using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Runtime.Serialization;
using System.Collections;

namespace Jc
{
    /// <summary>
    /// Collection 扩展
    /// </summary>
    public static class CollectionExpand
    {
        /// <summary>
        /// IEnumerable ToString 方法扩展
        /// </summary>
        /// <param name="enumAble">IEnumerable 对象</param>
        /// <param name="splitStr">分割字符串</param>
        /// <param name="ignoreNull">忽略Null值</param>
        /// <returns></returns>
        public static string ToString(this IEnumerable enumAble,string splitStr,bool ignoreNull = true)
        {
            string result = "";
            if(enumAble!=null)
            {
                StringBuilder strBuilder = new StringBuilder();
                //使用isFirstObj原因 (1)SplitStr可能与元素相同(2)obj null情况
                bool isFirstObj = true; 
                foreach (object obj in enumAble)
                {
                    if(obj == null && ignoreNull)
                    {
                        continue;
                    }
                    if(!isFirstObj)
                    {
                        strBuilder.Append(splitStr);
                    }
                    strBuilder.Append(obj);
                    isFirstObj = false;
                }
                result = strBuilder.ToString();
            }
            return result;
        }
    }
}
