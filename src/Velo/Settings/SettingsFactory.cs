using System;
using System.Reflection;
using Velo.DependencyInjection;
using Velo.DependencyInjection.Dependencies;
using Velo.DependencyInjection.Factories;
using Velo.Utils;

namespace Velo.Settings
{
    internal sealed class SettingsFactory : IDependencyFactory
    {
        private readonly Type _attributeType = typeof(SettingsAttribute);
        private readonly Type _dependencyType = typeof(SettingsDependency<>);
        
        public bool Applicable(Type contract)
        {
            return Attribute.IsDefined(contract, _attributeType);
        }

        public IDependency BuildDependency(Type contract, IDependencyEngine engine)
        {
            var configurationDependency = engine.GetDependency(Typeof<IConfiguration>.Raw);
            
            var path = contract.GetCustomAttribute<SettingsAttribute>().Path;
            var dependencyType = _dependencyType.MakeGenericType(contract);

            var dependencyParams = new object[] {configurationDependency, path};
            return (IDependency) Activator.CreateInstance(dependencyType, dependencyParams);
        }
    }
}