using Jc.Database.Query;
using System;
using System.Collections.Generic;
using System.Data;
using System.Reflection;
using System.Reflection.Emit;
using System.Text;

namespace Jc.Core.FrameworkTest
{
    public partial class EmitTest
    {
        /// <summary>
        /// IL生成SetValueMethod内容
        /// 独立出来为共用代码
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="il"></param>
        public static IEntityConvertor<T> GenerateEntityConvertor_ThrowEx<T>()
        {
            var asmName = new AssemblyName("Test");
            //var assemblyBuilder = AssemblyBuilder.DefineDynamicAssembly(asmName, AssemblyBuilderAccess.RunAndSave);
            var assemblyBuilder = AppDomain.CurrentDomain.DefineDynamicAssembly(asmName, AssemblyBuilderAccess.RunAndSave);
            var moduleBuilder = assemblyBuilder.DefineDynamicModule("KittyModule", "Kitty.exe");
            var typeBuilder = moduleBuilder.DefineType("HelloKittyClass", TypeAttributes.Class | TypeAttributes.Public);
            typeBuilder.AddInterfaceImplementation(typeof(IEntityConvertor<T>));
            MethodBuilder method = typeBuilder.DefineMethod("ConvertDto",
                               MethodAttributes.Public | MethodAttributes.HideBySig |
                               MethodAttributes.NewSlot | MethodAttributes.Virtual |
                               MethodAttributes.Final, typeof(T),
                              new Type[] { typeof(DataRow),typeof(EntityConvertResult) });

            ILGenerator il = method.GetILGenerator();
            LocalBuilder result = il.DeclareLocal(typeof(T));
            il.Emit(OpCodes.Newobj, typeof(T).GetConstructor(Type.EmptyTypes));
            il.Emit(OpCodes.Stloc, result);

            //取出dr中所有的列名集合
            il.DeclareLocal(typeof(DataColumnCollection));
            il.Emit(OpCodes.Ldarg_1);
            il.Emit(OpCodes.Callvirt, typeof(DataRow).GetMethod("get_Table"));
            il.Emit(OpCodes.Callvirt, typeof(DataTable).GetMethod("get_Columns"));
            il.Emit(OpCodes.Stloc_1); //var columns = dr.Table.Columns

            //定义局部变量 curColumn
            LocalBuilder curColumn = il.DeclareLocal(typeof(DataColumn));
            il.Emit(OpCodes.Ldnull);
            il.Emit(OpCodes.Box, typeof(DataColumn)); // boxing the value type to object
            il.Emit(OpCodes.Stloc, curColumn); //curColumn = null;

            //begin try
            Label tryLabel = il.BeginExceptionBlock();
            List<FieldMapping> piMapList = DtoMappingHelper.GetPiMapList<T>();
            foreach (FieldMapping piMap in piMapList)
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

                il.Emit(OpCodes.Ldarg_1);    //dr
                il.Emit(OpCodes.Ldstr, fieldName);     //Id
                il.Emit(OpCodes.Callvirt, typeof(DataRow).GetMethod("IsNull", new Type[] { typeof(string) }));
                il.Emit(OpCodes.Brtrue, endIfLabel); //判断dr.IsNull("Id")

                il.Emit(OpCodes.Ldloc_1);   //columns
                il.Emit(OpCodes.Ldstr, fieldName);
                il.Emit(OpCodes.Callvirt, typeof(DataColumnCollection).GetMethod("get_Item", new Type[] { typeof(string) }));
                il.Emit(OpCodes.Stloc, curColumn); //curColumn = columns["Id"];

                //自DataReader中读取值 调用get_Item方法 dataRow["Id"]
                il.Emit(OpCodes.Ldloc, result);  //result
                il.Emit(OpCodes.Ldarg_1);    //dataRow
                il.Emit(OpCodes.Ldstr, fieldName);   //Id
                il.Emit(OpCodes.Callvirt, typeof(DataRow).GetMethod("get_Item", new Type[] { typeof(string) }));

                Type type = piMap.PropertyType;  //拆箱                
                if (type.IsValueType)
                {   //直接拆箱 可空类型,也可以直接拆箱
                    if (piMap.IsEnum)
                    {   //如果为枚举类型,先转为int
                        il.Emit(OpCodes.Unbox_Any, typeof(int));
                        Type realType = type.GenericTypeArguments.Length > 0 ?
                                        type.GenericTypeArguments[0] : type;
                        //Type helperType = typeof(Helper.TEnumHelper<>);
                        //Type thealperType = helperType.MakeGenericType(realType);
                        //MethodInfo convertMethod = thealperType.GetMethod("ToEnum");
                        //il.Emit(OpCodes.Callvirt, convertMethod);
                        il.Emit(OpCodes.Unbox_Any, realType);
                    }
                    else
                    {
                        il.Emit(OpCodes.Unbox_Any, type);
                    }
                }
                else
                {   //引用类型
                    il.Emit(OpCodes.Castclass, type);
                }
                //给该属性设置对应值
                il.Emit(OpCodes.Callvirt, piMap.Pi.GetSetMethod());
                il.MarkLabel(endIfLabel);
            }
            //end try

            //begin catch 注意，这个时侯堆栈顶为异常信息ex
            il.BeginCatchBlock(typeof(Exception));

            LocalBuilder exception = il.DeclareLocal(typeof(Exception));
            il.Emit(OpCodes.Stloc, exception);

            il.Emit(OpCodes.Ldloc, exception);
            il.Emit(OpCodes.Call, typeof(Exception).GetMethod("get_Message"));
            il.Emit(OpCodes.Call, typeof(Console).GetMethod("WriteLine", new Type[] { typeof(string) }));

            il.Emit(OpCodes.Ldarg_2);
            il.Emit(OpCodes.Ldc_I4_1);
            il.Emit(OpCodes.Call, typeof(EntityConvertResult).GetMethod("set_IsException", new Type[] { typeof(bool) }));

            il.Emit(OpCodes.Ldarg_2);
            il.Emit(OpCodes.Ldloc, exception);
            il.Emit(OpCodes.Call, typeof(Exception).GetMethod("get_Message"));
            il.Emit(OpCodes.Call, typeof(EntityConvertResult).GetMethod("set_Message", new Type[] { typeof(string) }));

            il.Emit(OpCodes.Ldarg_2);
            il.Emit(OpCodes.Ldloc, curColumn); 
            il.Emit(OpCodes.Call, typeof(DataColumn).GetMethod("get_ColumnName"));
            il.Emit(OpCodes.Call, typeof(EntityConvertResult).GetMethod("set_ColumnName", new Type[] { typeof(string) }));

            il.Emit(OpCodes.Ldarg_2);
            il.Emit(OpCodes.Ldloc, exception);
            il.Emit(OpCodes.Call, typeof(EntityConvertResult).GetMethod("set_Exception", new Type[] { typeof(string) }));

            //end catch
            il.EndExceptionBlock();

            /*给本地变量（result）返回值*/
            il.Emit(OpCodes.Ldloc, result);
            il.Emit(OpCodes.Ret);

            TypeInfo typeInfo = typeBuilder.CreateTypeInfo();
            assemblyBuilder.Save("Kitty.exe");

            object obj = assemblyBuilder.CreateInstance(typeInfo.Name);

            return obj as IEntityConvertor<T>;
        }

    }
}
