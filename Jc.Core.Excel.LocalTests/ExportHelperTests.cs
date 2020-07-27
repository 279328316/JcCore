using Microsoft.VisualStudio.TestTools.UnitTesting;
using Jc.Core.Excel;
using System;
using System.Collections.Generic;
using System.Text;
using System.IO;

namespace Jc.Core.Excel.Tests
{
    [TestClass()]
    public class ExportHelperTests
    {
        [TestMethod()]
        public void ExportToFileTest()
        {
            List<VwAppointmentDto> appointments = GetAppointmentList();

            List<FieldMap> fieldMaps = GetExportFiledMapList();

            string filePath = @"d:\1.xlsx";

            ExcelHelper.ExportToFile(appointments,fieldMaps, filePath);
        }


        [TestMethod()]
        public void ExportToMemoryStreamTest()
        {
            List<VwAppointmentDto> appointments = GetAppointmentList();

            List<FieldMap> fieldMaps = GetExportFiledMapList();

            byte[] buffer;
            using (MemoryStream ms = ExcelHelper.ExportToMemoryStream(appointments, fieldMaps))
            {
                buffer = ms.ToArray();

                string filePath = @"d:\1.xlsx";
                using (FileStream fs = new FileStream(filePath, FileMode.Create, FileAccess.Write))
                {
                    fs.Write(buffer, 0, buffer.Length);
                    fs.Flush();
                }
                ms.Close();
            }
        }



        /// <summary>
        /// 获取预约数据
        /// </summary>
        /// <returns></returns>
        private List<VwAppointmentDto> GetAppointmentList()
        {
            List<VwAppointmentDto> list = new List<VwAppointmentDto>()
            {
                new VwAppointmentDto()
                {
                    PatientName = "张三",
                    PatientSex = Sex.Male,
                    Birthday = new DateTime(1965,1,31),
                    Height = 172,
                    Weight = 65,
                    IsDiabetic = null,
                    IdCardType = IdCardType.IdCard,
                    IdCardNumber = "123456789",
                    PhoneNumber = "185012412251",
                    AppointmentTime = DateTime.Now.Date.AddDays(3),
                    AppointmentState = AppointmentState.Confirmed
                },
                new VwAppointmentDto()
                {
                    PatientName = "李四",
                    PatientSex = Sex.Male,
                    Birthday = new DateTime(1975,1,31),
                    Height = 177,
                    Weight = 67,
                    IsDiabetic = true,
                    IdCardType = IdCardType.IdCard,
                    IdCardNumber = "123456789",
                    PhoneNumber = "187012412251",
                    AppointmentTime = DateTime.Now.Date.AddDays(5),
                    AppointmentState = AppointmentState.Confirmed
                },
                new VwAppointmentDto()
                {
                    PatientName = "王五",
                    PatientSex = Sex.FeMale,
                    Birthday = new DateTime(1985,1,31),
                    Height = 182,
                    Weight = 67,
                    IsDiabetic = false,
                    IdCardType = IdCardType.IdCard,
                    IdCardNumber = "123456789",
                    PhoneNumber = "189012412251",
                    AppointmentTime = DateTime.Now.Date.AddDays(6),
                    AppointmentState = AppointmentState.Confirmed
                },
            };
            return list;
        }

        /// <summary>
        /// 获取导出字段Map
        /// </summary>
        private List<FieldMap> GetExportFiledMapList()
        {
            FieldMapBuilder builder = new FieldMapBuilder();

            builder.Add(
                new FieldRowIndexMap(),
                new FieldMap<VwAppointmentDto>(a => a.AppointmentTime, "预约日期", FieldType.DateTime, "yyyy-MM-dd"),
                new FieldMap<VwAppointmentDto>(a => a.AppointmentTime, "预约时间", FieldType.DateTime, "HH:mm:ss"),
                new FieldMap<VwAppointmentDto>(a => a.ExamNumber, "检查号"),
                new FieldMap<VwAppointmentDto>(a => a.PatientName, "患者姓名"),
                new FieldMap<VwAppointmentDto>(a => a.ExamItemName, "检查项目"),
                new FieldMap<VwAppointmentDto>(a => a.AppointmentStateName, "预约状态"),
                new FieldMap<VwAppointmentDto>(a => a.Age, "患者年龄"),
                new FieldMap<VwAppointmentDto>(a => a.Birthday, "出生日期", FieldType.DateTime, "yyyy-MM-dd"),
                new FieldMap<VwAppointmentDto>(a => a.SexName, "患者性别"),
                new FieldMap<VwAppointmentDto>(a => a.Height, "患者身高"),
                new FieldMap<VwAppointmentDto>(a => a.Weight, "患者体重"),
                new FieldMap<VwAppointmentDto>(a => a.IsDiabetic, "是否糖尿病患者",FieldType.Boolean,"是|"),
                new FieldMap<VwAppointmentDto>(a => a.Diagnosis, "诊断"),
                new FieldMap<VwAppointmentDto>(a => a.IdCardTypeName, "证件类型"),
                new FieldMap<VwAppointmentDto>(a => a.IdCardNumber, "证件号"),
                new FieldMap<VwAppointmentDto>(a => a.PhoneNumber, "手机号"),
                new FieldMap<VwAppointmentDto>(a => a.Note, "备注"));
            List<FieldMap> fieldMaps = builder.GetFieldMaps();

            return fieldMaps;
        }
    }
}