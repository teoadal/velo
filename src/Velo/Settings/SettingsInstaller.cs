using System;
using Velo.Settings;
using Velo.Settings.Provider;
using Velo.Settings.Sources;

// ReSharper disable once CheckNamespace
namespace Velo.DependencyInjection
{
    public static class SettingsInstaller
    {
        public static DependencyCollection AddSettings(this DependencyCollection dependencies)
        {
            dependencies
                .AddFactory(new SettingsProviderFactory())
                .AddFactory(new SettingsFactory());

            return dependencies;
        }

        public static DependencyCollection AddEnvironmentSettings(this DependencyCollection dependencies)
        {
            dependencies.AddInstance<ISettingsSource>(new EnvironmentSource());
            return dependencies;
        }

        public static DependencyCollection AddCommandLineSettings(this DependencyCollection dependencies,
            string[] commandLineArgs)
        {
            dependencies.AddInstance<ISettingsSource>(new CommandLineSource(commandLineArgs));
            return dependencies;
        }

        public static DependencyCollection AddJsonSettings(this DependencyCollection dependencies,
            string path = "appsettings.json", bool required = false)
        {
            dependencies.AddInstance<ISettingsSource>(new JsonFileSource(path, required));
            return dependencies;
        }

        public static DependencyCollection AddJsonSettings(this DependencyCollection dependencies,
            Func<IDependencyScope, string> fileNameBuilder, bool required = false)
        {
            dependencies.AddSingleton<ISettingsSource>(ctx => new JsonFileSource(fileNameBuilder(ctx), required));
            return dependencies;
        }
    }
}