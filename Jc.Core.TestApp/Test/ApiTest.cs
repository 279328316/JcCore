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
            List<int> levels = new List<int>() { 1, 2 };
            List<Guid?> ids = new List<Guid?>() { Guid.Parse("1140f378-d1a3-407e-8864-68c2dab2ae52") };
            List<ApiDto> list = Dbc.Db.GetList<ApiDto>(a => areas.Contains(a.Area));
            list = Dbc.Db.GetList<ApiDto>(a => levels.Contains(a.ApiLevel.Value));
            list = Dbc.Db.GetList<ApiDto>(a => ids.Contains(a.Id));
            list = Dbc.Db.GetList<ApiDto>(a => a.Name.Contains(keywords.ToString()));
        }
    }
}
