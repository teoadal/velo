using System;

namespace Velo.DependencyInjection
{
    internal sealed class DelegateServiceResolver : IServiceProvider
    {
        private readonly Func<Type, object> _resolver;

        public DelegateServiceResolver(Func<Type, object> resolver)
        {
            _resolver = resolver;
        }

        public object GetService(Type serviceType)
        {
            return _resolver(serviceType);
        }
    }
}