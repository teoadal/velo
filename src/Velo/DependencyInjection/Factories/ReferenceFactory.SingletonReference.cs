using System;
using Velo.Utils;

namespace Velo.DependencyInjection.Factories
{
    internal sealed partial class ReferenceFactory
    {
        private sealed class SingletonReference<T> : IReference<T>
            where T : class
        {
            private T? _instance;
            private IServiceProvider _serviceProvider;

            private bool _disposed;

            public SingletonReference(IServiceProvider serviceProvider)
            {
                _serviceProvider = serviceProvider;
            }

            public T Value
            {
                get
                {
                    if (_disposed) throw Error.Disposed(nameof(IReference<T>));

                    return _instance ??= (T) _serviceProvider.GetService(Typeof<T>.Raw);
                }
            }

            public void Dispose()
            {
                if (_disposed) return;

                _instance = null;
                _serviceProvider = null!;
                _disposed = true;
            }
        }
    }
}