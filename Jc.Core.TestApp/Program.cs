﻿using Jc.Core.Helper;
using Jc.Core.TestApp.Test;
using System;

namespace Jc.Core.TestApp
{
    class Program
    {
        static void Main(string[] args)
        {
            ApiTest test = new ApiTest();
            test.IQueryTest();

            Console.WriteLine("测试完成,请按任意键继续.");
            Console.ReadKey();
        }
    }
}
