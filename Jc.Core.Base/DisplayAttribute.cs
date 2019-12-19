using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Reflection;

namespace Jc.Core
{
    /// <summary>
    /// DisplayAttribute
    /// </summary>
    public class DisplayAttribute : Attribute
    {
        /// <summary>
        /// 显示名称
        /// </summary>
        public string Name { get; set; }

        public DisplayAttribute()
        {
        }
        public DisplayAttribute(string name)
        {
            this.Name = name;
        }
    }
}
