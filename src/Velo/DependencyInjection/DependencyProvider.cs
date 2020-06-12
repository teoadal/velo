using System;
using System.Runtime.CompilerServices;
using Velo.Utils;

namespace Velo.DependencyInjection
{
    public sealed class DependencyProvider : IServiceProvider, IDisposable
    {
        private IDependencyScope _defaultScope;
        private bool _disposed;
        private IDependencyEngine _engine;

        internal DependencyProvider(IDependencyEngine engine)
        {
            _engine = engine;
            _defaultScope = new DependencyScope(engine, new object());
        }

        public object? GetService(Type contract)
        {
            EnsureNotDisposed();

            return _defaultScope.GetService(contract);
        }

        public IDependencyScope StartScope()
        {
            EnsureNotDisposed();

            return _defaultScope.StartScope();
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private void EnsureNotDisposed()
        {
            if (_disposed) throw Error.Disposed(nameof(IDependencyScope));
        }

        public void Dispose()
        {
            if (_disposed) return;

            _disposed = true; // contains self

            _defaultScope.Dispose();
            _defaultScope = null!;

            _engine.Dispose();
            _engine = null!;
        }
    }
}