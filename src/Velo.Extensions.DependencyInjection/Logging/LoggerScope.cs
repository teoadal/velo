using System;

namespace Velo.Extensions.DependencyInjection.Logging
{
    internal sealed class LoggerScope : IDisposable
    {
        public static readonly IDisposable Instance = new LoggerScope();

        public void Dispose()
        {
        }
    }
}