using log4net;
using log4net.Config;
using log4net.Repository;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

namespace Jc.Core.TestApp
{
    public class JcLogHelper
    {
        private static ILog logger;

        static JcLogHelper()
        {
            if (!LogManager.GetAllRepositories().Any(a => a.Name == Consts.LogRepository))
            {
                ILoggerRepository repository = LogManager.CreateRepository(Consts.LogRepository);
                string logConfigPath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "appLog.config");
                XmlConfigurator.Configure(repository, new FileInfo(logConfigPath));
            }
        }

        /// <summary>
        /// 日志记录
        /// </summary>
        public static ILog Logger
        {
            get
            {
                if (logger == null)
                {
                    logger = LogManager.GetLogger(Consts.LogRepository, Consts.LoggerName);
                }
                return logger;
            }
        }

        /// <summary>
        /// 记录日志
        /// </summary>
        /// <param name="msg"></param>
        public static void WriteLog(string msg)
        {
            if (Logger != null)
            {
                Logger.Info(msg);
            }
        }
    }
}
