namespace Quark.Utilities
{
    public static class Logger
    {
        public static LogLevel Level = LogLevel.None;
        public static LogType Type = LogType.Unity | LogType.File;

        private static void Add(string message, LogLevel level)
        {
            if (level > Logger.Level)
                return;
            if ((Logger.Type & LogType.Unity) == LogType.Unity)
            {
                if (Logger.Level >= LogLevel.Debug || Logger.Level == LogLevel.GC)
                    UnityEngine.Debug.Log(message);
                else if (Logger.Level == LogLevel.Warning)
                    UnityEngine.Debug.LogWarning(message);
                else
                    UnityEngine.Debug.LogError(message);
            }
        }

        public static void Info(string message)
        {
            Logger.Add(message, LogLevel.Info);
        }

        public static void Log(string message)
        {
            Logger.Add(message, LogLevel.Log);
        }

        public static void Debug(string message)
        {
            Logger.Add(message, LogLevel.Debug);
        }

        public static void Warn(string message)
        {
            Logger.Add(message, LogLevel.Warning);
        }

        public static void Error(string message)
        {
            Logger.Add(message, LogLevel.Error);
        }

        public static void GC(string message)
        {
            Logger.Add(message, LogLevel.GC);
        }
    }

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

