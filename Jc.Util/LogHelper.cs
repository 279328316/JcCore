using log4net;
using log4net.Repository;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Jc.Core
{
    /// <summary>
    /// Log Helper
    /// </summary>
    public class LogHelper
    {
        private static object loggerLock = new object();
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
        /// 初始化DbLogger
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
        /// 初始化DbErrorLogger
        /// </summary>
        public static void SetErrorLogger(string repositoryName, string errorLoggerName)
        {
            ILoggerRepository repository = LogManager.GetAllRepositories().FirstOrDefault(a => a.Name == repositoryName);
            if (repository != null)
            {
                Logger = LogManager.GetLogger(repository.Name, errorLoggerName);
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
                catch(Exception ex)
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
                catch(Exception ex)
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
    }
}
