using System;
using System.Diagnostics;
using Velo.DependencyInjection.Resolvers;

namespace Velo.DependencyInjection.Dependencies
{
    [DebuggerDisplay("Contract = {Contracts[0]}")]
    internal sealed class ScopedDependency : Dependency
    {
        public ScopedDependency(Type[] contracts, DependencyResolver resolver)
            : base(contracts, resolver, DependencyLifetime.Scoped)
        {
        }

        public override object GetInstance(Type contract, IServiceProvider services) => Resolve(contract, services);

        public override void Dispose()
        {
        }
    }
}