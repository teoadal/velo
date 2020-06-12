using System;
using Velo.Logging.Provider;

namespace Velo.Logging
{
    internal sealed class Logger<TSender> : ILogger<TSender>
    {
        private readonly ILogProvider _provider;
        private readonly Type _sender = typeof(TSender);

        public Logger(ILogProvider provider)
        {
            _provider = provider;
        }

        #region Log

        public void Log(LogLevel level, string message)
        {
            _provider.Write(level, _sender, message);
        }

        public void Log<T1>(LogLevel level, string template, T1 arg1)
        {
            _provider.Write(level, _sender, template, arg1);
        }

        public void Log<T1, T2>(LogLevel level, string template, T1 arg1, T2 arg2)
        {
            _provider.Write(level, _sender, template, arg1, arg2);
        }

        public void Log<T1, T2, T3>(LogLevel level, string template, T1 arg1, T2 arg2, T3 arg3)
        {
            _provider.Write(level, _sender, template, arg1, arg2, arg3);
        }

        public void Log(LogLevel level, string template, params object[] args)
        {
            _provider.Write(level, _sender, template, args);
        }

        #endregion

        #region Trace

        public void Trace(string message)
        {
            _provider.Write(LogLevel.Trace, _sender, message);
        }

        public void Trace<T1>(string template, T1 arg1)
        {
            _provider.Write(LogLevel.Trace, _sender, template, arg1);
        }

        public void Trace<T1, T2>(string template, T1 arg1, T2 arg2)
        {
            _provider.Write(LogLevel.Trace, _sender, template, arg1, arg2);
        }

        public void Trace<T1, T2, T3>(string template, T1 arg1, T2 arg2, T3 arg3)
        {
            _provider.Write(LogLevel.Trace, _sender, template, arg1, arg2, arg3);
        }

        public void Trace(string template, params object[] args)
        {
            _provider.Write(LogLevel.Trace, _sender, template, args);
        }

        #endregion

        #region Debug

        public void Debug(string message)
        {
            _provider.Write(LogLevel.Debug, _sender, message);
        }

        public void Debug<T1>(string template, T1 arg1)
        {
            _provider.Write(LogLevel.Debug, _sender, template, arg1);
        }

        public void Debug<T1, T2>(string template, T1 arg1, T2 arg2)
        {
            _provider.Write(LogLevel.Debug, _sender, template, arg1, arg2);
        }

        public void Debug<T1, T2, T3>(string template, T1 arg1, T2 arg2, T3 arg3)
        {
            _provider.Write(LogLevel.Debug, _sender, template, arg1, arg2, arg3);
        }

        public void Debug(string template, params object[] args)
        {
            _provider.Write(LogLevel.Debug, _sender, template, args);
        }

        #endregion

        #region Info

        public void Info(string message)
        {
            _provider.Write(LogLevel.Info, _sender, message);
        }

        public void Info<T1>(string template, T1 arg1)
        {
            _provider.Write(LogLevel.Info, _sender, template, arg1);
        }

        public void Info<T1, T2>(string template, T1 arg1, T2 arg2)
        {
            _provider.Write(LogLevel.Info, _sender, template, arg1, arg2);
        }

        public void Info<T1, T2, T3>(string template, T1 arg1, T2 arg2, T3 arg3)
        {
            _provider.Write(LogLevel.Info, _sender, template, arg1, arg2, arg3);
        }

        public void Info(string template, params object[] args)
        {
            _provider.Write(LogLevel.Info, _sender, template, args);
        }

        #endregion

        #region Warning

        public void Warning(string message)
        {
            _provider.Write(LogLevel.Warning, _sender, message);
        }

        public void Warning<T1>(string template, T1 arg1)
        {
            _provider.Write(LogLevel.Warning, _sender, template, arg1);
        }

        public void Warning<T1, T2>(string template, T1 arg1, T2 arg2)
        {
            _provider.Write(LogLevel.Warning, _sender, template, arg1, arg2);
        }

        public void Warning<T1, T2, T3>(string template, T1 arg1, T2 arg2, T3 arg3)
        {
            _provider.Write(LogLevel.Warning, _sender, template, arg1, arg2, arg3);
        }

        public void Warning(string template, params object[] args)
        {
            _provider.Write(LogLevel.Warning, _sender, template, args);
        }

        #endregion

        #region Error

        public void Error(string message)
        {
            _provider.Write(LogLevel.Error, _sender, message);
        }

        public void Error<T1>(string template, T1 arg1)
        {
            _provider.Write(LogLevel.Error, _sender, template, arg1);
        }

        public void Error<T1, T2>(string template, T1 arg1, T2 arg2)
        {
            _provider.Write(LogLevel.Error, _sender, template, arg1, arg2);
        }

        public void Error<T1, T2, T3>(string template, T1 arg1, T2 arg2, T3 arg3)
        {
            _provider.Write(LogLevel.Error, _sender, template, arg1, arg2, arg3);
        }

        public void Error(string template, params object[] args)
        {
            _provider.Write(LogLevel.Error, _sender, template, args);
        }

        #endregion
    }
}