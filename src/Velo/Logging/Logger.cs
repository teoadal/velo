using System;

namespace Velo.Logging
{
    internal sealed class Logger<TSource> : ILogger<TSource>
    {
        private readonly LogProvider _provider;
        private readonly Type _sender;

        public Logger(LogProvider provider)
        {
            _provider = provider;
            _sender = typeof(TSource);
        }

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
    }
}