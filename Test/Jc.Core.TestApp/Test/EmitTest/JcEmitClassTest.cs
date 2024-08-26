using Jc.Database.Query;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Reflection;
using System.Reflection.Emit;
using System.Reflection.PortableExecutable;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

namespace Jc.Core.TestApp.Test
{
    /// <summary>
    /// IEntityConvertor
    /// </summary>
    /// <typeparam name="T">Dto</typeparam>
    public interface IEntityConvertor<T>
    {
        T ConvertDto(DataRow dr, EntityConvertResult convertResult);
    }

    /// <summary>
    /// Jc Dto Emit 测试
    /// 生成IL到HolloKity.exe  
    /// 可以使用ILSpy查看生成的代码
    /// 可以使用通义千问来生成参照代码
    /// Sample : 使用ILGenerator动态生成if(columns.Contains("Id")){} 或大段代码都可以
    /// 
    /// 本例中的代码不能直接应用于Orm EntityConverter中
    /// 需要修改一下参数Index
    /// 因为本例中生成的为类中的方法.第0个参数为this
    /// 而函数中,不包含this,第0个对象为dr,第1个对象为EntityConvertResult
    /// </summary>
    public class JcEmitClassTest
    {
        public static void Test()
        {
            SaveTest<UserDto>();
        }

        public static void SaveTest<T>()
        {
            //string asmName = "Kitty_IEntityConvertor";
            //PersistedAssemblyBuilder ab = new PersistedAssemblyBuilder(new AssemblyName(asmName), typeof(object).Assembly);
            //ModuleBuilder mob = ab.DefineDynamicModule("KittyModule");
            //TypeBuilder tb = mob.DefineType("HelloKittyClass", TypeAttributes.Public | TypeAttributes.Class);
            //tb.AddInterfaceImplementation(typeof(IEntityConvertor<T>));
            //MethodBuilder methodBuilder = tb.DefineMethod("ConvertDto",
            //                   MethodAttributes.Public | MethodAttributes.HideBySig |
            //                   MethodAttributes.NewSlot | MethodAttributes.Virtual |
            //                   MethodAttributes.Final, typeof(T),
            //                  new Type[] { typeof(DataRow), typeof(EntityConvertResult) });
            //ILGenerator il = methodBuilder.GetILGenerator();
            //ILGenerateSetValueMethodContent<T>(il);
            //tb.CreateType();   // 输出之前必须调用一下
            //ab.Save($"{asmName}.exe");
        }

