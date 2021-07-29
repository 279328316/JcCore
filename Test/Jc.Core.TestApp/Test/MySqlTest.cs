using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Diagnostics;
using System.Text;

namespace Jc.Core.TestApp.Test
{
    public class MySqlTest
    {
        public void Test()
        {
            GetSortedListTest();
        }

        public void GetSortedListTest()
        {
            List<GuidUserDto> list = Dbc.Db.GetSortList<GuidUserDto>(a => a.IsDelete == false, a => a.NickName);
        }
    }
}
