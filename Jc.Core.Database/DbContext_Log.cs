﻿using System;
using System.Collections.Generic;
using System.Text;
using System.Data;
using System.Data.Common;
using Jc.Database.Provider;
using log4net;
using System.IO;
using log4net.Repository.Hierarchy;
using log4net.Appender;
using log4net.Core;
using log4net.Filter;
using System.Runtime.InteropServices.ComTypes;
using System.Xml.Linq;
using log4net.Config;
using log4net.Layout;
using log4net.Repository;
using System.Linq;

namespace Jc.Database
{
    /// <summary>
    /// DbContext
    /// DbContext数据操作访问
    /// </summary>
    public partial class DbContext
    {
        internal DbLogHelper logHelper = null; // logHelper

        internal DbLogHelper LogHelper { get => logHelper; }
        
        #region 对象方法

        #region 开启日志记录
        /// <summary>
        /// 输出日志,需要在Config AppSetting中,设置 DbContextLog = true
        /// 并在CreateDbContext时,启用日志或使用InitLogger初始化Logger
        /// 初始化 DbLogger 记录日志
        /// 默认输出到当前目录,Log/Db/ ,Log/Db/Error 目录
        /// 需要使用dbLog.Config做为配置文件
        /// </summary>
        public void InitLogger()
        {
            try
            {
                bool dbLogOpen = ConfigHelper.GetAppSetting("DbLog")?.ToLower() == "true";
                if (dbLogOpen)
                {
                    logHelper = new DbLogHelper();
                    logHelper.InitLogger(DbName);
                }
            }
            catch(Exception ex)
            {

            }
        }

        /// <summary>
        /// 输出日志,需要在Config AppSetting中,设置 DbContextLog = true
        /// 并在CreateDbContext时,启用日志或使用InitLogger初始化Logger
        /// </summary>
        /// <param name="logger">info logger</param>
        /// <param name="errorLogger"> error logger </param>
        public void InitLogger(ILog logger, ILog errorLogger = null)
        {
            bool dbLogOpen = ConfigHelper.GetAppSetting("DbLog")?.ToLower() == "true";
            if (dbLogOpen)
            {
                logHelper = new DbLogHelper();
                logHelper.InitLogger(logger, errorLogger);
            }
        }

        /// <summary>
        /// 输出日志,需要在Config AppSetting中,设置 DbContextLog = true
        /// 并在CreateDbContext时,启用日志或使用InitLogger初始化Logger
        /// 默认通过设置logConfigPath来读取,当前目录下dblog.config配置来生成日志,
        /// </summary>
        /// <param name="logConfigPath"></param>
        /// <param name="logger"></param>
        /// <param name="errorLogger"></param>
        public void InitLogger(string logConfigPath, string logger = "Logger",string errorLogger = "ErrorLogger")
        {
            try
            {
                //初始化DbContextLogger
                bool dbLogOpen = ConfigHelper.GetAppSetting("DbLog")?.ToLower() == "true";
                if (dbLogOpen && File.Exists(logConfigPath))
                {
                    logHelper = new DbLogHelper();
                    logHelper.InitLogger(logConfigPath, $"JcDbRepository_{DbName}", logger, errorLogger);
                }
            }
            catch (Exception ex)
            {
            }
        }
        #endregion

        #endregion
    }
}