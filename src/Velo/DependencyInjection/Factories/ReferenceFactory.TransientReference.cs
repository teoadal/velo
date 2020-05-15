using System;
using Velo.Utils;

namespace Velo.DependencyInjection.Factories
{
    internal sealed partial class ReferenceFactory
    {
        private sealed class TransientReference<T> : IReference<T>
            where T : class
        {
            private IServiceProvider _serviceProvider;

            private bool _disposed;

            public TransientReference(IServiceProvider serviceProvider)
            {
                _serviceProvider = serviceProvider;
            }

            public T Value
            {
                get
                {
                    if (_disposed) throw Error.Disposed(nameof(IReference<T>));

                    return (T) _serviceProvider.GetService(Typeof<T>.Raw);
                }
            }

            public void Dispose()
            {
                if (_disposed) return;

                _serviceProvider = null!;
                _disposed = true;
            }
        }
    }
}