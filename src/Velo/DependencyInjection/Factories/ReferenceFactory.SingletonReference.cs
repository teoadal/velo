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

            public SingletonReference(IServiceProvider serviceProvider)
            {
                _serviceProvider = serviceProvider;
            }

            public T Value => _instance ??= (T) _serviceProvider.GetService(Typeof<T>.Raw);

            public void Dispose()
            {
                _instance = null;
                _serviceProvider = null!;
            }
        }
    }
}