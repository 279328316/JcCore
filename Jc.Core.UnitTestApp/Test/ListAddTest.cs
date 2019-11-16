using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Diagnostics;
using System.Text;

namespace Jc.Core.UnitTestApp
{
    public class ListAddTest
    {
        public void Test()
        {
            AddTest();
            UpdateTest();
            AddGuidTest();
            UpdateGuidTest();
        }
        public void AddTest()
        {
            Console.WriteLine("批量插入测试采用拼接批量SQL方式实现");
            List<UserDto> users = new List<UserDto>();

            Stopwatch sw = new Stopwatch();
            sw.Start();
            int dataAmount = 1000;
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
                    //Sex = (Sex)Enum.Parse(typeof(Sex),(i%2).ToString()),
                    Birthday = DateTime.Now.AddYears(-1).AddHours(-1*i),
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
            Dbc.StockDb.GetSubTableDbContext<UserDto>("2019").AddList(users,a=>new { a.Id,a.UserName,a.Email,a.AddDate});
            sw.Stop();
            Console.WriteLine($"Int插入{users.Count}条记录，共耗时{sw.ElapsedMilliseconds / 1000}S");
        }
        
        public void UpdateTest()
        {
            Console.WriteLine("批量更新测试.使用拼接批量SQL方式实现");
            Stopwatch sw = new Stopwatch();
            sw.Start();
            List<UserDto> users = Dbc.StockDb.GetSubTableDbContext<UserDto>("2019").GetSortList<UserDto>(null, a => a.Id);
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
                //users[i].Sex = (Sex)Enum.Parse(typeof(Sex), (i % 2).ToString());
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
            Dbc.StockDb.GetSubTableDbContext<UserDto>("2019").UpdateList(users,a=>new {a.Id,a.UserName,a.Email,a.LastUpdateDate,a.NickName});
            sw.Stop();
            Console.WriteLine($"Int更新{users.Count}条记录，共耗时{sw.ElapsedMilliseconds / 1000}S");
        }


        public void AddGuidTest()
        {
            Console.WriteLine("Guid 主键测试");
            Console.WriteLine("批量插入测试采用拼接批量SQL方式实现");
            List<GUserDto> users = new List<GUserDto>();

            Stopwatch sw = new Stopwatch();
            sw.Start();
            int dataAmount = 1000;
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
                    Sex = i % 2,
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
            Dbc.StockDb.GetSubTableDbContext<GUserDto>("2019").AddList(users, a => new { a.Id, a.UserName, a.Email, a.AddDate });
            sw.Stop();
            Console.WriteLine($"Guid插入{users.Count}条记录，共耗时{sw.ElapsedMilliseconds / 1000}S");
        }

        public void UpdateGuidTest()
        {
            Console.WriteLine("Guid 主键测试");
            Console.WriteLine("批量更新测试.使用拼接批量SQL方式实现");
            Stopwatch sw = new Stopwatch();
            sw.Start();
            List<GUserDto> users = Dbc.StockDb.GetSubTableDbContext<GUserDto>("2019").GetSortList<GUserDto>(null, a => a.Id);
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
                users[i].Sex = i % 2;
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
            Dbc.StockDb.GetSubTableDbContext<GUserDto>("2019").UpdateList(users, a => new { a.UserName, a.Email, a.LastUpdateDate, a.NickName });
            sw.Stop();
            Console.WriteLine($"Guid更新{users.Count}条记录，共耗时{sw.ElapsedMilliseconds / 1000}S");
        }
    }
}
