using Velo.ECS;
using Velo.ECS.Actors;
using Velo.ECS.Assets;
using Velo.ECS.Systems;
using Velo.Utils;

// ReSharper disable once CheckNamespace
namespace Velo.DependencyInjection
{
    public static class WorldInstaller
    {
        // ReSharper disable once InconsistentNaming
        public static DependencyCollection AddECS(this DependencyCollection dependencies)
        {
            dependencies
                .AddSingleton<ActorContext>()
                .AddSingleton<AssetContext>()
                .AddSingleton<SystemService>()
                .AddSingleton<World>();

            return dependencies;
        }

        // ReSharper disable once InconsistentNaming
        public static DependencyCollection AddECSSystem<TSystem>(this DependencyCollection dependencies,
            DependencyLifetime lifetime = DependencyLifetime.Singleton)
            where TSystem : class, ISystem
        {
            var type = typeof(TSystem);
            var systemsInterfaces = ReflectionUtils.GetInterfaceImplementations(type, typeof(ISystem));
            systemsInterfaces.Add(type);
            
            dependencies.AddDependency(systemsInterfaces.ToArray(), type, lifetime);
            
            return dependencies;
        }
    }
}