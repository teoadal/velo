using System;
using System.Collections.Generic;
using Velo.DependencyInjection.Dependencies;
using Velo.DependencyInjection.Factories;
using Velo.DependencyInjection.Resolvers;
using Velo.Utils;

namespace Velo.DependencyInjection.Engines
{
    internal sealed class RuntimeEngine : DependencyEngine
    {
        public RuntimeEngine(Dictionary<Type, Dependency> dependencies, ResolverFactory[] factories)
            : base(dependencies, factories)
        {
        }

        protected override Dependency FindDependency(Type contract)
        {
            if (!contract.IsArray) return null;
            
            var elementType = ReflectionUtils.GetArrayElementType(contract);
            return Collection.TryGetValue(elementType, out var elementDependency) 
                ? InsertDependency(contract, new ArrayResolver(elementType, elementDependency)) 
                : null;
        }
        
        public override void Dispose()
        {
            CollectionUtils.DisposeValuesIfDisposable(Collection);
            base.Dispose();
        }
    }
}