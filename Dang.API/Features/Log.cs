using System;
using System.IO;

namespace Dang.API.Features
{
    public static class Log
    {
        private static readonly string LogFile = Path.Combine("Dang", "Logs", "Dang.log");
        private static LogLevel _logLevel = LogLevel.Info;

        public enum LogLevel { Debug, Info, Warning, Error }

        public static void SetLogLevel(LogLevel level) => _logLevel = level;

        public static void Debug(string message)
        {
            if (_logLevel <= LogLevel.Debug)
                Write("DEBUG", message);
        }

        public static void Info(string message)
        {
            if (_logLevel <= LogLevel.Info)
                Write("INFO", message);
        }

        public static void Warning(string message)
        {
            if (_logLevel <= LogLevel.Warning)
                Write("WARNING", message);
        }

        public static void Error(string message)
        {
            if (_logLevel <= LogLevel.Error)
                Write("ERROR", message);
        }

        private static void Write(string level, string message)
        {
            var logMessage = $"[{DateTime.Now:yyyy-MM-dd HH:mm:ss}] [{level}] {message}";
            Console.WriteLine(logMessage);

            try
            {
                File.AppendAllText(LogFile, logMessage + Environment.NewLine);
            }
            catch
            {
                Console.WriteLine("Failed to write to log file.");
            }
        }
    }
}