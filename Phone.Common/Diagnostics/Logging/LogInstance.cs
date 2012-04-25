using System;
using System.Diagnostics;
using Phone.Common.IOC;

namespace Phone.Common.Diagnostics.Logging
{
    public class LogInstance
    {
        private LogInstance()
        {
            return;
        }

        private static LogInstance _logger;

        private static LogInstance Instance
        {
            get { return _logger ?? (_logger = new LogInstance()); }
        }

        private ILog _log;

        public bool Log(LogLevel level, string msg, params object[] args)
        {
            if (null == _log)
                _log = IoC.TryGet<ILog>();

            if (null == _log)
                throw new NullReferenceException("Failed to resolve ILog; ensure it is DI/IOC registered");

            try
            {
                _log.Write(level, msg, args);
            }
            catch (LoggingException logEx)
            {
                Debug.WriteLine(string.Format("Failed to log message. Error: {0}. Message: {1}", logEx.Message, msg));
            }

            return true;
        }

        public static bool LogInfo(string msg, params object[] args)
        {
            return Instance.Log(LogLevel.Info, msg, args);
        }

        public static bool LogDebug(string msg, params object[] args)
        {
            return Instance.Log(LogLevel.Debug, msg, args);
        }

        public static bool LogError(string msg, params object[] args)
        {
            return Instance.Log(LogLevel.Error, msg, args);
        }

        public static bool LogWarning(string msg, params object[] args)
        {
            return Instance.Log(LogLevel.Warning, msg, args);
        }

        public static bool LogException(Exception ex)
        {
            var msg = Environment.NewLine + ex + Environment.NewLine;
            return Instance.Log(LogLevel.Error, msg);
        }

        public static bool LogCritical(string msg, params object[] args)
        {
            return Instance.Log(LogLevel.Critical, msg, args);
        }

        public static bool LogCritical(Exception ex)
        {
            return Instance.Log(LogLevel.Critical, ex.ToString());
        }

    }

    
}
