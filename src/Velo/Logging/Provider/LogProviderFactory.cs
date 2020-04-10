using System;
using Velo.DependencyInjection;
using Velo.DependencyInjection.Dependencies;
using Velo.DependencyInjection.Factories;
using Velo.DependencyInjection.Resolvers;
using Velo.Logging.Writers;

namespace Velo.Logging.Provider
{
    internal sealed class LogProviderFactory : IDependencyFactory
    {
        private readonly Type _providerType;

        public LogProviderFactory()
        {
            _providerType = typeof(ILogProvider);
        }

        public bool Applicable(Type contract)
        {
            return contract == _providerType;
        }

        public IDependency BuildDependency(Type contract, IDependencyEngine engine)
        {
            var contracts = new[] {_providerType};

            if (!engine.Contains(typeof(ILogWriter)))
            {
                return new InstanceDependency(contracts, new NullLogProvider());
            }

            var resolver = new CompiledResolver(typeof(LogProvider), engine);
            return Dependency.Build(DependencyLifetime.Scoped, contracts, resolver);
        }
    }
}