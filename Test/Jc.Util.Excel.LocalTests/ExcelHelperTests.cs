using Microsoft.VisualStudio.TestTools.UnitTesting;
using Jc.Excel;
using System;
using System.Collections.Generic;
using System.Text;
using Jc.Util.Excel.LocalTests;

namespace Jc.Excel.Tests
{
    [TestClass()]
    public class ExcelHelperTests
    {
        [TestMethod()]
        public void ExportToFileTest()
        {
            List<Student> students = new List<Student>()
            {
                new Student(){ Name = "张三" , ClassName = "一年级一班" },
                new Student(){ Name = "李四" , ClassName = "一年级二班" }
            };

            FieldMapBuilder builder = new FieldMapBuilder();
            builder.Add(new FieldRowIndexMap());
            builder.Add(new FieldMap<Student>(a => a.Name, "姓名"));
            builder.Add(new FieldMap<Student>(a => a.ClassName, "班级"));
            builder.Add(new FieldMap("Name", "姓名"));
            builder.Add(new FieldMap("ClassName", "班级"));

            List<FieldMap> testMaps = builder.FieldMaps;

            List<FieldMap> fieldMaps = new List<FieldMap>
            {
                new FieldRowIndexMap(),
                new FieldMap<Student>(a => a.Name, "姓名"),
                new FieldMap<Student>(a => a.ClassName, "班级"),
                new FieldMap("Name", "姓名"),
                new FieldMap("ClassName", "班级")
            };

            ExcelHelper.ExportToFile(students, fieldMaps, @"D:\1.xlsx");
        }
    }
}