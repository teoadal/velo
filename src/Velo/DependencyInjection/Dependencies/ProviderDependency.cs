using System;

namespace Velo.DependencyInjection.Dependencies
{
    internal sealed class ProviderDependency : IDependency
    {
        public Type[] Contracts { get; }

        public Type Implementation { get; }

        public DependencyLifetime Lifetime { get; }

        public ProviderDependency()
        {
            var providerType = typeof(IServiceProvider);

            Contracts = new[] {providerType};
            Implementation = providerType;

            Lifetime = DependencyLifetime.Scoped;
        }

        public bool Applicable(Type contract)
        {
            return Array.IndexOf(Contracts, contract) != -1;
        }

        public object GetInstance(Type contract, IServiceProvider services)
        {
            return services;
        }

        #region Interfaces

        void IDependency.Init(IDependencyEngine engine)
        {
        }

        void IDisposable.Dispose()
        {
        }

        #endregion
    }
}