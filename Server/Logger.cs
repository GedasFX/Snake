using System;

namespace Server
{
    /// <summary>
    /// A simple logger class used for logging messages to the console. Not thread-safe but it doesn't need to be.
    /// </summary>
    public class Logger
    {
        private static readonly Logger instance = new Logger();

        /// <summary>
        /// The instance of the logger singleton class.
        /// </summary>
        public static Logger Instance {
            get { return instance; }
        }

        /// <summary>
        /// Logs a regular message to the console.
        /// </summary>
        /// <param name="msg">Message to log.</param>
        public void LogMessage(string msg) => LogColor(ConsoleColor.White, msg);

        /// <summary>
        /// Logs a warning message to the console.
        /// </summary>
        /// <param name="msg">Warning message to log.</param>
        public void LogWarning(string msg) => LogColor(ConsoleColor.Yellow, "[WARNING]: " + msg);

        /// <summary>
        /// Logs an error message to the console.
        /// </summary>
        /// <param name="msg">Error message to log.</param>
        public void LogError(string msg) => LogColor(ConsoleColor.Red, "[ERROR]: " + msg);

        private void LogColor(ConsoleColor textColor, string msg)
        {
            Console.ForegroundColor = textColor;
            Console.WriteLine(msg);
            Console.ResetColor();
        }

        private Logger()
        {
            Console.WriteLine("Created logger instance!");
        }
    }
}