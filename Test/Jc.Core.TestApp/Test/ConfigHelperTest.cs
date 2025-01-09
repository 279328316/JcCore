using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Data;
using System.Diagnostics;
using System.Text;

namespace Jc.Core.TestApp.Test
{
    public class ConfigHelperTest
    {
        public static void Test()
        {
            string testConfigFilePath = "appsettings_Test.json";

            int? i = ConfigHelper.GetAppSetting<int?>("i", testConfigFilePath);
            int i1 = ConfigHelper.GetAppSetting<int>("i1", testConfigFilePath);
            int i2 = ConfigHelper.GetAppSetting<int>("i2", testConfigFilePath);
            DatabaseType dbType = ConfigHelper.GetAppSetting<DatabaseType>("i1", testConfigFilePath);
            DatabaseType? dbType1 = ConfigHelper.GetAppSetting<DatabaseType?>("i1", testConfigFilePath);

            DatabaseType dbType2 = ConfigHelper.GetAppSetting<DatabaseType>("dbEnum", testConfigFilePath);
            DatabaseType? dbType3 = ConfigHelper.GetAppSetting<DatabaseType?>("dbEnum", testConfigFilePath);

            List<int> ilist = ConfigHelper.GetAppSetting<List<int>>("ilist", testConfigFilePath);

            bool a = ConfigHelper.GetAppSetting<bool>("a", testConfigFilePath);
            bool a0 = ConfigHelper.GetAppSetting<bool>("a0", testConfigFilePath);
            bool a1 = ConfigHelper.GetAppSetting<bool>("a1", testConfigFilePath);

            DateTime dt2 = ConfigHelper.GetAppSetting<DateTime>("dt2", testConfigFilePath);
            TimeSpan t1 = ConfigHelper.GetAppSetting<TimeSpan>("t1", testConfigFilePath);
            double d1 = ConfigHelper.GetAppSetting<double>("d1", testConfigFilePath);
            double d2 = ConfigHelper.GetAppSetting<double>("d2", testConfigFilePath);

            float f = ConfigHelper.GetAppSetting<float>("f", testConfigFilePath);
            float f1 = ConfigHelper.GetAppSetting<float>("f1", testConfigFilePath);

            DateTime dt1 = ConfigHelper.GetAppSetting<DateTime>("dt1", testConfigFilePath);
            TimeSpan t2 = ConfigHelper.GetAppSetting<TimeSpan>("t2", testConfigFilePath);

            // Error
            int i_none = ConfigHelper.GetAppSetting<int>("i_none", testConfigFilePath);
            bool a2 = ConfigHelper.GetAppSetting<bool>("a2", testConfigFilePath);  // 0
            bool a3 = ConfigHelper.GetAppSetting<bool>("a3", testConfigFilePath);  // 1
        }
    }
}