using System;
using System.Diagnostics;
using Velo.DependencyInjection.Resolvers;
using Velo.Utils;

namespace Velo.DependencyInjection.Dependencies
{
    [DebuggerDisplay("Contract = {Contracts[0]}")]
    internal sealed class SingletonDependency : Dependency
    {
        private object? _instance;

        public SingletonDependency(Type[] contracts, DependencyResolver resolver)
            : base(contracts, resolver, DependencyLifetime.Singleton)
        {
        }

        public override object GetInstance(Type contract, IServiceProvider services)
        {
            return _instance ??= Resolve(contract, services);
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