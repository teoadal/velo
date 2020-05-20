using System;
using Velo.DependencyInjection;
using Velo.DependencyInjection.Dependencies;
using Velo.DependencyInjection.Resolvers;
using Velo.Settings.Provider;
using Velo.Utils;

namespace Velo.Settings
{
    internal sealed partial class SettingsFactory
    {
        private sealed class SettingsDependency<TSettings> : DependencyResolver, IDependency
            where TSettings : class
        {
            public Type[] Contracts => _contracts ??= new[] {Implementation};

            public DependencyLifetime Lifetime => DependencyLifetime.Singleton;

            private Type[]? _contracts;
            private readonly string _path;

            public SettingsDependency(string path) : base(Typeof<TSettings>.Raw)
            {
                _path = path;
            }

            public bool Applicable(Type contract)
            {
                return Implementation == contract;
            }

            protected override object ResolveInstance(Type contract, IServiceProvider services)
            {
                var configuration = services.GetRequired<ISettingsProvider>();
                return configuration.Get<TSettings>(_path);
            }

            object IDependency.GetInstance(Type contract, IServiceProvider services)
            {
                return ResolveInstance(contract, services);
            }

            public void Dispose()
            {
            }
        }
    }
}