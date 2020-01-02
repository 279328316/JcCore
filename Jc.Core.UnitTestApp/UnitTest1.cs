using System;
using System.Collections.Generic;
using System.Data;
using Jc.Core.Data.Query;
using Jc.Core.UnitTestApp.Test;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Jc.Core.UnitTestApp
{
    [TestClass]
    public class UnitTest1
    {
        [TestMethod]
        public void ConvertDtoTest()
        {
            DataRowToDtoTest.Test();
        }

        [TestMethod]
        public void EmitGenerateSetMethodTest()
        {
            EmitTest.ILGenerateSetValueMethodContent<UserDto>();
        }

        [TestMethod]
        public void EmitCreateTest()
        {
            IEntityConvertor<UserDto> convertor = EmitTest.GenerateEntityConvertor<UserDto>();
            //return;
            List<UserDto> users = new List<UserDto>();
            DataTable dt = Dbc.Db.GetDataTable("Select * from t_User");
            if (dt?.Rows.Count > 0)
            {
                for (int i = 0; i < dt.Rows.Count; i++)
                {
                    UserDto user = convertor.ConvertDto(dt.Rows[i]);
                    users.Add(user);
                }
            }
        }


        [TestMethod]
        public void PostgresqlCreateTest()
        {
            string connectString = "Server=10.10.11.100;Port=5432;Database=PETCT_PUMC;UserId=nice;Password=nice;";
            DbContext db = DbContext.CreateDbContext(connectString, DatabaseType.PostgreSql);
            db.ConTest();
        }
    }
}
