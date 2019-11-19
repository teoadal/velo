using System;
using System.Diagnostics;
using Velo.DependencyInjection.Resolvers;

namespace Velo.DependencyInjection.Dependencies
{
    [DebuggerDisplay("Contract = {Contracts[0]}")]
    internal sealed class TransientDependency : Dependency
    {
        private readonly DependencyResolver _resolver;

        public TransientDependency(Type[] contracts, DependencyResolver resolver) 
            : base(contracts, DependencyLifetime.Transient)
        {
            _resolver = resolver;
        }

        public TransientDependency(Type contract, DependencyResolver resolver) : this(new[] {contract}, resolver)
        {
        }

        public override object GetInstance(Type contract, IDependencyScope scope)
        {
            return _resolver.Resolve(contract, scope);
        }

        public override void Dispose()
        {
        }
    }
}