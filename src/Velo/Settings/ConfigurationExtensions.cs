using System;
using Velo.DependencyInjection;
using Velo.Settings.Sources;

namespace Velo.Settings
{
    public static class ConfigurationExtensions
    {
        public static DependencyCollection AddCommandLineConfiguration(this DependencyCollection collection,
            string[] commandLineArgs)
        {
            EnsureConfigurationRegistered(collection);

            collection.AddInstance<IConfigurationSource>(new CommandLineSource(commandLineArgs));
            return collection;
        }

        public static DependencyCollection AddJsonConfiguration(this DependencyCollection collection,
            string path = "appsettings.json", bool required = false)
        {
            EnsureConfigurationRegistered(collection);

            collection.AddInstance<IConfigurationSource>(new JsonFileSource(path, required));
            return collection;
        }

        public static DependencyCollection AddJsonConfiguration(this DependencyCollection collection,
            Func<IDependencyScope, string> fileNameBuilder, bool required = false)
        {
            EnsureConfigurationRegistered(collection);

            collection.AddSingleton<IConfigurationSource>(ctx => new JsonFileSource(fileNameBuilder(ctx), required));
            return collection;
        }

        private static void EnsureConfigurationRegistered(DependencyCollection collection)
        {
            if (!collection.Contains<IConfiguration>())
            {
                collection
                    .AddSingleton<IConfiguration, Configuration>()
                    .AddFactory(new SettingsFactory());
            }
        }
    }
}