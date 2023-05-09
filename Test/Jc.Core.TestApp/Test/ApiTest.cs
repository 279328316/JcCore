using Jc.Database;
using System;
using System.Collections.Generic;
using System.Collections.Specialized;
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

        public void IQueryTest()
        {
            NameValueCollection collection = new NameValueCollection();
            collection.Add("Area", "Account");
            collection.Add("ApiLevels", "1,2");
            collection.Add("Ids", "43EB97E4-F6F2-4768-9EAD-BC977AA3A6C8,5F8AD1CD-A217-4757-8AF0-F7289DB06AD8");
            IQuery<ApiDto> query = Dbc.Db.IQuery<ApiDto>(collection);
            List<ApiDto> list = query.ToList();
        }
    }
}
