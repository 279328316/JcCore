using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Diagnostics;
using System.Text;

namespace Jc.Core.TestApp.Test
{
    public class ListAddTest
    {
        public void Test()
        {
            int dataAmount = 10000;

            AddTest(dataAmount);
            UpdateTest();
            BulkCopyTest(dataAmount);
            AddGuidTest(dataAmount);
            UpdateGuidTest();
            BulkCopyGuidTest(dataAmount);
        }
        public void AddTest(int dataAmount)
        {
            Console.WriteLine("批量插入测试采用拼接批量SQL方式实现");
            List<UserDto> users = new List<UserDto>();

            if (!Dbc.Db.CheckTableExists<UserDto>())
            {
                Dbc.Db.CreateTable<UserDto>();
            }

            int clearRows = Dbc.Db.Delete<UserDto>(a => a.Id != 0);
            Console.WriteLine($"清理数据:{clearRows}");

            Stopwatch sw = new Stopwatch();
            sw.Start();
            for (int i = 0; i < dataAmount; i++)
            {
                users.Add(new UserDto()
                {
                    UserName = $"UserName{i}",
                    UserPwd = $"UserPwd{i}",
                    NickName = $"NickName{i}",
                    RealName = $"RealName{i}",
                    Email = $"Email{i}@qq.com",
                    Avatar = $"Avatar{i}",
                    PhoneNo = $"133810{i}".PadRight(11,'0'),
                    Sex = (Sex)Enum.Parse(typeof(Sex),(i%2).ToString()),
                    Birthday = DateTime.Now.AddYears(-1).AddHours(-1*i),
                    IsDelete = i % 2 == 0 ? true : false,
                    UserStatus = i % 2,
                    AddUser = Guid.NewGuid(),
                    AddDate = DateTime.Now,
                    LastUpdateUser = Guid.NewGuid(),
                    LastUpdateDate = DateTime.Now
                });
            }
            sw.Stop();
            Console.WriteLine($"构造{users.Count}条记录，共耗时{sw.ElapsedMilliseconds}Ms");
            sw.Reset();
            Console.WriteLine("执行数据插入...");
            sw.Start();
            Dbc.Db.AddList(users,a=>new { a.Id,a.UserName,a.Email,a.AddDate},true);
            sw.Stop();
            Console.WriteLine($"Int插入{users.Count}条记录，共耗时{sw.ElapsedMilliseconds } Ms");
        }
        
        public void UpdateTest()
        {
            Console.WriteLine("批量更新测试.使用拼接批量SQL方式实现");
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
                users[i].UserStatus = i % 2;
                users[i].LastUpdateUser = Guid.NewGuid();
                users[i].LastUpdateDate = DateTime.Now;
            }
            sw.Stop();
            Console.WriteLine($"构造{users.Count}条记录，共耗时{sw.ElapsedMilliseconds}Ms");
            sw.Reset();
            Console.WriteLine("执行数据更新...");
            sw.Start();
            Dbc.Db.UpdateList(users,a=>new {a.Id,a.UserName,a.Email,a.LastUpdateDate,a.NickName});
            sw.Stop();
            Console.WriteLine($"Int更新{users.Count}条记录，共耗时{sw.ElapsedMilliseconds } Ms");
        }

        public void BulkCopyTest(int dataAmount)
        {
            Console.WriteLine("批量插入测试采用BulkCopy方式实现");

            if (!Dbc.Db.CheckTableExists<UserDto>())
            {
                Dbc.Db.CreateTable<UserDto>();
            }
            int clearRows = Dbc.Db.Delete<UserDto>(a => a.Id != 0);
            Console.WriteLine($"清理数据:{clearRows}");

            List<UserDto> users = new List<UserDto>();

            Stopwatch sw = new Stopwatch();
            sw.Start();

            for (int i = 0; i < dataAmount; i++)
            {
                users.Add(new UserDto()
                {
                    UserName = $"UserName{i}",
                    UserPwd = $"UserPwd{i}",
                    NickName = $"NickName{i}",
                    RealName = $"RealName{i}",
                    Email = $"Email{i}@qq.com",
                    Avatar = $"Avatar{i}",
                    PhoneNo = $"133810{i}".PadRight(11, '0'),
                    Sex = (Sex)Enum.Parse(typeof(Sex), (i % 2).ToString()),
                    Birthday = DateTime.Now.AddYears(-1).AddHours(-1 * i),
                    IsDelete = i % 2 == 0 ? true : false,
                    UserStatus = i % 2,
                    AddUser = Guid.NewGuid(),
                    AddDate = DateTime.Now,
                    LastUpdateUser = Guid.NewGuid(),
                    LastUpdateDate = DateTime.Now
                });
            }
            sw.Stop();
            Console.WriteLine($"构造{users.Count}条记录，共耗时{sw.ElapsedMilliseconds}Ms");
            sw.Reset();
            Console.WriteLine("执行数据插入...");
            sw.Start();
            Dbc.Db.BulkCopy(users, 1000, 0, true, new Progress<float>((p)=> {
                Console.WriteLine($"进度:{Math.Round(p * 100.0, 2)}%");
            }));
            sw.Stop();
            Console.WriteLine($"Int BulkCopy 插入{users.Count}条记录，共耗时{sw.ElapsedMilliseconds } Ms");
        }

