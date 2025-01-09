using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Diagnostics;
using System.Text;

namespace Jc.Core.TestApp.Test
{
    public class JsonTest
    {
        public static void Test()
        {
            int i = JsonHelper.DeserializeObject<int>("0");
            int i1 = JsonHelper.DeserializeObject<int>("1");

            bool a = JsonHelper.DeserializeObject<bool>("true");
            bool a0 = JsonHelper.DeserializeObject<bool>("false");
            bool a3 = JsonHelper.DeserializeObject<bool>("0");
            bool a4 = JsonHelper.DeserializeObject<bool>("1");
            bool a5 = JsonHelper.DeserializeObject<bool>("2");

            DateTime dt2 = DateTime.Parse("2025-01-09 18:26:01");

            TimeSpan t1 = TimeSpan.Parse("18:26:01");
            double d1 = JsonHelper.DeserializeObject<double>("9.1");
            double d2 = JsonHelper.DeserializeObject<double>("12.51");

            float f = JsonHelper.DeserializeObject<float>("12.51");
            float f1 = JsonHelper.DeserializeObject<float>("12.51");

            // Error
            DateTime dt1 = JsonHelper.DeserializeObject<DateTime>("2025/1/09 18:26:01");
            TimeSpan t2 = JsonHelper.DeserializeObject<TimeSpan>("18:26:01");
        }
    }
}