using System;
using System.Collections.Generic;
using System.Text;
using System.Data;
using System.Reflection;
using System.Reflection.Emit;
using System.Linq.Expressions;
using System.ComponentModel;

namespace Jc.Database.Query
{
    /// <summary>
    /// EntityConvertor delegate
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="dr">DataRow</param>
    /// <param name="convertResult">转换结果对象 
    /// 因Emit中使用String.Format Concat方法异常,无法组合异常消息
    /// 故引入对象接收异常结果
    /// 暂未发现其它更好方法</param>
    /// <returns></returns>
    public delegate object EntityConvertorDelegate(DataRow dr, EntityConvertResult convertResult);

    public class EntityConvertor
    {
        /// <summary>
        /// 构造EntityConvertor Handler
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <returns></returns>
        public static EntityConvertorDelegate CreateEntityConvertor<T>()
        {
            DynamicMethod dynamicMethod = BuildSetValueMethod<T>();
            EntityConvertorDelegate handler = (EntityConvertorDelegate)dynamicMethod.CreateDelegate(typeof(EntityConvertorDelegate));
            return handler;
        }

        /// <summary>
        /// 构造转换动态方法(核心代码)
        /// </summary>
        /// <typeparam name="T">返回的实体类型</typeparam>
        /// <returns>实体对象</returns>
        public static DynamicMethod BuildSetValueMethod<T>()
        {
            DynamicMethod method = new DynamicMethod("Convert" + typeof(T).Name,
                    MethodAttributes.Public | MethodAttributes.Static,
                    CallingConventions.Standard, typeof(T),
                    new Type[] { typeof(DataRow),typeof(EntityConvertResult) }, typeof(T).Module, true);
            ILGenerator generator = method.GetILGenerator();
            ILGenerateSetValueMethodContent<T>(generator);
            return method;
        }

