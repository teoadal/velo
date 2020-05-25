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

            public TransientReference(IServiceProvider serviceProvider)
            {
                _serviceProvider = serviceProvider;
            }

            public T Value => (T) _serviceProvider.GetService(Typeof<T>.Raw);

            public void Dispose()
            {
                _serviceProvider = null!;
            }
        }
    }
}