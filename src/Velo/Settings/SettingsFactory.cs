using System;
using System.Reflection;
using Velo.DependencyInjection;
using Velo.DependencyInjection.Dependencies;
using Velo.DependencyInjection.Factories;

namespace Velo.Settings
{
    internal sealed partial class SettingsFactory : IDependencyFactory
    {
        public bool Applicable(Type contract)
        {
            return SettingsAttribute.IsDefined(contract);
        }

        public IDependency BuildDependency(Type contract, IDependencyEngine engine)
        {
            var path = contract.GetCustomAttribute<SettingsAttribute>().Path;
            var dependencyType = typeof(SettingsDependency<>).MakeGenericType(contract);

            return (IDependency) Activator.CreateInstance(dependencyType, path);
        }
    }
}