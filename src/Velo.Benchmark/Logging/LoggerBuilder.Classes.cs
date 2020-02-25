using System;
using System.IO;
using System.Text;
using NLog;
using NLog.Config;
using NLog.Targets;
using Serilog.Core;
using Serilog.Events;
using Velo.Logging;
using Velo.Logging.Writers;
using Velo.Serialization.Models;
using LogLevel = Velo.Logging.LogLevel;

namespace Velo.Benchmark.Logging
{
    internal readonly struct LogMessage
    {
        public readonly Guid Id;
        public readonly string Name;
        public readonly int Value;

        public LogMessage(Guid id, string name, int value)
        {
            Id = id;
            Name = name;
            Value = value;
        }
    }

    internal sealed class NullSink : ILogEventSink
    {
        public int Counter;

        public void Emit(LogEvent logEvent)
        {
            Counter++;
        }
    }

    internal sealed class NullLogWriter : ILogWriter
    {
        public int Counter;

        public LogLevel Level => LogLevel.Trace;

        public void Write(LogContext context, JsonObject message)
        {
            Counter++;
        }
    }

    [Target(nameof(NullLogTarget))]
    internal sealed class NullLogTarget : TargetWithLayout
    {
        public int Counter;

        [RequiredParameter] 
        public string Host { get; set; }

        public NullLogTarget()
        {
            Name = nameof(NullLogTarget);
            Host = "localhost";
        }

        protected override void Write(LogEventInfo logEvent)
        {
            Counter++;
        }
    }

    internal sealed class StringSink : ILogEventSink
    {
        private readonly StringBuilder _stringBuilder;
        private readonly StringWriter _stringWriter;

        public StringSink()
        {
            _stringBuilder = new StringBuilder(25000);
            _stringWriter = new StringWriter(_stringBuilder);
        }

        public void Emit(LogEvent logEvent)
        {
            logEvent.RenderMessage(_stringWriter);
        }

        public int Release()
        {
            var result = _stringBuilder.Length;
            _stringBuilder.Clear();

            return result;
        }
    }

    [Target(nameof(StringLogTarget))]
    internal sealed class StringLogTarget : TargetWithContext
    {
        private readonly StringBuilder _stringBuilder;

        public StringLogTarget()
        {
            IncludeEventProperties = true; // Include LogEvent Properties by default
            Name = nameof(StringLogTarget);
            Host = "localhost";

            _stringBuilder = new StringBuilder(25000);
        }

        [RequiredParameter] 
        public string Host { get; set; }

        public int Release()
        {
            var result = _stringBuilder.Length;
            _stringBuilder.Clear();

            return result;
        }

        protected override void Write(LogEventInfo logEvent)
        {
            var logMessage = RenderLogEvent(Layout, logEvent);
            if (GetAllProperties(logEvent).Count > 0)
            {
                _stringBuilder.AppendLine(logMessage);
            }
        }
    }

    internal sealed class StringLogWriter : ILogWriter
    {
        public LogLevel Level => LogLevel.Debug;

        private readonly StringBuilder _stringBuilder;
        private readonly ConsoleLogWriter _writer;

        public StringLogWriter()
        {
            _stringBuilder = new StringBuilder(25000);
            _writer = new ConsoleLogWriter(new StringWriter(_stringBuilder));
        }

        public int Release()
        {
            var result = _stringBuilder.Length;
            _stringBuilder.Clear();

            return result;
        }

        public void Write(LogContext context, JsonObject message)
        {
            _writer.Write(context, message);
        }
    }
}