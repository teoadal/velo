using System;

namespace Velo.DependencyInjection.Dependencies
{
    internal sealed class ProviderDependency : IDependency
    {
        public Type[] Contracts { get; }

        public Type Implementation { get; }

        public DependencyLifetime Lifetime { get; }

        private readonly DependencyProvider _provider;

        public ProviderDependency(DependencyProvider provider)
        {
            _provider = provider;
            var providerType = typeof(IServiceProvider);

            Contracts = new[] {providerType, typeof(DependencyProvider)};
            Implementation = providerType;
            Lifetime = DependencyLifetime.Scoped;
        }

        public bool Applicable(Type contract)
        {
            return Array.IndexOf(Contracts, contract) != -1;
        }

        public object GetInstance(Type contract, IServiceProvider services)
        {
            return contract == typeof(DependencyProvider)
                ? _provider
                : services;
        }

        public void Dispose()
        {
        }
    }
}