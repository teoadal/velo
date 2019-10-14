using System;
using System.Collections.Generic;
using Velo.DependencyInjection.Dependencies;
using Velo.DependencyInjection.Factories;
using Velo.DependencyInjection.Resolvers;
using Velo.Extensions;
using Velo.Utils;

namespace Velo.DependencyInjection.Engine
{
    internal sealed class ConstructorEngine : DependencyEngine
    {
        private readonly Dictionary<Type, DependencyDescription> _descriptions;
        private readonly ResolverFactory[] _factories;

        public ConstructorEngine(Dictionary<Type, DependencyDescription> descriptions, List<ResolverFactory> factories)
            : base(descriptions.Count, factories.ToArray())
        {
            _descriptions = descriptions;
            _factories = factories.ToArray();
        }

        public RuntimeEngine Build()
        {
            var descriptions = _descriptions;
            foreach (var (contract, description) in descriptions)
            {
                if (Collection.ContainsKey(contract)) continue;

                var mainDependency = InsertDependency(contract, description.Main);

                if (description.ResolversCount > 1)
                {
                    InsertArrayDependency(contract.MakeArrayType(), contract, description, mainDependency);
                }
            }

            descriptions.Clear();

            return new RuntimeEngine(Collection, _factories);
        }

        protected override Dependency FindDependency(Type contract)
        {
            var arrayRequested = contract.IsArray;
            var descriptionContract = arrayRequested
                ? ReflectionUtils.GetArrayElementType(contract)
                : contract;

            if (!_descriptions.TryGetValue(descriptionContract, out var description)) return null;

            var mainDependency = InsertDependency(descriptionContract, description.Main);
            return arrayRequested
                ? InsertArrayDependency(contract, descriptionContract, description, mainDependency)
                : mainDependency;
        }

        private Dependency InsertArrayDependency(Type contract, Type elementType, DependencyDescription description,
            Dependency mainDependency)
        {
            var arrayDependencies = new Dependency[description.ResolversCount];
            arrayDependencies[0] = mainDependency;

            var resolvers = description.OtherResolvers;
            if (resolvers != null)
            {
                for (var i = 0; i < resolvers.Count; i++)
                {
                    var resolver = resolvers[i];
                    var dependency = CreateDependency(elementType, resolver);
                    arrayDependencies[i + 1] = dependency;
                }
            }

            var arrayResolver = new ArrayResolver(elementType, arrayDependencies);
            var arrayDependency = InsertDependency(contract, arrayResolver);

            return arrayDependency;
        }

        public override void Dispose()
        {
            _descriptions.Clear();
            base.Dispose();
        }
    }
}