using System;

namespace Velo.Dependencies.Factories
{
    internal sealed class BuilderFactory<T> : Dependency
        where T : class
    {
        private readonly Func<DependencyContainer, T> _builder;

        public BuilderFactory(Type[] contracts, Func<DependencyContainer, T> builder) : base(contracts)
        {
            _builder = builder;
        }

        public override object Resolve(Type requestedType, DependencyContainer container)
        {
            return _builder(container);
        }
    }
}