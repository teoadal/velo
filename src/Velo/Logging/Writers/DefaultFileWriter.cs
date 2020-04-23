using System;
using System.IO;
using System.Text;
using Velo.Serialization.Models;
using Velo.Threading;

namespace Velo.Logging.Writers
{
    internal sealed class DefaultFileWriter : ILogWriter, IDisposable
    {
        public LogLevel Level { get; }

        private readonly object _lock;
        private readonly StreamWriter _output;

        public DefaultFileWriter(string path, LogLevel level = LogLevel.Debug)
        {
            Level = level;

            _lock = new object();

            var directory = Path.GetDirectoryName(path);
            if (!string.IsNullOrWhiteSpace(directory) && !Directory.Exists(directory))
            {
                Directory.CreateDirectory(directory);
            }

            var fileStream = File.Open(path, FileMode.Append, FileAccess.Write, FileShare.Read);
            _output = new StreamWriter(fileStream, Encoding.UTF8);
        }

        public void Write(LogContext context, JsonObject message)
        {
            using (Lock.Enter(_lock))
            {
                context.WriteMessage(message, _output);

                _output.WriteLine();
                _output.Flush();
            }
        }

        public void Dispose()
        {
            using (Lock.Enter(_lock))
            {
                _output.Dispose();
            }
        }
    }
}