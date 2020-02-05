using System;
using System.Diagnostics;
using System.Reflection;
using Velo.Utils;

namespace Velo.DependencyInjection.Resolvers
{
    [DebuggerDisplay("Implementation = {" + nameof(Implementation) + "}")]
    internal sealed class ActivatorResolver : DependencyResolver
    {
        private readonly ConstructorInfo _constructor;

        public ActivatorResolver(Type implementation)
            : base(implementation)
        {
            _constructor = ReflectionUtils.GetConstructor(implementation);
        }

        protected override object GetInstance(Type contract, IDependencyScope scope)
        {
            return scope.Activate(Implementation, _constructor);
        }
    }
}