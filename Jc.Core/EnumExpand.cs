using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Runtime.Serialization;

namespace Jc.Core
{
    /// <summary>
    /// Enum 扩展
    /// </summary>
    public static class EnumExpand
    {
        /// <summary>
        /// Enum 枚举值 Value
        /// </summary>
        /// <param name="expr"></param>
        /// <returns></returns>
        public static int Value(this Enum enumValue)
        {
            int value = (int)Convert.ChangeType(enumValue, typeof(int));
            return value;
        }
        /// <summary>
        /// Enum 枚举值 Name
        /// </summary>
        /// <param name="expr"></param>
        /// <returns></returns>
        public static string Name(this Enum enumValue)
        {
            return enumValue.ToString();
        }
    }
}
