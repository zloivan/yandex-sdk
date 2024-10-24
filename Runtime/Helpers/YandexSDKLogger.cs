using System;
using UnityEngine;
using Object = UnityEngine.Object;

namespace Yandex.Helpers
{
    public class YandexSDKLogger : ILogger
    {
        public ILogHandler logHandler { get; set; } = Debug.unityLogger;
        public bool logEnabled { get; set; } = true;
        public LogType filterLogType { get; set; } = LogType.Log;

        public void LogFormat(LogType logType, Object context, string format, params object[] args)
        {
            if (IsLogTypeAllowed(logType))
            {
                logHandler?.LogFormat(logType, context, format, args);
            }
        }

        public void LogException(Exception exception, Object context)
        {
            if (logEnabled && logHandler != null)
            {
                logHandler.LogException(exception, context);
            }
        }

        public bool IsLogTypeAllowed(LogType logType)
        {
            if (!logEnabled)
                return false;

            if (logType == LogType.Exception || logType == LogType.Error || logType == LogType.Warning)
                return true;

#if YANDEX_SDK_LOGGING
            return logType == LogType.Log;
#else
            return false;
#endif
        }

        public void Log(LogType logType, object message)
        {
            if (IsLogTypeAllowed(logType))
            {
                logHandler?.LogFormat(logType, null, "{0}", message);
            }
        }

        public void Log(LogType logType, object message, Object context)
        {
            if (IsLogTypeAllowed(logType))
            {
                logHandler?.LogFormat(logType, context, "{0}", message);
            }
        }

        public void Log(LogType logType, string tag, object message)
        {
            if (IsLogTypeAllowed(logType))
            {
                logHandler?.LogFormat(logType, null, "{0}: {1}", tag, message);
            }
        }

        public void Log(LogType logType, string tag, object message, Object context)
        {
            if (IsLogTypeAllowed(logType))
            {
                logHandler?.LogFormat(logType, context, "{0}: {1}", tag, message);
            }
        }

        public void Log(object message)
        {
            Log(LogType.Log, message);
        }

        public void Log(string tag, object message)
        {
            Log(LogType.Log, tag, message);
        }

        public void Log(string tag, object message, Object context)
        {
            Log(LogType.Log, tag, message, context);
        }

        public void LogWarning(string tag, object message)
        {
            Log(LogType.Warning, tag, message);
        }

        public void LogWarning(string tag, object message, Object context)
        {
            Log(LogType.Warning, tag, message, context);
        }

        public void LogError(string tag, object message)
        {
            Log(LogType.Error, tag, message);
        }

        public void LogError(string tag, object message, Object context)
        {
            Log(LogType.Error, tag, message, context);
        }

        public void LogFormat(LogType logType, string format, params object[] args)
        {
            if (IsLogTypeAllowed(logType))
            {
                logHandler?.LogFormat(logType, null, format, args);
            }
        }

        public void LogException(Exception exception)
        {
            LogException(exception, null);
        }
    }
}