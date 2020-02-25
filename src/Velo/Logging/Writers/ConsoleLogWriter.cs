using System;
using System.Collections.Concurrent;
using System.IO;
using Velo.Extensions;
using Velo.Serialization.Models;
using Velo.Utils;

namespace Velo.Logging.Writers
{
    internal sealed partial class ConsoleLogWriter : ILogWriter
    {
        public LogLevel Level => LogLevel.Debug;

        private readonly object _lock;
        private readonly TextWriter _output;
        private readonly ConcurrentDictionary<string, MessageWriter> _messageWriters;
        private readonly Func<string, MessageWriter> _messageWriterBuilder;

        public ConsoleLogWriter()
        {
            _lock = new object();
            _messageWriters = new ConcurrentDictionary<string, MessageWriter>();
            _messageWriterBuilder = BuildWriter;
        }

        public ConsoleLogWriter(TextWriter output) : this()
        {
            _output = output;
        }

        public void Write(LogContext context, JsonObject message)
        {
            var messageWriter = _messageWriters.GetOrAdd(context.Template, _messageWriterBuilder);
            
            using (Lock.Enter(_lock))
            {
                var output = GetOutput(context.Level);
                
                WritePrefixes(message, output);
                messageWriter.Write(message, output);
                output.Flush();
            }
        }

        private TextWriter GetOutput(LogLevel level)
        {
            if (_output != null) return _output;

            return level == LogLevel.Error
                ? Console.Error
                : Console.Out;
        }

        private static void WritePrefixes(JsonObject message, TextWriter writer)
        {
            foreach (var (propertyName, jsonData) in message)
            {
                if (propertyName[0] != '_') continue;
                writer.Write('[');
                jsonData.Serialize(writer);
                writer.Write("] ");
            }
        }
    }
}