        /// <summary>
        /// IL生成SetValueMethod内容
        /// 独立出来为共用代码
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="il"></param>
        public static void ILGenerateSetValueMethodContent<T>(ILGenerator il)
        {
            LocalBuilder result = il.DeclareLocal(typeof(T));
            il.Emit(OpCodes.Newobj, typeof(T).GetConstructor(Type.EmptyTypes));
            il.Emit(OpCodes.Stloc, result);

            //取出dr中所有的列名集合
            il.DeclareLocal(typeof(DataColumnCollection));
            il.Emit(OpCodes.Ldarg_1);   // dr
            il.Emit(OpCodes.Callvirt, typeof(DataRow).GetMethod("get_Table"));
            il.Emit(OpCodes.Callvirt, typeof(DataTable).GetMethod("get_Columns"));
            il.Emit(OpCodes.Stloc_1); //var columns = dr.Table.Columns

            //定义局部变量 curColumn flag1(column.Contains("Id")) flag2(!dataRow.IsNull(column))
            LocalBuilder curColumn = il.DeclareLocal(typeof(DataColumn));
            il.Emit(OpCodes.Ldnull);
            il.Emit(OpCodes.Box, typeof(DataColumn)); // boxing the value type to object
            il.Emit(OpCodes.Stloc, curColumn); //curColumn = null;

            //begin try
            Label tryLabel = il.BeginExceptionBlock();

            List<FieldMapping> piMapList = EntityMappingHelper.GetPiMapList<T>();
            foreach (FieldMapping piMap in piMapList)
            {
                if (piMap.IsIgnore || piMap.Pi.SetMethod == null)
                {
                    continue;
                }
                //去掉关键词转义
                string fieldName = piMap.FieldName.Replace("\"", "").Replace("'", "").Replace("[", "").Replace("]", "");
                //加入判断条件 if (columns.Contains("Id"))

                Label label_EndFieldSet = il.DefineLabel();
                il.Emit(OpCodes.Ldloc_1);    //columns
                il.Emit(OpCodes.Ldstr, fieldName); //Id
                il.Emit(OpCodes.Callvirt, typeof(DataColumnCollection).GetMethod("Contains", new Type[] { typeof(string) }));
                il.Emit(OpCodes.Brfalse_S, label_EndFieldSet); // Branch if "Id" does not exist

                il.Emit(OpCodes.Ldloc_1);   //columns
                il.Emit(OpCodes.Ldstr, fieldName);
                il.Emit(OpCodes.Callvirt, typeof(DataColumnCollection).GetMethod("get_Item", new Type[] { typeof(string) }));
                il.Emit(OpCodes.Stloc, curColumn); //赋值: curColumn = columns["Id"];

                if (piMap.IsNullable)
                {
                    ////自DataReader中读取值 调用get_Item方法 dataRow[curColumn]
                    il.Emit(OpCodes.Ldloc, result);  //result
                    il.Emit(OpCodes.Ldarg_1);    //dr
                    il.Emit(OpCodes.Ldloc, curColumn);   //Id
                    il.Emit(OpCodes.Callvirt, typeof(DataRow).GetMethod("get_Item", new Type[] { typeof(DataColumn) }));

                    Type type = piMap.PropertyType;  //拆箱
                    if (type.IsValueType)
                    {   //直接拆箱 可空类型,也可以直接拆箱
                        il.Emit(OpCodes.Unbox_Any, type);
                    }
                    else
                    {   //引用类型
                        il.Emit(OpCodes.Castclass, type);
                    }
                    ////给该属性设置对应值
                    il.Emit(OpCodes.Callvirt, piMap.Pi.GetSetMethod());
                    il.Emit(OpCodes.Br, label_EndFieldSet); // 跳转到 "label_EndFieldSet"
                }
                else
                {
                    Label label_ValueNull = il.DefineLabel();
                    il.Emit(OpCodes.Ldarg_1);    //dr
                    il.Emit(OpCodes.Ldloc, curColumn);     //curColumn
                    il.Emit(OpCodes.Callvirt, typeof(DataRow).GetMethod("IsNull", new Type[] { typeof(DataColumn) }));
                    il.Emit(OpCodes.Brtrue, label_ValueNull); //判断dr.IsNull(curColumn)

                    //不为空时,直接赋值
                    //自DataReader中读取值 调用get_Item方法 dataRow[curColumn]
                    il.Emit(OpCodes.Ldloc, result);  //result
                    il.Emit(OpCodes.Ldarg_1);    //dr
                    il.Emit(OpCodes.Ldloc, curColumn);   //Id
                    il.Emit(OpCodes.Callvirt, typeof(DataRow).GetMethod("get_Item", new Type[] { typeof(DataColumn) }));

                    Type type = piMap.PropertyType;  //拆箱
                    if (type.IsValueType)
                    {   //直接拆箱 可空类型,也可以直接拆箱
                        il.Emit(OpCodes.Unbox_Any, type);
                    }
                    else
                    {   //引用类型
                        il.Emit(OpCodes.Castclass, type);
                    }
                    ////给该属性设置对应值
                    il.Emit(OpCodes.Callvirt, piMap.Pi.GetSetMethod());
                    il.Emit(OpCodes.Br, label_EndFieldSet); // 跳转到 "label_EndFieldSet"

                    il.MarkLabel(label_ValueNull);

                    // 如果非可空类型,数据库中为空 throw new Exception("value is null");
                    il.Emit(OpCodes.Ldstr, "value is null"); // 加载字符串 "value is null" 到栈顶
                    il.Emit(OpCodes.Newobj, typeof(Exception).GetConstructor(new[] { typeof(string) })); // 创建 Exception 实例
                    il.Emit(OpCodes.Throw); // 抛出异常
                }
                il.MarkLabel(label_EndFieldSet);
            }

            //end try

            //begin catch 注意，这个时侯堆栈顶为异常信息ex
            il.BeginCatchBlock(typeof(Exception));

            LocalBuilder exception = il.DeclareLocal(typeof(Exception));
            il.Emit(OpCodes.Stloc, exception);

            il.Emit(OpCodes.Ldarg_2);   // entityConvertResult
            il.Emit(OpCodes.Ldc_I4_1);
            il.Emit(OpCodes.Call, typeof(EntityConvertResult).GetMethod("set_IsException", new Type[] { typeof(bool) }));

            il.Emit(OpCodes.Ldarg_2);   // entityConvertResult
            il.Emit(OpCodes.Ldloc, curColumn);
            il.Emit(OpCodes.Call, typeof(DataColumn).GetMethod("get_ColumnName"));
            il.Emit(OpCodes.Call, typeof(EntityConvertResult).GetMethod("set_ColumnName", new Type[] { typeof(string) }));

            il.Emit(OpCodes.Ldarg_2);   // entityConvertResult
            il.Emit(OpCodes.Ldloc, exception);
            il.Emit(OpCodes.Call, typeof(Exception).GetMethod("get_Message"));
            il.Emit(OpCodes.Call, typeof(EntityConvertResult).GetMethod("set_Message", new Type[] { typeof(string) }));

            //end catch
            il.EndExceptionBlock();

            /*给本地变量（result）返回值*/
            il.Emit(OpCodes.Ldloc, result);
            il.Emit(OpCodes.Ret);
        }
    }
}
