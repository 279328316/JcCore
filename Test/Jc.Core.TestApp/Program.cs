
using Jc.Core.TestApp.Test;
using Jc.Database;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace Jc.Core.TestApp
{
    class Program
    {
        static void Main(string[] args)
        {
            //初始化日志Helper
            string logConfigPath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "applog.config");
            LogHelper.InitLogger(logConfigPath, "AppRepository", "Logger", "ErrorLogger");
            LogHelper.Info($"Test服务启动成功.");
            Console.WriteLine("测试即将开始,请按任意键继续.");
            Console.ReadKey();

            try
            {
                PgSqlQueryTest test = new PgSqlQueryTest();
                test.Test();
            }
            catch(Exception ex)
            {
                LogHelper.Error($"执行失败:", ex);
            }
            Console.WriteLine("测试完成,请按任意键继续.");
            Console.ReadKey();
        }
    }
}
