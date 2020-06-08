using System;
using System.Collections.Generic;
using System.IO;
using Velo.DependencyInjection.Dependencies;
using Velo.DependencyInjection.Scan;
using Velo.ECS;
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
using Velo.ECS.Sources.Context;
using Velo.ECS.Sources.Json;
using Velo.ECS.State;
using Velo.ECS.Systems;
using Velo.ECS.Systems.Pipelines;
using Velo.Serialization;
using Velo.Utils;

// ReSharper disable once CheckNamespace
namespace Velo.DependencyInjection
{
    public static class ECSInstaller
    {
        private static readonly Type[] AssetSourceContracts = {Typeof<IEntitySource<Asset>>.Raw};

        public static DependencyCollection AddECS(this DependencyCollection dependencies)
        {
            dependencies
                .AddSingleton<IEntityState, EntityState>();

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
                .AddFactory(new SystemPipelineFactory());

            // source
            dependencies
                .AddInstance(new SourceDescriptions())
                .AddSingleton(typeof(IEntitySourceContext<>), typeof(EntitySourceContext<>))
                .EnsureJsonEnabled();

            return dependencies;
        }

        #region AddAssets

        public static DependencyCollection AddAssets(this DependencyCollection dependencies,
            IEntitySource<Asset> assetSource)
        {
            if (assetSource == null) throw Error.Null(nameof(assetSource));

            var dependency = new InstanceDependency(AssetSourceContracts, assetSource);
            dependencies.Add(dependency);

            return dependencies;
        }

        public static DependencyCollection AddAssets(
            this DependencyCollection dependencies,
            Func<IServiceProvider, IEntitySource<Asset>> sourceBuilder)
        {
            if (sourceBuilder == null) throw Error.Null(nameof(sourceBuilder));

            dependencies.AddDependency(AssetSourceContracts, sourceBuilder, DependencyLifetime.Singleton);

            return dependencies;
        }

        public static DependencyCollection AddAssets(
            this DependencyCollection dependencies,
            Func<IEntitySourceContext<Asset>, IEnumerable<Asset>> assets)
        {
            if (assets == null) throw Error.Null(nameof(assets));

            var instance = new DelegateSource<Asset>(assets);
            var dependency = new InstanceDependency(AssetSourceContracts, instance);

            dependencies.Add(dependency);

            return dependencies;
        }

        public static DependencyCollection AddAssets(this DependencyCollection dependencies, string filePath)
        {
            if (string.IsNullOrWhiteSpace(filePath)) throw Error.Null(nameof(filePath));

            dependencies.AddDependency(AssetSourceContracts,
                provider => new JsonFileSource<Asset>(
                    provider.GetRequired<IConvertersCollection>(),
                    provider.GetRequired<SourceDescriptions>(),
                    filePath),
                DependencyLifetime.Singleton);

            return dependencies;
        }

        public static DependencyCollection AddAssets(this DependencyCollection dependencies, Stream stream)
        {
            if (stream == null) throw Error.Null(nameof(stream));

            dependencies.AddDependency(AssetSourceContracts,
                provider => new JsonStreamSource<Asset>(
                    provider.GetRequired<IConvertersCollection>(),
                    provider.GetRequired<SourceDescriptions>(),
                    stream),
                DependencyLifetime.Singleton);

            return dependencies;
        }

        #endregion

        public static DependencyCollection AddSystem<TSystem>(
            this DependencyCollection dependencies,
            DependencyLifetime lifetime = DependencyLifetime.Singleton)
        {
            var implementation = typeof(TSystem);
            if (!ECSUtils.TryGetImplementedSystemInterfaces(implementation, out var contracts))
            {
                throw Error.InvalidOperation($"Type '{ReflectionUtils.GetName<TSystem>()}' isn't implemented system interfaces");
            }

            dependencies.AddDependency(contracts, implementation, lifetime);
            return dependencies;
        }

        public static DependencyScanner RegisterSystems(
            this DependencyScanner scanner,
            DependencyLifetime lifetime = DependencyLifetime.Singleton)
        {
            scanner.UseCollector(new SystemsCollector(lifetime));
            return scanner;
        }
    }
}