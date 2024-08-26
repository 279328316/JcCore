using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.Text;
using Jc.Tests;
using System.Data;
using Jc.Database.Query;

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
            DataTable dt = Dbc.Db.GetDataTable<UserDto>();
            if (dt?.Rows.Count > 0)
            {
                for (int i = 0; i < dt.Rows.Count; i++)
                {
                    EntityConvertResult convertResult = new EntityConvertResult();
                    UserDto user = (UserDto)convertor.Invoke(dt.Rows[i], convertResult);
                    //UserDto user = new UserDto(dt.Rows[i]);
                    if (convertResult.IsException)
                    {
                        throw new Exception(convertResult.Message);
                    }
                    users.Add(user);
                }
            }
        }
    }
}