using System;
using Velo.DependencyInjection.Dependencies;
using Velo.DependencyInjection.Resolvers;
using Velo.Utils;

namespace Velo.DependencyInjection.Factories
{
    internal sealed partial class ReferenceFactory : IDependencyFactory
    {
        private readonly Type _referenceGenericType;

        public ReferenceFactory()
        {
            _referenceGenericType = typeof(IReference<>);
        }

        public bool Applicable(Type contract)
        {
            return ReflectionUtils.IsGenericTypeImplementation(contract, _referenceGenericType);
        }

        public IDependency BuildDependency(Type contract, IDependencyEngine engine)
        {
            var dependencyType = contract.GenericTypeArguments[0];
            var dependency = engine.GetRequiredDependency(dependencyType);

            Type implementation;
            switch (dependency.Lifetime)
            {
                case DependencyLifetime.Singleton:
                    implementation = typeof(SingletonReference<>);
                    break;
                case DependencyLifetime.Transient:
                    implementation = typeof(TransientReference<>);
                    break;
                default:
                    throw Error.InvalidDependencyLifetime(
                        $"Dependency lifetime isn't supported for create {ReflectionUtils.GetName(contract)}");
            }

            implementation = implementation.MakeGenericType(dependencyType);
            var resolver = new DelegateResolver(implementation, provider => provider.Activate(implementation));
            return new SingletonDependency(new[] {contract}, resolver);
        }
    }
}