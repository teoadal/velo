using System;
using System.Reflection;
using Velo.Utils;

namespace Velo.Dependencies.Transients
{
    internal sealed class ActivatorTransient : Dependency
    {
        private readonly ConstructorInfo _constructor;
        private readonly Type _implementation;

        public ActivatorTransient(Type[] contracts, Type implementation) : base(contracts)
        {
            _implementation = implementation;
            _constructor = ReflectionUtils.GetConstructor(implementation);
        }

        public override object Resolve(Type contract, DependencyContainer container)
        {
            return container.Activate(_implementation, _constructor);
        }
    }
}