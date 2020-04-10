using System;
using Velo.DependencyInjection;
using Velo.DependencyInjection.Dependencies;
using Velo.DependencyInjection.Factories;
using Velo.DependencyInjection.Resolvers;
using Velo.Metrics.Counters;

namespace Velo.Metrics.Provider
{
    internal sealed class MetricsProviderFactory : IDependencyFactory
    {
        private readonly Type _contract;

        public MetricsProviderFactory()
        {
            _contract = typeof(IMetricsProvider);
        }

        public bool Applicable(Type contract)
        {
            return contract == _contract;
        }

        public IDependency BuildDependency(Type contract, IDependencyEngine engine)
        {
            var contracts = new[] {_contract};

            if (!engine.Contains(typeof(ICounter)))
            {
                return new InstanceDependency(contracts, new NullMetricsProvider());
            }

            var resolver = new ActivatorResolver(typeof(MetricsProvider));
            return new SingletonDependency(contracts, resolver);
        }
    }
}