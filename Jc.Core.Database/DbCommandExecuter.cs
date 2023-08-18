
using System;
using System.Collections.Generic;
using System.Data;
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
        internal static DbDataReader ExecuteReader(DbCommand command,DbLogHelper logHelper)
        {
            if (logHelper == null)
            {
                return command.ExecuteReader();
            }
            Stopwatch stopwatch = Stopwatch.StartNew();
            DbDataReader dr = null;
            try
            {
                dr = command.ExecuteReader();
                LogDbCommand(logHelper, command,stopwatch);
            }
            catch(Exception ex)
            {
                LogDbCommand(logHelper, command, stopwatch, ex);
                throw;
            }
            return dr;
        }

        /// <summary>
        /// 执行ExecuteReader
        /// </summary>
        /// <param name="command"></param>
        /// <returns></returns>
        internal static DataTable ExecuteDataTable(DbCommand command,int? limitLoadAmount, DbLogHelper logHelper)
        {
            DataTable dt;
            if (logHelper == null)
            {
                using (DbDataReader dr = command.ExecuteReader())
                {
                    dt = DataTableHelper.ConvertDataReaderToDataTable(dr, limitLoadAmount);
                    dr.Close();
                }
                return dt;
            }
            Stopwatch stopwatch = Stopwatch.StartNew();
            try
            {
                using (DbDataReader dr = command.ExecuteReader())
                {
                    dt = DataTableHelper.ConvertDataReaderToDataTable(dr, limitLoadAmount);
                    dr.Close();
                }
                LogDbCommand(logHelper, command, stopwatch, dt?.Rows.Count);
            }
            catch (Exception ex)
            {
                LogDbCommand(logHelper, command, stopwatch, ex);
                throw;
            }
            return dt;
        }

        /// <summary>
        /// 执行ExecuteReader
        /// </summary>
        /// <param name="command"></param>
        /// <returns></returns>
        internal static object ExecuteScalar(DbCommand command, DbLogHelper logHelper)
        {
            if (logHelper == null)
            {
                return command.ExecuteScalar();
            }
            Stopwatch stopwatch = Stopwatch.StartNew();
            Object obj = null;
            try
            {
                obj = command.ExecuteScalar();
                LogDbCommand(logHelper, command,stopwatch);
            }
            catch(Exception ex)
            {
                LogDbCommand(logHelper, command, stopwatch, ex);
                throw;
            }
            return obj;
        }

        /// <summary>
        /// 执行ExecuteReader
        /// </summary>
        /// <param name="command"></param>
        /// <returns></returns>
        internal static int ExecuteNonQuery(DbCommand command, DbLogHelper logHelper)
        {
            if (logHelper == null)
            {
                return command.ExecuteNonQuery();
            }
            Stopwatch stopwatch = Stopwatch.StartNew();
            int rowCount = 0;
            try
            {
                rowCount = command.ExecuteNonQuery();
                LogDbCommand(logHelper, command, stopwatch, rowCount);
            }
            catch(Exception ex)
            {
                LogDbCommand(logHelper, command, stopwatch, ex);
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
        private static void LogDbCommand(DbLogHelper logHelper, DbCommand command, Stopwatch stopwatch, Exception ex = null)
        {
            LogDbCommand(logHelper,command, stopwatch, null, ex);
        }

        /// <summary>
        /// 记录DbCommand执行结果
        /// </summary>
        /// <param name="command"></param>
        /// <param name="stopwatch"></param>
        /// <param name="ex"></param>
        private static void LogDbCommand(DbLogHelper logHelper, DbCommand command, Stopwatch stopwatch, int? rowCount, Exception ex = null)
        {
            try
            {
                if (stopwatch.IsRunning)
                {
                    stopwatch.Stop();
                }
                StringBuilder sb = new StringBuilder();
                string result = ex == null ? "success" : "error";
                sb.AppendLine($"DbCommand run {result} :");
                sb.AppendLine($"Command: {command.CommandText}");
                if (command.Parameters.Count > 0)
                {
                    sb.AppendLine($"Parameter: ");
                    for (int i = 0; i < command.Parameters.Count; i++)
                    {
                        DbParameter item = command.Parameters[i];
                        sb.AppendLine($"{i + 1} {item.Direction.Name()} {item.DbType} {item.ParameterName} : {item.Value}");
                    }
                }
                if (rowCount.HasValue)
                {
                    sb.Append($"Number of rows: {rowCount} \t");
                }
                sb.Append($"Execution duration: {stopwatch.ElapsedMilliseconds} ms");
                string infoMsg = sb.ToString();
                if (ex != null)
                {
                    logHelper.Error(infoMsg, ex);
                }
                else
                {
                    logHelper.Info(infoMsg);
                }
            }
            catch (Exception ex1)
            {
            }
        }
    }
}
