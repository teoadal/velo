using System;
using Velo.DependencyInjection;

namespace Velo.Pools
{
    public static class PoolExtensions
    {
        public static DependencyCollection AddPool<T>(this DependencyCollection collection, T[] buffer)
            where T : class
        {
            collection.AddInstance<IPool<T>>(new Pool<T>(buffer));
            return collection;
        }

        public static DependencyCollection AddPool<T>(this DependencyCollection collection,
            Func<T> builder, int capacity = 10)
            where T : class
        {
            collection.AddInstance<IPool<T>>(new Pool<T>(capacity, builder));
            return collection;
        }

        public static DependencyCollection AddPool<T>(this DependencyCollection collection,
            Func<IDependencyScope, T> builder, int capacity = 10)
            where T : class
        {
            collection.AddSingleton<IPool<T>>(ctx => new Pool<T>(capacity, () => builder(ctx)));
            return collection;
        }
    }
}