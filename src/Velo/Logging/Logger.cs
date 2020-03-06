using System;
using Velo.Logging.Provider;

namespace Velo.Logging
{
    internal sealed class Logger<TSource> : ILogger<TSource>
    {
        private static readonly Type Sender = typeof(TSource);

        private readonly ILogProvider _provider;

        public Logger(ILogProvider provider)
        {
            _provider = provider;
        }

        #region Log

        public void Log(LogLevel level, string template)
        {
            _provider.Write(level, Sender, template);
        }

        public void Log<T1>(LogLevel level, string template, T1 arg1)
        {
            _provider.Write(level, Sender, template, arg1);
        }

        public void Log<T1, T2>(LogLevel level, string template, T1 arg1, T2 arg2)
        {
            _provider.Write(level, Sender, template, arg1, arg2);
        }

        public void Log<T1, T2, T3>(LogLevel level, string template, T1 arg1, T2 arg2, T3 arg3)
        {
            _provider.Write(level, Sender, template, arg1, arg2, arg3);
        }

        public void Log(LogLevel level, string template, params object[] args)
        {
            _provider.Write(level, Sender, template, args);
        }

        #endregion

        #region Trace

        public void Trace(string template)
        {
            _provider.Write(LogLevel.Trace, Sender, template);
        }

        public void Trace<T1>(string template, T1 arg1)
        {
            _provider.Write(LogLevel.Trace, Sender, template, arg1);
        }

        public void Trace<T1, T2>(string template, T1 arg1, T2 arg2)
        {
            _provider.Write(LogLevel.Trace, Sender, template, arg1, arg2);
        }

        public void Trace<T1, T2, T3>(string template, T1 arg1, T2 arg2, T3 arg3)
        {
            _provider.Write(LogLevel.Trace, Sender, template, arg1, arg2, arg3);
        }

        public void Trace(string template, params object[] args)
        {
            _provider.Write(LogLevel.Trace, Sender, template, args);
        }

        #endregion

        #region Debug

        public void Debug(string template)
        {
            _provider.Write(LogLevel.Debug, Sender, template);
        }

        public void Debug<T1>(string template, T1 arg1)
        {
            _provider.Write(LogLevel.Debug, Sender, template, arg1);
        }

        public void Debug<T1, T2>(string template, T1 arg1, T2 arg2)
        {
            _provider.Write(LogLevel.Debug, Sender, template, arg1, arg2);
        }

        public void Debug<T1, T2, T3>(string template, T1 arg1, T2 arg2, T3 arg3)
        {
            _provider.Write(LogLevel.Debug, Sender, template, arg1, arg2, arg3);
        }

        public void Debug(string template, params object[] args)
        {
            _provider.Write(LogLevel.Debug, Sender, template, args);
        }

        #endregion

        #region Info

        public void Info(string template)
        {
            _provider.Write(LogLevel.Info, Sender, template);
        }

        public void Info<T1>(string template, T1 arg1)
        {
            _provider.Write(LogLevel.Info, Sender, template, arg1);
        }

        public void Info<T1, T2>(string template, T1 arg1, T2 arg2)
        {
            _provider.Write(LogLevel.Info, Sender, template, arg1, arg2);
        }

        public void Info<T1, T2, T3>(string template, T1 arg1, T2 arg2, T3 arg3)
        {
            _provider.Write(LogLevel.Info, Sender, template, arg1, arg2, arg3);
        }

        public void Info(string template, params object[] args)
        {
            _provider.Write(LogLevel.Info, Sender, template, args);
        }

        #endregion

        #region Warning

        public void Warning(string template)
        {
            _provider.Write(LogLevel.Warning, Sender, template);
        }

        public void Warning<T1>(string template, T1 arg1)
        {
            _provider.Write(LogLevel.Warning, Sender, template, arg1);
        }

        public void Warning<T1, T2>(string template, T1 arg1, T2 arg2)
        {
            _provider.Write(LogLevel.Warning, Sender, template, arg1, arg2);
        }

        public void Warning<T1, T2, T3>(string template, T1 arg1, T2 arg2, T3 arg3)
        {
            _provider.Write(LogLevel.Warning, Sender, template, arg1, arg2, arg3);
        }

        public void Warning(string template, params object[] args)
        {
            _provider.Write(LogLevel.Warning, Sender, template, args);
        }

        #endregion

        #region Error

        public void Error(string template)
        {
            _provider.Write(LogLevel.Error, Sender, template);
        }

        public void Error<T1>(string template, T1 arg1)
        {
            _provider.Write(LogLevel.Error, Sender, template, arg1);
        }

        public void Error<T1, T2>(string template, T1 arg1, T2 arg2)
        {
            _provider.Write(LogLevel.Error, Sender, template, arg1, arg2);
        }

        public void Error<T1, T2, T3>(string template, T1 arg1, T2 arg2, T3 arg3)
        {
            _provider.Write(LogLevel.Error, Sender, template, arg1, arg2, arg3);
        }

        public void Error(string template, params object[] args)
        {
            _provider.Write(LogLevel.Error, Sender, template, args);
        }

        #endregion
    }
}