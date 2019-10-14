using System;
using Velo.DependencyInjection.Resolvers;

namespace Velo.DependencyInjection.Dependencies
{
    internal sealed class SingletonDependency : Dependency, IDisposable
    {
        private object _instance;

        public SingletonDependency(DependencyResolver resolver): base(resolver)
        {
        }

        public override object GetInstance(DependencyProvider scope)
        {
            return _instance ??= Resolver.Resolve(scope);
        }

        public void Dispose()
        {
            if (_instance != null && _instance is IDisposable disposable)
            {
                disposable.Dispose();
            }
        }
    }
}