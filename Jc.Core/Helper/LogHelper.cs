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
            if (ErrorLogger != null)
            {
                try
                {
                    ErrorLogger.Error(msg);
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
        /// <param name="ex">异常信息</param>
        public static void Error(string msg,Exception ex)
        {
            if (ErrorLogger != null)
            {
                try
                {
                    if (string.IsNullOrEmpty(msg))
                    {
                        msg = "";
                    }
                    msg += $"{ex.Message}\r\n{ex.StackTrace}";
                    if (ex.InnerException != null)
                    {
                        ex = GetOriginalException(ex.InnerException);
                        msg += $"{ex.Message}\r\n{ex.StackTrace}";
                    }
                    ErrorLogger.Error(msg);
                }
                catch (Exception ex1)
                {

                }
            }
        }

        /// <summary>
        /// 获取Original Exception
        /// </summary>
        /// <param name="ex"></param>
        /// <returns></returns>
        private static Exception GetOriginalException(Exception ex)
        {
            while (ex.InnerException != null)
            {
                return GetOriginalException(ex.InnerException);
            }
            return ex;
        }
    }
}
