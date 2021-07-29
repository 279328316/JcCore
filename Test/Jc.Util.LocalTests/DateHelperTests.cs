using Microsoft.VisualStudio.TestTools.UnitTesting;
using Jc;
using System;
using System.Collections.Generic;
using System.Text;

namespace Jc.Tests
{
    [TestClass()]
    public class DateHelperTests
    {
        [TestMethod()]
        public void DateDifTest()
        {
            string str = DateHelper.DateDif(DateTime.Now.AddTicks(-1 * new TimeSpan(26, 10, 12).Ticks));
            string str1 = DateHelper.DateDif(DateTime.Now.AddTicks(-1 * new TimeSpan(6, 10, 12).Ticks));
            string str2 = DateHelper.DateDif(DateTime.Now.AddTicks(-1 * new TimeSpan(56, 10, 12).Ticks));

            Console.WriteLine(str);
            Console.WriteLine(str1);
            Console.WriteLine(str2);
        }
    }
}