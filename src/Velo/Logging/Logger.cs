using System;
using Velo.Logging.Provider;

namespace Velo.Logging
{
    internal sealed class Logger<TSource> : ILogger<TSource>
    {
        private readonly ILogProvider _provider;
        private readonly Type _sender;

        public Logger(ILogProvider provider)
        {
            _provider = provider;
            _sender = typeof(TSource);
        }

        #region Trace

        public void Trace(string template)
        {
            _provider.Write(LogLevel.Trace, _sender, template);
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

        public void Debug(string template)
        {
            _provider.Write(LogLevel.Debug, _sender, template);
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

        public void Info(string template)
        {
            _provider.Write(LogLevel.Info, _sender, template);
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

        public void Warning(string template)
        {
            _provider.Write(LogLevel.Warning, _sender, template);
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

        public void Error(string template)
        {
            _provider.Write(LogLevel.Error, _sender, template);
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