using NPOI.SS.UserModel;
using NPOI.XSSF.UserModel;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;

namespace Jc.Core.Excel
{
    /// <summary>
    /// Excel Import/Export Helper
    /// </summary>
    public class ExcelHelper
    {
        #region Export Method

        /// <summary>
        /// 导出到文件 输出到指定路径
        /// </summary>
        /// <param name="dataList">数据List</param>
        /// <param name="exportFields">导出字段Map</param>
        /// <param name="filePath">文件路径</param>
        /// <param name="sheetName">sheet名称</param>
        public static void ExportToFile<T>(List<T> dataList, List<FieldMap> exportFields, string filePath, string sheetName = null)
        {
            if (File.Exists(filePath))
            {
                FileInfo fileInfo = new FileInfo(filePath);
                throw new Exception($"文件{fileInfo.Name}已存在");
            }

            #region 写入文件
            //保存为Excel文件
            using (FileStream fs = new FileStream(filePath, FileMode.Create, FileAccess.Write))
            {
                byte[] buffer = ExportToByte(dataList, exportFields, sheetName);
                fs.Write(buffer, 0, buffer.Length);
                fs.Flush();
            }
            #endregion
        }
        
        /// <summary>
        /// 导出文件 获取导出内容Byte
        /// </summary>
        /// <param name="dataList">数据List</param>
        /// <param name="exportFields">导出字段Map</param>
        /// <param name="sheetName">sheet名称</param>
        public static byte[] ExportToByte<T>(List<T> dataList, List<FieldMap> exportFields, string sheetName = null)
        {
            byte[] buffer;
            using (MemoryStream ms = new MemoryStream())
            {
                ExportToStream(dataList, exportFields, sheetName, ms);
                buffer = ms.ToArray();
                ms.Close();
            }
            return buffer;
        }

        /// <summary>
        /// 导出文件 获取MemoryStream
        /// </summary>
        /// <param name="dataList">数据List</param>
        /// <param name="exportFields">导出字段Map</param>
        /// <param name="sheetName">sheet名称</param>
        public static MemoryStream ExportToMemoryStream<T>(List<T> dataList, List<FieldMap> exportFields, string sheetName = null)
        {
            MemoryStream ms = new MemoryStream();
            ExportToStream(dataList, exportFields, sheetName, ms);                
            return ms;
        }

        /// <summary>
        /// 导出到文件 写入Stream
        /// </summary>
        /// <param name="dataList">数据List</param>
        /// <param name="exportFields">导出字段Map</param>
        /// <param name="sheetName">sheet名称</param>
        public static void ExportToStream<T>(List<T> dataList,List<FieldMap> exportFields,string sheetName,Stream stream)
        {
            if (exportFields == null || exportFields.Count<=0)
            {
                throw new Exception("导出字段不能为空");
            }
            #region 生成Excel
            try
            {
                IWorkbook workbook = new XSSFWorkbook();
                ISheet sheet;
                if (!string.IsNullOrEmpty(sheetName))
                {
                    sheet = workbook.CreateSheet(sheetName);
                }
                else
                {
                    sheet = workbook.CreateSheet();
                }
                if (dataList?.Count > 0)
                {
                    Type dataType = dataList[0].GetType();
                    List<PropertyInfo> piList = dataType.GetProperties().ToList();

                    Dictionary<Guid, PropertyInfo> piDic =
                        exportFields.ToDictionary(
                            field => field.Id,
                            field => piList.FirstOrDefault(p => p.Name == field.PiName));


                    IRow row;

                    #region 标题
                    row = sheet.CreateRow(0);
                    for (int i = 0; i < exportFields.Count; i++)
                    {
                        FieldMap fieldItem = exportFields[i];
                        string fieldText = !string.IsNullOrEmpty(fieldItem.FieldText) ? fieldItem.FieldText : fieldItem.PiName; 
                        row.CreateCell(i).SetCellValue(fieldText);
                    }
                    #endregion

                    #region 数据
                    for (int m = 0; m < dataList.Count; m++)
                    {
                        row = sheet.CreateRow(m + 1);
                        for (int i = 0; i < exportFields.Count; i++)
                        {
                            FieldMap map = exportFields[i];

                            if (map.IsRowIndex)
                            {
                                row.CreateCell(i).SetCellValue(m + 1);
                                continue;
                            }

                            PropertyInfo pi = piDic[map.Id];
                            if (pi == null)
                            {
                                throw new Exception($"属性{map.PiName}在{dataType.Name}中不存在");
                            }

                            object piValue = pi.GetValue(dataList[m]);
                            string cellValue = "";
                            if (piValue != null)
                            {
                                if (!string.IsNullOrEmpty(map.FieldFormat))
                                {
                                    if (map.FieldType == FieldType.DateTime)
                                    {
                                        cellValue = ((DateTime)piValue).ToString(map.FieldFormat);
                                    }
                                    if (map.FieldType == FieldType.Boolean)
                                    {
                                        if (!string.IsNullOrEmpty(map.FieldFormat) && map.FieldFormat.IndexOf('|') != -1)
                                        {
                                            string[] boolValues = map.FieldFormat.Split('|');
                                            if (boolValues.Length == 2)
                                            {
                                                cellValue = ((bool)piValue) ? boolValues[0] : boolValues[1];
                                            }
                                        }
                                        else
                                        {
                                            cellValue = piValue.ToString();
                                        }
                                    }
                                    else
                                    {
                                        cellValue = piValue.ToString();
                                    }
                                }
                                else
                                {
                                    cellValue = piValue.ToString();
                                }
                            }
                            row.CreateCell(i).SetCellValue(cellValue);
                        }
                    }
                    #endregion
                }
                workbook.Write(stream);
            }
            catch (Exception ex)
            {
                throw ex;
            }
            #endregion
        }
        
        #endregion
    }
}
