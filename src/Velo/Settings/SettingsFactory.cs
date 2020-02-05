using System;
using System.Reflection;
using Velo.DependencyInjection;
using Velo.DependencyInjection.Dependencies;
using Velo.DependencyInjection.Factories;
using Velo.DependencyInjection.Resolvers;

namespace Velo.Settings
{
    internal sealed class SettingsFactory : IDependencyFactory
    {
        private readonly Type _attributeType = typeof(SettingsAttribute);
        private readonly Type _resolverType = typeof(SettingsResolver<>);
        
        public bool Applicable(Type contract)
        {
            return Attribute.IsDefined(contract, _attributeType);
        }

        public IDependency BuildDependency(Type contract, IDependencyEngine engine)
        {
            var path = contract.GetCustomAttribute<SettingsAttribute>().Path;
            var resolverType = _resolverType.MakeGenericType(contract);
            var resolver = (DependencyResolver) Activator.CreateInstance(resolverType, path);
            
            return new TransientDependency(contract, resolver);
        }
    }
}