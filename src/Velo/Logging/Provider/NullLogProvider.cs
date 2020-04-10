using System;

namespace Velo.Logging.Provider
{
    internal sealed class NullLogProvider : ILogProvider
    {
        public static readonly ILogProvider Instance = new NullLogProvider();
        
        public void Write(LogLevel level, Type sender, string template)
        {
        }

        public void Write<T1>(LogLevel level, Type sender, string template, T1 arg1)
        {
        }

        public void Write<T1, T2>(LogLevel level, Type sender, string template, T1 arg1, T2 arg2)
        {
        }

        public void Write<T1, T2, T3>(LogLevel level, Type sender, string template, T1 arg1, T2 arg2, T3 arg3)
        {
        }

        public void Write(LogLevel level, Type sender, string template, params object[] args)
        {
        }
    }
}