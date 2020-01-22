using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Runtime.Serialization;

namespace Jc.Core
{
    /// <summary>
    /// EnumModel
    /// </summary>
    public class EnumModel
    {
        /// <summary>
        /// 名称
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// 显示名称
        /// </summary>
        public string DisplayName { get; set; }

        /// <summary>
        /// EnumItem项
        /// </summary>
        public List<EnumItemModel> EnumItems { get; set; } = new List<EnumItemModel>();
    }

    /// <summary>
    /// EnumItemModel
    /// </summary>
    public class EnumItemModel
    {
        /// <summary>
        /// 名称
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// 显示名称
        /// </summary>
        public string DisplayName { get; set; }

        /// <summary>
        /// 值
        /// </summary>
        public int? Value { get; set; }
    }
}