        /// <summary>
        /// IL生成SetValueMethod内容
        /// 独立出来为共用代码
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="il"></param>
        public static void ILGenerateSetValueMethodContent<T>(ILGenerator il)
        {   // 使用变量 : Ldarg_0  dr Ldarg_1  entityConvertResult

            // UserDto result = new UserDto();
            LocalBuilder result = il.DeclareLocal(typeof(T));
            il.Emit(OpCodes.Newobj, typeof(T).GetConstructor(Type.EmptyTypes));
            il.Emit(OpCodes.Stloc, result);

            il.Emit(OpCodes.Nop);

            // var columns = dr.Table.Columns
            LocalBuilder columns = il.DeclareLocal(typeof(DataColumnCollection));
            il.Emit(OpCodes.Ldarg_0);   // dr
            il.Emit(OpCodes.Call, typeof(DataRow).GetMethod("get_Table"));
            il.Emit(OpCodes.Call, typeof(DataTable).GetMethod("get_Columns"));
            il.Emit(OpCodes.Stloc, columns); //

            il.Emit(OpCodes.Nop);

            //DataColumn curColumn = null;
            LocalBuilder curColumn = il.DeclareLocal(typeof(DataColumn));
            il.Emit(OpCodes.Ldnull);
            il.Emit(OpCodes.Box, typeof(DataColumn)); // boxing the value type to object
            il.Emit(OpCodes.Stloc, curColumn);

            //begin try
            Label tryLabel = il.BeginExceptionBlock();

            List<FieldMapping> piMapList = EntityMappingHelper.GetPiMapList<T>();
            for (int i = 0; i < piMapList.Count; i++)
            {
                FieldMapping piMap = piMapList[i];
                if (piMap.IsIgnore || piMap.Pi.SetMethod == null)
                {
                    continue;
                }
                //去掉关键词转义
                string fieldName = piMap.FieldName.Replace("\"", "").Replace("'", "").Replace("[", "").Replace("]", "");

                Type type = piMap.PropertyType;  //拆箱

                //加入判断条件 if (columns.Contains("Id"))
                Label label_EndField = il.DefineLabel();
                il.Emit(OpCodes.Ldloc, columns);    //columns
                il.Emit(OpCodes.Ldstr, fieldName); //Id
                il.Emit(OpCodes.Call, typeof(DataColumnCollection).GetMethod("Contains", new Type[] { typeof(string) }));
                il.Emit(OpCodes.Brfalse_S, label_EndField); // Branch if "Id" does not exist

                il.Emit(OpCodes.Nop);

                //赋值: curColumn = columns["Id"];
                il.Emit(OpCodes.Ldloc, columns);   //columns
                il.Emit(OpCodes.Ldstr, fieldName);
                il.Emit(OpCodes.Call, typeof(DataColumnCollection).GetMethod("get_Item", new Type[] { typeof(string) }));
                il.Emit(OpCodes.Stloc, curColumn);   // curColumn

                il.Emit(OpCodes.Nop);

                // 判断dr.IsNull(curColumn)
                Label label_ValueNull = il.DefineLabel();
                il.Emit(OpCodes.Ldarg_0);    //dr
                il.Emit(OpCodes.Ldstr, fieldName);     //Id
                il.Emit(OpCodes.Call, typeof(DataRow).GetMethod("IsNull", new Type[] { typeof(string) }));
                il.Emit(OpCodes.Brtrue_S, label_ValueNull); //判断dr.IsNull("Id")

                il.Emit(OpCodes.Nop);

                //不为空时,直接赋值
                // result.Id = (int?)dr[dataColumn]; 之前调试时出现过后面属性值为null情况,不确定是否与使用方式有关
                // 先使用 result.Id = (int?)dr["Id"];
                if (type.IsValueType)
                {   //直接拆箱 可空类型,也可以直接拆箱
                    il.Emit(OpCodes.Ldloc, result);  //result
                    il.Emit(OpCodes.Ldarg_0);    //dr
                    il.Emit(OpCodes.Ldstr, fieldName);   //Id
                    il.Emit(OpCodes.Call, typeof(DataRow).GetMethod("get_Item", new Type[] { typeof(string) }));
                    //il.Emit(OpCodes.Ldloc, curColumn);   //Id
                    //il.Emit(OpCodes.Call, typeof(DataRow).GetMethod("get_Item", new Type[] { typeof(DataColumn) }));

                    if (piMap.IsEnum)
                    {   // 如果为可空枚举,需要先拆箱为原类型
                        Type underlyingType = Nullable.GetUnderlyingType(type);
                        if (underlyingType != null)
                        {
                            il.Emit(OpCodes.Unbox_Any, underlyingType);
                            il.Emit(OpCodes.Newobj, type.GetConstructor(new[] { underlyingType }));
                        }
                        else
                        {
                            il.Emit(OpCodes.Unbox_Any, type);
                        }
                    }
                    else
                    {
                        il.Emit(OpCodes.Unbox_Any, type);
                    }
                    il.Emit(OpCodes.Call, piMap.Pi.GetSetMethod());
                }
                else
                {   //引用类型 直接转换 result.UserName = (string)dataRow[dataColumn];
                    il.Emit(OpCodes.Ldloc, result);  //result
                    il.Emit(OpCodes.Ldarg_0);    //dr
                    il.Emit(OpCodes.Ldstr, fieldName);   //Id
                    il.Emit(OpCodes.Call, typeof(DataRow).GetMethod("get_Item", new Type[] { typeof(string) }));
                    //il.Emit(OpCodes.Ldloc, curColumn);   //dataColumn
                    //il.Emit(OpCodes.Call, typeof(DataRow).GetMethod("get_Item", new Type[] { typeof(DataColumn) }));
                    il.Emit(OpCodes.Castclass, type);
                    il.Emit(OpCodes.Call, piMap.Pi.GetSetMethod());
                }

                il.Emit(OpCodes.Nop);
                il.Emit(OpCodes.Br_S, label_EndField); // 跳转到 "label_EndFieldSet"

                il.Emit(OpCodes.Nop);

                il.MarkLabel(label_ValueNull);
                if (type.IsValueType)
                {
                    Type underlyingType = Nullable.GetUnderlyingType(type);
                    if (underlyingType != null)
                    {   // 可空值类型 赋值为 null
                        LocalBuilder nullValue = il.DeclareLocal(type);
                        il.Emit(OpCodes.Ldloca_S, nullValue);  // 加载值类型地址
                        il.Emit(OpCodes.Initobj, type); // null  初始化

                        il.Emit(OpCodes.Ldloc, result);  //result
                        il.Emit(OpCodes.Ldloca_S, nullValue);   // 使用值类型,需要先加载地址,再读取值
                        il.Emit(OpCodes.Ldobj, type);  // 加载对应类型的值到栈顶
                        il.Emit(OpCodes.Call, piMap.Pi.GetSetMethod()); // 为属性赋值
                        il.Emit(OpCodes.Nop);
                    }
                    else
                    {   // 如果非可空类型,数据库中为空 throw new Exception("value is null");
                        il.Emit(OpCodes.Ldstr, "field value is null"); // 加载字符串 "value is null" 到栈顶
                        il.Emit(OpCodes.Newobj, typeof(Exception).GetConstructor(new[] { typeof(string) })); // 创建 Exception 实例
                        il.Emit(OpCodes.Throw); // 抛出异常
                        il.Emit(OpCodes.Nop);
                    }
                }
                else
                {   // 如果为引用类型 直接赋值null
                    il.Emit(OpCodes.Ldloc, result);  //result
                    il.Emit(OpCodes.Ldnull); //null  // 给该属性设置 null
                    il.Emit(OpCodes.Call, piMap.Pi.GetSetMethod());
                    il.Emit(OpCodes.Nop);
                }
                il.MarkLabel(label_EndField);
            }

            //end try

            //begin catch 注意，这个时侯堆栈顶为异常信息ex
            il.BeginCatchBlock(typeof(Exception));

            il.Emit(OpCodes.Nop);
            il.Emit(OpCodes.Nop);
            LocalBuilder exception = il.DeclareLocal(typeof(Exception));
            il.Emit(OpCodes.Stloc, exception);

            // exception.IsException = true;
            il.Emit(OpCodes.Ldarg_1);   // entityConvertResult
            il.Emit(OpCodes.Ldc_I4_1);
            il.Emit(OpCodes.Call, typeof(EntityConvertResult).GetMethod("set_IsException", new Type[] { typeof(bool) }));

            // exception.ColumnName = curColumn.ColumnName;
            il.Emit(OpCodes.Ldarg_1);   // entityConvertResult
            il.Emit(OpCodes.Ldloc, curColumn);
            il.Emit(OpCodes.Call, typeof(DataColumn).GetMethod("get_ColumnName"));
            il.Emit(OpCodes.Call, typeof(EntityConvertResult).GetMethod("set_ColumnName", new Type[] { typeof(string) }));

            // exception.Message = ex.Message;
            il.Emit(OpCodes.Ldarg_1);   // entityConvertResult
            il.Emit(OpCodes.Ldloc, exception);
            il.Emit(OpCodes.Call, typeof(Exception).GetMethod("get_Message"));
            il.Emit(OpCodes.Call, typeof(EntityConvertResult).GetMethod("set_Message", new Type[] { typeof(string) }));

            il.Emit(OpCodes.Nop);

            // end catch
            il.EndExceptionBlock();

            /*给本地变量（result）返回值*/
            il.Emit(OpCodes.Ldloc, result);
            il.Emit(OpCodes.Ret);
        }
    }
}