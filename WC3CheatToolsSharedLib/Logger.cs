using System;
using System.Text;

namespace WC3CheatToolsSharedLib
{
    /// <summary>
    /// A simple logger
    /// </summary>
    public static class Logger
    {
        private const string DATETIME_FORMAT = "s";

        private static LogLevel _logLevel = LogLevel.INFO;

        public static int NumberOfErrors { get; private set; }

        public static int NumberOfWarnings { get; private set; }

        /// <summary>
        /// Sets the maximum log level that will be logged. Logs that are lower priority will not show up.
        /// </summary>
        /// <param name="level"></param>
        public static void SetLogLevel(LogLevel level)
        {
            _logLevel = level;
        }

        public static void Debug(string message)
        {
            if (LogLevel.DEBUG <= _logLevel)
            {
                log(message, LogLevel.DEBUG);
            }
        }

        public static void Log(string message)
        {
            if (LogLevel.INFO <= _logLevel)
            {
                log(message, LogLevel.INFO);
            }
        }

        public static void Warn(string message)
        {
            if (LogLevel.WARN <= _logLevel)
            {
                log(message, LogLevel.WARN);
            }

            NumberOfWarnings++;
        }
        public static void Error(string message)
        {
            if (LogLevel.ERROR <= _logLevel)
            {
                log(message, LogLevel.ERROR);
            }

            NumberOfErrors++;
        }

        public static void Error(Exception e)
        {
            Error(e.ToString());
        }


        /// <summary>
        /// Logs an empty line (with no date or log level)
        /// </summary>
        public static void LogEmptyLine()
        {
            if (_logLevel != LogLevel.OFF)
            {
                Console.WriteLine();
            }
        }

        private static void log(string message, LogLevel level)
        {
            StringBuilder sb = new StringBuilder();
            sb.Append(Enum.GetName(typeof(LogLevel), level));
            sb.Append(getLogLevelWhitespace(level));
            sb.Append(DateTime.Now.ToString(DATETIME_FORMAT));
            sb.Append(" - ");
            sb.Append(message);

            Console.WriteLine(sb.ToString());
        }

        /// <summary>
        /// Returns a string of whitespace that is sized accordingly to keep the different LogLevels aligned
        /// </summary>
        /// <param name="level"></param>
        /// <returns>The correct whitespace</returns>
        private static string getLogLevelWhitespace(LogLevel level)
        {
            const int MAX_WS = 6;
            string levelText = Enum.GetName(typeof(LogLevel), level);
            int numWhiteSpacesNeeded = MAX_WS - levelText.Length;
            return new string(' ', numWhiteSpacesNeeded);
        }

        public enum LogLevel
        {
            DEBUG = 444,
            INFO = 333,
            WARN = 222,
            ERROR = 111,
            OFF = 0
        }
    }
}
