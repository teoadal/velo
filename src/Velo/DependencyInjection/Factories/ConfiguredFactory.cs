using System;
using Velo.DependencyInjection.Dependencies;
using Velo.DependencyInjection.Resolvers;

namespace Velo.DependencyInjection.Factories
{
    internal sealed class ConfiguredFactory : IDependencyFactory
    {
        private readonly Type _contract;
        private readonly DependencyLifetime? _lifetime;
        private readonly Type _implementation;

        private readonly Type _nullService;
        private readonly Predicate<IDependencyEngine>? _nullServicePredicate;

        public ConfiguredFactory(
            Type contract,
            DependencyLifetime? lifetime,
            Type implementation,
            Type nullService,
            Predicate<IDependencyEngine>? nullServicePredicate)
        {
            _contract = contract;
            _lifetime = lifetime;
            _implementation = implementation;
            _nullService = nullService;
            _nullServicePredicate = nullServicePredicate;
        }

        public bool Applicable(Type contract)
        {
            return contract == _contract;
        }

        public IDependency BuildDependency(Type contract, IDependencyEngine engine)
        {
            var contracts = new[] {contract};

            if (_nullServicePredicate != null && _nullServicePredicate(engine))
            {
                return new InstanceDependency(contracts, Activator.CreateInstance(_nullService));
            }

            var lifetime = _lifetime ?? engine.DefineLifetime(_implementation);
            var resolver = DependencyResolver.Build(lifetime, _implementation, engine);

            return Dependency.Build(lifetime, contracts, resolver);
        }
    }
}