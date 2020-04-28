using Velo.ECS.Components;

namespace Velo.ECS.Actors.Factory
{
    public interface IActorFactory
    {
        ActorConfigurator Configure();

        Actor Create(IComponent[]? components = null, int? actorId = null);

        TActor Create<TActor>(IComponent[]? components = null, int? actorId = null) where TActor : Actor;
    }
}