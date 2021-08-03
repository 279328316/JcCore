using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.Text;
using Jc.Tests;
using System.Data;
using Jc.Data.Query;

namespace Jc.Core.Data.Query.Tests
{
    [TestClass()]
    public class EntityConvertorTests
    {
        [TestMethod()]
        public void CreateEntityConvertorTest()
        {
            EntityConvertorDelegate convertor = EntityConvertor.CreateEntityConvertor<UserDto>();
            List<UserDto> users = new List<UserDto>();
            DataTable dt = Dbc.Db.GetDataTable("Select * from t_User");
            if (dt?.Rows.Count > 0)
            {
                for (int i = 0; i < dt.Rows.Count; i++)
                {
                    //UserDto user = convertor.ConvertDto(dt.Rows[i]);
                    UserDto user = new UserDto(dt.Rows[i]);
                    users.Add(user);
                }
            }
        }
    }
}