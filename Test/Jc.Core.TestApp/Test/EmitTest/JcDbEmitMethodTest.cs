using Jc.Database.Query;
using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Reflection.Emit;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

namespace Jc.Core.TestApp.Test
{
    /// <summary>
    /// Jc Dto Emit 测试
    /// 本例中的代码可以直接应用于Orm EntityConverter中
    /// </summary>
    public class JcDbEmitMethodTest
    {
        public static void Test()
        {
            SaveTest<UserDto>();
        }

        public static void SaveTest<T>()
        {
            //string asmName = "Kitty_DbEntityConvertor";
            //PersistedAssemblyBuilder ab = new PersistedAssemblyBuilder(new AssemblyName(asmName), typeof(object).Assembly);
            //ModuleBuilder mob = ab.DefineDynamicModule("KittyModule");
            //TypeBuilder tb = mob.DefineType("HelloKittyClass", TypeAttributes.Public | TypeAttributes.Class);
            //MethodBuilder methodBuilder = tb.DefineMethod("ConvertDto",
            //                       MethodAttributes.Public | MethodAttributes.HideBySig | MethodAttributes.Static
            //                       , typeof(T),
            //                      new Type[] { typeof(DataRow), typeof(EntityConvertResult) });

            //ILGenerator il = methodBuilder.GetILGenerator();
            //EntityConvertor.ILGenerateSetValueMethodContent<T>(il);
            //tb.CreateType();
            //ab.Save($"{asmName}.exe");
        }
    }
}
