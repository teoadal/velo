using System;

namespace Velo.Dependencies.Transients
{
    internal sealed class BuilderTransient<T> : Dependency
        where T : class
    {
        private readonly Func<DependencyContainer, T> _builder;

        public BuilderTransient(Type[] contracts, Func<DependencyContainer, T> builder) : base(contracts)
        {
            _builder = builder;
        }

        public override object Resolve(Type contract, DependencyContainer container)
        {
            return _builder(container);
        }
    }
}