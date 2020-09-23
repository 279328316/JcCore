using Microsoft.VisualStudio.TestTools.UnitTesting;
using Jc.Core.Helper;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using Jc.Core.Util;

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

        [TestMethod()]
        public void DownloadTest()
        {
            HttpHelper httpHelper = new HttpHelper();
            string url = "http://docs.cn-healthcare.com/sharedoc/img_files_fox/20161103/3101624402888704/3101624402888704_{0}.png";
            string dir = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Temp");
            if (!Directory.Exists(dir))
            {
                Directory.CreateDirectory(dir);
            }
            for (int i = 1; i <= 56; i++)
            {
                string filePath = Path.Combine(dir,$"{i}.jpg");
                string curUrl = string.Format(url, i);
                httpHelper.Download(curUrl, filePath,true);
            }
        }
    }
}