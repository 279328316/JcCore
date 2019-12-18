
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;

namespace Jc.Core
{
    /// <summary>
    /// 分页结果对象
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public class PageResult<T>
    {
        /// <summary>
        /// 总记录数
        /// </summary>
        [DisplayName("Total")]
        public int Total { get; set; }
        /// <summary>
        /// 列表对象
        /// </summary>
        [DisplayName("Rows")]
        public List<T> Rows { get; set; }

        /// <summary>
        /// 构造方法
        /// </summary>
        public PageResult()
        {
            Total = 0;
            Rows = new List<T>();
        }

        /// <summary>
        /// 构造方法
        /// </summary>
        /// <param name="totalCount">总记录数</param>
        /// <param name="list">结果列表</param>
        public PageResult(int totalCount, List<T> list)
        {
            this.Total = totalCount;
            this.Rows = list;
        }

    }
}
