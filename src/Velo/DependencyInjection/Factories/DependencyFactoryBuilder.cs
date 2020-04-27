using System;

namespace Velo.DependencyInjection.Factories
{
    internal ref struct DependencyFactoryBuilder<TContract, TImplementation>
        where TImplementation : TContract
    {
        private DependencyLifetime? _lifetime;

        private Type _nullService;
        private Predicate<IDependencyEngine> _nullServicePredicate;

        public DependencyFactoryBuilder<TContract, TImplementation> CreateIf<TNullImplementation>(
            Predicate<IDependencyEngine> predicate)
            where TNullImplementation : class
        {
            _nullService = typeof(TNullImplementation);
            _nullServicePredicate = predicate;

            return this;
        }

        public DependencyFactoryBuilder<TContract, TImplementation> DependedLifetime()
        {
            _lifetime = null;
            return this;
        }

        public DependencyFactoryBuilder<TContract, TImplementation> Lifetime(DependencyLifetime lifetime)
        {
            _lifetime = lifetime;
            return this;
        }

        public IDependencyFactory Build()
        {
            var contract = typeof(TContract);
            var implementation = typeof(TImplementation);
            return new ConfiguredDependencyFactory(
                contract, _lifetime, implementation,
                _nullService, _nullServicePredicate);
        }
    }
}