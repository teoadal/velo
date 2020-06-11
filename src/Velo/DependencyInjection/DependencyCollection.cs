using System;
using System.Runtime.CompilerServices;
using Velo.DependencyInjection.Dependencies;
using Velo.DependencyInjection.Factories;
using Velo.DependencyInjection.Resolvers;
using Velo.DependencyInjection.Scan;
using Velo.Utils;

namespace Velo.DependencyInjection
{
    public sealed class DependencyCollection
    {
        private readonly DependencyEngine _engine;

        public DependencyCollection(int capacity = 64)
        {
            _engine = new DependencyEngine(capacity);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public DependencyCollection Add(IDependency dependency)
        {
            _engine.AddDependency(dependency);
            return this;
        }

        #region AddDependency

        public DependencyCollection AddDependency(Type contract, Type implementation, DependencyLifetime lifetime)
        {
            if (contract.IsGenericTypeDefinition)
            {
                CheckIsGenericTypeDefinition(implementation);
                return AddFactory(new GenericFactory(contract, implementation, lifetime));
            }

            var contracts = new[] {contract};
            return AddDependency(contracts, implementation, lifetime);
        }

        public DependencyCollection AddDependency(Type[] contracts, Type implementation, DependencyLifetime lifetime)
        {
            var resolver = DependencyResolver.Build(lifetime, implementation);
            var dependency = Dependency.Build(lifetime, contracts, resolver);
            return Add(dependency);
        }

        public DependencyCollection AddDependency(
            Type[] contracts,
            Func<IServiceProvider, object> builder,
            DependencyLifetime lifetime, Type? implementation = null)
        {
            var resolver = new DelegateResolver(implementation ?? contracts[0], builder);

            var dependency = Dependency.Build(lifetime, contracts, resolver);
            return Add(dependency);
        }

        public DependencyCollection AddDependency<TResult>(
            Type[] contracts,
            Func<IServiceProvider, TResult> builder,
            DependencyLifetime lifetime)
            where TResult : class
        {
            var implementation = typeof(TResult);
            foreach (var contract in contracts)
            {
                if (contract.IsAssignableFrom(implementation)) continue;

                var contractName = ReflectionUtils.GetName(contract);
                var resultName = ReflectionUtils.GetName<TResult>();

                throw Error.InvalidOperation($"Type {resultName} is not assignable from {contractName}");
            }

            var resolver = new DelegateResolver<TResult>(builder);

            var dependency = Dependency.Build(lifetime, contracts, resolver);
            return Add(dependency);
        }

        #endregion

        public DependencyCollection AddFactory(IDependencyFactory factory)
        {
            _engine.AddFactory(factory);
            return this;
        }

        public DependencyCollection AddInstance(Type[] contracts, object instance)
        {
            return Add(new InstanceDependency(contracts, instance));
        }

        public DependencyProvider BuildProvider()
        {
            var provider = new DependencyProvider(_engine);

            _engine.AddDependency(new ProviderDependency());
            _engine.AddDependency(new InstanceDependency(new[] {typeof(DependencyProvider)}, provider));

            _engine.Init();

            return provider;
        }

        public bool Contains(Type contract) => _engine.Contains(contract);

        public IDependency[] GetApplicable(Type contract) => _engine.GetApplicable(contract);

        public IDependency? GetDependency(Type contract) => _engine.GetDependency(contract);

        public IDependency GetRequiredDependency(Type contract) => _engine.GetRequiredDependency(contract);

        public DependencyCollection Scan(Action<DependencyScanner> action)
        {
            var scanner = new DependencyScanner(this);

            action(scanner);
            scanner.Execute();

            return this;
        }

        public bool Remove(Type contract)
        {
            return _engine.Remove(contract);
        }

        private static void CheckIsGenericTypeDefinition(Type type)
        {
            if (type != null && !type.IsGenericTypeDefinition)
            {
                throw Error.InvalidOperation($"{ReflectionUtils.GetName(type)} is not generic type definition");
            }
        }
    }
}