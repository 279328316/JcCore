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
            Console.WriteLine("采用拼接批量SQL插入的方式实现");
            List<UserDto> users = new List<UserDto>();

            Stopwatch sw = new Stopwatch();
            sw.Start();
            for (int i = 0; i < 1000000; i++)
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
                    Sex = i%2,
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
            Console.WriteLine($"构造{users.Count}条记录，共耗时{sw.ElapsedMilliseconds / 1000}秒");
            sw.Reset();
            sw.Start();
            Dbc.StockDb.AddList(users,a=>new { a.Id,a.UserName,a.Email,a.AddDate});
            sw.Stop();
            Console.WriteLine($"插入{users.Count}条记录，共耗时{sw.ElapsedMilliseconds / 1000}秒");
        }
    }
}
