using Jc.Core.Helper;
using Jc.Core.TestApp.Test;
using System;

namespace Jc.Core.TestApp
{
    class Program
    {
        static void Main(string[] args)
        {
            Console.WriteLine("测试即将开始,请按任意键继续.");
            Console.ReadKey();
            DtoMappingHelperTest test = new DtoMappingHelperTest();
            test.GetPiMapListTest_MultiThread();  //7951    8002
            //test.GetPiMapListWithoutCatchTest_MultiThread();    //7771 7786

            //test.GetPiMapListTest();    //7693  8648
            //test.GetPiMapListWithoutCatchTest();    //7269 7466


            Console.WriteLine("测试完成,请按任意键继续.");
            Console.ReadKey();
        }
    }
}