        public void AddGuidTest(int dataAmount)
        {
            Console.WriteLine("Guid 主键测试");
            Console.WriteLine("批量插入测试采用拼接批量SQL方式实现");

            if (!Dbc.Db.CheckTableExists<GUserDto>())
            {
                Dbc.Db.CreateTable<GUserDto>();
            }
            int clearRows = Dbc.Db.Delete<GUserDto>(a => a.Id != Guid.Empty);
            Console.WriteLine($"清理数据:{clearRows}");

            List<GUserDto> users = new List<GUserDto>();

            Stopwatch sw = new Stopwatch();
            sw.Start();
            for (int i = 0; i < dataAmount; i++)
            {
                users.Add(new GUserDto()
                {
                    UserName = $"UserName{i}",
                    UserPwd = $"UserPwd{i}",
                    NickName = $"NickName{i}",
                    RealName = $"RealName{i}",
                    Email = $"Email{i}@qq.com",
                    Avatar = $"Avatar{i}",
                    PhoneNo = $"133810{i}".PadRight(11, '0'),
                    Sex = (Sex)(i % 2),
                    Birthday = DateTime.Now.AddYears(-1).AddHours(-1 * i),
                    WeChatOpenId = $"WeChatOpenId{i}",
                    IsDelete = i % 2 == 0 ? true : false,
                    UserStatus = i % 2,
                    AddUser = Guid.NewGuid(),
                    AddDate = DateTime.Now,
                    LastUpdateUser = Guid.NewGuid(),
                    LastUpdateDate = DateTime.Now
                });
            }
            sw.Stop();
            Console.WriteLine($"构造{users.Count}条记录，共耗时{sw.ElapsedMilliseconds}Ms");
            sw.Reset();
            Console.WriteLine("执行数据插入...");
            sw.Start();
            Dbc.Db.AddList(users, a => new { a.Id, a.UserName, a.Email, a.AddDate }, true);
            sw.Stop();
            Console.WriteLine($"Guid插入{users.Count}条记录，共耗时{sw.ElapsedMilliseconds } Ms");
        }

        public void UpdateGuidTest()
        {
            Console.WriteLine("Guid 主键测试");
            Console.WriteLine("批量更新测试.使用拼接批量SQL方式实现");
            Stopwatch sw = new Stopwatch();
            sw.Start();
            List<GUserDto> users = Dbc.Db.GetSortList<GUserDto>(null, a => a.Id);
            Console.WriteLine($"查询{users.Count}条记录，共耗时{sw.ElapsedMilliseconds}Ms");
            sw.Reset();
            sw.Start();
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
                users[i].Sex = (Sex)(i % 2);
                users[i].Birthday = DateTime.Now.AddYears(-1).AddHours(-1 * i);
                users[i].WeChatOpenId = $"WeChatOpenId{i}";
                users[i].IsDelete = i % 2 == 0 ? true : false;
                users[i].UserStatus = i % 2;
                users[i].LastUpdateUser = Guid.NewGuid();
                users[i].LastUpdateDate = DateTime.Now;
            }
            sw.Stop();
            Console.WriteLine($"构造{users.Count}条记录，共耗时{sw.ElapsedMilliseconds}Ms");
            sw.Reset();
            Console.WriteLine("执行数据更新...");
            sw.Start();
            Dbc.Db.UpdateList(users, a => new { a.UserName, a.Email, a.LastUpdateDate, a.NickName });
            sw.Stop();
            Console.WriteLine($"Guid更新{users.Count}条记录，共耗时{sw.ElapsedMilliseconds } Ms");
        }

        public void BulkCopyGuidTest(int dataAmount)
        {
            Console.WriteLine("Guid 主键测试");
            Console.WriteLine("批量插入测试采用BulkCopy方式实现");

            if (!Dbc.Db.CheckTableExists<GUserDto>())
            {
                Dbc.Db.CreateTable<GUserDto>();
            }
            int clearRows = Dbc.Db.Delete<GUserDto>(a => a.Id != Guid.Empty);
            Console.WriteLine($"清理数据:{clearRows}");

            List<GUserDto> users = new List<GUserDto>();

            Stopwatch sw = new Stopwatch();
            sw.Start();
            for (int i = 0; i < dataAmount; i++)
            {
                users.Add(new GUserDto()
                {
                    UserName = $"UserName{i}",
                    UserPwd = $"UserPwd{i}",
                    NickName = $"NickName{i}",
                    RealName = $"RealName{i}",
                    Email = $"Email{i}@qq.com",
                    Avatar = $"Avatar{i}",
                    PhoneNo = $"133810{i}".PadRight(11, '0'),
                    Sex = (Sex)(i % 2),
                    Birthday = DateTime.Now.AddYears(-1).AddHours(-1 * i),
                    WeChatOpenId = $"WeChatOpenId{i}",
                    IsDelete = i % 2 == 0 ? true : false,
                    UserStatus = i % 2,
                    AddUser = Guid.NewGuid(),
                    AddDate = DateTime.Now,
                    LastUpdateUser = Guid.NewGuid(),
                    LastUpdateDate = DateTime.Now
                });
            }
            {   //Add 事务 异常抛出 Test
                users[users.Count - 1].Id = Guid.NewGuid();
                users[users.Count - 2].Id = users[users.Count - 1].Id;
            }
            sw.Stop();
            Console.WriteLine($"构造{users.Count}条记录，共耗时{sw.ElapsedMilliseconds}Ms");
            sw.Reset();
            Console.WriteLine("执行数据插入...");
            sw.Start();
            Dbc.Db.BulkCopy(users, 1000, 0, true, new Progress<float>((p) => {
                Console.WriteLine($"进度:{Math.Round(p * 100.0, 2)}%");
            }));
            sw.Stop();
            Console.WriteLine($"Guid BulkCopy插入{users.Count}条记录，共耗时{sw.ElapsedMilliseconds } Ms");
        }
    }
}
