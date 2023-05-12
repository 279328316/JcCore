using log4net;
using log4net.Config;
using log4net.Repository;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

namespace Jc.Database
{
    /// <summary>
    /// Db Log Helper
    /// </summary>
    internal class DbLogHelper
    {
        private static ILog logger;
        private static ILog errorLogger;

        internal static bool DbLogOn
        {
            get
            {
                return logger != null;
            }
        }

        /// <summary>
        /// 日志记录
        /// </summary>
        internal static ILog Logger
        {
            get
            {
                return logger;
            }
            set
            {
                logger = value;
            }
        }

        /// <summary>
        /// Error日志记录
        /// </summary>
        internal static ILog ErrorLogger
        {
            get
            {
                return errorLogger;
            }
            set
            {
                errorLogger = value;
            }
        }

        static DbLogHelper()
        {
            try
            { 
                if(ConfigHelper.GetAppSetting("DbLog") == "true")
                {
                    //初始化日志Helper
                    string logConfigPath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "applog.config");
                    InitLogger(logConfigPath, "DbRepository", "DbLogger");               
                }
            }
            catch (Exception ex)
            {
            }
        }

        /// <summary>
        /// 初始化Logger
        /// </summary>
        internal static void SetLogger(string repositoryName, string loggerName)
        {
            ILoggerRepository repository = LogManager.GetAllRepositories().FirstOrDefault(a => a.Name == repositoryName);
            if (repository != null)
            {
                Logger = LogManager.GetLogger(repository.Name, loggerName);
            }
        }

        /// <summary>
        /// 初始化ErrorLogger
        /// </summary>
        internal static void SetErrorLogger(string repositoryName, string errorLoggerName)
        {
            ILoggerRepository repository = LogManager.GetAllRepositories().FirstOrDefault(a => a.Name == repositoryName);
            if (repository != null)
            {
                ErrorLogger = LogManager.GetLogger(repository.Name, errorLoggerName);
            }
        }

        /// <summary>
        /// 初始化Logger
        /// </summary>
        internal static void InitLogger(ILog logger, ILog errorLogger = null)
        {
            Logger = logger;
            ErrorLogger = errorLogger;
        }

        /// <summary>
        /// 初始化Logger
        /// </summary>
        internal static void InitLogger(string logConfigPath, string repositoryName, string loggerName, string errorLoggerName = null)
        {
            if (logger == null)
            {
                ILoggerRepository repository = GetLogRepository(logConfigPath, repositoryName);
                logger = LogManager.GetLogger(repository.Name, loggerName);
            }
            if (errorLogger == null && !string.IsNullOrEmpty(errorLoggerName))
            {
                ILoggerRepository repository = GetLogRepository(logConfigPath, repositoryName);
                errorLogger = LogManager.GetLogger(repository.Name, errorLoggerName);
            }
        }

        /// <summary>
        /// 获取LogRepository
        /// </summary>
        /// <returns></returns>
        private static ILoggerRepository GetLogRepository(string logConfigPath, string repositoryName)
        {
            ILoggerRepository repository = LogManager.GetAllRepositories().FirstOrDefault(a => a.Name == repositoryName);
            if (repository == null)
            {
                repository = LogManager.CreateRepository(repositoryName);
                XmlConfigurator.Configure(repository, new FileInfo(logConfigPath));
            }
            return repository;
        }
        /// <summary>
        /// 记录日志
        /// </summary>
        /// <param name="msg"></param>
        internal static void WriteLog(string msg)
        {
            Info(msg);
        }

        /// <summary>
        /// 记录日志
        /// </summary>
        /// <param name="msg"></param>
        internal static void Info(string msg)
        {
            if (Logger != null)
            {
                try
                {
                    Logger.Info(msg);
                }
                catch (Exception ex)
                {

                }
            }
        }

        /// <summary>
        /// 记录日志
        /// </summary>
        /// <param name="msg"></param>
        internal static void Warn(string msg)
        {
            if (Logger != null)
            {
                try
                {
                    Logger.Warn(msg);
                }
                catch (Exception ex)
                {

                }
            }
        }

        /// <summary>
        /// 记录日志
        /// </summary>
        /// <param name="msg"></param>
        internal static void Error(string msg)
        {
            try
            {
                if (ErrorLogger != null)
                {
                    ErrorLogger.Error(msg);
                }
                else
                {
                    Info(msg);
                }
            }
            catch
            {
            }
        }

        /// <summary>
        /// 记录日志
        /// </summary>
        /// <param name="msg"></param>
        internal static void Error(string msg ,Exception ex)
        {
            try
            {
                msg = $"{msg}{GetExceptionInfo(ex)}";
                Error(msg);
            }
            catch
            {
            }
        }

        /// <summary>
        /// 记录日志
        /// </summary>
        /// <param name="msg"></param>
        internal static void Error(Exception ex)
        {
            try
            {
                string msg = GetExceptionInfo(ex);
                Error(msg);
            }
            catch
            {
            }
        }

        /// <summary>
        /// 获取Exception Info
        /// </summary>
        /// <param name="ex"></param>
        /// <returns></returns>
        private static string GetExceptionInfo(Exception ex)
        {
            string msg = $"{ex.Message}\r\n{ex.StackTrace}";
            Exception innerEx = ex.InnerException;
            while (innerEx != null)
            {
                msg += $"{innerEx.Message}\r\n{innerEx.StackTrace}";
                innerEx = innerEx.InnerException;
            }
            return msg;
        }
    }
}
