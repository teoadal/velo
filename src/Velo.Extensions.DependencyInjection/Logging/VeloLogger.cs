using System;
using System.Collections.Generic;
using Microsoft.Extensions.Logging;
using Velo.Collections.Local;
using Velo.Logging.Provider;

namespace Velo.Extensions.DependencyInjection.Logging
{
    internal sealed class VeloLogger<T> : ILogger<T>
    {
        private const string OriginalFormat = "{OriginalFormat}";

        private readonly ILogProvider _provider;
        private readonly Type _sender;

        public VeloLogger(ILogProvider provider)
        {
            _provider = provider;
            _sender = typeof(T);
        }

        public IDisposable BeginScope<TState>(TState state)
        {
            return LoggerScope.Instance;
        }

        public bool IsEnabled(LogLevel logLevel)
        {
            var level = ConvertLogLevel(logLevel);
            return level >= _provider.Level;
        }

        public void Log<TState>(LogLevel logLevel, EventId eventId, TState state, Exception exception,
            Func<TState, Exception, string> formatter)
        {
            string? template = null;
            var arguments = new LocalList<object?>();

            var valuesCollection = (IReadOnlyList<KeyValuePair<string, object>>) state!;
            foreach (var (name, value) in valuesCollection)
            {
                if (name == OriginalFormat && value is string originalFormat)
                {
                    template = originalFormat;
                }
                else arguments.Add(value);
            }

            var level = ConvertLogLevel(logLevel);
            if (template == null || arguments.Any(arg => arg == null))
            {
                var message = formatter(state, exception);
                _provider.Write(level, _sender, message);
            }
            else if (arguments.Length == 0)
            {
                _provider.Write(level, _sender, template);
            }
            else
            {
                _provider.Write(level, _sender, template, arguments.ToArray()!);
            }
        }

        private static Velo.Logging.LogLevel ConvertLogLevel(LogLevel level)
        {
            switch (level)
            {
                case LogLevel.Debug:
                    return Velo.Logging.LogLevel.Debug;
                case LogLevel.Information:
                    return Velo.Logging.LogLevel.Info;
                case LogLevel.Warning:
                    return Velo.Logging.LogLevel.Warning;
                case LogLevel.Critical:
                case LogLevel.Error:
                    return Velo.Logging.LogLevel.Error;
                default:
                    return Velo.Logging.LogLevel.Trace;
            }
        }
    }
}