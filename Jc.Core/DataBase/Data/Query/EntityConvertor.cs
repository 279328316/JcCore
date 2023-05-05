using System;
using System.Collections.Generic;
using System.Text;
using System.Data;
using System.Reflection;
using System.Reflection.Emit;
using System.Linq.Expressions;
using System.ComponentModel;

namespace Jc.Data.Query
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
        {
            LocalBuilder result = il.DeclareLocal(typeof(T));
            il.Emit(OpCodes.Newobj, typeof(T).GetConstructor(Type.EmptyTypes));
            il.Emit(OpCodes.Stloc, result);

            //取出dr中所有的列名集合
            il.DeclareLocal(typeof(DataColumnCollection));
            il.Emit(OpCodes.Ldarg_0);
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

                il.Emit(OpCodes.Ldloc_1);   //columns
                il.Emit(OpCodes.Ldstr, fieldName);
                il.Emit(OpCodes.Callvirt, typeof(DataColumnCollection).GetMethod("get_Item", new Type[] { typeof(string) }));
                il.Emit(OpCodes.Stloc, curColumn); //赋值: curColumn = columns["Id"];

                //自DataReader中读取值 调用get_Item方法 dataRow["Id"]
                il.Emit(OpCodes.Ldloc, result);  //result
                il.Emit(OpCodes.Ldarg_0);    //dataRow
                il.Emit(OpCodes.Ldstr, fieldName);   //Id
                il.Emit(OpCodes.Callvirt, typeof(DataRow).GetMethod("get_Item", new Type[] { typeof(string) }));

                Type type = piMap.PropertyType;  //拆箱
                if (type.IsValueType)
                {   //直接拆箱 可空类型,也可以直接拆箱
                    Type realType = type;
                    if (piMap.IsEnum)
                    {
                        if (type.GenericTypeArguments.Length > 0)
                        {   //如果为可空枚举类型,抛出异常
                            realType = type.GenericTypeArguments[0];
                            throw new Exception($"对象{typeof(T).Name}属性{piMap.Pi.Name}[{realType.Name}]转换异常.不支持可空枚举类型.");
                        }
                    }
                    il.Emit(OpCodes.Unbox_Any, realType);
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

            il.Emit(OpCodes.Ldarg_1);
            il.Emit(OpCodes.Ldc_I4_1);
            il.Emit(OpCodes.Call, typeof(EntityConvertResult).GetMethod("set_IsException", new Type[] { typeof(bool) }));

            il.Emit(OpCodes.Ldarg_1);
            il.Emit(OpCodes.Ldloc, curColumn);
            il.Emit(OpCodes.Call, typeof(DataColumn).GetMethod("get_ColumnName"));
            il.Emit(OpCodes.Call, typeof(EntityConvertResult).GetMethod("set_ColumnName", new Type[] { typeof(string) }));

            il.Emit(OpCodes.Ldarg_1);
            il.Emit(OpCodes.Ldloc, exception);
            il.Emit(OpCodes.Call, typeof(Exception).GetMethod("get_Message"));
            il.Emit(OpCodes.Call, typeof(EntityConvertResult).GetMethod("set_Message", new Type[] { typeof(string) }));

            //il.Emit(OpCodes.Ldarg_1);     //不对外输出Exception
            //il.Emit(OpCodes.Ldloc, exception);
            //il.Emit(OpCodes.Call, typeof(EntityConvertResult).GetMethod("set_Exception", new Type[] { typeof(Exception) }));

            //end catch
            il.EndExceptionBlock();

            /*给本地变量（result）返回值*/
            il.Emit(OpCodes.Ldloc, result);
            il.Emit(OpCodes.Ret);            
        }
    }
}