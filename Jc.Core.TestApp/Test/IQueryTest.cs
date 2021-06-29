using System;
using System.Collections.Generic;
using System.Text;
using Jc.Core;

namespace Jc.Core.TestApp.Test
{
    public class IQueryTest
    {
        public void Test()
        {
            UserDto user = new UserDto() { IsDelete = false,Birthday = DateTime.Parse("2020-06-15")};
            Dbc.Db.Set(user);

            try
            {
                DateTime? dateTime = null;
                List<UserDto> users = Dbc.Db.GetList<UserDto>(a => !a.IsDelete && a.Birthday >= dateTime.Value.AddDays(1));
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Null Date Test,Error:{ex.Message}");
            }

            try
            {
                DateTime? dateTime = DateTime.Parse("2020-06-14");
                List<UserDto> users = Dbc.Db.GetList<UserDto>(a => !a.IsDelete && a.Birthday >= dateTime.Value.AddDays(1));

                Console.WriteLine($"Date Test,Users:{users.Count}");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error:{ex.Message}");
            }
        }


        public void BoolQueryTest()
        {
            bool? isDeleted = null;
            List<UserDto> queryUsers = Dbc.Db.GetList<UserDto>(a => a.IsDelete == isDeleted);

            List<UserDto> deletedUsers = Dbc.Db.GetList<UserDto>(a => a.IsDelete);

            List<UserDto> users = Dbc.Db.GetList<UserDto>(a => !a.IsDelete);

        }
    }
}
