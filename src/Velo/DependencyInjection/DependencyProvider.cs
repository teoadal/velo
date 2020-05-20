using System;
using Velo.Utils;

namespace Velo.DependencyInjection
{
    public sealed class DependencyProvider : IServiceProvider, IDisposable
    {
        private readonly DependencyScope _defaultScope;
        private readonly IDependencyEngine _engine;
        private readonly object _lock;

        private bool _disposed;

        internal DependencyProvider(IDependencyEngine engine)
        {
            _engine = engine;
            _lock = new object();

            _defaultScope = new DependencyScope(_engine, _lock);
        }

        public IDependencyScope StartScope()
        {
            return new DependencyScope(_engine, _lock);
        }

        object? IServiceProvider.GetService(Type contract)
        {
            if (_disposed) throw Error.Disposed(nameof(DependencyProvider));

            return _defaultScope.GetService(contract);
        }

        public void Dispose()
        {
            if (_disposed) return;

            _disposed = true; // contains self

            _engine.Dispose();
        }
    }
}