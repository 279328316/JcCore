using log4net;
using log4net.Config;
using log4net.Repository;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

namespace Jc
{
    /// <summary>
    /// Log Helper
    /// </summary>
    public class LogHelper
    {
        private static ILog logger;
        private static ILog errorLogger;

        /// <summary>
        /// 日志记录
        /// </summary>
        public static ILog Logger
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
        public static ILog ErrorLogger
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

        /// <summary>
        /// 初始化Logger
        /// </summary>
        public static void SetLogger(string repositoryName,string loggerName)
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
        public static void SetErrorLogger(string repositoryName, string errorLoggerName)
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
        public static void InitLogger(ILog logger,ILog errorLogger = null)
        {
            Logger = logger;
            ErrorLogger = errorLogger;
        }

        /// <summary>
        /// 初始化Logger
        /// </summary>
        public static void InitLogger(string logConfigPath,string repositoryName, string loggerName, string errorLoggerName = null)
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
        private static ILoggerRepository GetLogRepository(string logConfigPath,string repositoryName)
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
        public static void WriteLog(string msg)
        {
            Info(msg);
        }

        /// <summary>
        /// 记录日志
        /// </summary>
        /// <param name="msg"></param>
        public static void Info(string msg)
        {
            if (Logger != null)
            {
                try
                {
                    Logger.Info(msg);
                }
                catch(Exception ex)
                {

                }
            }
        }

        /// <summary>
        /// 记录日志
        /// </summary>
        /// <param name="msg"></param>
        public static void Warn(string msg)
        {
            if (Logger != null)
            {
                try
                {
                    Logger.Warn(msg);
                }
                catch(Exception ex)
                {

                }
            }
        }

        /// <summary>
        /// 记录日志
        /// </summary>
        /// <param name="msg"></param>
        public static void Error(string msg)
        {
            try
            {
                if (ErrorLogger != null)
                {
                    ErrorLogger.Error(msg);
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
        public static void Error(Exception ex)
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
        /// 记录日志
        /// </summary>
        /// <param name="msg"></param>
        /// <param name="ex">异常信息</param>
        public static void Error(string msg,Exception ex)
        {
            try
            {
                if (string.IsNullOrEmpty(msg))
                {
                    msg = "";
                }
                msg = msg + GetExceptionInfo(ex);
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
