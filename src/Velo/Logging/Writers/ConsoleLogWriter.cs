using System;
using System.IO;
using Velo.Serialization.Models;
using Velo.Utils;

namespace Velo.Logging.Writers
{
    internal sealed class ConsoleLogWriter : ILogWriter
    {
        public LogLevel Level { get; }

        private readonly object _lock;

        public ConsoleLogWriter(LogLevel level = LogLevel.Debug)
        {
            Level = level;

            _lock = new object();
        }

        public void Write(LogContext context, JsonObject message)
        {
            using (Lock.Enter(_lock))
            {
                var output = GetOutput(context.Level);

                Console.ForegroundColor = GetColor(context.Level);

                context.WriteMessage(message, output);

                output.WriteLine();
                output.Flush();

                Console.ResetColor();
            }
        }

        private static ConsoleColor GetColor(LogLevel level)
        {
            switch (level)
            {
                case LogLevel.Trace:
                    return ConsoleColor.Gray;
                case LogLevel.Debug:
                    return ConsoleColor.DarkGray;
                case LogLevel.Warning:
                    return ConsoleColor.Yellow;
                case LogLevel.Error:
                    return ConsoleColor.Red;
                default:
                    return ConsoleColor.White;
            }
        }

        private static TextWriter GetOutput(LogLevel level)
        {
            return level == LogLevel.Error
                ? Console.Error
                : Console.Out;
        }
    }
}