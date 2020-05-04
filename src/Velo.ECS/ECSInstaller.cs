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
using Velo.ECS.Assets.Factory;
using Velo.ECS.Assets.Filters;
using Velo.ECS.Assets.Groups;
using Velo.ECS.Components;
using Velo.ECS.Injection;
using Velo.ECS.Sources;
using Velo.ECS.Sources.Json;
using Velo.ECS.Systems;
using Velo.ECS.Systems.Handlers;
using Velo.Utils;

// ReSharper disable once CheckNamespace
namespace Velo.DependencyInjection
{
    public static class ECSInstaller
    {
        private static readonly Type[] AssetSourceContracts = {Typeof<ISource<Asset>>.Raw};

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
                .AddSingleton<IAssetContext, AssetContext>()
                .AddSingleton<AssetFactory>();

            // components
            dependencies
                .AddSingleton<IComponentFactory, ComponentFactory>();

            // systems
            dependencies
                .AddFactory<ISystemService, SystemService>(systemService => systemService.DependedLifetime())
                .AddFactory(new SystemHandlerFactory());

            // source
            dependencies
                .AddInstance(new SourceDescriptions())
                .AddTransient(typeof(ISourceContext<>), typeof(SourceContext<>))
                .AddDependency(jsonEntityReader => jsonEntityReader
                    .Contract<IJsonEntityReader<Actor>>()
                    .Contract<IJsonEntityReader<Asset>>()
                    .Implementation<JsonEntityReader>()
                    .Singleton())
                .EnsureJsonEnabled();

            return dependencies;
        }

        public static DependencyCollection AddAssets(this DependencyCollection dependencies, ISource<Asset> assetSource)
        {
            if (assetSource == null) throw Error.Null(nameof(assetSource));

            var dependency = new InstanceDependency(AssetSourceContracts, assetSource);
            dependencies.AddDependency(dependency);

            return dependencies;
        }

        public static DependencyCollection AddAssets(
            this DependencyCollection dependencies,
            Func<IDependencyScope, ISource<Asset>> sourceBuilder)
        {
            if (sourceBuilder == null) throw Error.Null(nameof(sourceBuilder));

            dependencies.AddDependency(AssetSourceContracts, sourceBuilder, DependencyLifetime.Singleton);

            return dependencies;
        }

        public static DependencyCollection AddAssets(
            this DependencyCollection dependencies,
            Func<ISourceContext<Asset>, IEnumerable<Asset>> assets)
        {
            if (assets == null) throw Error.Null(nameof(assets));

            var instance = new DelegateSource<Asset>(assets);
            var dependency = new InstanceDependency(AssetSourceContracts, instance);

            dependencies.AddDependency(dependency);

            return dependencies;
        }

        public static DependencyCollection AddJsonAssets(this DependencyCollection dependencies, string path)
        {
            if (string.IsNullOrWhiteSpace(path)) throw Error.Null(nameof(path));

            dependencies.AddDependency(
                AssetSourceContracts,
                scope => new JsonFileSource<Asset>(scope.GetRequiredService<IJsonEntityReader<Asset>>(), path),
                DependencyLifetime.Singleton);

            return dependencies;
        }
    }
}