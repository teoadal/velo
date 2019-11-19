using System;
using System.Collections.Generic;
using Velo.DependencyInjection.Dependencies;
using Velo.DependencyInjection.Resolvers;
using Velo.Utils;

namespace Velo.DependencyInjection.Factories
{
    internal sealed partial class ArrayFactory : IDependencyFactory
    {
        private static readonly Type EmptyResolverType = typeof(EmptyArrayResolver<>);
        private static readonly Type EnumerableType = typeof(IEnumerable<>);
        private static readonly Type ResolverType = typeof(ArrayResolver<>);
        
        public bool Applicable(Type contract)
        {
            return contract.IsArray ||
                   contract.IsGenericType && contract.GetGenericTypeDefinition() == EnumerableType;
        }

        public IDependency BuildDependency(Type contract, IDependencyEngine engine)
        {
            var elementType = contract.IsArray 
                ? ReflectionUtils.GetArrayElementType(contract)
                : contract.GetGenericArguments()[0];

            var contracts = new[] {elementType.MakeArrayType(), EnumerableType.MakeGenericType(elementType)};
            var dependencies = engine.GetApplicable(elementType);
            
            if (dependencies.Length == 0)
            {
                var emptyResolverType = EmptyResolverType.MakeGenericType(elementType);
                var emptyResolver = Activator.CreateInstance(emptyResolverType);
                return new SingletonDependency(contracts, (DependencyResolver) emptyResolver);
            }

            var resolverType = ResolverType.MakeGenericType(elementType);
            var resolverParameters = new object[] {dependencies.ToArray()};
            var resolver = (DependencyResolver) Activator.CreateInstance(resolverType, resolverParameters);

            return dependencies.All(d => d.Lifetime == DependencyLifetime.Singleton)
                ? (IDependency) new SingletonDependency(contracts, resolver)
                : new TransientDependency(contracts, resolver);
        }
    }
}