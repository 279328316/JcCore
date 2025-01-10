using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Diagnostics;
using System.Linq;
using System.Security.Cryptography;
using System.Text;

namespace Jc.Core.TestApp.Test
{
    public class PgSqlQueryTest
    {
        private class QueryObj
        {
            public int? Id { get; set; }

            public string UserName { get; set; }

            public List<int?> Ids { get; set; }

            public int MinId { get; set; }

            public int MaxId { get; set; }

            public DateTime MinDt { get; set; }

            public DateTime MaxDt { get; set; }

            public QueryObj Query { get; set; }
        }

        public void Test()
        {
            AddUserTest();
            UpdateTest();
            NTest();
            CommonCompareTest();
            DateTimeCompareTest();
            ContainsTest();
            //FieldCompareTest();
        }

        public void ContainsTest()
        {
            var queryObj = new QueryObj() { UserName = "Abc", Ids = new List<int?> { 1, 2, 3 } };
            queryObj.Query = queryObj;

            List<PgUserDto> list = null;
            list = Dbc.PgTestDb.GetList<PgUserDto>(a => a.UserName.Contains("a"));
            list = Dbc.PgTestDb.GetList<PgUserDto>(a => a.UserName.ToLower().Contains("A".ToLower()) && a.IsActived);
            list = Dbc.PgTestDb.GetList<PgUserDto>(a => a.UserName.ToLower().Contains("A".ToLower()));
            list = Dbc.PgTestDb.GetList<PgUserDto>(a => a.UserName.ToLower() == "A".ToLower());

            list = Dbc.PgTestDb.GetList<PgUserDto>(a => a.UserName.ToLower().Contains("a") && a.IsActived);
            list = Dbc.PgTestDb.GetList<PgUserDto>(a => a.UserName.ToLower().Contains(queryObj.UserName) && a.IsActived);
            list = Dbc.PgTestDb.GetList<PgUserDto>(a => a.UserName.ToLower().Contains(queryObj.UserName.ToLower()) && a.IsActived);
            list = Dbc.PgTestDb.GetList<PgUserDto>(a => a.UserName.ToLower().Contains("ABC".ToLower()) && a.IsActived);
            list = Dbc.PgTestDb.GetList<PgUserDto>(a => a.UserName.ToLower().Contains("ABC".ToLower().Substring(0, 1).ToUpper()) && a.IsActived);
            list = Dbc.PgTestDb.GetList<PgUserDto>(a => a.UserName.ToLower().Contains("ABC".ToLower().Substring(0, 1)) && a.IsActived);
            list = Dbc.PgTestDb.GetList<PgUserDto>(a => a.UserName.ToLower().Contains("ABC".Substring(0, 1).ToLower()) && a.IsActived);
            list = Dbc.PgTestDb.GetList<PgUserDto>(a => a.UserName.ToLower().Contains(queryObj.UserName.Substring(0, 1).ToLower()) && a.IsActived);

            List<int?> ids = new List<int?> { 1, 2, 3 };
            list = Dbc.PgTestDb.GetList<PgUserDto>(a => ids.Contains(a.Id));
            list = Dbc.PgTestDb.GetList<PgUserDto>(a => ids.Contains(a.Id));

            //list = Dbc.PgTestDb.GetList<PgUserDto>(a => ids[0] == a.Id);
            list = Dbc.PgTestDb.GetList<PgUserDto>(a => a.Id == ids[0]);
            //list = Dbc.PgTestDb.GetList<PgUserDto>(a => ids[0].Equals(a.Id));
            list = Dbc.PgTestDb.GetList<PgUserDto>(a => a.Id.Equals(ids[0]));

            list = Dbc.PgTestDb.GetList<PgUserDto>(a => queryObj.Ids.Contains(a.Id));
            list = Dbc.PgTestDb.GetList<PgUserDto>(a => queryObj.Ids.Contains(a.Id));

            List<string> userNames = new List<string> { "abc", "test" };
            list = Dbc.PgTestDb.GetList<PgUserDto>(a => userNames.Contains(a.UserName.ToLower()));

            list = Dbc.PgTestDb.GetList<PgUserDto>(a => a.UserName.StartsWith("a"));
            list = Dbc.PgTestDb.GetList<PgUserDto>(a => a.UserName.ToLower().StartsWith("A".ToLower()) && a.IsActived);
            list = Dbc.PgTestDb.GetList<PgUserDto>(a => a.UserName.ToLower().StartsWith("A".ToLower()));

            list = Dbc.PgTestDb.GetList<PgUserDto>(a => a.UserName.EndsWith("or"));
            list = Dbc.PgTestDb.GetList<PgUserDto>(a => a.UserName.ToLower().EndsWith("oR".ToLower()) && a.IsActived);
            list = Dbc.PgTestDb.GetList<PgUserDto>(a => a.UserName.ToLower().EndsWith("OR".ToLower()));
        }

