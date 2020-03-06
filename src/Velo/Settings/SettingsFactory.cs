using System;
using System.Reflection;
using Velo.DependencyInjection;
using Velo.DependencyInjection.Dependencies;
using Velo.DependencyInjection.Factories;
using Velo.DependencyInjection.Resolvers;

namespace Velo.Settings
{
    internal sealed partial class SettingsFactory : IDependencyFactory
    {
        private readonly Type _resolverType = typeof(SettingsResolver<>);

        public bool Applicable(Type contract)
        {
            return SettingsAttribute.IsDefined(contract);
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