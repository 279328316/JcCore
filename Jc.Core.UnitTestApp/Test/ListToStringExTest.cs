using System;
using System.Collections.Generic;
using System.Text;

namespace Jc.Core.UnitTestApp
{
    public class ListToStringExTest
    {
        public void Test()
        {
            List<string> list = new List<string>() { null,"1",null,"Test"};
            string str = list.ToString(",");            
        }
    }
}
