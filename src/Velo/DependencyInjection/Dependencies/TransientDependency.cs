using System;
using System.Diagnostics;
using Velo.DependencyInjection.Resolvers;

namespace Velo.DependencyInjection.Dependencies
{
    [DebuggerDisplay("Contract = {Contracts[0]}")]
    internal sealed class TransientDependency : Dependency
    {
        public TransientDependency(Type[] contracts, DependencyResolver resolver)
            : base(contracts, resolver, DependencyLifetime.Transient)
        {
        }

        public override object GetInstance(Type contract, IServiceProvider services) => Resolve(contract, services);

        public override void Dispose()
        {
        }
    }
}