using System;
using System.Globalization;
using Velo.DependencyInjection.Dependencies;
using Velo.Serialization;

// ReSharper disable once CheckNamespace
namespace Velo.DependencyInjection
{
    public static class SerializationInstaller
    {
        private static readonly Type[] ConverterContracts = {typeof(IJsonConverter)};

        public static DependencyCollection AddJson(this DependencyCollection dependencies,
            CultureInfo? culture = null)
        {
            dependencies
                .AddSingleton<IConvertersCollection>(provider => new ConvertersCollection(provider, culture))
                .AddSingleton<JConverter>();

            return dependencies;
        }

        public static DependencyCollection AddJsonConverter<TConverter>(this DependencyCollection dependencies)
            where TConverter : class, IJsonConverter
        {
            dependencies.AddDependency(ConverterContracts, typeof(TConverter), DependencyLifetime.Singleton);

            return dependencies;
        }

        public static DependencyCollection AddJsonConverter(this DependencyCollection dependencies, IJsonConverter converter)
        {
            dependencies.Add(new InstanceDependency(ConverterContracts, converter));

            return dependencies;
        }

        internal static DependencyCollection EnsureJsonEnabled(this DependencyCollection dependencies)
        {
            if (!dependencies.Contains(typeof(IConvertersCollection)))
            {
                AddJson(dependencies);
            }

            return dependencies;
        }
    }
}