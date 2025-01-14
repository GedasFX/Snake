﻿using System;

namespace Server
{
    /// <summary>
    /// A simple logger class used for logging messages to the console. Not thread-safe but it doesn't need to be.
    /// </summary>
    public class Logger
    {
        /// <summary>
        /// The instance of the logger singleton class.
        /// </summary>
        public static Logger Instance { get; } = new Logger();

        /// <summary>
        /// Logs a regular message to the console.
        /// </summary>
        /// <param name="msg">Message to log.</param>
        public void LogMessage(string msg) => LogWithColor(ConsoleColor.White, msg);

        /// <summary>
        /// Logs a warning message to the console.
        /// </summary>
        /// <param name="msg">Warning message to log.</param>
        public void LogWarning(string msg) => LogWithColor(ConsoleColor.Yellow, "[WARNING]: " + msg);

        /// <summary>
        /// Logs an error message to the console.
        /// </summary>
        /// <param name="msg">Error message to log.</param>
        public void LogError(string msg) => LogWithColor(ConsoleColor.Red, "[ERROR]: " + msg);

        public void LogWithColor(ConsoleColor textColor, string msg)
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