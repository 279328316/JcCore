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

namespace Jc.Core.FrameworkTest
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
            //AssemblyName assemblyName = new AssemblyName(asmName);
            ////AssemblyBuilder assemblyBuilder = AssemblyBuilder.DefineDynamicAssembly(asmName, AssemblyBuilderAccess.RunAndSave);
            //AssemblyBuilder assemblyBuilder = AppDomain.CurrentDomain.DefineDynamicAssembly(assemblyName, AssemblyBuilderAccess.RunAndSave);
            //var moduleBuilder = assemblyBuilder.DefineDynamicModule("KittyModule", $"{asmName}.exe");
            //var typeBuilder = moduleBuilder.DefineType("HelloKittyClass", TypeAttributes.Class | TypeAttributes.Public);
            //MethodBuilder method = typeBuilder.DefineMethod("ConvertDto",
            //                   MethodAttributes.Public | MethodAttributes.HideBySig | MethodAttributes.Static
            //                   , typeof(T),
            //                  new Type[] { typeof(DataRow), typeof(EntityConvertResult) });

            //ILGenerator generator = method.GetILGenerator();
            ////EntityConvertor.ILGenerateSetValueMethodContent<T>(generator);

            //// 静态方法须 SetEntryPoint
            //Type classType = typeBuilder.CreateType();
            //assemblyBuilder.SetEntryPoint(classType.GetMethod("ConvertDto"));
            //assemblyBuilder.Save($"{asmName}.exe");
        }
    }
}