        private void NTest()
        {
            var queryObj = new QueryObj() { UserName = "Abc", Id = 1, Ids = new List<int?> { 1, 2, 3 }, MinDt = DateTime.Now };
            queryObj.Query = queryObj;

            List<PgUserDto> list = null;
            list = Dbc.PgTestDb.GetList<PgUserDto>(a => a.UserName == queryObj.UserName);
            list = Dbc.PgTestDb.GetList<PgUserDto>(a => a.UserName == queryObj.Query.UserName);
            list = Dbc.PgTestDb.GetList<PgUserDto>(a => a.UserName.ToLower() == queryObj.UserName.ToLower());
            list = Dbc.PgTestDb.GetList<PgUserDto>(a => a.Id == 1);
            list = Dbc.PgTestDb.GetList<PgUserDto>(a => a.Id == queryObj.Id);
            list = Dbc.PgTestDb.GetList<PgUserDto>(a => queryObj.Ids.Contains(a.Id));
            list = Dbc.PgTestDb.GetList<PgUserDto>(a => queryObj.Ids.Contains(a.Id) && a.IsActived
                                                                    && a.LastUpdateDate >= DateTime.Parse(DateTime.Now.ToString("yyyy-MM-01"))
                                                                    && a.LastUpdateDate >= queryObj.MinDt.AddDays(1));

            //list = Dbc.PgTestDb.GetList<PgUserDto>(a => "Abc".ToLower() == a.UserName.ToLower());
            //list = Dbc.PgTestDb.GetList<PgUserDto>(a => "Abcd".Substring(0, 3).ToLower() == a.UserName.ToLower());
            //list = Dbc.PgTestDb.GetList<PgUserDto>(a => queryObj.UserName.Substring(0, 3).ToLower() == a.UserName.ToLower());
        }

