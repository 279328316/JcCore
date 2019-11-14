using Jc.Core.Data.Query;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.Data;
using System.Reflection;
using System.Reflection.Emit;
using System.Text;

namespace Jc.Core.UnitTestApp
{
    public class EmitTest
    {

        /// <summary>
        /// IL生成SetValueMethod内容
        /// 独立出来为共用代码
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="il"></param>
        [TestMethod]
        public static void ILGenerateSetValueMethodContent<T>()
        {
            var asmName = new AssemblyName("Test");
            var asmBuilder = AppDomain.CurrentDomain.DefineDynamicAssembly(asmName, AssemblyBuilderAccess.Save);
            var moduleBuilder = asmBuilder.DefineDynamicModule("Main", "Main.dll");
            DynamicMethod method = new DynamicMethod("Convert" + typeof(T).Name,
                    MethodAttributes.Public | MethodAttributes.Static,
                    CallingConventions.Standard, typeof(T),
                    new Type[] { typeof(DataRow) }, moduleBuilder, true);
            ILGenerator il = method.GetILGenerator();
            LocalBuilder result = il.DeclareLocal(typeof(T));
            il.Emit(OpCodes.Newobj, typeof(T).GetConstructor(Type.EmptyTypes));
            il.Emit(OpCodes.Stloc, result);

            //取出dr中所有的列名集合
            il.DeclareLocal(typeof(DataColumnCollection));
            il.Emit(OpCodes.Ldarg_0);
            il.Emit(OpCodes.Callvirt, typeof(DataRow).GetMethod("get_Table"));
            il.Emit(OpCodes.Callvirt, typeof(DataTable).GetMethod("get_Columns"));
            il.Emit(OpCodes.Stloc_1); //var columns = dr.Table.Columns

            List<PiMap> piMapList = DtoMappingHelper.GetPiMapList<T>();
            foreach (PiMap piMap in piMapList)
            {
                if (piMap.IsIgnore || piMap.Pi.SetMethod == null)
                {
                    continue;
                }
                //去掉关键词转义
                string fieldName = piMap.FieldName.Replace("\"", "").Replace("'", "").Replace("[", "").Replace("]", "");
                //加入判断条件 if (columns.Contains("Id") && !dataRow.IsNull("Id"))
                var endIfLabel = il.DefineLabel();
                il.Emit(OpCodes.Ldloc_1);    //columns
                il.Emit(OpCodes.Ldstr, fieldName); //Id
                il.Emit(OpCodes.Callvirt, typeof(DataColumnCollection).GetMethod("Contains", new Type[] { typeof(string) }));
                il.Emit(OpCodes.Brfalse, endIfLabel); //判断columns.Contains("Id")

                il.Emit(OpCodes.Ldarg_0);    //dr
                il.Emit(OpCodes.Ldstr, fieldName);     //Id
                il.Emit(OpCodes.Callvirt, typeof(DataRow).GetMethod("IsNull", new Type[] { typeof(string) }));
                il.Emit(OpCodes.Brtrue, endIfLabel); //判断dr.IsNull("Id")

                //自DataReader中读取值 调用get_Item方法 dataRow["Id"]
                il.Emit(OpCodes.Ldloc, result);  //result
                il.Emit(OpCodes.Ldarg_0);    //dataRow
                il.Emit(OpCodes.Ldstr, fieldName);   //Id
                il.Emit(OpCodes.Callvirt, typeof(DataRow).GetMethod("get_Item", new Type[] { typeof(string) }));

                Type type = piMap.PropertyType;  //拆箱
                if (type.IsValueType)
                {   //直接拆箱 可空类型,也可以直接拆箱
                    il.Emit(OpCodes.Unbox_Any, type);
                }
                else
                {   //引用类型
                    il.Emit(OpCodes.Castclass, type);
                }
                //给该属性设置对应值
                il.Emit(OpCodes.Callvirt, piMap.Pi.GetSetMethod());
                il.MarkLabel(endIfLabel);
            }
            /*给本地变量（result）返回值*/
            il.Emit(OpCodes.Ldloc, result);
            il.Emit(OpCodes.Ret);
            asmBuilder.Save("Test.dll");
        }
    }
}
