using System;
using System.Diagnostics;
using Velo.DependencyInjection.Resolvers;
using Velo.Utils;

namespace Velo.DependencyInjection.Dependencies
{
    [DebuggerDisplay("Contract = {Contracts[0]}")]
    internal sealed class SingletonDependency : Dependency
    {
        private object _instance;
        private readonly DependencyResolver _resolver;

        public SingletonDependency(Type[] contracts, DependencyResolver resolver) 
            : base(contracts, DependencyLifetime.Singleton)
        {
            _resolver = resolver;
        }

        public SingletonDependency(Type contract, DependencyResolver resolver) : this(new[] {contract}, resolver)
        {
        }

        public override object GetInstance(Type contract, IDependencyScope scope)
        {
            return _instance ??= _resolver.Resolve(contract, scope);
        }

        public override void Dispose()
        {
            if (ReflectionUtils.IsDisposable(_instance, out var disposable))
            {
                disposable.Dispose();
            }
        }
    }
}