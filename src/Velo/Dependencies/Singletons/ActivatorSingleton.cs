using System;

namespace Velo.Dependencies.Singletons
{
    internal sealed class ActivatorSingleton : IDependency
    {
        private readonly Type _contract;
        private object _instance;

        public ActivatorSingleton(Type contract)
        {
            _contract = contract;
        }

        public bool Applicable(Type requestedType)
        {
            return _contract == requestedType;
        }

        public void Destroy()
        {
            if (_instance != null && _instance is IDisposable disposable)
            {
                disposable.Dispose();
            }
        }

        public object Resolve(Type requestedType, DependencyContainer container)
        {
            return _instance ?? (_instance = container.Activate(_contract));
        }
    }
}