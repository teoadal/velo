using System;
using System.IO;
using Velo.Serialization.Models;
using Velo.Threading;

namespace Velo.Logging.Writers
{
    internal sealed class DefaultConsoleWriter : ILogWriter, IDisposable
    {
        public LogLevel Level { get; }

        private readonly object _lock;
        private readonly TextWriter _errorOutput;
        private readonly TextWriter _output;

        public DefaultConsoleWriter(LogLevel level = LogLevel.Debug)
        {
            Level = level;

            _lock = new object();

            _errorOutput = Console.Error;
            _output = Console.Out;
        }

        public void Write(LogContext context, JsonObject message)
        {
            var output = context.Level == LogLevel.Error
                ? _errorOutput
                : _output;
            
            using (Lock.Enter(_lock))
            {
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

        public void Dispose()
        {
            _errorOutput.Dispose();
            _output.Dispose();
        }
    }
}