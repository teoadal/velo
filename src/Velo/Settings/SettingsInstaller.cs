using System;
using Velo.DependencyInjection.Dependencies;
using Velo.Settings;
using Velo.Settings.Provider;
using Velo.Settings.Sources;
using Velo.Utils;

// ReSharper disable once CheckNamespace
namespace Velo.DependencyInjection
{
    public static class SettingsInstaller
    {
        private static readonly Type[] SettingsSourceContract = {Typeof<ISettingsSource>.Raw};

        public static DependencyCollection AddSettings(this DependencyCollection dependencies)
        {
            dependencies
                .AddFactory<ISettingsProvider, SettingsProvider>(factory => factory
                    .CreateIf<NullSettingsProvider>(engine => !engine.Contains(typeof(ISettingsSource)))
                    .Lifetime(DependencyLifetime.Singleton))
                .AddFactory(new SettingsFactory())
                .EnsureJsonEnabled();

            return dependencies;
        }

        public static DependencyCollection AddEnvironmentSettings(this DependencyCollection dependencies)
        {
            var dependency = new InstanceDependency(SettingsSourceContract, new EnvironmentSource());
            dependencies.AddDependency(dependency);

            return dependencies;
        }

        public static DependencyCollection AddCommandLineSettings(this DependencyCollection dependencies,
            string[] commandLineArgs)
        {
            var dependency = new InstanceDependency(SettingsSourceContract, new CommandLineSource(commandLineArgs));
            dependencies.AddDependency(dependency);

            return dependencies;
        }

        public static DependencyCollection AddJsonSettings(this DependencyCollection dependencies,
            string path = "appsettings.json", bool required = false)
        {
            var dependency = new InstanceDependency(SettingsSourceContract, new JsonFileSource(path, required));
            dependencies.AddDependency(dependency);

            return dependencies;
        }

        public static DependencyCollection AddJsonSettings(this DependencyCollection dependencies,
            Func<IServiceProvider, string> fileNameBuilder, bool required = false)
        {
            dependencies.AddDependency(
                SettingsSourceContract,
                provider => new JsonFileSource(fileNameBuilder(provider), required),
                DependencyLifetime.Singleton);

            return dependencies;
        }
    }
}