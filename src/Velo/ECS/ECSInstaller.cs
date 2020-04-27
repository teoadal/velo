using Velo.ECS.Actors;
using Velo.ECS.Actors.Context;
using Velo.ECS.Actors.Factory;
using Velo.ECS.Actors.Filters;
using Velo.ECS.Actors.Groups;
using Velo.ECS.Components;
using Velo.ECS.Injection;
using Velo.ECS.Systems;
using Velo.ECS.Systems.Handlers;

// ReSharper disable once CheckNamespace
namespace Velo.DependencyInjection
{
    // ReSharper disable once InconsistentNaming
    public static class ECSInstaller
    {
        // ReSharper disable once InconsistentNaming
        public static DependencyCollection AddECS(this DependencyCollection dependencies)
        {
            dependencies
                .AddFactory<ISystemService, SystemService>(factory => factory.DependedLifetime())
                .AddFactory(new SystemHandlerFactory())
                .AddFactory(new FilterFactory<IActorContext>(typeof(IActorFilter)))
                .AddFactory(new GroupFactory<IActorContext>(typeof(IActorGroup)))
                .AddFactory(new SingleFactory<IActorContext>(typeof(SingleActor<>)))
                .AddSingleton<IActorContext, ActorContext>()
                .AddSingleton<IActorFactory, ActorFactory>()
                .AddSingleton<IComponentFactory, ComponentFactory>();

            return dependencies;
        }
    }
}