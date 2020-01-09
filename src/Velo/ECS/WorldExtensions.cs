using Velo.DependencyInjection;
using Velo.ECS.Actors;
using Velo.ECS.Assets;
using Velo.ECS.Systems;
using Velo.Utils;

namespace Velo.ECS
{
    public static class WorldExtensions
    {
        // ReSharper disable once InconsistentNaming
        public static DependencyCollection AddECS(this DependencyCollection collection)
        {
            collection
                .AddSingleton<ActorContext>()
                .AddSingleton<AssetContext>()
                .AddSingleton<SystemService>()
                .AddSingleton<World>();

            return collection;
        }

        // ReSharper disable once InconsistentNaming
        public static DependencyCollection AddECSSystem<TSystem>(this DependencyCollection collection,
            DependencyLifetime lifetime = DependencyLifetime.Singleton)
            where TSystem : class, ISystem
        {
            var type = typeof(TSystem);
            var systemsInterfaces = ReflectionUtils.GetInterfaceImplementations(type, typeof(ISystem));
            systemsInterfaces.Add(type);
            
            collection.AddDependency(systemsInterfaces.ToArray(), type, lifetime);
            
            return collection;
        }
    }
}