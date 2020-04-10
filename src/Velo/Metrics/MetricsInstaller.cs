using System;
using Velo.Metrics.Counters;
using Velo.Metrics.Provider;

// ReSharper disable once CheckNamespace
namespace Velo.DependencyInjection
{
    public static class MetricsInstaller
    {
        public static DependencyCollection AddMetrics(this DependencyCollection dependencies)
        {
            dependencies.AddFactory(new MetricsProviderFactory());
            return dependencies;
        }

        public static DependencyCollection AddMetricsCounter<TSender>(this DependencyCollection dependencies,
            string name, string description, params ICounterLabel[] labels)
        {
            var contracts = new[] {typeof(ICounter), typeof(ICounter<TSender>)};
            var counter = new Counter<TSender>(name, description, labels);

            dependencies.AddInstance(contracts, counter);

            return dependencies;
        }

        public static DependencyCollection AddMetricsCounter<TSender>(this DependencyCollection dependencies,
            ICounter<TSender> counter)
        {
            var contracts = new[] {typeof(ICounter), typeof(ICounter<TSender>)};
            dependencies.AddInstance(contracts, counter);

            return dependencies;
        }

        public static DependencyCollection AddMetricsCounter<TSender>(this DependencyCollection dependencies,
            Func<IDependencyScope, ICounter<TSender>> builder)
        {
            var contracts = new[] {typeof(ICounter), typeof(ICounter<TSender>)};
            dependencies.AddDependency(contracts, builder, DependencyLifetime.Singleton);

            return dependencies;
        }
    }
}