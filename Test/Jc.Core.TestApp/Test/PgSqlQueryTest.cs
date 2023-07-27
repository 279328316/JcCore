using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Diagnostics;
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
            List<PgUserDto> list = Dbc.PgTestDb.GetList<PgUserDto>(a => a.UserName.ToLower().Contains("A".ToLower()) && a.IsActived);
        }
    }
}
