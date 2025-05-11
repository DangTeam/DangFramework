using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Dang.Features
{
    public static class Log
    {
        private static readonly string LogFile = Path.Combine(Path.Combine(Environment.CurrentDirectory, "Dang"), "Logs", "Dang.log");
        private static LogLevel _logLevel = LogLevel.Info;

        public enum LogLevel
        {
            Debug,
            Info,
            Warning,
            Error
        }

        public static void SetLogLevel(LogLevel level)
        {
            _logLevel = level;
        }

        public static void Debug(string message)
        {
            if (_logLevel <= LogLevel.Debug)
                WriteLog("DEBUG", message);//, ConsoleColor.Gray);
        }

        public static void Info(string message)
        {
            if (_logLevel <= LogLevel.Info)
                WriteLog("INFO", message);//, ConsoleColor.White);
        }

        public static void Warning(string message)
        {
            if (_logLevel <= LogLevel.Warning)
                WriteLog("WARNING", message);//, ConsoleColor.Yellow);
        }

        public static void Error(string message)
        {
            if (_logLevel <= LogLevel.Error)
                WriteLog("ERROR", message);//, ConsoleColor.Red);
        }

        private static void WriteLog(string level, string message)//, ConsoleColor color)
        {
            var logMessage = $"[{DateTime.Now:yyyy-MM-dd HH:mm:ss}] [{level}] {message}";
            //Console.ForegroundColor = color;
            Console.WriteLine(logMessage);
            //Console.ResetColor();
            try
            {
                File.AppendAllText(LogFile, logMessage + "\n");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Ошибка записи в лог файл: {ex.Message}");
            }
        }
    }
}
