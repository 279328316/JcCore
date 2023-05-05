﻿using System;
using System.Collections.Generic;
using System.Data.SQLite;
using System.IO;
using System.Text;

namespace Jc.Core.Sqlite
{

    /// <summary>
    /// SQLite Helper
    /// </summary>
    public class SQLiteHelper
    {
        #region Methods

        /// <summary>
        /// 获取连接串
        /// </summary>
        private static string GetConnectionString(string dbFilePath, string password = null)
        {
            if (string.IsNullOrWhiteSpace(dbFilePath))
            {
                throw new Exception("数据访问异常:未设置数据文件路径.");
            }
            SQLiteConnectionStringBuilder builder = new SQLiteConnectionStringBuilder();
            builder.DataSource = dbFilePath;
            builder.Password = password;
            return builder.ConnectionString;
        }

        /// <summary>
        /// 检查数据库是否存在
        /// </summary>
        /// <returns></returns>
        public static bool CheckDbExistsByConnectString(string connectingString)
        {
            bool result = false;
            try
            {
                using (SQLiteConnection con = new SQLiteConnection(connectingString))
                {
                    con.Open();
                    //if (File.Exists(con.State))
                    //{
                    //    result = true;
                    //}
                    if (con.State == System.Data.ConnectionState.Open)
                    {
                        result = true;
                    }
                    con.Close();
                }
            }
            catch
            {
            }
            return result;
        }

        /// <summary>
        /// 检查数据库是否存在
        /// </summary>
        /// <returns></returns>
        public static bool CheckDbExists(string dbFilePath, string password = null)
        {
            return File.Exists(dbFilePath);
        }

        /// <summary>
        /// 创建数据库
        /// </summary>
        /// <returns></returns>
        public static bool CreateDb(string dbFilePath, string password = null)
        {
            bool result = false;
            try
            {
                if (string.IsNullOrWhiteSpace(dbFilePath))
                {
                    throw new Exception("数据访问异常:未设置数据文件路径.");
                }
                if (File.Exists(dbFilePath))
                {
                    throw new Exception("数据访问异常:数据文件已存在.");
                }
                if (!Directory.GetParent(dbFilePath).Exists)
                {
                    Directory.GetParent(dbFilePath).Create();
                }
                SQLiteConnection.CreateFile(dbFilePath);
                if (!string.IsNullOrEmpty(password))
                {
                    string conString = string.Format("Data Source={0};password={1};BinaryGUID=False;", dbFilePath, password);
                    SQLiteConnection con = new SQLiteConnection(conString);
                    con.Open();
                    //con.ChangePassword(password);
                    con.Close();
                }
                result = true;
            }
            catch (Exception ex)
            {
                throw ex;
            }
            return result;
        }


        /// <summary>
        /// 修改数据库密码
        /// </summary>
        /// <param name="newPassword"></param>
        /// <returns></returns>
        public static bool CheckDbPassword(string tdbFilePath, string nowpassword = null)
        {
            bool result = false;
            try
            {
                using (SQLiteConnection con = new SQLiteConnection(GetConnectionString(tdbFilePath, nowpassword)))
                {
                    con.Open();
                    SQLiteCommand cmd = new SQLiteCommand(con);
                    cmd.CommandText = "select tbl_name from sqlite_master where type='table' order by tbl_name limit 1 offset 0";
                    cmd.ExecuteNonQuery();
                    con.Close();
                }
                result = true;
            }
            catch
            {
                throw new Exception("密码不正确.");
            }
            return result;
        }

        /// <summary>
        /// 修改数据库密码
        /// </summary>
        /// <param name="newPassword"></param>
        /// <returns></returns>
        private static bool ChangePassword(string tdbFilePath, string nowpassword = null, string newPassword = null)
        {
            bool result = false;
            try
            {
                SQLiteConnection con = new SQLiteConnection(GetConnectionString(tdbFilePath, nowpassword));
                con.Open();
                //con.ChangePassword(newPassword);
                con.Close();
                result = true;
            }
            catch (Exception ex)
            {
                throw new Exception("修改数据库密码失败:" + ex.Message);
            }
            return result;
        }

        #endregion
    }
}