        private void CommonCompareTest()
        {
            int minId = 1;
            int maxId = 4;
            var queryObj = new QueryObj() { UserName = "Abc", Ids = new List<int?> { 1, 2, 3 }, MinId = minId, MaxId = maxId };

            List<PgUserDto> list = null;
            list = Dbc.PgTestDb.GetList<PgUserDto>(a => a.Id == 1);
            list = Dbc.PgTestDb.GetList<PgUserDto>(a => a.Id > 1);
            list = Dbc.PgTestDb.GetList<PgUserDto>(a => a.Id >= 1);
            list = Dbc.PgTestDb.GetList<PgUserDto>(a => a.Id < 4);
            list = Dbc.PgTestDb.GetList<PgUserDto>(a => a.Id <= 4);

            list = Dbc.PgTestDb.GetList<PgUserDto>(a => a.Id == 1 + 1);
            list = Dbc.PgTestDb.GetList<PgUserDto>(a => a.Id > 1 + 1);
            list = Dbc.PgTestDb.GetList<PgUserDto>(a => a.Id >= 1 + 1);
            list = Dbc.PgTestDb.GetList<PgUserDto>(a => a.Id < 4 + 1);
            list = Dbc.PgTestDb.GetList<PgUserDto>(a => a.Id <= 4 + 1);

            //list = Dbc.PgTestDb.GetList<PgUserDto>(a => 1 == a.Id);
            //list = Dbc.PgTestDb.GetList<PgUserDto>(a => 1 < a.Id);
            //list = Dbc.PgTestDb.GetList<PgUserDto>(a => 1 <= a.Id);
            //list = Dbc.PgTestDb.GetList<PgUserDto>(a => 4 > a.Id);
            //list = Dbc.PgTestDb.GetList<PgUserDto>(a => 4 >= a.Id);

            //list = Dbc.PgTestDb.GetList<PgUserDto>(a => 1 + 1 == a.Id);
            //list = Dbc.PgTestDb.GetList<PgUserDto>(a => 1 + 1 < a.Id);
            //list = Dbc.PgTestDb.GetList<PgUserDto>(a => 1 + 1 <= a.Id);
            //list = Dbc.PgTestDb.GetList<PgUserDto>(a => 4 + 1 > a.Id);
            //list = Dbc.PgTestDb.GetList<PgUserDto>(a => 4 + 1 >= a.Id);

            list = Dbc.PgTestDb.GetList<PgUserDto>(a => a.Id == minId);
            list = Dbc.PgTestDb.GetList<PgUserDto>(a => a.Id > minId);
            list = Dbc.PgTestDb.GetList<PgUserDto>(a => a.Id >= minId);
            list = Dbc.PgTestDb.GetList<PgUserDto>(a => a.Id < maxId);
            list = Dbc.PgTestDb.GetList<PgUserDto>(a => a.Id <= maxId);
            //list = Dbc.PgTestDb.GetList<PgUserDto>(a => minId == a.Id);
            //list = Dbc.PgTestDb.GetList<PgUserDto>(a => minId < a.Id);
            //list = Dbc.PgTestDb.GetList<PgUserDto>(a => minId <= a.Id);
            //list = Dbc.PgTestDb.GetList<PgUserDto>(a => maxId > a.Id);
            //list = Dbc.PgTestDb.GetList<PgUserDto>(a => maxId >= a.Id);

            list = Dbc.PgTestDb.GetList<PgUserDto>(a => a.Id == minId + 1);
            list = Dbc.PgTestDb.GetList<PgUserDto>(a => a.Id > minId + 1);
            list = Dbc.PgTestDb.GetList<PgUserDto>(a => a.Id >= minId + 1);
            list = Dbc.PgTestDb.GetList<PgUserDto>(a => a.Id < maxId + 1);
            list = Dbc.PgTestDb.GetList<PgUserDto>(a => a.Id <= maxId + 1);

            list = Dbc.PgTestDb.GetList<PgUserDto>(a => a.Id > queryObj.MinId);
            list = Dbc.PgTestDb.GetList<PgUserDto>(a => a.Id >= queryObj.MinId);
            list = Dbc.PgTestDb.GetList<PgUserDto>(a => a.Id < queryObj.MaxId);
            list = Dbc.PgTestDb.GetList<PgUserDto>(a => a.Id <= queryObj.MaxId);

            list = Dbc.PgTestDb.GetList<PgUserDto>(a => a.Id > queryObj.MinId + 1);
            list = Dbc.PgTestDb.GetList<PgUserDto>(a => a.Id >= queryObj.MinId + 1);
            list = Dbc.PgTestDb.GetList<PgUserDto>(a => a.Id < queryObj.MaxId + 1);
            list = Dbc.PgTestDb.GetList<PgUserDto>(a => a.Id <= queryObj.MaxId + 1);
        }

        private void DateTimeCompareTest()
        {
            int minId = 1;
            int maxId = 4;
            DateTime minDt = DateTime.Parse("2022-06-20");
            DateTime maxDt = DateTime.Parse("2022-06-30");
            var queryObj = new QueryObj()
            {
                UserName = "Abc",
                Ids = new List<int?> { 1, 2, 3 },
                MinId = minId,
                MaxId = maxId,
                MinDt = minDt,
                MaxDt = maxDt
            };

            List<PgUserDto> list = null;
            list = Dbc.PgTestDb.GetList<PgUserDto>(a => a.LastUpdateDate >= DateTime.Now);
            list = Dbc.PgTestDb.GetList<PgUserDto>(a => a.LastUpdateDate > DateTime.Now);
            list = Dbc.PgTestDb.GetList<PgUserDto>(a => a.LastUpdateDate >= DateTime.Now);
            list = Dbc.PgTestDb.GetList<PgUserDto>(a => a.LastUpdateDate < DateTime.Now);
            list = Dbc.PgTestDb.GetList<PgUserDto>(a => a.LastUpdateDate <= DateTime.Now);
            //list = Dbc.PgTestDb.GetList<PgUserDto>(a => DateTime.Now == a.LastUpdateDate);
            //list = Dbc.PgTestDb.GetList<PgUserDto>(a => DateTime.Now < a.LastUpdateDate);
            //list = Dbc.PgTestDb.GetList<PgUserDto>(a => DateTime.Now <= a.LastUpdateDate);
            //list = Dbc.PgTestDb.GetList<PgUserDto>(a => DateTime.Now > a.LastUpdateDate);
            //list = Dbc.PgTestDb.GetList<PgUserDto>(a => DateTime.Now >= a.LastUpdateDate);

            list = Dbc.PgTestDb.GetList<PgUserDto>(a => a.LastUpdateDate == minDt);
            list = Dbc.PgTestDb.GetList<PgUserDto>(a => a.LastUpdateDate > minDt);
            list = Dbc.PgTestDb.GetList<PgUserDto>(a => a.LastUpdateDate >= minDt);
            list = Dbc.PgTestDb.GetList<PgUserDto>(a => a.LastUpdateDate < maxDt);
            list = Dbc.PgTestDb.GetList<PgUserDto>(a => a.LastUpdateDate <= maxDt);
            //list = Dbc.PgTestDb.GetList<PgUserDto>(a => minDt == a.LastUpdateDate);
            //list = Dbc.PgTestDb.GetList<PgUserDto>(a => minDt < a.LastUpdateDate);
            //list = Dbc.PgTestDb.GetList<PgUserDto>(a => minDt <= a.LastUpdateDate);
            //list = Dbc.PgTestDb.GetList<PgUserDto>(a => maxDt > a.LastUpdateDate);
            //list = Dbc.PgTestDb.GetList<PgUserDto>(a => maxDt >= a.LastUpdateDate);

            list = Dbc.PgTestDb.GetList<PgUserDto>(a => a.LastUpdateDate > queryObj.MinDt);
            list = Dbc.PgTestDb.GetList<PgUserDto>(a => a.LastUpdateDate >= queryObj.MinDt);
            list = Dbc.PgTestDb.GetList<PgUserDto>(a => a.LastUpdateDate < queryObj.MaxDt);
            list = Dbc.PgTestDb.GetList<PgUserDto>(a => a.LastUpdateDate <= queryObj.MaxDt);
        }

