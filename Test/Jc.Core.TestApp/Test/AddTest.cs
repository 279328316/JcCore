using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Diagnostics;
using System.Linq;
using System.Text;

namespace Jc.Core.TestApp.Test
{
    public class AddTest
    {
        public static void Test()
        {
            AddUserTest();
            UpdateTest();
            DeleteTest();
        }

        public static void AddUserTest()
        {
            Console.WriteLine("插入测试");

            if (!Dbc.Db.CheckTableExists<UserDto>())
            {
                Dbc.Db.CreateTable<UserDto>();
            }

            //int clearRows = Dbc.Db.Delete<UserDto>(a => a.Id != 0);
            //Console.WriteLine($"清理数据:{clearRows}");

            Stopwatch sw = new Stopwatch();
            sw.Start();
            int i = Random.Shared.Next(1, 100);
            UserDto user = new UserDto()
            {
                UserName = $"UserName{i}",
                UserPwd = $"UserPwd{i}",
                NickName = $"NickName{i}",
                RealName = $"RealName{i}",
                Email = $"Email{i}@qq.com",
                Avatar = $"Avatar{i}",
                PhoneNo = $"133810{i}".PadRight(11, '0'),
                Sex = i % 3 == 0 ? null : (i % 3 == 1 ? Sex.Male : Sex.Female),
                Birthday = DateTime.Now.AddYears(-1).AddHours(-1 * i),
                IsDelete = i % 2 == 0 ? true : false,
                UserStatus = (UserStatus)(i % 2),
                AddUser = null,
                AddDate = null,
                LastUpdateUser = null,
                LastUpdateDate = null
            };
            sw.Stop();
            Console.WriteLine("执行数据插入...");
            sw.Start();
            Dbc.Db.Add(user);
            sw.Stop();
            Console.WriteLine($"Int插入1条记录，共耗时{sw.ElapsedMilliseconds} Ms");
        }

        public static void UpdateTest()
        {
            Console.WriteLine("更新测试.使用拼接批量SQL方式实现");
            Stopwatch sw = new Stopwatch();
            sw.Start();
            List<UserDto> users = Dbc.Db.GetSortList<UserDto>(null, a => a.Id);
            Console.WriteLine($"查询{users.Count}条记录，共耗时{sw.ElapsedMilliseconds}Ms");
            sw.Reset();
            sw.Start();
            Console.WriteLine("执行更新数据构造...");
            int dataAmount = users.Count;
            for (int i = 0; i < users.Count; i++)
            {
                users[i].UserName = $"UpUserName{i}";
                users[i].UserPwd = $"UserPwd{i}";
                users[i].NickName = $"UpNickName{i}";
                users[i].RealName = $"UpRealName{i}";
                users[i].Email = $"UpEmail{i}@qq.com";
                users[i].Avatar = $"Avatar{i}";
                users[i].PhoneNo = $"133810{i}".PadRight(11, '0');
                users[i].Sex = (Sex)Enum.Parse(typeof(Sex), (i % 2).ToString());
                users[i].Birthday = DateTime.Now.AddYears(-1).AddHours(-1 * i);
                users[i].IsDelete = i % 2 == 0 ? true : false;
                users[i].UserStatus = (UserStatus)(i % 2);
                users[i].LastUpdateUser = Guid.NewGuid();
                users[i].LastUpdateDate = DateTime.Now;
            }
            sw.Stop();
            Console.WriteLine($"构造{users.Count}条记录，共耗时{sw.ElapsedMilliseconds}Ms");
            sw.Reset();
            Console.WriteLine("执行数据更新...");
            sw.Start();
            Dbc.Db.UpdateList(users, a => new { a.Id, a.UserName, a.Email, a.LastUpdateDate, a.NickName });
            sw.Stop();
            Console.WriteLine($"Int更新{users.Count}条记录，共耗时{sw.ElapsedMilliseconds} Ms");
        }

        public static void DeleteTest()
        {
            Console.WriteLine("删除测试.");
            Stopwatch sw = new Stopwatch();
            sw.Start();
            List<UserDto> users = Dbc.Db.GetSortList<UserDto>(a=>!a.IsDelete, a => a.Id);
            Console.WriteLine($"查询{users.Count}条记录，共耗时{sw.ElapsedMilliseconds}Ms");
            sw.Reset();
            sw.Start();
            List<int?> ids = users.Where(a => a.IsDelete).Select(a => a.Id).ToList();
            Dbc.Db.Delete<UserDto>(a => ids.Contains(a.Id));
            sw.Stop();
            Console.WriteLine($"Int更新{users.Count}条记录，共耗时{sw.ElapsedMilliseconds} Ms");
        }
    }
}