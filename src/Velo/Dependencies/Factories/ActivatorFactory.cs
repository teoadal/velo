using System;
using System.Reflection;
using Velo.Utils;

namespace Velo.Dependencies.Factories
{
    internal sealed class ActivatorFactory : Dependency
    {
        private readonly ConstructorInfo _constructor;
        private readonly Type _implementation;

        public ActivatorFactory(Type[] contracts, Type implementation) : base(contracts)
        {
            _constructor = ReflectionUtils.GetConstructor(implementation);
            _implementation = implementation;
        }

        public override object Resolve(Type requestedType, DependencyContainer container)
        {
            return container.Activate(_implementation, _constructor);
        }
    }
}