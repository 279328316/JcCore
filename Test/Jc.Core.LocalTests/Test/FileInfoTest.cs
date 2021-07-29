using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace Jc.Core.LocalTests
{
    public class FileInfoTest
    {
        public void Test()
        {
            string filePath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, @"Test\1.exe");
            FileInfo fileInfo = new FileInfo(filePath);

        }
    }
}
