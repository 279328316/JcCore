using Microsoft.VisualStudio.TestTools.UnitTesting;
using Jc.Core.Helper;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Jc.Core.Helper.Tests
{
    [TestClass()]
    public class HttpHelperTests
    {
        [TestMethod()]
        public void GetTest()
        {
            HttpHelper httpHelper = new HttpHelper();
            string url = "https://stock.gtimg.cn/data/index.php?appn=rank&t=ranka/chr&p=1&o=0&l=10000&v=list_data";
            string result = httpHelper.Get(url);
        }
    }
}