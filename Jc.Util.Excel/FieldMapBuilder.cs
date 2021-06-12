using NPOI.SS.UserModel;
using NPOI.XSSF.UserModel;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using System.Text;

namespace Jc.Excel
{
    /// <summary>
    /// FieldMap Builder
    /// </summary>
    public class FieldMapBuilder
    {
        public List<FieldMap> FieldMaps { get; set; } = new List<FieldMap>();

        #region Add FieldMap

        /// <summary>
        /// 添加FieldMap
        /// </summary>
        /// <param name="fieldMap"></param>
        /// <returns></returns>
        public FieldMapBuilder Add(params FieldMap[] fieldMapArgs)
        {
            foreach (FieldMap fieldMap in fieldMapArgs)
            {
                fieldMap.ColumnIndex = FieldMaps.Count;
                FieldMaps.Add(fieldMap);
            }
            return this;
        }        
        #endregion

        /// <summary>
        /// 获取已添加的FieldMap列表
        /// </summary>
        /// <returns></returns>
        public List<FieldMap> GetFieldMaps()
        {
            return FieldMaps;
        }
    }
}
