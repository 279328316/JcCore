using Microsoft.VisualStudio.TestTools.UnitTesting;
using Jc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Jc.Tests;

namespace Jc.Tests
{
    [TestClass()]
    public class DbContextTests
    {
        [TestMethod()]
        public void GetListTest()
        {
            try
            {
                List<UserDto> users = Dbc.Db.GetList<UserDto>(a=>a.Email.Contains("@") && a.IsDelete == false);
            }
            catch (Exception ex)
            { 
            }
        }
    }
}