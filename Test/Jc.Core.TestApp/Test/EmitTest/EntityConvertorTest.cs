using Jc.Database.Query;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Jc.Core.TestApp.Test
{
    public class EntityConvertorTest
    {
        public static void Test()
        {
            JcEmitMethodTest.Test();
            JcDbEmitMethodTest.Test();

            Test_ByDynamicMethod();
            Test_ByJcEntityConvertor();

            Console.WriteLine($"EntityConvertorTest 测试完成,请按任意键继续");
            Console.ReadKey();
        }

        /// <summary>
        /// 使用DynamicMethod的方式进行转换
        /// </summary>
        public static void Test_ByDynamicMethod()
        {
            Console.WriteLine("开始测试Test_ByDynamicMethod");
            try
            {
                DataTable dt = Dbc.Db.GetDataTable<DrUserDto>();

                Console.WriteLine($"查询测试数据数量 : {dt?.Rows.Count}");

                EntityConvertorDelegate entityConvertor = JcEmitMethodTest.GetEntityConvertor<DrUserDto>();
                List<DrUserDto> users = new List<DrUserDto>();
                if (dt?.Rows.Count > 0)
                {
                    for (int i = 0; i < dt.Rows.Count; i++)
                    {
                        EntityConvertResult convertResult = new EntityConvertResult();
                        DataRow dr = dt.Rows[i];
                        DrUserDto user = (DrUserDto)entityConvertor.Invoke(dr, convertResult);
                        users.Add(user);

                        DrUserDto drUser = new DrUserDto(dr);

                        string unEqualPis = "";
                        List<FieldMapping> piMapList = EntityMappingHelper.GetPiMapList<DrUserDto>();
                        foreach (FieldMapping piMap in piMapList)
                        {
                            object dynamicUserValue = piMap.Pi.GetValue(user);
                            object drUserValue = piMap.Pi.GetValue(drUser);

                            if (Object.Equals(dynamicUserValue, drUserValue))
                            {
                            }
                            else
                            {
                                string piName = piMap.PiName;
                                unEqualPis += $"{piMap.PiName};";
                            }
                        }
                        if (!string.IsNullOrEmpty(unEqualPis))
                        {
                            throw new Exception($"属性值不匹配:{unEqualPis}");
                        }
                    }
                }
                if (dt?.Rows.Count > 0)
                {
                    Console.WriteLine($"Test_ByDynamicMethod 测试通过");
                }
                else
                {
                    throw new Exception("没有查询到测试数据");
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Test_ByDynamicMethod 测试未通过:{ex.Message}");
            }
        }


        /// <summary>
        /// 使用JcEntityConvertor接口创建类的方式进行转换
        /// </summary>
        public static void Test_ByJcEntityConvertor()
        {
            Console.WriteLine("开始测试Test_ByJcEntityConvertor");
            try
            {
                DataTable dt = Dbc.Db.GetDataTable<UserDto>();

                Console.WriteLine($"查询测试数据数量 : {dt?.Rows.Count}");

                EntityConvertorDelegate entityConvertor = EntityConvertor.CreateEntityConvertor<DrUserDto>();
                List<DrUserDto> users = new List<DrUserDto>();
                if (dt?.Rows.Count > 0)
                {
                    for (int i = 0; i < dt.Rows.Count; i++)
                    {
                        EntityConvertResult convertResult = new EntityConvertResult();
                        DataRow dr = dt.Rows[i];
                        DrUserDto user = (DrUserDto)entityConvertor.Invoke(dr, convertResult);
                        users.Add(user);

                        DrUserDto drUser = new DrUserDto(dr);

                        string unEqualPis = "";
                        List<FieldMapping> piMapList = EntityMappingHelper.GetPiMapList<DrUserDto>();
                        foreach (FieldMapping piMap in piMapList)
                        {
                            object dynamicUserValue = piMap.Pi.GetValue(user);
                            object drUserValue = piMap.Pi.GetValue(drUser);

                            if (Object.Equals(dynamicUserValue, drUserValue))
                            {
                            }
                            else
                            {
                                string piName = piMap.PiName;
                                unEqualPis += $"{piMap.PiName};";
                            }
                        }
                        if(!string.IsNullOrEmpty(unEqualPis))
                        {
                            throw new Exception($"属性值不匹配:{unEqualPis}");
                        }
                    }
                }
                if (dt?.Rows.Count > 0)
                {
                    Console.WriteLine($"Test_ByJcEntityConvertor 测试通过");
                }
                else
                {
                    throw new Exception("没有查询到测试数据");
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Test_ByJcEntityConvertor 测试未通过:{ex.Message}");
            }
        }
    }
}
