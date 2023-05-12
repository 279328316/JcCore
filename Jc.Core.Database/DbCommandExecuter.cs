
using System;
using System.Data.Common;
using System.Diagnostics;
using System.Text;

namespace Jc.Database
{
    internal class DbCommandExecuter
    {
        /// <summary>
        /// 执行ExecuteReader
        /// </summary>
        /// <param name="command"></param>
        /// <returns></returns>
        internal static DbDataReader ExecuteReader(DbCommand command)
        {
            if (!DbLogHelper.DbLogOn)
            {
                return command.ExecuteReader();
            }
            Stopwatch stopwatch = Stopwatch.StartNew();
            DbDataReader dr = null;
            try
            {
                dr = command.ExecuteReader();
                LogDbCommand(command,stopwatch);
            }
            catch(Exception ex)
            {
                LogDbCommand(command, stopwatch, ex);
                throw;
            }
            return dr;
        }

        /// <summary>
        /// 执行ExecuteReader
        /// </summary>
        /// <param name="command"></param>
        /// <returns></returns>
        internal static object ExecuteScalar(DbCommand command)
        {
            if (!DbLogHelper.DbLogOn)
            {
                return command.ExecuteScalar();
            }
            Stopwatch stopwatch = Stopwatch.StartNew();
            Object obj = null;
            try
            {
                obj = command.ExecuteScalar();
                LogDbCommand(command,stopwatch);
            }
            catch(Exception ex)
            {
                LogDbCommand(command, stopwatch, ex);
                throw;
            }
            return obj;
        }

        /// <summary>
        /// 执行ExecuteReader
        /// </summary>
        /// <param name="command"></param>
        /// <returns></returns>
        internal static int ExecuteNonQuery(DbCommand command)
        {
            if (!DbLogHelper.DbLogOn)
            {
                return command.ExecuteNonQuery();
            }
            Stopwatch stopwatch = Stopwatch.StartNew();
            int rowCount = 0;
            try
            {
                rowCount = command.ExecuteNonQuery();
                LogDbCommand(command, stopwatch, rowCount);
            }
            catch(Exception ex)
            {
                LogDbCommand(command, stopwatch, ex);
                throw;
            }
            return rowCount;
        }

        /// <summary>
        /// 记录DbCommand执行结果
        /// </summary>
        /// <param name="command"></param>
        /// <param name="stopwatch"></param>
        /// <param name="ex"></param>
        private static void LogDbCommand(DbCommand command,Stopwatch stopwatch, Exception ex = null)
        {
            LogDbCommand(command, stopwatch, null, ex);
        }

        /// <summary>
        /// 记录DbCommand执行结果
        /// </summary>
        /// <param name="command"></param>
        /// <param name="stopwatch"></param>
        /// <param name="ex"></param>
        private static void LogDbCommand(DbCommand command, Stopwatch stopwatch, int? rowCount, Exception ex = null)
        {
            try
            {
                if (stopwatch.IsRunning)
                {
                    stopwatch.Stop();
                }
                StringBuilder sb = new StringBuilder();
                string result = ex == null ? "成功" : "失败";
                sb.AppendLine($"DbCommand 执行{result} :");
                sb.AppendLine($"CommandText : {command.CommandText}");
                if (rowCount.HasValue)
                {
                    sb.AppendLine($"受影响行数 : {rowCount} 行");
                }
                if (command.Parameters.Count > 0)
                {
                    sb.AppendLine($"CommandParameter : ");
                    for (int i = 0; i < command.Parameters.Count; i++)
                    {
                        DbParameter item = command.Parameters[i];
                        sb.AppendLine($"{i + 1} {item.Direction.Name()} {item.DbType} {item.ParameterName} : {item.Value}");
                    }
                }
                sb.Append($"执行时间 : {stopwatch.ElapsedMilliseconds} ms");
                string infoMsg = sb.ToString();
                if (ex != null)
                {
                    DbLogHelper.Error(infoMsg, ex);
                }
                else
                {
                    DbLogHelper.Info(infoMsg);
                }
            }
            catch (Exception ex1)
            {
            }
        }
    }
}
