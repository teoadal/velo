using System;

namespace Velo.Dependencies.Factories
{
    internal sealed class BuilderFactory<T> : IDependency
        where T : class
    {
        private readonly Type _contract;
        private Func<DependencyContainer, T> _builder;

        public BuilderFactory(Type contract, Func<DependencyContainer, T> builder)
        {
            _contract = contract;
            _builder = builder;
        }

        public bool Applicable(Type requestedType)
        {
            return _contract == requestedType;
        }

        public void Destroy()
        {
            _builder = null;
        }
        
        public object Resolve(Type requestedType, DependencyContainer container)
        {
            return _builder(container);
        }
    }
}