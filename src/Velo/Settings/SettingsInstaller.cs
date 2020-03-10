using System;
using Velo.Settings;
using Velo.Settings.Provider;
using Velo.Settings.Sources;

// ReSharper disable once CheckNamespace
namespace Velo.DependencyInjection
{
    public static class SettingsInstaller
    {
        public static DependencyCollection AddSettings(this DependencyCollection collection)
        {
            collection
                .AddFactory(new SettingsProviderFactory())
                .AddFactory(new SettingsFactory());

            return collection;
        }

        public static DependencyCollection AddEnvironmentSettings(this DependencyCollection collection)
        {
            collection.AddInstance<ISettingsSource>(new EnvironmentSource());
            return collection;
        }

        public static DependencyCollection AddCommandLineSettings(this DependencyCollection collection,
            string[] commandLineArgs)
        {
            collection.AddInstance<ISettingsSource>(new CommandLineSource(commandLineArgs));
            return collection;
        }

        public static DependencyCollection AddJsonSettings(this DependencyCollection collection,
            string path = "appsettings.json", bool required = false)
        {
            collection.AddInstance<ISettingsSource>(new JsonFileSource(path, required));
            return collection;
        }

        public static DependencyCollection AddJsonSettings(this DependencyCollection collection,
            Func<IDependencyScope, string> fileNameBuilder, bool required = false)
        {
            collection.AddSingleton<ISettingsSource>(ctx => new JsonFileSource(fileNameBuilder(ctx), required));
            return collection;
        }
    }
}