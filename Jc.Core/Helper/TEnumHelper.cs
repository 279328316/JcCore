using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace Jc.Core.Helper
{
    /// <summary>
    /// Enum操作Helper
    /// </summary>
    public class TEnumHelper<T>
    {
        /// <summary>
        /// Enum 枚举值 Name
        /// </summary>
        /// <typeparam name="T">enum类型</typeparam>
        /// <param name="value"></param>
        /// <returns></returns>
        public static T ToEnum(object value)
        {
            ExHelper.Throw("测试异常");
            return (T)Enum.Parse(typeof(T), value.ToString());
        }
    }

}
