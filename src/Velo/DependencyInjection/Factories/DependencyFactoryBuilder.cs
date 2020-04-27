using System;

namespace Velo.DependencyInjection.Factories
{
    internal struct DependencyFactoryBuilder
    {
        private readonly Type _contract;
        private readonly Type _implementation;

        private DependencyLifetime? _lifetime;
        private Type _nullService;
        private Predicate<IDependencyEngine> _nullServicePredicate;

        internal DependencyFactoryBuilder(Type contract, Type implementation)
        {
            _contract = contract;
            _implementation = implementation;

            _lifetime = null;
            _nullService = null!;
            _nullServicePredicate = null!;
        }

        public DependencyFactoryBuilder CreateIf<TNullImplementation>(
            Predicate<IDependencyEngine> predicate)
            where TNullImplementation : class
        {
            _nullService = typeof(TNullImplementation);
            _nullServicePredicate = predicate;

            return this;
        }

        public DependencyFactoryBuilder DependedLifetime()
        {
            _lifetime = null;
            return this;
        }

        public DependencyFactoryBuilder Lifetime(DependencyLifetime lifetime)
        {
            _lifetime = lifetime;
            return this;
        }

        public IDependencyFactory Build()
        {
            return new ConfiguredFactory(
                _contract, _lifetime, _implementation,
                _nullService, _nullServicePredicate);
        }
    }
}