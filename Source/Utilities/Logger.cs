using System;
using System.Diagnostics;

namespace Quark.Utilities
{
    public static class Logger
    {
        public static LogLevel Level = LogLevel.None;
        public static LogType Type = LogType.Unity | LogType.File;

        private static void Add(string message, LogLevel level)
        {
            if (level > Level)
                return;
            if ((Type & LogType.Unity) == LogType.Unity)
            {
                if (Level >= LogLevel.Debug || Level == LogLevel.GC)
                    UnityEngine.Debug.Log(message);
                else if (Level == LogLevel.Warning)
                    UnityEngine.Debug.LogWarning(message);
                else
                    UnityEngine.Debug.LogError(message);
            }
        }

        public static void Info(string message)
        {
            Add(message, LogLevel.Info);
        }

        public static void Log(string message)
        {
            Add(message, LogLevel.Log);
        }

        public static void Debug(string message)
        {
#if DEBUG
            Add(message, LogLevel.Debug);
#endif
        }

        public static void Warn(string message)
        {
            Add(message, LogLevel.Warning);
        }

        public static void Error(string message)
        {
            Add(message, LogLevel.Error);
        }

        public static void GC(string message)
        {
#if DEBUG
            Add("GC: " + message, LogLevel.GC);
#endif
        }

        public static void GC()
        {
#if DEBUG
            StackTrace stackTrace = new StackTrace();
            StackFrame callingFrame = stackTrace.GetFrame(1);
            GC(callingFrame.GetMethod().DeclaringType.Name + "::" + callingFrame.GetMethod().Name);
#endif
        }
    }

    [Flags]
    public enum LogLevel
    {
        Info = 6,
        Log = 5,
        Debug = 4,
        Warning = 3,
        Error = 2,
        GC = 1,
        None = 0
    }

    public enum LogType
    {
        Unity = 1,
        Console = 2,
        File = 4
    }
}

