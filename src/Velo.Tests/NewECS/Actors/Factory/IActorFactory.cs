using Velo.Tests.NewECS.Components;

namespace Velo.Tests.NewECS.Actors.Factory
{
    public interface IActorFactory
    {
        ActorConfigurator Configure();

        Actor Create(IComponent[] components = null, int? actorId = null);

        TActor Create<TActor>(IComponent[] components = null, int? actorId = null) where TActor : Actor;
    }
}