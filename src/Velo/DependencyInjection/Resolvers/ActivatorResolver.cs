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
            _constructor = ReflectionUtils.GetConstructor(implementation)
                           ?? throw Error.DefaultConstructorNotFound(implementation);
        }

        public override void Init(DependencyLifetime lifetime, IDependencyEngine engine)
        {
            EnsureValidDependenciesLifetime(_constructor, lifetime, engine);
        }

        protected override object ResolveInstance(Type contract, IServiceProvider services)
        {
            return services.Activate(Implementation, _constructor);
        }
    }
}