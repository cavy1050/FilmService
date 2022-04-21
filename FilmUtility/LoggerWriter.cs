using log4net;
using log4net.Config;
using System;
using System.IO;
namespace FilmUtility
{
    public class LoggerWriter
    {
        public static void LogerInit()
        {
            FileInfo configFile = new FileInfo(AppDomain.CurrentDomain.BaseDirectory + "Log4Net.config");
            XmlConfigurator.Configure(configFile);
        }
        public static void Warn(string message)
        {
            if (!string.IsNullOrEmpty(message))
            {
                if (Log.IsWarnEnabled)
                {
                    Log.Warn(message);
                }
            }
        }


        public static void Info(string message)
        {
            if (!string.IsNullOrEmpty(message))
            {
                if (Log.IsInfoEnabled)
                {
                    Log.Info(message);
                }
            }
        }


        public static void Info(string LogKey, string MessageName, string Message)
        {
            if (!string.IsNullOrEmpty(Message))
            {
                if (Log.IsInfoEnabled)
                {
                    Log.Info("****** " + LogKey + " " + MessageName + " " + Message + " ******");
                }
            }
        }

        public static void Info(object obj)
        {
            if (null != obj)
            {
                Log.Info(obj);
            }
        }

        public static void Error(Exception ex)
        {
            if (ex != null)
            {
                if (Log.IsErrorEnabled)
                {
                    Log.Error(ex.Message, ex);
                }
            }
        }
        public static void Error(string LogKey, Exception ex)
        {
            if (ex != null)
            {
                if (Log.IsErrorEnabled)
                {
                    Log.Error("****** " + LogKey + ex.Message + " ******", ex);
                }
            }
        }
        private static readonly ILog Log = LogManager.GetLogger("日志");
    }

}
