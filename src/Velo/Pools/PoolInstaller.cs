using System;
using Velo.Pools;

// ReSharper disable once CheckNamespace
namespace Velo.DependencyInjection
{
    public static class PoolInstaller
    {
        public static DependencyCollection AddPool<T>(this DependencyCollection dependencies, T[] buffer)
            where T : class
        {
            dependencies.AddInstance<IPool<T>>(new Pool<T>(buffer));
            return dependencies;
        }

        public static DependencyCollection AddPool<T>(this DependencyCollection dependencies,
            Func<T> builder, int capacity = 10)
            where T : class
        {
            dependencies.AddInstance<IPool<T>>(new Pool<T>(capacity, builder));
            return dependencies;
        }

        public static DependencyCollection AddPool<T>(this DependencyCollection dependencies,
            Func<IServiceProvider, T> builder, int capacity = 10)
            where T : class
        {
            dependencies.AddSingleton<IPool<T>>(provider => new Pool<T>(capacity, () => builder(provider)));
            return dependencies;
        }
    }
}