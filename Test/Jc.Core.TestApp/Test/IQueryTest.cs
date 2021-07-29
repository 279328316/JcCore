using System;
using System.Collections.Generic;
using System.Text;
using Jc;

namespace Jc.Core.TestApp.Test
{
    public class IQueryTest
    {
        public void Test()
        {
            TaskDto task1 = new TaskDto() { RunCount = 1, MaxRunCount = 10 };
            TaskDto task2 = new TaskDto() { RunCount = 12, MaxRunCount = 10 };
            TaskDto task3 = new TaskDto() { RunCount = 13, MaxRunCount = 10 };
            TaskDto task4 = new TaskDto() { RunCount = 1, MaxRunCount = 10 };
            TaskDto task5 = new TaskDto() { RunCount = 1, MaxRunCount = 10 };
            List<TaskDto> tasks = new List<TaskDto>() { task1, task2, task3, task4, task5 };
            Dbc.Db.SetList(tasks);

            try
            {
                List<TaskDto> taskList = Dbc.Db.GetList<TaskDto>(a => a.RunCount > a.MaxRunCount);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Field Compare Test,Error:{ex.Message}");
            }
        }

        public void FieldCompareTest()
        {
            TaskDto task1 = new TaskDto() { RunCount = 1, MaxRunCount = 10 };
            TaskDto task2 = new TaskDto() { RunCount = 12, MaxRunCount = 10 };
            TaskDto task3 = new TaskDto() { RunCount = 13, MaxRunCount = 10 };
            TaskDto task4 = new TaskDto() { RunCount = 1, MaxRunCount = 10 };
            TaskDto task5 = new TaskDto() { RunCount = 1, MaxRunCount = 10 };
            List<TaskDto> tasks = new List<TaskDto>() { task1, task2, task3, task4, task5 };
            Dbc.Db.SetList(tasks);

            try
            {
                List<TaskDto> taskList = Dbc.Db.GetList<TaskDto>(a => a.RunCount > a.MaxRunCount);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Field Compare Test,Error:{ex.Message}");
            }
        }

        public void NullValueQueryTest()
        {
            UserDto user = new UserDto() { IsDelete = false, Birthday = DateTime.Parse("2020-06-15") };
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
                DateTime? dateTime = null;
                List<UserDto> users = Dbc.Db.GetList<UserDto>(a => !a.IsDelete && a.Birthday >= dateTime);

                Console.WriteLine($"Null Date Compare Test:{users.Count}");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Null Date Compare Test,Error:{ex.Message}");
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
