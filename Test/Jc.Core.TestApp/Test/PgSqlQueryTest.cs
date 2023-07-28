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
        public void Test()
        {
            GetSortedListTest();
        }

        public void GetSortedListTest()
        {
            List<PgUserDto> list0 = Dbc.PgTestDb.GetList<PgUserDto>(a => a.UserName.Contains("a"));
            List<PgUserDto> list = Dbc.PgTestDb.GetList<PgUserDto>(a => a.UserName.ToLower().Contains("A".ToLower()) && a.IsActived);
            List<PgUserDto> list1 = Dbc.PgTestDb.GetList<PgUserDto>(a => a.UserName.ToLower().Contains("A".ToLower()));
            
            List<int?> ids = new List<int?> { 1, 2, 3 };
            List<PgUserDto> list2 = Dbc.PgTestDb.GetList<PgUserDto>(a => ids.Contains(a.Id));
            List<PgUserDto> list3 = Dbc.PgTestDb.GetList<PgUserDto>(a => ids.Contains(a.Id.Value));

            List<string> userNames = new List<string> { "administrator","doctor" };
            List<PgUserDto> list4 = Dbc.PgTestDb.GetList<PgUserDto>(a => userNames.Contains(a.UserName.ToLower()));

            List<PgUserDto> list5 = Dbc.PgTestDb.GetList<PgUserDto>(a => a.UserName.StartsWith("a"));
            List<PgUserDto> list6 = Dbc.PgTestDb.GetList<PgUserDto>(a => a.UserName.ToLower().StartsWith("A".ToLower()) && a.IsActived);
            List<PgUserDto> list7 = Dbc.PgTestDb.GetList<PgUserDto>(a => a.UserName.ToLower().StartsWith("A".ToLower()));

            List<PgUserDto> list8 = Dbc.PgTestDb.GetList<PgUserDto>(a => a.UserName.EndsWith("or"));
            List<PgUserDto> list9 = Dbc.PgTestDb.GetList<PgUserDto>(a => a.UserName.ToLower().EndsWith("oR".ToLower()) && a.IsActived);
            List<PgUserDto> list10 = Dbc.PgTestDb.GetList<PgUserDto>(a => a.UserName.ToLower().EndsWith("OR".ToLower()));

            List<PgUserDto> list11 = Dbc.PgTestDb.GetList<PgUserDto>(a => a.Id<10);
            List<PgUserDto> list12 = Dbc.PgTestDb.GetList<PgUserDto>(a => a.Id < 2 && a.IsActived);
            List<PgUserDto> list13 = Dbc.PgTestDb.GetList<PgUserDto>(a => a.Id < 3);
        }
    }
}
