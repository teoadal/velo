using System;
using Velo.DependencyInjection;
using Velo.DependencyInjection.Resolvers;
using Velo.Settings.Provider;
using Velo.Utils;

namespace Velo.Settings
{
    internal sealed partial class SettingsFactory
    {
        private sealed class SettingsResolver<TSettings> : DependencyResolver
            where TSettings: class
        {
            private readonly string _path;

            public SettingsResolver(string path) : base(Typeof<TSettings>.Raw)
            {
                _path = path;
            }

            protected override object ResolveInstance(Type contract, IDependencyScope scope)
            {
                var configuration = scope.GetRequiredService<ISettingsProvider>();
                return configuration.Get<TSettings>(_path);
            }
        }
    }
}