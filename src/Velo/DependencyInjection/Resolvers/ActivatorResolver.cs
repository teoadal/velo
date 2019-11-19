using System;
using System.Diagnostics;
using System.Reflection;
using Velo.Utils;

namespace Velo.DependencyInjection.Resolvers
{
    [DebuggerDisplay("Implementation = {" + nameof(_implementation) + "}")]
    internal sealed class ActivatorResolver : DependencyResolver
    {
        private readonly ConstructorInfo _constructor;
        private readonly Type _implementation;

        public ActivatorResolver(Type implementation)
        {
            _constructor = ReflectionUtils.GetConstructor(implementation);
            _implementation = implementation;
        }

        protected override object GetInstance(Type contract, IDependencyScope scope)
        {
            return scope.Activate(_implementation, _constructor);
        }
    }
}