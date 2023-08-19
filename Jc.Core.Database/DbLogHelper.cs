using log4net;
using log4net.Appender;
using log4net.Config;
using log4net.Core;
using log4net.Filter;
using log4net.Layout;
using log4net.Repository;
using log4net.Repository.Hierarchy;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Xml.Linq;

namespace Jc.Database
{
    /// <summary>
    /// Db Log Helper
    /// </summary>
    internal class DbLogHelper
    {
        private ILog logger;
        private ILog errorLogger;

        /// <summary>
        /// 日志记录
        /// </summary>
        internal ILog Logger { get => logger; set { logger = value; } }

        /// <summary>
        /// Error日志记录
        /// </summary>
        internal ILog ErrorLogger { get => errorLogger; set { errorLogger = value; } }

        /// <summary>
        /// 初始化Logger
        /// </summary>
        internal void SetLogger(string repositoryName, string loggerName)
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
        internal void SetErrorLogger(string repositoryName, string errorLoggerName)
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
        internal void InitLogger(string dbName)
        {
            try
            {
                string repositoryName = $"{dbName}_DbRepository";
                string loggerName = "Logger";
                string errorLoggerName = "ErrorLogger";
                string logDir = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, $"DbLog/{dbName}/");
                string errorLogDir = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, $"DbLog/{dbName}/Error/");
                ILoggerRepository repository = LogManager.GetAllRepositories().FirstOrDefault(a => a.Name == repositoryName);
                if (repository == null)
                {
                    repository = LogManager.CreateRepository(repositoryName);
                }
                Hierarchy hierarchy = (Hierarchy)repository;
                Logger logger = (Logger)hierarchy.GetLogger(loggerName);
                if (logger.Appenders.Count <= 0)
                {
                    logger.AddAppender(GetFileAppender(logDir));
                }

                Logger errorLogger = (Logger)hierarchy.GetLogger(errorLoggerName);
                if (errorLogger.Appenders.Count <= 0)
                {
                    errorLogger = hierarchy.LoggerFactory.CreateLogger(hierarchy, errorLoggerName);
                    errorLogger.AddAppender(GetFileAppender(errorLogDir, "'Error_'yyyy-MM-dd'.log'"));
                }
                hierarchy.Configured = true;
                // logger.Additivity = false;   // loggingEvent won't be bubbled to its parent logger in this case.

                this.logger = LogManager.GetLogger(repository.Name, loggerName); 
                this.errorLogger = LogManager.GetLogger(repository.Name, errorLoggerName); 
            }
            catch(Exception ex)
            {

            }
        }

        /// <summary>
        /// 获取 File Appender
        /// </summary>
        /// <param name="logDir"></param>
        /// <param name="dataPattern"></param>
        /// <param name="maximumFileSize"></param>
        private RollingFileAppender GetFileAppender(string logDir,string dataPattern = "'Log_'yyyy-MM-dd'.log'", string maximumFileSize = "10MB")
        {
            //配置输出日志格式。%m表示message即日志信息。%n表示newline换行
            PatternLayout layout = new PatternLayout(@"%date{yyyy-MM-dd HH:mm:ss} %-5level %message%newline");
            layout.ActivateOptions();

            //配置日志级别为所有级别
            LevelMatchFilter filter = new LevelMatchFilter();
            filter.LevelToMatch = Level.All;
            filter.ActivateOptions();

            //配置日志【循环附加，累加】
            RollingFileAppender appender = new RollingFileAppender();

            appender.File = logDir;
            appender.ImmediateFlush = true;
            appender.MaxSizeRollBackups = 50;
            appender.MaximumFileSize = maximumFileSize;
            appender.RollingStyle = RollingFileAppender.RollingMode.Composite;
            appender.StaticLogFileName = false;
            appender.DatePattern = dataPattern;
            appender.LockingModel = new FileAppender.MinimalLock();
            appender.PreserveLogFileNameExtension = true;
            appender.AddFilter(filter);
            appender.Layout = layout;
            appender.AppendToFile = true;
            appender.ActivateOptions();
            return appender;
        }

        /// <summary>
        /// 初始化Logger
        /// </summary>
        internal void InitLogger(ILog logger, ILog errorLogger = null)
        {
            Logger = logger;
            ErrorLogger = errorLogger;
        }

        /// <summary>
        /// 初始化Logger
        /// </summary>
        internal void InitLogger(string logConfigPath, string repositoryName, string loggerName, string errorLoggerName = null)
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
        private ILoggerRepository GetLogRepository(string logConfigPath, string repositoryName)
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
        internal void WriteLog(string msg)
        {
            Info(msg);
        }

        /// <summary>
        /// 记录日志
        /// </summary>
        /// <param name="msg"></param>
        internal void Info(string msg)
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
        internal void Warn(string msg)
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
        internal void Error(string msg)
        {
            try
            {
                if (ErrorLogger != null)
                {
                    Info(msg);
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
        internal void Error(string msg ,Exception ex)
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
        internal void Error(Exception ex)
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
        private string GetExceptionInfo(Exception ex)
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
