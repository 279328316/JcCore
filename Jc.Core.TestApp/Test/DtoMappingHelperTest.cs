using Jc.Core.Data.Query;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Jc.Core.TestApp.Test
{
    /// <summary>
    /// 对DtoMappingHelper GetPiMapList 是否使用缓存进行测试
    /// </summary>
    public class DtoMappingHelperTest
    {
        private int runCount = 100000;

        /// <summary>
        /// 测试方法
        /// 测试结果:不使用缓存结果更快
        /// </summary>
        public void Test()
        {
            GetPiMapListTest_MultiThread();  //7951    8002
            //GetPiMapListWithoutCatchTest_MultiThread();    //7771 7786

            //GetPiMapListTest();    //7693  8648
            //GetPiMapListWithoutCatchTest();    //7269 7466 
        }

        public void GetPiMapListTest()
        {
            string runType = "GetPiMapList使用缓存方式";
            Console.WriteLine(runType);
            List<UserDto> users = new List<UserDto>();

            Stopwatch sw = new Stopwatch();
            sw.Start();
            var exp = ExpressionHelper.CreateExpression<ApiDto>(a => new {
                a.Area,
                a.Controller,
                a.Action,
                a.AllowAnonymous,
                a.ApiLevel,
                a.MaybeDeleted,
                a.Note
            });
            var unexp = ExpressionHelper.CreateExpression<ApiDto>(a => new {
                a.Id,
                a.AddUser,
                a.AddDate,
                a.LastUpdateUser,
                a.LastUpdateDate
            });

            int curCount = 0;
            for (int i = 0; i < runCount; i++)
            {
                JcLogHelper.WriteLog($"正在执行:{i+1}");
                DtoMappingHelper.GetPiMapList<ApiDto>(exp);
                DtoMappingHelper.GetPiMapList<ApiDto>(null,unexp);
                DtoMappingHelper.GetPiMapList<ApiDto>(exp,unexp);
            }
            while (curCount>0)
            {
                Thread.Sleep(10);
                continue;
            }
            sw.Stop();
            JcLogHelper.WriteLog($"{runType},执行{runCount}次，共耗时{sw.ElapsedMilliseconds}Ms");
        }

        public void GetPiMapListWithoutCatchTest()
        {
            string runType = "GetPiMapList无缓存方式";
            Console.WriteLine(runType);
            List<UserDto> users = new List<UserDto>();

            Stopwatch sw = new Stopwatch();
            sw.Start();
            var exp = ExpressionHelper.CreateExpression<ApiDto>(a => new {
                a.Area,
                a.Controller,
                a.Action,
                a.AllowAnonymous,
                a.ApiLevel,
                a.MaybeDeleted,
                a.Note
            });
            var unexp = ExpressionHelper.CreateExpression<ApiDto>(a => new {
                a.Id,
                a.AddUser,
                a.AddDate,
                a.LastUpdateUser,
                a.LastUpdateDate
            });

            int curCount = 0;
            for (int i = 0; i < runCount; i++)
            {
                JcLogHelper.WriteLog($"正在执行:{i + 1}");                
                DtoMappingHelper.GetPiMapListWithCatch<ApiDto>(exp);
                DtoMappingHelper.GetPiMapListWithCatch<ApiDto>(null, unexp);
                DtoMappingHelper.GetPiMapListWithCatch<ApiDto>(exp, unexp);
                //curCount--;
            }
            while (curCount > 0)
            {
                Thread.Sleep(1);
                continue;
            }
            sw.Stop();
            JcLogHelper.WriteLog($"{runType},执行{runCount}次，共耗时{sw.ElapsedMilliseconds}Ms");
        }


        public void GetPiMapListTest_MultiThread()
        {
            string runType = "GetPiMapList使用缓存方式";
            Console.WriteLine(runType);
            List<UserDto> users = new List<UserDto>();

            Stopwatch sw = new Stopwatch();
            sw.Start();
            var exp = ExpressionHelper.CreateExpression<ApiDto>(a => new {
                a.Area,
                a.Controller,
                a.Action,
                a.AllowAnonymous,
                a.ApiLevel,
                a.MaybeDeleted,
                a.Note
            });
            var unexp = ExpressionHelper.CreateExpression<ApiDto>(a => new {
                a.Id,
                a.AddUser,
                a.AddDate,
                a.LastUpdateUser,
                a.LastUpdateDate
            });

            int curCount = 0;
            for (int i = 0; i < runCount; i++)
            {
                while (curCount > 5)
                {
                    Thread.Sleep(1);
                    continue;
                }
                JcLogHelper.WriteLog($"正在执行:{i + 1}");
                curCount++;
                Task t = new Task(() =>
                {
                    try
                    {
                        DtoMappingHelper.GetPiMapList<ApiDto>(exp);
                        DtoMappingHelper.GetPiMapList<ApiDto>(null,unexp);
                        DtoMappingHelper.GetPiMapList<ApiDto>(exp, unexp);
                    }
                    catch (Exception ex)
                    {
                        JcLogHelper.WriteLog($"执行异常:{ex.Message}");
                    }
                    curCount--;
                });
                t.Start();
            }
            while (curCount > 3)
            {
                Thread.Sleep(10);
                continue;
            }
            sw.Stop();
            JcLogHelper.WriteLog($"{runType},执行{runCount}次，共耗时{sw.ElapsedMilliseconds}Ms");
        }

        public void GetPiMapListWithoutCatchTest_MultiThread()
        {
            string runType = "GetPiMapList无缓存方式";
            Console.WriteLine(runType);
            List<UserDto> users = new List<UserDto>();

            Stopwatch sw = new Stopwatch();
            sw.Start();
            var exp = ExpressionHelper.CreateExpression<ApiDto>(a => new {
                a.Area,
                a.Controller,
                a.Action,
                a.AllowAnonymous,
                a.ApiLevel,
                a.MaybeDeleted,
                a.Note
            });
            var unexp = ExpressionHelper.CreateExpression<ApiDto>(a => new {
                a.Id,
                a.AddUser,
                a.AddDate,
                a.LastUpdateUser,
                a.LastUpdateDate
            });

            int curCount = 0;
            for (int i = 0; i < runCount; i++)
            {
                while (curCount > 5)
                {
                    Thread.Sleep(1);
                    continue;
                }
                JcLogHelper.WriteLog($"正在执行:{i + 1}");
                curCount++;
                Task t = new Task(() =>
                {
                    try
                    {
                        DtoMappingHelper.GetPiMapListWithCatch<ApiDto>(exp);
                        DtoMappingHelper.GetPiMapListWithCatch<ApiDto>(null, unexp);
                        DtoMappingHelper.GetPiMapListWithCatch<ApiDto>(exp, unexp);
                    }
                    catch (Exception ex)
                    {
                        JcLogHelper.WriteLog($"执行异常:{ex.Message}");
                    }
                    curCount--;
                });
                t.Start();
            }
            while (curCount > 3)
            {
                Thread.Sleep(1);
                continue;
            }
            sw.Stop();
            JcLogHelper.WriteLog($"{runType},执行{runCount}次，共耗时{sw.ElapsedMilliseconds}Ms");
        }
    }
}
