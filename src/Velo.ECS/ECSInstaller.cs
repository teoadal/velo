using System;
using System.Collections.Generic;
using Velo.DependencyInjection.Dependencies;
using Velo.ECS.Actors;
using Velo.ECS.Actors.Context;
using Velo.ECS.Actors.Factory;
using Velo.ECS.Actors.Filters;
using Velo.ECS.Actors.Groups;
using Velo.ECS.Assets;
using Velo.ECS.Assets.Context;
using Velo.ECS.Assets.Filters;
using Velo.ECS.Assets.Groups;
using Velo.ECS.Assets.Sources;
using Velo.ECS.Components;
using Velo.ECS.Injection;
using Velo.ECS.Sources;
using Velo.ECS.Systems;
using Velo.ECS.Systems.Handlers;
using Velo.Utils;

// ReSharper disable once CheckNamespace
namespace Velo.DependencyInjection
{
    public static class ECSInstaller
    {
        private static readonly Type[] AssetSourceContracts = {Typeof<IAssetSource>.Raw};
        
        public static DependencyCollection AddECS(this DependencyCollection dependencies)
        {
            //actors
            dependencies
                .AddFactory(new FilterFactory<IActorContext>(typeof(IActorFilter)))
                .AddFactory(new GroupFactory<IActorContext>(typeof(IActorGroup)))
                .AddFactory(new SingleFactory<IActorContext>(typeof(SingleActor<>)))
                .AddInstance<IActorContext>(new ActorContext())
                .AddSingleton<IActorFactory, ActorFactory>();

            // assets
            dependencies
                .AddFactory(new FilterFactory<IAssetContext>(typeof(IAssetFilter)))
                .AddFactory(new GroupFactory<IAssetContext>(typeof(IAssetGroup)))
                .AddFactory(new SingleFactory<IAssetContext>(typeof(SingleAsset<>)))
                .AddSingleton<IAssetContext, AssetContext>();

            // components
            dependencies
                .AddSingleton<IComponentFactory, ComponentFactory>();

            // systems
            dependencies
                .AddFactory<ISystemService, SystemService>(factory => factory.DependedLifetime())
                .AddFactory(new SystemHandlerFactory());

            // source
            dependencies
                .AddSingleton<JsonEntityConverters>()
                .EnsureJsonEnabled();

            return dependencies;
        }

        public static DependencyCollection AddJsonAssets(this DependencyCollection dependencies, string path)
        {
            if (string.IsNullOrWhiteSpace(path)) throw Error.Null(nameof(path));

            dependencies.AddDependency(
                AssetSourceContracts,
                scope => new JsonAssetSource(scope.GetRequiredService<JsonEntityConverters>(), path),
                DependencyLifetime.Singleton);

            return dependencies;
        }

        public static DependencyCollection AddMemoryAssets(this DependencyCollection dependencies, IEnumerable<Asset> assets)
        {
            if (assets == null) throw Error.Null(nameof(assets));

            var dependency = new InstanceDependency(AssetSourceContracts, new MemoryAssetSource(assets));
            dependencies.AddDependency(dependency);

            return dependencies;
        }
    }
}