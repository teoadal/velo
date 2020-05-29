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

            var implementation = dependency.Lifetime switch
            {
                DependencyLifetime.Singleton => typeof(SingletonReference<>),
                DependencyLifetime.Transient => typeof(TransientReference<>),
                _ => throw Error.InvalidDependencyLifetime(
                    $"Dependency lifetime of {ReflectionUtils.GetName(dependencyType)} (Scoped) isn't supported for create reference")
            };

            var resolver = new ActivatorResolver(implementation.MakeGenericType(dependencyType));
            return new SingletonDependency(new[] {contract}, resolver);
        }
    }
}