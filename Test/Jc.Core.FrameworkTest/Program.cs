using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection.Emit;
using System.Text;
using System.Threading.Tasks;

namespace Jc.Core.FrameworkTest
{
    class Program
    {
        static void Main(string[] args)
        {
            Console.WriteLine("测试即将开始,请按任意键继续.");
            //Console.ReadKey();           
            //EmitTest.ILGenerateSetValueMethodContent<UserDto>();
            //EmitTest.Test();
            //return;
            JcEmitMethodTest.Test();
            JcDbEmitMethodTest.Test();
            EntityConvertorTest.Test();

            Console.WriteLine("测试完成,请按任意键继续.");
            //Console.ReadKey();
        }
    }
}
