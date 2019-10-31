using System;
using System.Collections.Generic;
using System.Text;

namespace Jc.Core.TestApp.Test
{
    public class ApiTest
    {
        public void Test()
        {
            string keywords = "Test";
            List<string> areas = new List<string>();
            List<int> levels = new List<int>() { 1,2};
            List<ApiDto> list = Dbc.Db.GetList<ApiDto>(a => areas.Contains(a.Area));
            list = Dbc.Db.GetList<ApiDto>(a => levels.Contains(a.ApiLevel.Value));
            list = Dbc.Db.GetList<ApiDto>(a => a.Name.Contains(keywords.ToString()));
        }
    }
}
