
using System;
using System.Collections.Generic;
using System.Linq;

namespace Jc.Core.AntChain
{
    class Program
    {
        static void Main(string[] args)
        {
            RecordModel insertModel = new RecordModel() {
                RecordId = "Sino-Test008",
                ExamId = "00000000-0000-0000-0000-0000000006",
                PatientName = "风清扬",
                ExamNumber = "Wb35006",
                ExamDate = DateTime.Parse("2020-08-13 16:43"),
                UploadDate = DateTime.Parse("2020-08-13 18:43"),
                Uploader = "赛诺联合检查中心",
                Operation = "文件上传",
                Operator = "小赛云",
                PowerUser = "赛诺联合检查中心",
                PowerDate = DateTime.Parse("2020-08-13 18:43"),
                PowerNote = "检查数据上传",
            };
            string insertHash = SinoChainHelper.Instance.SetExamRecord(insertModel);
            Console.WriteLine($"InsertHash:{insertHash}");

            string insertHash1 = SinoChainHelper.Instance.SetExamRecord(insertModel);
            Console.WriteLine($"InsertHash1:{insertHash1}");

            RecordModel updateModel1 = new RecordModel()
            {
                RecordId = "Sino-Test008",
                ExamId = "00000000-0000-0000-0000-0000000006",
                PatientName = "风清扬",
                ExamNumber = "Wb35006",
                ExamDate = DateTime.Parse("2020-08-13 16:43"),
                UploadDate = DateTime.Parse("2020-08-13 18:43"),
                Uploader = "赛诺联合检查中心",
                Operation = "数据下载",
                Operator = "王医生",
                PowerUser = "赛诺联合检查中心",
                PowerDate = DateTime.Parse("2020-08-14 18:43"),
                PowerNote = "读片医生读片授权",
            };
            string updateHash1 = SinoChainHelper.Instance.SetExamRecord(updateModel1);
            Console.WriteLine($"updateHash1:{updateHash1}");

            RecordModel updateModel2 = new RecordModel()
            {
                RecordId = "Sino-Test008",
                ExamId = "00000000-0000-0000-0000-0000000006",
                PatientName = "风清扬",
                ExamNumber = "Wb35006",
                ExamDate = DateTime.Parse("2020-08-13 16:43"),
                UploadDate = DateTime.Parse("2020-08-13 18:43"),
                Uploader = "赛诺联合检查中心",
                Operation = "数据下载",
                Operator = "风清扬",
                PowerUser = "赛诺联合检查中心",
                PowerDate = DateTime.Parse("2020-08-15 12:43"),
                PowerNote = "用户下载影像数据",
            };
            string updateHash2 = SinoChainHelper.Instance.SetExamRecord(updateModel2);
            Console.WriteLine($"updateHash2:{updateHash2}");


            RecordModel updateModel3 = new RecordModel()
            {
                RecordId = "Sino-Test008",
                ExamId = "00000000-0000-0000-0000-0000000006",
                PatientName = "风清扬",
                ExamNumber = "Wb35006",
                ExamDate = DateTime.Parse("2020-08-13 16:43"),
                UploadDate = DateTime.Parse("2020-08-13 18:43"),
                Uploader = "赛诺联合检查中心",
                Operation = "数据下载",
                Operator = "张医生",
                PowerUser = "赛诺联合检查中心",
                PowerDate = DateTime.Parse("2020-08-15 18:43"),
                PowerNote = "报告检查",
            };
            string updateHash3 = SinoChainHelper.Instance.SetExamRecord(updateModel3);
            Console.WriteLine($"updateHash3:{updateHash3}");

            Console.WriteLine("Press Any Key To Exit.");
            Console.ReadKey();
        }
    }
}
