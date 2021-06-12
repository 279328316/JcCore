using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Jc.Data.Query
{
    /// <summary>
    /// 分页
    /// </summary>
    public class Pager
    {
        private int pageIndex = 0;  //页序号
        private int pageSize = 0;   //页大小
        private int filterStartIndex = -1;    //开始Index 自0开始
        private int filterEndIndex = -1;  //结束Index   (pageIndex+1)*pageSize
        private int totalCount = 0; //总记录数

        /// <summary>
        /// 页序号
        /// </summary>
        public int PageIndex { get { return pageIndex; } }
        /// <summary>
        /// 页大小
        /// </summary>
        public int PageSize { get { return pageSize; } }
        /// <summary>
        /// 分页开始Index 自0开始 (pageIndex-1)*pageSize
        /// </summary>
        public int FilterStartIndex { get { return filterStartIndex; } }
        /// <summary>
        /// 分页结束Index
        /// </summary>
        public int FilterEndIndex { get { return filterEndIndex; } }

        /// <summary>
        /// 结果总条数用于分页
        /// </summary>
        public int TotalCount { get { return totalCount; } set { totalCount = value; } }


        /// <summary>
        /// Ctor
        /// <param name="pageIndex">第几页</param>
        /// <param name="pageSize">页大小</param>
        /// </summary>
        public Pager(int pageIndex = 1)
        {
        }

        /// <summary>
        /// Ctor 初始化分页信息
        /// <param name="pageIndex">第几页</param>
        /// <param name="pageSize">页大小</param>
        /// </summary>
        public Pager(int pageIndex = 1, int pageSize = 10)
        {
            InitPage(pageIndex, pageSize);
        }


        /// <summary>
        /// Ctor 初始化分页信息
        /// <param name="pageIndex">第几页</param>
        /// <param name="pageSize">页大小</param>
        /// </summary>
        public void InitPage(int pageIndex = 1, int pageSize = 10)
        {
            this.pageSize = pageSize;
            this.pageIndex = pageIndex;
            filterStartIndex = (pageIndex - 1) * pageSize;
            filterEndIndex = pageIndex * pageSize;
        }
    }
}
