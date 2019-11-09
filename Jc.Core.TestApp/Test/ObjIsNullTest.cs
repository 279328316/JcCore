using System;
using System.Collections.Generic;
using System.Text;

namespace Jc.Core.TestApp.Test
{
    public class ObjIsNullTest
    {
        public void Test()
        {
            int i = 0;
            Console.WriteLine($"int i:{ExHelper.IsNullOrEmpty(i)}");

            int? t = 0;
            Console.WriteLine($"int? t:{ExHelper.IsNullOrEmpty(t)}");

            long l = 0;
            Console.WriteLine($"long l:{ExHelper.IsNullOrEmpty(l)}");

            long? ln = 0;
            Console.WriteLine($"long? ln = 0:{ExHelper.IsNullOrEmpty(ln)}");
            ln = null;
            Console.WriteLine($"long? ln = null:{ExHelper.IsNullOrEmpty(ln)}");

            Guid g = Guid.Empty;
            Console.WriteLine($"Guid g:{ExHelper.IsNullOrEmpty(g)}");

            Guid? gn = Guid.Empty;
            Console.WriteLine($"Guid? gn:{ExHelper.IsNullOrEmpty(gn)}");

            string str = null;
            Console.WriteLine($"string str = null:{ExHelper.IsNullOrEmpty(str)}");

            string strn = "";
            Console.WriteLine($"string strn = \"\":{ExHelper.IsNullOrEmpty(strn)}");

            List<int> list = null;
            Console.WriteLine($"List<int> list:{ExHelper.IsNullOrEmpty(list)}");

            List<int> listn = new List<int>();
            Console.WriteLine($"List<int> listn:{ExHelper.IsNullOrEmpty(listn)}");
        }
    }
}
