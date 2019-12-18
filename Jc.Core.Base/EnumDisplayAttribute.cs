using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Reflection;

namespace Jc.Core
{
    /// <summary>
    /// 枚举DisplayAttribute
    /// </summary>
    public class DisplayNameAttribute : Attribute
    {
        /// <summary>
        /// 显示名称
        /// </summary>
        public string DisplayName { get; set; }

        public DisplayNameAttribute()
        {
        }
        public DisplayNameAttribute(string displayName)
        {
            this.DisplayName = displayName;
        }
    }
}
