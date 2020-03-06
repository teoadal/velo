using System;
using Velo.DependencyInjection;
using Velo.DependencyInjection.Dependencies;
using Velo.DependencyInjection.Factories;
using Velo.DependencyInjection.Resolvers;
using Velo.Settings.Sources;

namespace Velo.Settings.Provider
{
    internal sealed class SettingsProviderFactory : IDependencyFactory
    {
        private readonly Type _contract;

        public SettingsProviderFactory()
        {
            _contract = typeof(ISettings);
        }

        public bool Applicable(Type contract)
        {
            return contract == _contract;
        }

        public IDependency BuildDependency(Type contract, IDependencyEngine engine)
        {
            var contracts = new[] {_contract};

            if (!engine.Contains(typeof(ISettingsSource)))
            {
                return new InstanceDependency(contracts, new NullProvider());
            }

            var resolver = new ActivatorResolver(typeof(SettingsProvider));
            return new SingletonDependency(contracts, resolver);
        }
    }
}