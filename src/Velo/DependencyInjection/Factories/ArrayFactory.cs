using System;
using System.Collections.Generic;
using Velo.DependencyInjection.Dependencies;
using Velo.DependencyInjection.Resolvers;
using Velo.Utils;

namespace Velo.DependencyInjection.Factories
{
    internal sealed partial class ArrayFactory : IDependencyFactory
    {
        private readonly Type _enumerableType = typeof(IEnumerable<>);

        public bool Applicable(Type contract)
        {
            return contract.IsArray ||
                   contract.IsGenericType && contract.GetGenericTypeDefinition() == _enumerableType;
        }

        public IDependency BuildDependency(Type contract, IDependencyEngine engine)
        {
            var elementType = contract.IsArray
                ? ReflectionUtils.GetArrayElementType(contract)
                : contract.GenericTypeArguments[0];

            var contracts = new[] {elementType.MakeArrayType(), _enumerableType.MakeGenericType(elementType)};
            var dependencies = engine.GetApplicable(elementType);

            if (dependencies.Length == 0)
            {
                var emptyDependencyType = typeof(EmptyArrayDependency<>).MakeGenericType(elementType);
                return (IDependency) Activator.CreateInstance(emptyDependencyType);
            }

            var resolverType = typeof(ArrayResolver<>).MakeGenericType(elementType);
            var resolverParameters = new object[] {dependencies};
            var resolver = (DependencyResolver) Activator.CreateInstance(resolverType, resolverParameters);

            var lifetime = dependencies.DefineLifetime();
            return Dependency.Build(lifetime, contracts, resolver);
        }
    }
}