        private void FieldCompareTest()
        {
            var queryObj = new { UserName = "Abc", Ids = new List<int?> { 1, 2, 3 } };

            List<PgUserDto> list;
            list = Dbc.PgTestDb.GetList<PgUserDto>(a => a.UserName == a.FirstName);
            list = Dbc.PgTestDb.GetList<PgUserDto>(a => a.UserName.ToLower() == a.FirstName);
            list = Dbc.PgTestDb.GetList<PgUserDto>(a => a.UserName == a.FirstName.ToLower());
            list = Dbc.PgTestDb.GetList<PgUserDto>(a => a.UserName.ToLower() == a.FirstName.ToLower());
        }

        public static void AddUserTest()
        {
            Console.WriteLine("插入测试");

            if (!Dbc.PgTestDb.CheckTableExists<PgUserDto>())
            {
                Dbc.PgTestDb.CreateTable<PgUserDto>();
            }

            //int clearRows = Dbc.PgTestDb.Delete<PgUserDto>(a => a.Id != 0);
            //Console.WriteLine($"清理数据:{clearRows}");

            Stopwatch sw = new Stopwatch();
            sw.Start();
            int i = Random.Shared.Next(1, 100);
            PgUserDto user = new PgUserDto()
            {
                UserName = $"UserName{i}",
                UserPwd = $"UserPwd{i}",
                LastUpdateDate = null
            };
            sw.Stop();
            Console.WriteLine("执行数据插入...");
            sw.Start();
            Dbc.PgTestDb.Add(user);
            sw.Stop();
            Console.WriteLine($"Int插入1条记录，共耗时{sw.ElapsedMilliseconds} Ms");
        }

        public static void UpdateTest()
        {
            Console.WriteLine("更新测试.使用拼接批量SQL方式实现");
            Stopwatch sw = new Stopwatch();
            sw.Start();
            List<PgUserDto> users = Dbc.PgTestDb.GetSortList<PgUserDto>(null, a => a.Id);
            Console.WriteLine($"查询{users.Count}条记录，共耗时{sw.ElapsedMilliseconds}Ms");
            sw.Reset();
            sw.Start();
            Console.WriteLine("执行更新数据构造...");
            int dataAmount = users.Count;
            for (int i = 0; i < users.Count; i++)
            {
                users[i].UserName = $"UpUserName{i}";
                users[i].UserPwd = $"UserPwd{i}";
                users[i].LastUpdateDate = DateTime.Now;
            }
            sw.Stop();
            Console.WriteLine($"构造{users.Count}条记录，共耗时{sw.ElapsedMilliseconds}Ms");
            sw.Reset();
            Console.WriteLine("执行数据更新...");
            sw.Start();
            Dbc.PgTestDb.UpdateList(users, a => new { a.Id, a.UserName, a.LastUpdateDate });
            sw.Stop();
            Console.WriteLine($"Int更新{users.Count}条记录，共耗时{sw.ElapsedMilliseconds} Ms");
        }
    }
}