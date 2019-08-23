using System;

namespace Velo.Dependencies.Factories
{
    internal sealed class ActivatorFactory : Dependency
    {
        private readonly Type _implementation;

        public ActivatorFactory(Type[] contracts, Type implementation) : base(contracts)
        {
            _implementation = implementation;
        }

        public override object Resolve(Type requestedType, DependencyContainer container)
        {
            return container.Activate(_implementation);
        }
    }
}