using System;

namespace Velo.Dependencies.Singletons
{
    internal sealed class InstanceSingleton : IDependency
    {
        private readonly Type _contract;
        private readonly object _instance;

        public InstanceSingleton(Type contract, object instance)
        {
            _contract = contract;
            _instance = instance;
        }

        public bool Applicable(Type requestedType)
        {
            return _contract == requestedType;
        }

        public object Resolve(Type requestedType, DependencyContainer container)
        {
            return _instance;
        }
    }
}