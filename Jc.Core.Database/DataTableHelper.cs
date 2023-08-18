using System;
using System.Collections.Generic;
using System.Data.Common;
using System.Data;
using System.Text;

namespace Jc.Database
{
    /// <summary>
    /// DataTable Helper
    /// </summary>
    internal class DataTableHelper
    {
        /// <summary>
        /// 将DataReader转换为DataTable
        /// </summary>
        /// <param name="dr">DataReader</param>
        /// <param name="loadAmount">加载数量</param>
        /// <returns></returns>
        internal static DataTable ConvertDataReaderToDataTable(DbDataReader dr, int? loadAmount = null)
        {
            try
            {
                DataTable dt = new DataTable();
                int fieldCount = dr.FieldCount;
                for (int intCounter = 0; intCounter < fieldCount; ++intCounter)
                {
                    dt.Columns.Add(dr.GetName(intCounter), dr.GetFieldType(intCounter));
                }
                dt.BeginLoadData();

                object[] objValues = new object[fieldCount];
                int rowsCount = 0;
                while (dr.Read())
                {
                    rowsCount++;
                    dr.GetValues(objValues);
                    dt.LoadDataRow(objValues, true);

                    if (loadAmount.HasValue && rowsCount >= loadAmount)
                    {
                        break;
                    }
                }
                dt.EndLoadData();
                return dt;
            }
            catch (Exception ex)
            {
                throw new Exception($"读取数据出错:{ex.Message}", ex);
            }

        }

    }
}
