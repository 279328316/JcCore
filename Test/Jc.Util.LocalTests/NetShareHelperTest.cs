using Jc.Helper;
using Jc;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace Jc.Tests
{
    public class NetShareHelperTest
    {
        public void Test()
        {
            string shareUser = "nice";
            string sharePwd = "nice";
            string vnetForN = @"\\z820-test\PoleStar";
            string vnetForT = @"\\10.10.11.199\Share";
            string vnetForQ = @"\\10.10.11.199\Sinogram";
            
            if (!Directory.Exists(vnetForN))
            {
                throw new Exception($"访问共享目录:{vnetForN}异常.");
            }
            if (!Directory.Exists(vnetForT))
            {
                throw new Exception($"访问共享目录:{vnetForT}异常.");
            }
            if (!Directory.Exists(vnetForQ))
            {
                throw new Exception($"访问共享目录:{vnetForQ}异常.");
            }
        }
    }
}
